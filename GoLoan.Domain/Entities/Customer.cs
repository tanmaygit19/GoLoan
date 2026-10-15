using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace GoLoan.Domain.Entities
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [RegularExpression(@"^[A-Za-z]{2,12}$", ErrorMessage = "It should be alphabet only")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        [RegularExpression(@"^[A-Za-z]{2,12}$", ErrorMessage = "It should be alphabet only")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required")]
        [Range(18, 60, ErrorMessage = "Age must be between 18 and 60")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile Number is required")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter valid 10 digit mobile number")]
        public string MobileNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "PAN Number is required")]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN Number")]
        public string Pan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Aadhaar Number is required")]
        [RegularExpression(@"^[0-9]{12}$", ErrorMessage = "Aadhaar must be 12 digits")]
        public string AadhaarNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Employment Type is required")]
        public string EmploymentType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Monthly Income is required")]
        [Range(1000, 10000000, ErrorMessage = "Enter valid income")]
        public decimal MonthlyIncome { get; set; }

        [Required(ErrorMessage = "Monthly Investment is required")]
        [Range(0, 10000000, ErrorMessage = "Enter valid investment amount")]
        public decimal MonthlyInvestment { get; set; }

        public bool IsEmailVerified { get; set; } = false;

        public List<KycDocument>? KycDocuments { get; set; }
        public List<CibilReport>? CibilReports { get; set; }
        public List<EligibilityResult>? EligibilityResults { get; set; }
        public List<ScoreCard>? ScoreCards { get; set; }
        public List<LoanDeal>? LoanDeals { get; set; }
    }
}
