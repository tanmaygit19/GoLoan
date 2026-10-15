using System.Text;
using System.Text.RegularExpressions;
using Hangfire;
using GoLoan.Application;
using GoLoan.Application.DTO.Auth;
using GoLoan.Application.Interfaces;
using GoLoan.Application.Interfaces.Auth;
using GoLoan.Application.Mapper;
using GoLoan.Infrastructure.Data;
using GoLoan.Infrastructure.Repositories;
using GoLoan.Infrastructure.Repositories.Auth;
using GoLoan.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn")));

// JWT and Email configuration
builder.Services.Configure<JwtSettingsDTO>(
    builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddHangfire(configuration =>
    configuration.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("dbconn")
    ));
builder.Services.AddHangfireServer();
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<EmailSettingsDTO>(
    builder.Configuration.GetSection("EmailSettings"));

// AutoMapper: register all assemblies used by either branch.
var mapperAssemblies = AppDomain.CurrentDomain.GetAssemblies()
    .Append(typeof(LoanAccountMapper).Assembly)
    .Append(typeof(DashboardMappingProfile).Assembly)
    .Append(typeof(SupportTicketMappingProfile).Assembly)
    .Append(typeof(MappingData).Assembly)
    .Distinct()
    .ToArray();

builder.Services.AddAutoMapper(cfg => { }, mapperAssemblies);

// JWT Authentication
var jwtKey = builder.Configuration["JwtSettings:Key"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue(
                    "access_token", out var token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    });

// Authentication and 2FA services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();

// Foreclosure and payment services
builder.Services.AddScoped<IForeClosureService, ForeClosureService>();
builder.Services.AddScoped<RazorpayService>();
builder.Services.AddScoped<IEmiPaymentService, EmiPaymentService>();
builder.Services.AddScoped<ICustomerPaymentHistoryService, CustomerPaymentHistoryService>();

// Customer and support services

builder.Services.AddScoped<ISupportTicketService, SupportTicketService>();
builder.Services.AddScoped<ICustomerDashboardService, CustomerDashboardService>();

// Reports and loan services
builder.Services.AddScoped<IReportsService, ReportsService>();
builder.Services.AddScoped<ILMSReportsService, LMSReportsService>();
builder.Services.AddScoped<ILoanAccountService, LoanAccountService>();

// Notification and EMI scheduling services
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IEmiSchedular, EmiSchedular>();

// Other loan services
builder.Services.AddScoped<Isanctionletter, sanctionletterservice>();
builder.Services.AddScoped<ILoanDealCreation, LoanDealCreation>();
builder.Services.AddScoped<IDisbursement, DisbursementService>();

// Hangfire
builder.Services.AddHangfire(configuration =>
    configuration.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("dbconn")));

builder.Services.AddHangfireServer();

// API Explorer and Swagger with JWT authorization
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token"
    });

    options.OperationFilter<AuthorizeCheckOperationFilter>();
});

// Built-in OpenAPI
builder.Services.AddOpenApi();

// Angular frontend CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(cfg => { },typeof(LoanAccountMapper).Assembly
); builder.Services.AddCors(options =>
{
    options.AddPolicy("myapp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
         .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

var app = builder.Build();

// Register recurring Hangfire job
var recurringJobManager =
    app.Services.GetRequiredService<IRecurringJobManager>();

recurringJobManager.AddOrUpdate<INotificationService>(
    "emi-reminders",
    service => service.CreateEmiReminders(),
    Cron.Daily);

// Swagger JSON security middleware
app.Use(async (context, next) =>
{
    if (context.Request.Path.Value != null &&
        context.Request.Path.Value.EndsWith(
            "swagger.json", StringComparison.OrdinalIgnoreCase))
    {
        var originalBody = context.Response.Body;

        using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        try
        {
            await next();

            buffer.Seek(0, SeekOrigin.Begin);
            var json = await new StreamReader(buffer).ReadToEndAsync();

            json = Regex.Replace(
                json,
                @"""security"":\s*\[\s*\{\s*\}\s*\]",
                "\"security\":[{\"Bearer\":[]}]");

            var bytes = Encoding.UTF8.GetBytes(json);
            context.Response.Body = originalBody;
            context.Response.ContentLength = bytes.Length;
            await originalBody.WriteAsync(bytes);
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }
    else
    {
        await next();
    }
});

app.UseSwagger();
app.UseSwaggerUI();

// Built-in OpenAPI in Development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("MyApp");
app.UseAuthentication();
app.UseCors("myapp");
app.UseAuthorization();

// Hangfire Dashboard
app.UseHangfireDashboard();

app.MapControllers();
app.Run();

// Swagger authorization operation filter
public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(
        OpenApiOperation operation,
        OperationFilterContext context)
    {
        var hasAuthorize =
            context.MethodInfo.DeclaringType?
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Any() == true
            || context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Any();

        if (hasAuthorize)
        {
            operation.Security ??= new List<OpenApiSecurityRequirement>();

            operation.Security.Add(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer"),
                        new List<string>()
                    }
                });
        }
    }
}
