using GoLoan.Application.DTO;
using GoLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.Interfaces
{
    public interface IForeClosureService
    {
        Task CreateForeClosureRequest(ForeClosureRequestDto dto);
        Task<List<GetForeClosureRequestDto>> GetAllForeClosureRequests();
        Task<List<GetForeClosureRequestAdminDto>>GetAllForeClosureRequestsForAdmin();
        Task<List<GetClosureRequestDto>> GetAllClosureRequests();
        Task ApproveRequest(int requestId);
        Task RejectRequest(int requestId);
        Task<string> CreateForeClosurePaymentOrder(int requestId);
        Task VerifyForeClosurePayment(VerifyForeClosurePaymentDto dto);
    }
}
