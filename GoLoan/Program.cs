using GoLoan.Application.Interfaces;
using GoLoan.Application.Mapper;
using GoLoan.Infrastructure.Data;
using GoLoan.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ILoanAccountService, LoanAccountService>();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(option =>
    option.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn")
    ));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ISupportTicketService, SupportTicketService>();
builder.Services.AddScoped<ICustomerDashboardService, CustomerDashboardService>();
builder.Services.AddScoped<IReportsService, ReportsService>();
builder.Services.AddScoped<ILMSReportsService, LMSReportsService>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(LoanAccountMapper));
builder.Services.AddAutoMapper(typeof(DashboardMappingProfile));
builder.Services.AddAutoMapper(typeof(SupportTicketMappingProfile));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
