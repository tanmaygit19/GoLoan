using GoLoan.Application.DTO.Reports;
using GoLoan.Application.Interfaces;
using GoLoan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoLoan.Infrastructure.Repositories
{
    public class ReportsService : IReportsService
    {
        private readonly AppDbContext db;

        public ReportsService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<ReportsDashboardDTO> GetReportsDashboardAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = db.LoanDeals.AsQueryable();

            if (fromDate.HasValue)
            {
                query = query.Where(x => x.AppliedDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                var endDate = toDate.Value.Date.AddDays(1);
                query = query.Where(x => x.AppliedDate < endDate);
            }

            var applications = await query.ToListAsync();

            var dealIds = applications.Select(x => x.DealId).ToList();

            var disbursedDealIds = await db.Disbursements
                .Where(x => dealIds.Contains(x.DealId) && x.Status == "Completed")
                .Select(x => x.DealId)
                .Distinct()
                .ToListAsync();

            var customerIds = applications
                .Select(x => x.CustomerId)
                .Distinct()
                .ToList();

            var cibilReports = await db.CibilReports
                .Where(x => customerIds.Contains(x.CustomerId))
                .ToListAsync();

            var scoreCards = await db.ScoreCards
                .Where(x => customerIds.Contains(x.CustomerId))
                .ToListAsync();

            var dashboard = new ReportsDashboardDTO();

            // 1. Summary
            dashboard.Summary.TotalApplications = applications.Count;

            dashboard.Summary.ApplicationsApproved =
                applications.Count(x => x.CurrentStatus == "Approved");

            dashboard.Summary.ApplicationsRejected =
                applications.Count(x => x.CurrentStatus == "Rejected");

            dashboard.Summary.ApplicationsPending =
                applications.Count(x =>
                    x.CurrentStatus == "Pending" ||
                    x.CurrentStatus == "In Progress" ||
                    x.CurrentStatus == "Under Review");

            // 2. Application Funnel
            dashboard.ApplicationFunnel = new List<ApplicationFunnelDTO>
            {
                new ApplicationFunnelDTO
                {
                    Stage = "Total Applications",
                    Count = applications.Count
                },
                new ApplicationFunnelDTO
                {
                    Stage = "In Progress",
                    Count = applications.Count(x => x.CurrentStatus == "In Progress")
                },
                new ApplicationFunnelDTO
                {
                    Stage = "Under Review",
                    Count = applications.Count(x => x.CurrentStatus == "Under Review")
                },
                new ApplicationFunnelDTO
                {
                    Stage = "Approved",
                    Count = applications.Count(x => x.CurrentStatus == "Approved")
                },
                new ApplicationFunnelDTO
                {
                    Stage = "Disbursed",
                    Count = disbursedDealIds.Count
                }
            };

            // 3. CIBIL Score Range Report
            var cibilGroups = cibilReports
                .GroupBy(x =>
                    x.CibilScore >= 750 ? "750 - 900" :
                    x.CibilScore >= 650 ? "650 - 749" :
                    x.CibilScore >= 550 ? "550 - 649" :
                    "300 - 549")
                .ToList();

            foreach (var group in cibilGroups)
            {
                var groupCustomerIds = group
                    .Select(x => x.CustomerId)
                    .Distinct()
                    .ToList();

                var groupApplications = applications
                    .Where(x => groupCustomerIds.Contains(x.CustomerId))
                    .ToList();

                var applicants = groupApplications.Count;

                var approved = groupApplications.Count(x =>
                    x.CurrentStatus == "Approved");

                dashboard.CibilScoreRanges.Add(new CibilScoreRangeDTO
                {
                    CibilScoreRange = group.Key,
                    Applicants = applicants,
                    Approved = approved,
                    ApprovalPercentage = applicants > 0
                        ? Math.Round((decimal)approved / applicants * 100, 2)
                        : 0
                });
            }

            // 4. Score Card Wise Report
            var scoreCardGroups = scoreCards
                .GroupBy(x => x.RiskCategory)
                .ToList();

            foreach (var group in scoreCardGroups)
            {
                var groupCustomerIds = group
                    .Select(x => x.CustomerId)
                    .Distinct()
                    .ToList();

                var groupApplications = applications
                    .Where(x => groupCustomerIds.Contains(x.CustomerId))
                    .ToList();

                var applicants = groupApplications.Count;

                var approved = groupApplications.Count(x =>
                    x.CurrentStatus == "Approved");

                var rejected = groupApplications.Count(x =>
                    x.CurrentStatus == "Rejected");

                var pending = groupApplications.Count(x =>
                    x.CurrentStatus == "Pending" ||
                    x.CurrentStatus == "In Progress" ||
                    x.CurrentStatus == "Under Review");

                dashboard.ScoreCardWise.Add(new ScoreCardWiseDTO
                {
                    ScoreCard = group.Key,
                    Applicants = applicants,
                    Approved = approved,
                    Rejected = rejected,
                    Pending = pending,
                    ApprovalPercentage = applicants > 0
                        ? Math.Round((decimal)approved / applicants * 100, 2)
                        : 0
                });
            }

            // 5. Product Wise Report
            var productGroups = applications
                .GroupBy(x => x.LoanType)
                .ToList();

            foreach (var group in productGroups)
            {
                var totalApplications = group.Count();

                var approved = group.Count(x =>
                    x.CurrentStatus == "Approved");

                var disbursed = group.Count(x =>
                    disbursedDealIds.Contains(x.DealId));

                dashboard.ProductWise.Add(new ProductWiseDTO
                {
                    Product = group.Key,
                    TotalApplications = totalApplications,
                    Approved = approved,
                    Disbursed = disbursed,
                    DisbursementPercentage = totalApplications > 0
                        ? Math.Round((decimal)disbursed / totalApplications * 100, 2)
                        : 0
                });
            }

            return dashboard;
        }
    }
}