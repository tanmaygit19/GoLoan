
using GoLoan.Application.DTO;
using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using GoLoan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Infrastructure.Repositories
{
    public class ForeClosureService : IForeClosureService
    {
        AppDbContext data;
        RazorpayService razorpayService;
        public ForeClosureService(AppDbContext data, RazorpayService razorpayService)
        {
            this.data = data;
            this.razorpayService = razorpayService;
        }


        public async Task  CreateForeClosureRequest(ForeClosureRequestDto dto)
        {
            var loanAccount = await data.LoanAccounts.FirstOrDefaultAsync(x => x.LoanAccountId == dto.LoanAccountId);
            if (loanAccount == null)
            {
                throw new Exception("Loan Account Not Found");
            }

            if(loanAccount.LoanStatus == "Closed")
            {
                throw new Exception("Cannot create foreclosure request for a closed loan account");
            }

            var existrequest = await data.ForeClosureRequests.FirstOrDefaultAsync(x => x.LoanAccountId == dto.LoanAccountId && (x.Status == "Pending" || ( x.Status == "Approved" && !x.IsPaid)));


            if(existrequest != null)
            {
                throw new Exception("An existing foreclosure request is still pending payment");
            }

            if(dto.ForeClosureType != "Full" && dto.ForeClosureType != "Partial")
            {
                throw new Exception("Invalid ForeClosure Type");
            }

            if (dto.ForeClosureType == "Partial")
            {
                if (dto.NoOfEmi == null || dto.NoOfEmi <= 0)
                {
                    throw new Exception("No. of EMI must be greater than 0");
                }
            }


            decimal amount = 0;

            if (dto.ForeClosureType == "Full")
            {
                amount = loanAccount.OutstandingPrincipal;
            }

            else
            {
                var emiList = await data.EmiSchedules.Where(x => x.LoanAccountId == dto.LoanAccountId && x.PaymentStatus == "Pending").OrderBy(x => x.InstallmentNo).Take(dto.NoOfEmi.Value).ToListAsync();


                if (emiList.Count < dto.NoOfEmi.Value)
                {
                    throw new Exception("Not Enough Pending EMIs");

                }

                foreach (var emi in emiList)
                {
                    amount = amount + emi.PrincipalAmount;
                }
            }

            var request = new ForeClosureRequest
            {
                LoanAccountId = dto.LoanAccountId,
                ForeClosureType = dto.ForeClosureType,
                ForeClosureAmount = amount,
                NoOfEmi = dto.NoOfEmi,
                RequestedDate = DateTime.Now,
                ExpectedClosureDate = DateTime.Now,
                Reason = dto.Reason,
                Status = "Pending",
                IsPaid = false
            };

            data.ForeClosureRequests.Add(request);

            await data.SaveChangesAsync();

        }



        public async Task ApproveRequest(int requestId)
        {
            var request = await data.ForeClosureRequests.FirstOrDefaultAsync(x => x.RequestId == requestId);


            if(request == null)
            {
                throw new Exception("ForeClosure Request Not Found");
            }

            if(request.Status != "Pending")
            {
                throw new Exception("Only Pending Requests Can Be Approved");

            }

            request.Status = "Approved";
            request.ApprovedDate = DateTime.Now;

            await data.SaveChangesAsync();
        }


        public async Task RejectRequest(int requestId)
        {
            var request = await data.ForeClosureRequests.FirstOrDefaultAsync(x => x.RequestId == requestId);

            if(request == null)
            {
                throw new Exception("Foreclosure Request Not Found");
            }


            if( request.Status != "Pending")
            {
                throw new Exception("Only Pending Requests Can Be Rejected");
            }

            request.Status = "Rejected";
            await data.SaveChangesAsync();
        }



        public async Task<string> CreateForeClosurePaymentOrder(int requestId)
        {
            var request = await data.ForeClosureRequests.FirstOrDefaultAsync(x => x.RequestId == requestId);

            if (request == null)
            {
                throw new Exception("Request not found");
            }

            if (request.Status != "Approved")
            {
                throw new Exception("Request is not approved");
            }

            if (request.IsPaid)
            {
                throw new Exception("Already paid");
            }

            if (!string.IsNullOrEmpty(request.RazorpayOrderId))
            {
                return request.RazorpayOrderId;
            }

            string orderId = razorpayService.CreateOrder(request.ForeClosureAmount, requestId);

            request.RazorpayOrderId = orderId;

            await data.SaveChangesAsync();

            return orderId;
        }

        public async Task VerifyForeClosurePayment(VerifyForeClosurePaymentDto dto)
        {
            var request = await data.ForeClosureRequests.FirstOrDefaultAsync(x => x.RequestId == dto.RequestId);

            if (request == null)
            {
                throw new Exception("Request not found");
            }

            if (request.Status != "Approved" || request.IsPaid)
            {
                throw new Exception("Request is not valid for payment");
            }

            if (request.RazorpayOrderId != dto.RazorpayOrderId)
            {
                throw new Exception("Invalid Order ID");
            }


            bool signatureValid = razorpayService.VerifyPayment(dto.RazorpayOrderId,dto.RazorpayPaymentId,dto.RazorpaySignature);

            bool paymentValid = razorpayService.VerifyPaymentDetails(dto.RazorpayPaymentId,dto.RazorpayOrderId,request.ForeClosureAmount);

            if (!signatureValid || !paymentValid)
            {
                throw new Exception("Payment verification failed");
            }

     
            var payment = new LoanPayment
            {
                LoanAccountId = request.LoanAccountId,
                PaymentName = "Foreclosure",
                PaymentDate = DateTime.Now,
                PaidAmount = request.ForeClosureAmount,
                PaymentMode = "Online",
                TransactionReference = dto.RazorpayPaymentId,
                PrincipalPaid = request.ForeClosureAmount,
                InterestPaid = 0,
                PenaltyPaid = 0,
                PaymentStatus = "Success"
            };

            data.LoanPayments.Add(payment);

            request.IsPaid = true;
            request.PaidDate = DateTime.Now;
            request.Status = "Completed";

            if (request.ForeClosureType == "Full")
            {
                var loanAccount = await data.LoanAccounts.FirstOrDefaultAsync(x => x.LoanAccountId == request.LoanAccountId);

                if (loanAccount == null)
                {
                    throw new Exception("Loan Account Not Found");
                }

                loanAccount.LoanStatus = "Closed";
                loanAccount.OutstandingPrincipal = 0;
            }

            await data.SaveChangesAsync();
        }
    }
}
