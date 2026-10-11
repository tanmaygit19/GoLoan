using GoLoan.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.Interfaces
{
    public interface IEmiPaymentService
    {
        Task<EmiPaymentDetailsDto> GetEmiPaymentDetails(int emiScheduleId);

        Task<string> CreateEmiPaymentOrder(int emiScheduleId);
        Task VerifyEmiPayment(VerifyEmiPaymentDto dto);
    }
}
