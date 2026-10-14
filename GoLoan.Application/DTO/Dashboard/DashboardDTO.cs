using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.DTO.Dashboard
{
    public class DashboardDTO
    {
        public DashboardSummaryDTO Summary { get; set; } = new();
        public List<LoanApplicationDTO> Applications { get; set; } = new();
        public List<ActiveLoanDTO> ActiveLoans { get; set; } = new();
        public EmiSummaryDTO EmiSummary { get; set; } = new();
        public List<RecentPaymentDTO> RecentPayments { get; set; } = new();
        public List<LoanDocumentDTO> Documents { get; set; } = new();
    }

    public class DashboardSummaryDTO
    {
        public int TotalApplications { get; set; }
        public int ActiveLoans { get; set; }
        public int EmisPaid { get; set; }
        public decimal UpcomingEmiAmount { get; set; }
        public decimal TotalAmountPaid { get; set; }
    }

    public class LoanApplicationDTO
    {
        public int ApplicationId { get; set; }
        public string LoanType { get; set; } = string.Empty;
        public decimal LoanAmount { get; set; }
        public decimal ApprovedAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime AppliedDate { get; set; }
    }

    public class ActiveLoanDTO
    {
        public int LoanAccountId { get; set; }
        public string LoanAccountNo { get; set; } = string.Empty;
        public string LoanType { get; set; } = string.Empty;
        public decimal LoanAmount { get; set; }
        public decimal OutstandingPrincipal { get; set; }
        public double InterestRate { get; set; }
        public int TenureMonths { get; set; }
        public decimal EmiAmount { get; set; }
        public DateTime DisbursementDate { get; set; }
        public decimal TotalPaidAmount { get; set; }
    }

    public class EmiSummaryDTO
    {
        public int TotalEmis { get; set; }
        public int PaidEmis { get; set; }
        public int UpcomingEmis { get; set; }
        public int RemainingEmis { get; set; }
        public decimal TotalEmiAmount { get; set; }
        public decimal PaidEmiAmount { get; set; }
        public decimal RemainingEmiAmount { get; set; }
        public DateTime? NextEmiDate { get; set; }
        public decimal? NextEmiAmount { get; set; }
    }

    public class RecentPaymentDTO
    {
        public int PaymentId { get; set; }
        public string PaymentName { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public decimal PaidAmount { get; set; }
        public string PaymentMode { get; set; } = string.Empty;
        public string TransactionReference { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
    }

    public class LoanDocumentDTO
    {
        public int DocumentId { get; set; }
        public int DealId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public decimal LoanAmount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
