using System.Text;
using EZRide_Project.Data;
using EZRide_Project.Helpers;
using EZRide_Project.Model.Entities;
using EZRide_Project.Repositories;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(op =>
op.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

builder.Services.AddControllers();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<EmailService>();


builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IVehicleImageRepository,VehicleImageRepository>();
builder.Services.AddScoped<IVehicleImageService, VehicleImageService>();
builder.Services.AddScoped<IPricingRuleRepository, PricingRuleRepository>();
builder.Services.AddScoped<IPricingRuleService, PricingRuleService>();
builder.Services.AddScoped<IVehicleDetailsRepository, VehicleDetailsRepository>();
builder.Services.AddScoped<IVehicleDetailsService, VehicleDetailsService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ISecurityDepositRepository, SecurityDepositRepository>();
builder.Services.AddScoped<ISecurityDepositService, SecurityDepositService>();
builder.Services.AddScoped<ICustomerDocumentRepository, CustomerDocumentRepository>();
builder.Services.AddScoped<ICustomerDocumentService, CustomerDocumentService>();
builder.Services.AddScoped<IPaymentDetailsRepository, PaymentDetailsRepository>();
builder.Services.AddScoped<IPaymentDetailsService, PaymentDetailsService>();
builder.Services.AddScoped<IPaymentReceiptRepository, PaymentReceiptRepository>();
builder.Services.AddScoped<IPaymentReceiptService, PaymentReceiptService>();
builder.Services.AddScoped<IFeedbackRepository,FeedbackRepository>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IBookingSummaryRepository, BookingSummaryRepository>();
builder.Services.AddScoped<IBookingSummaryService, BookingSummaryService>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddSingleton<WhatsAppService>();
builder.Services.AddScoped<IAdminUserBookingInfoRepository, AdminUserBookingInfoRepository>();
builder.Services.AddScoped<IAdminUserBookingInfoService, AdminUserBookingInfoService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


// Add Swagger with JWT Bearer support

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EZRide API", Version = "v1" });

   
    // JWT config in Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your JWT token.\n\nExample: **Bearer eyJhbGci...**"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

// Add CORS middleware BEFORE Authorization and routing
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
