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
            decimal penalty = 0;
            decimal bonus = 0;

            DateTime today = DateTime.Today;
            DateTime dueDate = emi.DueDate.Date;

            if (today < dueDate)
            {
                throw new Exception($"EMI payment is allowed only on or after {dueDate:dd-MMM-yyyy}");
            }

            if (today == dueDate)
            {
                bonus = 100; 
            }
            else if (today > dueDate)
            {
                lateDays = (today - dueDate).Days;
                penalty = lateDays * 1;
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
            var emi = await data.EmiSchedules
                .FirstOrDefaultAsync(x => x.EmiScheduleId == emiScheduleId);

            if (emi == null)
            {
                throw new Exception("EMI not found");

            }

            if (emi.PaymentStatus == "Paid")
            {
                throw new Exception("EMI already paid");

            }

            var details = await GetEmiPaymentDetails(emiScheduleId);

            if (!string.IsNullOrEmpty(emi.RazorpayOrderId)&& emi.RazorpayAmount == details.TotalAmount)
            {
                long existingAmount = razorpayService.GetOrderAmount(emi.RazorpayOrderId);

                if (existingAmount == decimal.ToInt64(details.TotalAmount * 100))
                {
                    return emi.RazorpayOrderId;
                }
            }

            string orderId = razorpayService.CreateOrder(details.TotalAmount,emiScheduleId);

            emi.RazorpayOrderId = orderId;
            emi.RazorpayAmount = details.TotalAmount;
            emi.RazorpayPenalty = details.Penalty;
            emi.RazorpayBonus = details.Bonus;

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

            if (string.IsNullOrEmpty(emi.RazorpayOrderId) ||
                emi.RazorpayOrderId != dto.RazorpayOrderId ||
                emi.RazorpayAmount == null ||
                emi.RazorpayPenalty == null ||
                emi.RazorpayBonus == null)
            {
                throw new Exception("Invalid or expired EMI payment order");
            }

            decimal finalAmount = emi.RazorpayAmount.Value;
            decimal finalPenalty = emi.RazorpayPenalty.Value;
            decimal finalBonus = emi.RazorpayBonus.Value;

            razorpayService.VerifyPayment(dto.RazorpayOrderId,dto.RazorpayPaymentId, dto.RazorpaySignature);

            bool isValid = razorpayService.VerifyPaymentDetails(dto.RazorpayPaymentId,dto.RazorpayOrderId,finalAmount);
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
                PaidAmount = finalAmount,
                PaymentMode = "Online",
                TransactionReference = dto.RazorpayPaymentId,
                PrincipalPaid = emi.PrincipalAmount,
                InterestPaid = emi.InterestAmount,
                PenaltyPaid = finalPenalty,
                BonusPaid = finalBonus,
                PaymentStatus = "Success"
            });
            var loanAccount = await data.LoanAccounts.FirstOrDefaultAsync(x =>x.LoanAccountId == emi.LoanAccountId);

            if (loanAccount == null)
            {
                throw new Exception("Loan account not found");
            }

            loanAccount.OutstandingPrincipal -= emi.PrincipalAmount;
            loanAccount.TotalPaidAmount += emi.Emi;

            await data.SaveChangesAsync();
        }


    }
}
