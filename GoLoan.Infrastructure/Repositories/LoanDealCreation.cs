using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using GoLoan.Infrastructure.Data;
using GoLoan.Application.DTO;
using AutoMapper;

namespace GoLoan.Infrastructure.Services
{
    public class LoanDealCreation : ILoanDealCreation
    {
        AppDbContext db;
        IMapper mapper;

        public LoanDealCreation(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public void addloan(ApplyLoanDTO app,int customer)
        {
            var apply = mapper.Map<LoanDeal>(app);
            apply.CurrentStatus = "Pending";
            db.LoanDeals.Add(apply);
            db.SaveChanges();
        }

        public List<ApplyLoanDTO> fetchdetails()
        {
            var data = db.LoanDeals.ToList();
            var mdata = mapper.Map<List<ApplyLoanDTO>>(data);
            return mdata;
        }

        public ApplyLoanDTO? fetchloanbyid(int customerid)
        {
            var data = db.LoanDeals.FirstOrDefault(x => x.CustomerId == customerid) ;
            if (data != null)
            {
                var mdata = mapper.Map<ApplyLoanDTO>(data);
                return mdata;
            }
            return null;

        }


        public void approveloan(LoanApprovalDTO app)
        {
            var data = db.LoanDeals.FirstOrDefault(x => x.DealId == app.DealId);
            if (data != null && data.CurrentStatus == "Pending")
            {
                data.ApprovedAmount = app.ApprovedAmount;
                data.CurrentStatus = "Approved";
                db.SaveChanges();
            }
        }

        public void rejectloan(LoanApprovalDTO app)
        {
            var data = db.LoanDeals.FirstOrDefault(x => x.DealId == app.DealId);
            if (data != null && data.CurrentStatus == "Pending")
            {
                data.CurrentStatus = "Rejected";
                data.RejectionReason = app.RejectionReason;
                db.SaveChanges();
            }
        }

        public List<LoanStatusDTO> fetchloansbystatus(string status)
        {
            var data = db.LoanDeals.Where(x =>x.CurrentStatus == status).ToList();
            var mdata = mapper.Map<List<LoanStatusDTO>>(data);
            return mdata;
        }


    }
}