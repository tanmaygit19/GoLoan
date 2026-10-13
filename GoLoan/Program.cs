using GoLoan.Application;
using GoLoan.Application.Interfaces;
using GoLoan.Infrastructure.Data;
using GoLoan.Infrastructure.Repositories;
using GoLoan.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(option =>
    option.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn")
    ));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<Isanctionletter, sanctionletterservice>();
builder.Services.AddScoped<ILoanDealCreation, LoanDealCreation>();
builder.Services.AddScoped<IDisbursement, DisbursementService>();


builder.Services.AddAutoMapper(cfg =>
{
}, typeof(MappingData).Assembly);

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
