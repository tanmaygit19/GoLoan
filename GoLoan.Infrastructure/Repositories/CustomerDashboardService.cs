using AutoMapper;
using GoLoan.Application.DTO.Dashboard;
using GoLoan.Application.Interfaces;
using GoLoan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoLoan.Infrastructure.Repositories
{
    public class CustomerDashboardService : ICustomerDashboardService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public CustomerDashboardService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<DashboardDTO> GetDashboardAsync(int customerId)
        {
            var dashboard = new DashboardDTO();

            var applications = await db.LoanDeals.Where(x => x.CustomerId == customerId).OrderByDescending(x => x.AppliedDate).ToListAsync();

            var loans = await db.LoanAccounts.Where(x => x.CustomerId == customerId).Include(x => x.LoanDeal).ToListAsync();

            var loanAccountIds = loans.Select(x => x.LoanAccountId).ToList();

            var emis = await db.EmiSchedules.Where(x => loanAccountIds.Contains(x.LoanAccountId)).ToListAsync();

            var payments = await db.LoanPayments.Where(x => loanAccountIds.Contains(x.LoanAccountId)).OrderByDescending(x => x.PaymentDate).ToListAsync();

         
            dashboard.Summary.TotalApplications = applications.Count;
            dashboard.Summary.ActiveLoans = loans.Count(x => x.LoanStatus == "Active");
            dashboard.Summary.EmisPaid = emis.Count(x => x.PaymentStatus == "Paid");

            dashboard.Summary.UpcomingEmiAmount = emis.Where(x => x.PaymentStatus != "Paid" && x.DueDate >= DateTime.Today)
                .OrderBy(x => x.DueDate).Select(x => (decimal?)x.Emi).FirstOrDefault() ?? 0;

            dashboard.Summary.TotalAmountPaid = payments.Where(x => x.PaymentStatus == "Success" || x.PaymentStatus == "Completed").Sum(x => x.PaidAmount);

          
            dashboard.Applications = mapper.Map<List<LoanApplicationDTO>>(applications);

           
            dashboard.ActiveLoans = mapper.Map<List<ActiveLoanDTO>>(
                loans.Where(x => x.LoanStatus == "Active").ToList());

           
            var paidEmis = emis.Where(x => x.PaymentStatus == "Paid").ToList();
            var unpaidEmis = emis.Where(x => x.PaymentStatus != "Paid").ToList();

            var nextEmi = unpaidEmis.Where(x => x.DueDate >= DateTime.Today).OrderBy(x => x.DueDate).FirstOrDefault();

            dashboard.EmiSummary.TotalEmis = emis.Count;
            dashboard.EmiSummary.PaidEmis = paidEmis.Count;
            dashboard.EmiSummary.UpcomingEmis = unpaidEmis.Count(x => x.DueDate >= DateTime.Today);
            dashboard.EmiSummary.RemainingEmis = unpaidEmis.Count;
            dashboard.EmiSummary.TotalEmiAmount = emis.Sum(x => x.Emi);
            dashboard.EmiSummary.PaidEmiAmount = paidEmis.Sum(x => x.Emi);
            dashboard.EmiSummary.RemainingEmiAmount = unpaidEmis.Sum(x => x.Emi);
            dashboard.EmiSummary.NextEmiDate = nextEmi?.DueDate;
            dashboard.EmiSummary.NextEmiAmount = nextEmi?.Emi;

  
            dashboard.RecentPayments = mapper.Map<List<RecentPaymentDTO>>(
                payments.Take(5).ToList());

            var dealIds = applications.Select(x => x.DealId).ToList();

            var sanctionLetters = await db.SanctionLetters.Where(x => dealIds.Contains(x.DealId)).OrderByDescending(x => x.CreatedAt).ToListAsync();

            dashboard.Documents = mapper.Map<List<LoanDocumentDTO>>(sanctionLetters);

            return dashboard;
        }
    }
}

