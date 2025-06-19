using Application.Service.Auth;
using Application.Service.Events;
using Application.Service.Users;
using Infrastructure.Repository.Blood;
using Infrastructure.Data;
using Infrastructure.Repository.BloodRegistrationRepo;
using Infrastructure.Repository.HealthProcedureRepo;
using Application.Service.HealthProcedureServ;
using Infrastructure.Repository.Auth;
using Infrastructure.Repository.Events;
using Infrastructure.Repository.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Infrastructure.Repository.BloodProcedureRepo;
using Application.Service.BloodProcedureServ;
using Infrastructure.Repository.BloodInventoryRepo;
using Infrastructure.Repository.VolunteerRepo;
using Application.DTO.SendEmailDTO;
using Application.Service.EmailServ;
using Infrastructure.Repository.Facilities;
using Application.Service.BloodRegistrationServ;
using Application.Service.VolunteerServ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")    // your React app
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();                      // if you send cookies/auth
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
builder.Services.AddScoped<IVolunteerRepository,VolunteerRepository>();
builder.Services.AddScoped<IVolunteerService, VolunteerService>();
builder.Services.AddScoped<IFacilityRepository, FacilityRepository>();
builder.Services.AddScoped<IBloodTypeRepository, BloodTypeRepository>();
builder.Services.AddScoped<IVolunteerRepository, VolunteerRepository>();
builder.Services.AddScoped<IVolunteerService, VolunteerService>();


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