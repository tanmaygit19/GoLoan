using Microsoft.AspNetCore.Http;

namespace GoLoan.Application.DTO.Kyc
{
    public class KycUploadDTO
    {
        public int CustomerId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public IFormFile? File { get; set; }
    }
}
