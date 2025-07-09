using Application.DTO.SendEmailDTO;
using Domain.Entities;
using Infrastructure.Repository.Blood;
using Infrastructure.Repository.Events;
using Infrastructure.Repository.Facilities;
using Infrastructure.Repository.Users;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Application.Service.EmailServ
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly IUserRepository _repoUser;
        private readonly IEventRepository _repoEvent;
        private readonly IFacilityRepository _repoFacil;
        private readonly IBloodTypeRepository _repoBloodType;

        public EmailService(IOptions<EmailSettings> settings, IUserRepository repoUser,
            IEventRepository repoEvent, IFacilityRepository repoFacil, 
            IBloodTypeRepository repoBloodType)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings), "Email settings cannot be null.");
            _repoUser = repoUser;
            _repoEvent = repoEvent;
            _repoFacil = repoFacil;
            _repoBloodType = repoBloodType;
        } 

        public async Task SendEmailBloodCollectionAsync(BloodRegistration bloodRegistration)
        {
            var (member, eventObj, facility) = await GetNecessariesForSendEmail(bloodRegistration);

            // Nếu member chưa có gmail thì khỏi gửi
            if (member?.Gmail == null)
                return;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Trung tâm Hiến Máu Nhân Đạo Ép Pê Tê", _settings.From));
            message.To.Add(MailboxAddress.Parse(member?.Gmail));
            message.Subject = $"Cảm ơn bạn đã tham gia hiến máu tại sự kiện {eventObj?.Title}";

            var builder = new BodyBuilder();

            builder.HtmlBody = $@"
    <html>
    <body style='font-family: Arial, sans-serif; color: #333;'>
        <p>Xin chào <strong>{member?.FirstName}</strong>,</p>

        <p>
            Chúng tôi xin gửi lời cảm ơn chân thành vì bạn đã tham gia hiến máu tại sự kiện <strong>{eventObj?.Title}</strong> được tổ chức tại địa điểm <strong>{facility?.Name}</strong>.
            Những giọt máu của bạn sẽ giúp cứu sống nhiều người đang cần được truyền máu – Bạn chính là người hùng của chúng tôi!
        </p>

        <h3>🩸 Một vài lưu ý nhỏ để phục hồi sau hiến máu:</h3>
        <ul>
            <li>Uống nhiều nước trong 24 giờ tới.</li>
            <li>Tránh hoạt động thể chất nặng trong ngày hôm nay.</li>
            <li>Ăn đầy đủ, đặc biệt là thực phẩm giàu sắt như thịt đỏ, rau lá xanh,…</li>
            <li>Nếu cảm thấy chóng mặt, hãy nằm nghỉ và nâng cao chân.</li>
        </ul>

        <h3>📅 Khi nào bạn có thể hiến máu lần tiếp theo?</h3>
        <p>
            Bạn có thể hiến máu lần tiếp theo sau <strong>{ (member?.Gender == true ? "12 tuần" : "16 tuần") }</strong> kể từ ngày hôm nay.
        </p>
        <p>
            Một lần nữa, cảm ơn bạn đã góp phần lan toả sự sống.
        </p>

        <p>
            Nếu bạn có bất kỳ câu hỏi nào, đừng ngần ngại liên hệ với chúng tôi qua <strong>{_settings.From}</strong>.
        </p>

        <p>
            Trân trọng,<br>
            <strong>{facility?.Name}</strong>
        </p>
    </body>
    </html>";

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendEmailFindDonorsAsync(BloodRegistration bloodRegistration)
        {
            var (member, eventObj, facility) = await GetNecessariesForSendEmail(bloodRegistration);
            var bloodType = await _repoBloodType.GetBloodTypeByIdAsync(eventObj?.BloodTypeId);

            // Nếu member chưa có gmail thì khỏi gửi
            if (member?.Gmail == null)
                return;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Trung tâm Hiến Máu Nhân Đạo Ép Pê Tê", _settings.From));
            message.To.Add(MailboxAddress.Parse(member?.Gmail));
            message.Subject = $"Một giọt máu, ngàn hy vọng 💖";

            var builder = new BodyBuilder();
            builder.HtmlBody = $@"
    <p>Chào <strong>{member?.FirstName}</strong>,</p>
    <p>
        Hiện tại, lượng máu dự trữ tại <strong>{facility?.Name}</strong> đang ở mức thấp – và chúng tôi đang rất cần sự giúp đỡ từ cộng đồng những người hiến máu đầy yêu thương như bạn.
    </p>

    <p>
        Mỗi đơn vị máu bạn cho đi không chỉ là một hành động nhân ái, mà còn là nguồn sống thiết thực cho những bệnh nhân đang điều trị khẩn cấp.
    </p>

    <p>
        Nếu bạn đang đủ điều kiện sức khỏe và sẵn sàng tiếp tục hành trình sẻ chia này, hãy để chúng tôi biết nhé.
    </p>

    <p style=""font-size: 16px;"">
        👉 <a href='' style=""color: #0066cc; text-decoration: none; font-weight: bold;"">Bấm vào đây để huỷ</a>
    </p>

    <div style=""background: #f9f9f9; padding: 12px; border-left: 4px solid #cc0033; margin-top: 16px;"">
        <p><strong>📍 Thông tin hiến máu:</strong></p>
        <ul>
            <li>Địa điểm: <strong>{facility?.Address}</strong></li>
            <li>Thời gian linh hoạt: <strong>{facility?.OpeningHour.ToString(@"hh\:mm")}</strong> đến <strong>{facility?.ClosingHour.ToString(@"hh\:mm")}</strong></li>
            <li>Loại máu cần ưu tiên: <strong>{bloodType?.Type}</strong></li>
        </ul>
    </div>

    <p>
        Chúng tôi biết rằng không phải lúc nào cũng thuận tiện để quay lại, nhưng nếu bạn có thể, hành động của bạn sẽ tạo nên sự khác biệt rất lớn.
    </p>

    <p>
        Cảm ơn bạn vì đã luôn đồng hành và lan toả điều tốt đẹp.
    </p>

    <p>Trân trọng,<br>
        <strong>{facility?.Name}</strong>
    </p>";

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        private async Task<(User? member, Event? eventObj, Facility? facility)> GetNecessariesForSendEmail(BloodRegistration bloodRegistration)
        {
            var member = await _repoUser.GetUserByIdAsync(bloodRegistration.MemberId);
            var eventObj = await _repoEvent.GetEventByIdAsync(bloodRegistration.EventId);
            var facility = await _repoFacil.GetByIdAsync(eventObj?.FacilityId);

            return (member, eventObj, facility);
        }
    }
}