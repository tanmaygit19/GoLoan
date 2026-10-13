namespace GoLoan.Application.DTO.LMSReports
{
    public class EMICollectionReportDTO
    {
        public string Particulars { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }
}