namespace GoLoan.Application.DTO.LMSReports
{
    public class PrepaymentReportDTO
    {
        public string Type { get; set; } = string.Empty;
        public int Accounts { get; set; }
        public decimal PrepaymentAmount { get; set; }
    }
}