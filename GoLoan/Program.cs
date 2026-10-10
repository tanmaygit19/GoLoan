
using Microsoft.EntityFrameworkCore;
using GoLoan.Infrastructure.Data;
using GoLoan.Application.Interfaces;
using GoLoan.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(option =>
    option.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn")
    ));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register ForeClosure service
builder.Services.AddScoped<IForeClosureService, ForeClosureService>();
builder.Services.AddScoped<RazorpayService>();
builder.Services.AddScoped<IEmiPaymentService, EmiPaymentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
