using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using GoLoan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Infrastructure.Repositories
{
    public class LoanAccountService : ILoanAccountService

    {
        private readonly AppDbContext db;
        public LoanAccountService(AppDbContext db)
        {
            this.db = db;
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
