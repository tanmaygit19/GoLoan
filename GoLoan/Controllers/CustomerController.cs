using global::GoLoan.Domain.Entities;
using global::GoLoan.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoLoan.Application.DTO;

namespace GoLoan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CustomerController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPut("{customerId}/Profile")]
        public async Task<IActionResult> UpdateCustomerProfile(int customerId, UpdateCustomerProfileDTO dto)
        {
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if (customer == null)
            {
                return NotFound(new { success = false, message = "Customer not found." });
            }

            if (!string.IsNullOrWhiteSpace(dto.FirstName))
                customer.FirstName = dto.FirstName.Trim();

            if (!string.IsNullOrWhiteSpace(dto.LastName))
                customer.LastName = dto.LastName.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Mobile))
                customer.MobileNo = dto.Mobile.Trim();

            if (dto.Age.HasValue && dto.Age.Value >= 18 && dto.Age.Value <= 60)
                customer.Age = dto.Age.Value;

            if (!string.IsNullOrWhiteSpace(dto.EmploymentType))
                customer.EmploymentType = dto.EmploymentType.Trim();

            if (dto.MonthlyIncome.HasValue && dto.MonthlyIncome.Value >= 0)
                customer.MonthlyIncome = dto.MonthlyIncome.Value;

            var user = await _db.Users.FirstOrDefaultAsync(u => u.CustomerId == customerId);
            if (user != null)
            {
                if (!string.IsNullOrWhiteSpace(dto.FirstName))
                    user.FirstName = dto.FirstName.Trim();

                if (!string.IsNullOrWhiteSpace(dto.LastName))
                    user.LastName = dto.LastName.Trim();

                if (!string.IsNullOrWhiteSpace(dto.Mobile))
                    user.Mobile = dto.Mobile.Trim();
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Profile updated successfully.",
                profile = new
                {
                    customerId = customer.CustomerId,
                    firstName = customer.FirstName,
                    lastName = customer.LastName,
                    mobile = customer.MobileNo,
                    age = customer.Age,
                    employmentType = customer.EmploymentType,
                    monthlyIncome = customer.MonthlyIncome
                }
            });
        }

        [HttpGet("OfficerCustomerTable")]
        public async Task<IActionResult> GetCustomerTable(
            string? kycStatus = null,
            string? loanStatus = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? search = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _db.Customers
                    .Include(c => c.KycDocuments)
                    .Include(c => c.CibilReports)
                    .Include(c => c.EligibilityResults)
                    .Include(c => c.LoanDeals)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var s = search.Trim().ToLower();
                    query = query.Where(c =>
                        c.FirstName.ToLower().Contains(s) ||
                        c.LastName.ToLower().Contains(s) ||
                        c.Email.ToLower().Contains(s) ||
                        c.MobileNo.Contains(s));
                }

                var customerList = await query.ToListAsync();

                var rows = customerList.Select(c =>
                {
                    var docs = c.KycDocuments ?? new();
                    string kyc = "Pending";
                    if (docs.Count == 3 && docs.All(d => d.VerificationStatus == "Approved"))
                        kyc = "Approved";
                    else if (docs.Any(d => d.VerificationStatus == "Rejected"))
                        kyc = "Rejected";

                    var cibil = c.CibilReports?.OrderByDescending(r => r.checkDate).FirstOrDefault()?.cibilScore ?? 0;

                    var latestDeal = c.LoanDeals?.OrderByDescending(d => d.AppliedDate).FirstOrDefault();
                    decimal loanAmount = latestDeal?.LoanAmount ?? 0;
                    string status = latestDeal?.CurrentStatus ?? "None";
                    DateTime? date = latestDeal?.AppliedDate;

                    return new
                    {
                        id = c.CustomerId,
                        name = $"{c.FirstName} {c.LastName}",
                        kyc = kyc,
                        cibil = cibil,
                        loan = loanAmount,
                        status = status,
                        date = date?.ToString("dd-MM-yyyy") ?? "-",
                        appliedDate = date
                    };
                }).AsQueryable();

                if (!string.IsNullOrWhiteSpace(kycStatus) && kycStatus != "All")
                {
                    rows = rows.Where(r => r.kyc.Equals(kycStatus, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(loanStatus) && loanStatus != "All")
                {
                    rows = rows.Where(r => r.status.Equals(loanStatus, StringComparison.OrdinalIgnoreCase));
                }

                if (fromDate.HasValue)
                {
                    rows = rows.Where(r => r.appliedDate >= fromDate.Value.Date);
                }
                if (toDate.HasValue)
                {
                    rows = rows.Where(r => r.appliedDate <= toDate.Value.Date.AddDays(1));
                }

                var allResults = rows.ToList();
                int totalEntries = allResults.Count;

                var paginatedData = allResults
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(new
                {
                    success = true,
                    total = totalEntries,
                    page = page,
                    pageSize = pageSize,
                    data = paginatedData
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{customerId}/Profile")]
        public async Task<IActionResult> GetCustomerProfile(int customerId)
        {
            var customer = await _db.Customers
                .Include(c => c.KycDocuments)
                .Include(c => c.CibilReports)
                .Include(c => c.EligibilityResults)
                .Include(c => c.LoanDeals)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
                return NotFound(new { success = false, message = "Customer not found." });

            return Ok(new
            {
                success = true,
                profile = new
                {
                    customerId = customer.CustomerId,
                    firstName = customer.FirstName,
                    lastName = customer.LastName,
                    email = customer.Email,
                    mobile = customer.MobileNo,
                    age = customer.Age,
                    pan = customer.Pan,
                    aadhaarNo = customer.AadhaarNo,
                    employmentType = customer.EmploymentType,
                    monthlyIncome = customer.MonthlyIncome,
                    monthlyInvestment = customer.MonthlyInvestment,
                    isEmailVerified = customer.IsEmailVerified,
                    kycDocuments = customer.KycDocuments?.Select(d => new
                    {
                        d.DocumentId,
                        d.DocumentType,
                        d.FilePath,
                        d.VerificationStatus
                    }),
                    cibilScore = customer.CibilReports?.OrderByDescending(x => x.checkDate).FirstOrDefault()?.cibilScore ?? 0,
                    loans = customer.LoanDeals?.Select(l => new
                    {
                        l.DealId,
                        l.LoanType,
                        l.LoanAmount,
                        l.TenureMonths,
                        l.InterestRate,
                        l.CurrentStatus,
                        l.AppliedDate
                    })
                }
            });
        }
    }
}
