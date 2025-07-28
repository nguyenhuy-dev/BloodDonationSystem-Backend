using Application.DTO.SendEmailDTO;
using Application.Service.Auth;
using Application.Service.BlogSer;
using Application.Service.BloodCompatibilitySer;
using Application.Service.BloodHistoryServ;
using Application.Service.BloodInventoryServ;
using Application.Service.BloodProcedureServ;
using Application.Service.BloodRegistrationServ;
using Application.Service.BloodTypeServ;
using Application.Service.CommentServ;
using Application.Service.EmailServ;
using Application.Service.Events;
using Application.Service.HealthProcedureServ;
using Application.Service.ReportServ;
using Application.Service.Users;
using Application.Service.VolunteerServ;
using BloodDonationSystem.BackgroundServices;
using Infrastructure.Data;
using Infrastructure.Repository.Auth;
using Infrastructure.Repository.BlogRepo;
using Infrastructure.Repository.Blood;
using Infrastructure.Repository.BloodCompatibilityRepo;
using Infrastructure.Repository.BloodInventoryRepo;
using Infrastructure.Repository.BloodProcedureRepo;
using Infrastructure.Repository.BloodRegistrationRepo;
using Infrastructure.Repository.CommentRepo;
using Infrastructure.Repository.Events;
using Infrastructure.Repository.Facilities;
using Infrastructure.Repository.HealthProcedureRepo;
using Infrastructure.Repository.ReportRepository;
using Infrastructure.Repository.Users;
using Infrastructure.Repository.VolunteerRepo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalPolicy", policy =>
    {
        policy
            .WithOrigins(
            "https://cors-test.codehappy.dev/",
            "http://localhost:5173",
            "https://localhost:5173",
            "https://Blood-Donation-Support-System.somee.com",
            "https://blood-donation-support-system-fe.vercel.app")    // your React app
                                                                  //.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();                     // if you send cookies/auth
    });
});


//Dependency Injection (DI) for donation
builder.Services.AddScoped<IGoogleService, GoogleService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IBloodRegistrationRepository, BloodRegistrationRepository>();
builder.Services.AddScoped<IBloodRegistrationService, BloodRegistrationService>();

builder.Services.AddScoped<IHealthProcedureRepository, HealthProcedureRepository>();
builder.Services.AddScoped<IHealthProcedureService, HealthProcedureService>();

builder.Services.AddScoped<IBloodProcedureRepository, BloodProcedureRepository>();
builder.Services.AddScoped<IBloodProcedureService, BloodProcedureService>();

builder.Services.AddScoped<IBloodInventoryRepository, BloodInventoryRepository>();
builder.Services.AddScoped<IBloodInventoryService, BloodInventoryService>();

builder.Services.AddScoped<IVolunteerRepository,VolunteerRepository>();
builder.Services.AddScoped<IVolunteerService, VolunteerService>();

builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddScoped<IFacilityRepository, FacilityRepository>();

builder.Services.AddScoped<IBloodTypeRepository, BloodTypeRepository>();
builder.Services.AddScoped<IBloodTypeService, BloodTypeService>();

builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<IBlogService, BlogService>();

builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddScoped<IBloodHistoryService, BloodHistoryService>();

builder.Services.AddScoped<IBloodCompatibilityRepository, BloodCompatibilityRepository>();
builder.Services.AddScoped<IBloodCompatibilityService, BloodCompatibilityService>();

//Add background service
builder.Services.AddHostedService<EventExpiryBackgroundService>();
builder.Services.AddHostedService<BloodRegistrationExpiryBackgroundService>();
builder.Services.AddHostedService<VolunteerExpiryBackgroundService>();
builder.Services.AddHostedService<ReminderMailBackgroundService>();
builder.Services.AddHostedService<BloodUnitsExpiryBackgroundService>();


// Add configuration for email service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailService, EmailService>();

//Add token config
var secretKey = builder.Configuration["AppSettings:SecretKey"];
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
            ClockSkew = TimeSpan.Zero // Disable clock skew for immediate expiration
        };
    });


// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "JWT Swagger API", Version = "v1" });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Description = "Nhập 'Bearer <token>' vào đây",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});


builder.Services.AddDbContext<BloodDonationSystemContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


var app = builder.Build();

// In Program.cs, add this middleware before UseCors

// Configure the HTTP request pipeline.
app.UseCors("LocalPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();              // Bật middleware xác thực
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();                         // Bật Swagger middleware
    app.UseSwaggerUI();                       // Giao diện UI
}

app.MapControllers();

app.Run();