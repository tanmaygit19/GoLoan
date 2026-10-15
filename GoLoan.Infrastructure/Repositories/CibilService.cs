using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using GoLoan.Infrastructure.Data;
using LoanApp.Application.DTOs;

namespace GoLoan.Infrastructure.Repositories
{
    public class CibilService : ICibilService
    {
        AppDbContext db;

        public CibilService(AppDbContext db)
        {
            this.db = db;
        }

        // Generate and save CIBIL report
        public CibilReportDto GenerateCibilReport(int id)
        {
            var data = db.Customers.Find(id);

            if (data == null)
            {
                return null;
            }

            decimal income = data.MonthlyIncome;
            int incomeScore = 0;

            if (income < 25000)
            {
                incomeScore = 100;
            }
            else if (income < 50000)
            {
                incomeScore = 200;
            }
            else if (income <= 100000)
            {
                incomeScore = 300;
            }
            else
            {
                incomeScore = 400;
            }

            string employmentType = data.EmploymentType;
            int employmentScore = 0;

            if (employmentType == "Government job")
            {
                employmentScore = 200;
            }
            else if (employmentType == "Private job")
            {
                employmentScore = 150;
            }
            else if (employmentType == "Self-employed" ||
                     employmentType == "Business")
            {
                employmentScore = 100;
            }
            else
            {
                throw new Exception("Invalid employment type.");
            }

            int age = data.Age;
            int ageScore = 0;

            if (age >= 21 && age <= 24)
            {
                ageScore = 50;
            }
            else if (age >= 25 && age <= 45)
            {
                ageScore = 150;
            }
            else if (age >= 46 && age <= 60)
            {
                ageScore = 100;
            }
            else
            {
                throw new Exception(
                    "Customer age must be between 21 and 60.");
            }

            // FOIR calculation
            if (income <= 0)
            {
                throw new Exception(
                    "Monthly income must be greater than zero.");
            }

            decimal totalMonthlyDebtPayment = income * 0.40m;
            decimal foir = (totalMonthlyDebtPayment / income) * 100;

            int foirScore = 0;

            if (foir < 30)
            {
                foirScore = 250;
            }
            else if (foir <= 50)
            {
                foirScore = 150;
            }
            else if (foir <= 60)
            {
                foirScore = 75;
            }
            else
            {
                foirScore = 0;
            }

            int cibilScore = incomeScore + employmentScore+ ageScore + foirScore;

            // Save CIBIL report
            CibilReport cibilReport = new CibilReport();

            cibilReport.customerId = data.CustomerId;
            cibilReport.panNo = data.Pan;
            cibilReport.cibilScore = cibilScore;
            cibilReport.checkDate = DateTime.Now;

            db.CibilReports.Add(cibilReport);
            db.SaveChanges();

            // Return generated report
            CibilReportDto report = new CibilReportDto();

            report.CustomerId = data.CustomerId;
            report.IncomeScore = incomeScore;
            report.EmploymentScore = employmentScore;
            report.AgeScore = ageScore;
            report.FoirScore = foirScore;
            report.CibilScore = cibilScore;

            return report;
        }

        // View latest saved CIBIL report
        public CibilReportViewDto? GetCibilReport(int id)
        {
            var data = db.CibilReports
                .Where(x => x.customerId == id)
                .OrderByDescending(x => x.checkDate)
                .FirstOrDefault();

            if (data == null)
            {
                return null;
            }

            CibilReportViewDto report = new CibilReportViewDto();

            report.CustomerId = data.customerId;
            report.CibilScore = data.cibilScore;

            return report;
        }
    }
}
