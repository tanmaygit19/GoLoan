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

        public void addloan(ApplyLoanDTO app)
        {
            var apply = mapper.Map<LoanDeal>(app);

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
    }
}