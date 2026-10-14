namespace LoanApp.Application.DTOs
{
    public class CibilReportDto
    {
        public int CustomerId { get; set; }
        public int IncomeScore { get; set; }
        public int EmploymentScore { get; set; }
        public int AgeScore { get; set; }
        public int FoirScore { get; set; }
        public int CibilScore { get; set; }
    }
}