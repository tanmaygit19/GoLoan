using System.Collections.Generic;

namespace GoLoan.Application.DTO.Kyc
{
    public class BulkKycUpdateDTO
    {
        public List<int> DocumentIds { get; set; } = new();
        public string ActionType { get; set; } = "Approve";
    }
}
