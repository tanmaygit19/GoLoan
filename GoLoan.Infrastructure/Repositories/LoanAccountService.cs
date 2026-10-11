using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using GoLoan.Application.DTO;
using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using GoLoan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoLoan.Infrastructure.Repositories
{
    public class LoanAccountService : ILoanAccountService

    {
        private readonly AppDbContext db;
        private readonly  IMapper mapper;
        public LoanAccountService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }
        public async Task<bool> AccountExist(int id)
        {
            return await db.LoanAccounts.AnyAsync(x => x.DealId == id);
        }

        public async Task AddLoanAccount(LoanAccount acc)
        {
            await db.LoanAccounts.AddAsync(acc);
            await db.SaveChangesAsync();
        }


        public async Task<LoanAccountDTO?> CreateLoanAccount(int id)
        {
            var deal = await GetDeal(id);

            if (deal == null)
                return null;

            if (await AccountExist(id))
                return null;

            var disbursement = deal.Disbursements?.FirstOrDefault(x =>x.Status == "Success" ||x.Status == "Completed");

            if (disbursement == null)
                return null;


            var account = new LoanAccount
            {
                DealId = deal.DealId,
                CustomerId = deal.CustomerId,
                DisbursementId = disbursement.DisbursementId,
                LoanAccountNo = "LA" +DateTime.Now.ToString("yyyyMMdd"),
                LoanAmount = disbursement.DisburseAmount,
                OutstandingPrincipal = disbursement.DisburseAmount,
                LoanStatus = "Active",
                InterestRate = Convert.ToDecimal(deal.InterestRate),
                TenureMonths = deal.TenureMonths,
                EmiAmount = deal.EmiAmount,
                DisbursementDate = disbursement.DisbursementDate,
                TotalPaidAmount = 0,
                CreatedAt = DateTime.Now
            };

            await AddLoanAccount(account);

            return mapper.Map<LoanAccountDTO>(account);
        }

        public async Task<List<LoanAccount>> GetAccounts()
        {
            return await db.LoanAccounts.ToListAsync();
        }

        public async Task<LoanAccount?> GetById(int id)
        {
            return await  db.LoanAccounts.FirstOrDefaultAsync(x => x.LoanAccountId == id);
        }

        public async Task<LoanDeal?> GetDeal(int id)
        {
           return await db.LoanDeals.Include(x => x.Disbursements).FirstOrDefaultAsync(x => x.DealId == id);
        }
    }
}
