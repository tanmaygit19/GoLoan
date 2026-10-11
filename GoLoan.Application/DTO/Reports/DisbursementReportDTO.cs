namespace GoLoan.Application.DTO.LMSReports
{
    public class DisbursementReportDTO
    {
        public string Month { get; set; } = string.Empty;
        public int LoansDisbursed { get; set; }
        public decimal DisbursementAmount { get; set; }
    }
}