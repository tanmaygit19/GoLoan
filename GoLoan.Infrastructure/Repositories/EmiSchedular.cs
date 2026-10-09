using AutoMapper;
using GoLoan.Application.DTO;
using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using GoLoan.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace GoLoan.Infrastructure.Repositories
{
    public class EmiSchedular : IEmiSchedular
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public EmiSchedular(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }
        public async Task GenerateEmiSchedule(int loanAccountId)
        {
           var account = await db.LoanAccounts.FirstOrDefaultAsync(x => x.LoanAccountId == loanAccountId);

                if (account == null)
                    return;

                bool exists = await db.EmiSchedules.AnyAsync(x => x.LoanAccountId == loanAccountId);

                if (exists)
                    return;

                decimal balance = account.LoanAmount;
                decimal monthlyRate = account.InterestRate / 1200m;

                for (int i = 1; i <= account.TenureMonths; i++)
                {
                    decimal openingBalance = balance;
                    decimal interest = Math.Round(openingBalance * monthlyRate, 2);
                    decimal principal = account.EmiAmount - interest;

                    if (i == account.TenureMonths || principal > balance)
                        principal = balance;

                    decimal emi = principal + interest;
                    balance = Math.Max(0, openingBalance - principal);

                    var dto = new EmiSchedularDTO
                    {
                        LoanAccountId = account.LoanAccountId,
                        InstallmentNo = i,
                        DueDate = account.DisbursementDate.AddMonths(i),
                        Emi = emi,
                        PrincipalAmount = principal,
                        InterestAmount = interest,
                        OpeningBalance = openingBalance,
                        ClosingBalance = balance,
                        PaymentStatus = "Pending",
                        PaidDate = null
                    };

                    var schedule = mapper.Map<EmiSchedule>(dto);
                    db.EmiSchedules.Add(schedule);
                }

                await db.SaveChangesAsync();
            }
        }
    }
