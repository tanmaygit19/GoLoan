using GoLoan.Application.DTO.Dashboard;
using GoLoan.Application.Interfaces;
using GoLoan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Infrastructure.Repositories
{
    public class CustomerDashboardService : ICustomerDashboardService
    {

        private readonly AppDbContext db;

        public CustomerDashboardService(AppDbContext db)
        {
            this.db = db;
        }
        public async Task<DashboardDTO> GetDashboardAsync(int customerId)
        {
            var dashboard = new DashboardDTO();

            var applications = await db.LoanDeals
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.AppliedDate)
                .ToListAsync();

            var loans = await db.LoanAccounts
                .Where(x => x.CustomerId == customerId)
                .Include(x => x.LoanDeal)
                .ToListAsync();

            var loanAccountIds = loans.Select(x => x.LoanAccountId).ToList();

            var emis = await db.EmiSchedules
                .Where(x => loanAccountIds.Contains(x.LoanAccountId))
                .ToListAsync();

            var payments = await db.LoanPayments
                .Where(x => loanAccountIds.Contains(x.LoanAccountId))
                .OrderByDescending(x => x.PaymentDate)
                .ToListAsync();

            dashboard.Summary.TotalApplications = applications.Count;
            dashboard.Summary.ActiveLoans = loans.Count(x => x.LoanStatus == "Active");
            dashboard.Summary.EmisPaid = emis.Count(x => x.PaymentStatus == "Paid");
            dashboard.Summary.UpcomingEmiAmount = emis
                .Where(x => x.PaymentStatus != "Paid" && x.DueDate >= DateTime.Today)
                .OrderBy(x => x.DueDate)
                .Select(x => (decimal?)x.Emi)
                .FirstOrDefault() ?? 0;

            dashboard.Summary.TotalAmountPaid = payments
                .Where(x => x.PaymentStatus == "Success" || x.PaymentStatus == "Completed")
                .Sum(x => x.PaidAmount);

            dashboard.Applications = applications.Select(x => new LoanApplicationDTO
            {
                ApplicationId = x.DealId,
                LoanType = x.LoanType,
                LoanAmount = x.LoanAmount,
                ApprovedAmount = x.ApprovedAmount,
                Status = x.CurrentStatus,
                AppliedDate = x.AppliedDate
            }).ToList();

            dashboard.ActiveLoans = loans
                .Where(x => x.LoanStatus == "Active")
                .Select(x => new ActiveLoanDTO
                {
                    LoanAccountId = x.LoanAccountId,
                    LoanAccountNo = x.LoanAccountNo,
                    LoanType = x.LoanDeal != null ? x.LoanDeal.LoanType : string.Empty,
                    LoanAmount = x.LoanAmount,
                    OutstandingPrincipal = x.OutstandingPrincipal,
                    InterestRate = (double)x.InterestRate,
                    TenureMonths = x.TenureMonths,
                    EmiAmount = x.EmiAmount,
                    DisbursementDate = x.DisbursementDate,
                    TotalPaidAmount = x.TotalPaidAmount
                }).ToList();

            var paidEmis = emis.Where(x => x.PaymentStatus == "Paid").ToList();
            var unpaidEmis = emis.Where(x => x.PaymentStatus != "Paid").ToList();

            var nextEmi = unpaidEmis
                .Where(x => x.DueDate >= DateTime.Today)
                .OrderBy(x => x.DueDate)
                .FirstOrDefault();

            dashboard.EmiSummary.TotalEmis = emis.Count;
            dashboard.EmiSummary.PaidEmis = paidEmis.Count;
            dashboard.EmiSummary.UpcomingEmis = unpaidEmis.Count(x => x.DueDate >= DateTime.Today);
            dashboard.EmiSummary.RemainingEmis = unpaidEmis.Count;
            dashboard.EmiSummary.TotalEmiAmount = emis.Sum(x => x.Emi);
            dashboard.EmiSummary.PaidEmiAmount = paidEmis.Sum(x => x.Emi);
            dashboard.EmiSummary.RemainingEmiAmount = unpaidEmis.Sum(x => x.Emi);
            dashboard.EmiSummary.NextEmiDate = nextEmi?.DueDate;
            dashboard.EmiSummary.NextEmiAmount = nextEmi?.Emi;

            dashboard.RecentPayments = payments
                .Take(5)
                .Select(x => new RecentPaymentDTO
                {
                    PaymentId = x.PaymentId,
                    PaymentName = x.PaymentName,
                    PaymentDate = x.PaymentDate,
                    PaidAmount = x.PaidAmount,
                    PaymentMode = x.PaymentMode,
                    TransactionReference = x.TransactionReference,
                    PaymentStatus = x.PaymentStatus
                }).ToList();

            var dealIds = applications.Select(x => x.DealId).ToList();

            var sanctionLetters = await db.SanctionLetters
                .Where(x => dealIds.Contains(x.DealId))
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            dashboard.Documents = sanctionLetters.Select(x => new LoanDocumentDTO
            {
                DocumentId = x.SanctionId,
                DealId = x.DealId,
                DocumentType = "Sanction Letter",
                LoanAmount = x.LoanAmount,
                CreatedDate = x.CreatedAt
            }).ToList();

            return dashboard;
        }
    }
}
