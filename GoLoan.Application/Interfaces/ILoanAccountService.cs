using GoLoan.Application.DTO;
using GoLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.Interfaces
{
    public interface ILoanAccountService
    {
        Task<LoanDeal?> GetDeal(int id);
        Task<bool> AccountExist(int id);

        Task<List<LoanAccount>> GetAccounts();

        Task<LoanAccount?> GetById(int id);

        Task AddLoanAccount(LoanAccount acc);
        Task<LoanAccountDTO?> CreateLoanAccount(int id);

    }
}
