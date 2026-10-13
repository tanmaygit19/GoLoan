namespace GoLoan.Application.DTO.LMSReports
{
    public class LMSReportsDashboardDTO
    {
        public LMSReportsSummaryDTO Summary { get; set; } = new();

        public List<LoanPortfolioChartDTO> LoanPortfolioChart { get; set; } = new();

        public List<CollectionEfficiencyChartDTO> CollectionEfficiencyChart { get; set; } = new();
    }

    public class LMSReportsSummaryDTO
    {
        public int TotalLoans { get; set; }
        public int ActiveLoans { get; set; }
        public int ClosedLoans { get; set; }
    }

    public class LoanPortfolioChartDTO
    {
        public string Product { get; set; } = string.Empty;
        public decimal OutstandingAmount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class CollectionEfficiencyChartDTO
    {
        public string Month { get; set; } = string.Empty;
        public decimal CollectionPercentage { get; set; }
    }
}