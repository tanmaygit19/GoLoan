using AutoMapper;
using GoLoan.Application.DTO;
using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using GoLoan.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace GoLoan.Infrastructure.Repositories
{
    public class DisbursementService : IDisbursement
    {
        AppDbContext db;
        IMapper mapper;

        public DisbursementService(AppDbContext db,IMapper mapper)
        {
            this.db = db;
            this.mapper= mapper;
        }
        public void adddisbursement(DisbursementDTO dis)
        {
            var loan = db.LoanDeals.FirstOrDefault(x => x.DealId == dis.DealId);
            var data = mapper.Map<Disbursement>(dis);
            data.DisbursementDate = DateTime.Now;
            data.Status = "Pending";
            db.Disbursements.Add(data);
            db.SaveChanges();

        }

        public List<DisbursementDTO2> fetchdisbursement()
        {
        
            var data = db.Disbursements.ToList();

            var mdata = mapper.Map<List<DisbursementDTO2>>(data);

            return mdata;
        }

        public DisbursementDTO2 fetchdisbursementbyid(int id)
        {
            var data = db.Disbursements.FirstOrDefault(x => x.DisbursementId == id);

            var mdata = mapper.Map<DisbursementDTO2>(data);

            return mdata;
        }

    }
}
