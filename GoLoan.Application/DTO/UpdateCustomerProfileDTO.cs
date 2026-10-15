namespace GoLoan.Application.DTO
{
    public class UpdateCustomerProfileDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Mobile { get; set; }
        public int? Age { get; set; }
        public string? EmploymentType { get; set; }
        public decimal? MonthlyIncome { get; set; }
    }
}
