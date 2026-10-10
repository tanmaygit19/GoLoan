using GoLoan.Application.Interfaces;
using GoLoan.Application.Mapper;
using GoLoan.Infrastructure.Data;
using GoLoan.Infrastructure.Repositories;
using Hangfire;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(option =>
    option.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn")
    ));


builder.Services.AddHangfire(configuration =>
    configuration.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("dbconn")
    ));
builder.Services.AddHangfireServer();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ISupportTicketService, SupportTicketService>();
builder.Services.AddScoped<ICustomerDashboardService, CustomerDashboardService>();
builder.Services.AddScoped<ILoanAccountService, LoanAccountService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IEmiSchedular, EmiSchedular>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(LoanAccountMapper));

var app = builder.Build();
var recurringJobManager =
    app.Services.GetRequiredService<IRecurringJobManager>();

recurringJobManager.AddOrUpdate<INotificationService>(
    "emi-reminders",
    service => service.CreateEmiReminders(),
    Cron.Daily
);
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.UseHangfireDashboard();
app.MapControllers();

app.Run();
