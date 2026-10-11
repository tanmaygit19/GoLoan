namespace GoLoan.Application.DTO.LMSReports
{
    public class OverdueReportDTO
    {
        public string DaysOverdue { get; set; } = string.Empty;
        public int Accounts { get; set; }
        public decimal OutstandingAmount { get; set; }
    }
}