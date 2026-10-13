using GoLoan.Application.DTO;
using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using GoLoan.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Infrastructure.Repositories
{
    public class EmiPaymentService : IEmiPaymentService
    {
        AppDbContext data;
        RazorpayService razorpayService;
        public EmiPaymentService(AppDbContext data,RazorpayService razorpayService)
        {
            this.data = data;
            this.razorpayService = razorpayService;
        }



        public async Task<EmiPaymentDetailsDto> GetEmiPaymentDetails(int emiScheduleId)
        {
            var emi = await data.EmiSchedules.FirstOrDefaultAsync(x => x.EmiScheduleId == emiScheduleId);

            if (emi == null)
            {
                throw new Exception("EMI not found");
            }

            if (emi.PaymentStatus == "Paid")
            {
                throw new Exception("EMI already paid");
            }

            int lateDays = 0;
            int penalty = 0;
            int bonus = 0;

            if (DateTime.Today > emi.DueDate.Date)
            {
                lateDays = (DateTime.Today - emi.DueDate.Date).Days;
                penalty = lateDays * 1;
            }
            else
            {
                bonus = 10;
            }

            decimal total = emi.Emi + penalty - bonus;

            return new EmiPaymentDetailsDto
            {
                EmiScheduleId = emi.EmiScheduleId,
                InstallmentNo = emi.InstallmentNo,
                EmiPay = emi.Emi,
                Bonus = bonus,
                Penalty = penalty,
                LateDays = lateDays,
                GST = 0,
                TotalAmount = total
            };
        }


        public async Task<string> CreateEmiPaymentOrder(int emiScheduleId)
        {
            var emi = await data.EmiSchedules.FirstOrDefaultAsync(x => x.EmiScheduleId == emiScheduleId);

            if (emi == null)
            {
                throw new Exception("EMI not found");
            }

            if (emi.PaymentStatus == "Paid")
            {
                throw new Exception("EMI already paid");
            }

            
            if (!string.IsNullOrEmpty(emi.RazorpayOrderId))
            {
                return emi.RazorpayOrderId;
            }
               

            var details = await GetEmiPaymentDetails(emiScheduleId);

            string orderId = razorpayService.CreateOrder(details.TotalAmount,emiScheduleId);

            emi.RazorpayOrderId = orderId;

            await data.SaveChangesAsync();

            return orderId;
        }


        public async Task VerifyEmiPayment(VerifyEmiPaymentDto dto)
          { 
             var emi = await data.EmiSchedules.FirstOrDefaultAsync(x => x.EmiScheduleId == dto.EmiScheduleId);

            if (emi == null)
            {
                throw new Exception("EMI not found");
            }

            if (emi.PaymentStatus == "Paid")
            {
                throw new Exception("EMI already paid");
            }

            var details = await GetEmiPaymentDetails(dto.EmiScheduleId);

            razorpayService.VerifyPayment(dto.RazorpayOrderId,dto.RazorpayPaymentId, dto.RazorpaySignature);

            bool isValid = razorpayService.VerifyPaymentDetails(dto.RazorpayPaymentId,dto.RazorpayOrderId,details.TotalAmount);

            if (!isValid)
            {
                throw new Exception("Payment verification failed");
            }

            emi.PaymentStatus = "Paid";
            emi.PaidDate = DateTime.Now;

            data.LoanPayments.Add(new LoanPayment
            {
                LoanAccountId = emi.LoanAccountId,
                EmiScheduleId = emi.EmiScheduleId,
                PaymentName = "EMI",
                PaymentDate = DateTime.Now,
                PaidAmount = details.TotalAmount,
                PaymentMode = "Online",
                TransactionReference = dto.RazorpayPaymentId,
                PrincipalPaid = emi.PrincipalAmount,
                InterestPaid = emi.InterestAmount,
                PenaltyPaid = details.Penalty,
                PaymentStatus = "Success"
            });

            await data.SaveChangesAsync();
        }


    }
}
