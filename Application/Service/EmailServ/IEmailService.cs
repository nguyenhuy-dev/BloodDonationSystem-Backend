using Domain.Entities;

namespace Application.Service.EmailServ
{
    public interface IEmailService
    {
        Task SendEmailBloodCollectionAsync(BloodRegistration bloodRegistration);
        Task SendEmailFindDonorsAsync(BloodRegistration bloodRegistration);
    }
}
