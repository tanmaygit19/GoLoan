using global::GoLoan.Domain.Entities;
using global::GoLoan.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoLoan.Application.DTO.Kyc;
namespace GoLoan.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class KycDocumentController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public KycDocumentController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(KycUploadDTO dto)
        {
            try
            {
                var file = dto.File ?? Request.Form.Files.FirstOrDefault();
                if (file == null || file.Length == 0)
                    return BadRequest(new { success = false, message = "Please select a file to upload." });

                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (fileExtension != ".pdf" || file.ContentType != "application/pdf")
                {
                    return BadRequest(new { success = false, message = "Only PDF documents (.pdf) are allowed." });
                }

                if (file.Length > 10 * 1024 * 1024)
                {
                    return BadRequest(new { success = false, message = "File size cannot exceed 10 MB." });
                }

                var customer = await _db.Customers.FindAsync(dto.CustomerId);
                if (customer == null)
                    return NotFound(new { success = false, message = "Customer not found." });

                var existingDocs = await _db.KycDocuments
                    .Where(x => x.CustomerId == dto.CustomerId)
                    .ToListAsync();

                if (existingDocs.Count == 3 && existingDocs.All(x => x.VerificationStatus == "Approved"))
                {
                    return BadRequest(new { success = false, message = "KYC is already fully verified and approved." });
                }

                var existingTypeDoc = existingDocs.FirstOrDefault(x => x.DocumentType.Equals(dto.DocumentType, StringComparison.OrdinalIgnoreCase));
                if (existingTypeDoc != null && existingTypeDoc.VerificationStatus == "Approved")
                {
                    return BadRequest(new { success = false, message = $"{dto.DocumentType} document is already approved." });
                }

                var uploadDir = Path.Combine(_env.ContentRootPath, "wwwroot", "Documents");
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var fullPath = Path.Combine(uploadDir, uniqueFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = $"/Documents/{uniqueFileName}";

                if (existingTypeDoc != null)
                {
                    existingTypeDoc.FilePath = relativePath;
                    existingTypeDoc.VerificationStatus = "Pending";
                }
                else
                {
                    var newDoc = new KycDocument
                    {
                        CustomerId = dto.CustomerId,
                        DocumentType = dto.DocumentType,
                        FilePath = relativePath,
                        VerificationStatus = "Pending"
                    };
                    _db.KycDocuments.Add(newDoc);
                }

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = $"{dto.DocumentType} uploaded successfully as PDF.",
                    filePath = relativePath
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }



        [HttpGet("Customer/{customerId}")]
        public async Task<IActionResult> GetCustomerDocuments(int customerId)
        {
            var docs = await _db.KycDocuments
                .Where(x => x.CustomerId == customerId)
                .Select(x => new
                {
                    x.DocumentId,
                    x.CustomerId,
                    x.DocumentType,
                    x.FilePath,
                    x.VerificationStatus
                })
                .ToListAsync();

            bool isAllApproved = docs.Count == 3 && docs.All(x => x.VerificationStatus == "Approved");

            return Ok(new
            {
                success = true,
                isKycApproved = isAllApproved,
                documents = docs
            });
        }


        [HttpDelete("{documentId}")]
        public async Task<IActionResult> DeleteDocument(int documentId)
        {
            var doc = await _db.KycDocuments.FindAsync(documentId);
            if (doc == null)
                return NotFound(new { success = false, message = "Document not found." });

            if (doc.VerificationStatus == "Approved")
                return BadRequest(new { success = false, message = "Approved documents cannot be deleted." });

            var fullPath = Path.Combine(_env.ContentRootPath, "wwwroot", doc.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            _db.KycDocuments.Remove(doc);
            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "Document deleted successfully." });
        }

        [HttpGet("Officer/Index")]
        public async Task<IActionResult> GetOfficerKycList()
        {
            var customers = await _db.Customers
                .Include(c => c.KycDocuments)
                .Where(c => c.KycDocuments.Any())
                .Select(c => new
                {
                    customerId = c.CustomerId,
                    customerName = $"{c.FirstName} {c.LastName}",
                    email = c.Email,
                    mobile = c.MobileNo,
                    totalUploaded = c.KycDocuments.Count,
                    approvedCount = c.KycDocuments.Count(d => d.VerificationStatus == "Approved"),
                    pendingCount = c.KycDocuments.Count(d => d.VerificationStatus == "Pending"),
                    rejectedCount = c.KycDocuments.Count(d => d.VerificationStatus == "Rejected"),
                    status = (c.KycDocuments.Count == 3 && c.KycDocuments.All(d => d.VerificationStatus == "Approved"))
                        ? "Approved"
                        : c.KycDocuments.Any(d => d.VerificationStatus == "Rejected")
                            ? "Rejected"
                            : "Pending"
                })
                .ToListAsync();
            return Ok(customers);
        }


        [HttpGet("Officer/CustomerDocs/{customerId}")]
        public async Task<IActionResult> GetCustomerDocsForReview(int customerId)
        {
            var customer = await _db.Customers
                .Include(c => c.KycDocuments)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
                return NotFound(new { success = false, message = "Customer not found." });

            return Ok(new
            {
                customerId = customer.CustomerId,
                customerName = $"{customer.FirstName} {customer.LastName}",
                email = customer.Email,
                mobile = customer.MobileNo,
                pan = customer.Pan,
                aadhaarNo = customer.AadhaarNo,
                documents = customer.KycDocuments.Select(d => new
                {
                    d.DocumentId,
                    d.DocumentType,
                    d.FilePath,
                    d.VerificationStatus
                })
            });
        }

        [HttpPut("Approve/{documentId}")]
        public async Task<IActionResult> ApproveDocument(int documentId)
        {
            var doc = await _db.KycDocuments.FindAsync(documentId);
            if (doc == null)
                return NotFound(new { success = false, message = "Document not found." });

            doc.VerificationStatus = "Approved";
            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "Document approved successfully." });
        }

        [HttpPut("Reject/{documentId}")]
        public async Task<IActionResult> RejectDocument(int documentId)
        {
            var doc = await _db.KycDocuments.FindAsync(documentId);
            if (doc == null)
                return NotFound(new { success = false, message = "Document not found." });

            doc.VerificationStatus = "Rejected";
            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "Document rejected." });
        }

        [HttpGet("DocumentTypes")]
        public IActionResult GetDocumentTypes()
        {
            var documentTypes = new List<string>
    {
        "Aadhaar Card",
        "PAN Card",
        "Salary Slip"
    };

            return Ok(new
            {
                success = true,
                data = documentTypes
            });
        }

        [HttpPost("BulkUpdate")]
        public async Task<IActionResult> BulkUpdate(BulkKycUpdateDTO dto)
        {
            var docs = await _db.KycDocuments
                .Where(x => dto.DocumentIds.Contains(x.DocumentId))
                .ToListAsync();

            foreach (var doc in docs)
            {
                doc.VerificationStatus = dto.ActionType == "Approve" ? "Approved" : "Rejected";
            }

            await _db.SaveChangesAsync();
            return Ok(new { success = true, message = $"Bulk update {dto.ActionType} completed successfully." });
        }
    }
}