using GoLoan.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.Interfaces
{
    public interface ICustomerPaymentHistoryService
    {

        Task<List<CustomerPaymentHistoryDto>> GetEmiPaymentHistory(int? loanAccount, string period);

        Task<List<ForeClosurePaymentDto>> GetForeClosurePaymentHistory(int? loanAccount, string period);

    }
}
