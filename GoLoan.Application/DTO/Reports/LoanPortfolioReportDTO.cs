namespace GoLoan.Application.DTO.LMSReports
{
    public class LoanPortfolioReportDTO
    {
        public string Product { get; set; } = string.Empty;
        public int ActiveLoans { get; set; }
        public decimal OutstandingAmount { get; set; }
        public decimal Percentage { get; set; }
    }
}