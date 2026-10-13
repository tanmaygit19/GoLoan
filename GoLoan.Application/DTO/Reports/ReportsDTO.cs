namespace GoLoan.Application.DTO.Reports
{
    public class ReportsDashboardDTO
    {
        public ReportsSummaryDTO Summary { get; set; } = new();
        public List<ApplicationFunnelDTO> ApplicationFunnel { get; set; } = new();
        public List<CibilScoreRangeDTO> CibilScoreRanges { get; set; } = new();
        public List<ScoreCardWiseDTO> ScoreCardWise { get; set; } = new();
        public List<ProductWiseDTO> ProductWise { get; set; } = new();
    }

    public class ReportsSummaryDTO
    {
        public int TotalApplications { get; set; }
        public int ApplicationsApproved { get; set; }
        public int ApplicationsRejected { get; set; }
        public int ApplicationsPending { get; set; }
    }

    public class ApplicationFunnelDTO
    {
        public string Stage { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class CibilScoreRangeDTO
    {
        public string CibilScoreRange { get; set; } = string.Empty;
        public int Applicants { get; set; }
        public int Approved { get; set; }
        public decimal ApprovalPercentage { get; set; }
    }

    public class ScoreCardWiseDTO
    {
        public string ScoreCard { get; set; } = string.Empty;
        public int Applicants { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int Pending { get; set; }
        public decimal ApprovalPercentage { get; set; }
    }

    public class ProductWiseDTO
    {
        public string Product { get; set; } = string.Empty;
        public int TotalApplications { get; set; }
        public int Approved { get; set; }
        public int Disbursed { get; set; }
        public decimal DisbursementPercentage { get; set; }
    }

    public class TurnaroundTimeDTO
    {
        public string Stage { get; set; } = string.Empty;
        public decimal AverageDays { get; set; }
        public int MinimumDays { get; set; }
        public int MaximumDays { get; set; }
    }
}

