using GoLoan.Application.DTO;
using GoLoan.Application.Interfaces;
using GoLoan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace GoLoan.Infrastructure.Repositories
{
    public class CustomerPaymentHistoryService : ICustomerPaymentHistoryService
    {
        AppDbContext data;

    public CustomerPaymentHistoryService(AppDbContext data)
        {
            this.data = data;
        }

        public async Task<List<CustomerPaymentHistoryDto>> GetEmiPaymentHistory(int? loanAccount, string period)
        {
            var payments = await data.LoanPayments.Include(p => p.LoanAccount).Where(p => p.PaymentStatus == "Success").ToListAsync();

            if (loanAccount != null)
            {
                payments = payments.Where(p => p.LoanAccountId == loanAccount).ToList();
            }

            if (period == "1month")
            {
                payments = payments.Where(p => p.PaymentDate >= DateTime.Today.AddMonths(-1)).ToList();
            }
            else if (period == "3month")
            {
                payments = payments.Where(p => p.PaymentDate >= DateTime.Today.AddMonths(-3)).ToList();
            }
            else if (period == "6month")
            {
                payments = payments.Where(p => p.PaymentDate >= DateTime.Today.AddMonths(-6)).ToList();
            }
            else if (period == "1year")
            {
                payments = payments.Where(p => p.PaymentDate >= DateTime.Today.AddYears(-1)).ToList();
            }

            var result = payments.Select(p => new CustomerPaymentHistoryDto
            {
                Date = p.PaymentDate,
                LoanAccountNo = p.LoanAccount!.LoanAccountNo,
                Amount = p.PaidAmount,
                PaymentMode = p.PaymentMode,
                Reference = p.TransactionReference,
                Status = p.PaymentStatus,
                PaymentType = "EMI Payment"
            }).ToList();

            return result;
        }

        public async Task<List<ForeClosurePaymentDto>> GetForeClosurePaymentHistory(int? loanAccount, string period)
        {
            var payments = await data.ForeClosureRequests.Where(f => f.IsPaid == true && f.PaidDate != null).ToListAsync();

            if (loanAccount != null)
            {
                payments = payments.Where(f => f.LoanAccountId == loanAccount).ToList();
            }


            if (period == "1month")
            {
                payments = payments.Where(f => f.PaidDate >= DateTime.Today.AddMonths(-1)).ToList();
            }
            else if (period == "3month")
            {
                payments = payments.Where(f => f.PaidDate >= DateTime.Today.AddMonths(-3)).ToList();
            }
            else if (period == "6month")
            {
                payments = payments.Where(f => f.PaidDate >= DateTime.Today.AddMonths(-6)).ToList();
            }
            else if (period == "1year")
            {
                payments = payments.Where(f => f.PaidDate >= DateTime.Today.AddYears(-1)).ToList();
            }
                

            var result = payments.Select(f => new ForeClosurePaymentDto
            {
                RequestId = f.RequestId,
                LoanAccountId = f.LoanAccountId,
                ForeClosureType = f.ForeClosureType,
                ForeClosureAmount = f.ForeClosureAmount,
                Status = f.Status,
                PaidDate = f.PaidDate.Value
            }).ToList();

            return result;
        }
    }
}
