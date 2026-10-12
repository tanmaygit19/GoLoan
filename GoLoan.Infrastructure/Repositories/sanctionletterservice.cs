
using AutoMapper;
using GoLoan.Application.DTO;
using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using GoLoan.Infrastructure.Data;

namespace GoLoan.Infrastructure.Services
{
    public class sanctionletterservice : Isanctionletter
    {
        AppDbContext db;
        IMapper mapper;

        public sanctionletterservice(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public int generatesanction(int loandealid)
        {
            var loan = db.LoanDeals
                .FirstOrDefault(x => x.DealId == loandealid);

            if (loan == null)
            {
                return 0;
            }

            double rate;

            if (loan.TenureMonths == 36)
            {
                rate = 11.5;
            }
            else if (loan.TenureMonths == 60 ||
                     loan.TenureMonths == 120)
            {
                rate = 11;
            }
            else
            {
                return 0;
            }

            decimal loanamount = loan.LoanAmount;

            decimal totalinterest = loanamount * (decimal)rate
                * loan.TenureMonths / (100 * 12);

            decimal emi = (loanamount + totalinterest)
                / loan.TenureMonths;

            var data = new SanctionLetterDTO
            {
                DealId = loan.DealId,
                LoanType=loan.LoanType,
                LoanAmount = loan.LoanAmount,
                InterestRate = rate,
                TenureMonths = loan.TenureMonths,
                EmiAmount = Math.Round(emi, 2),
                CreatedAt = DateTime.Now
            };

            var sanction = mapper.Map<SanctionLetter>(data);

            db.SanctionLetters.Add(sanction);
            db.SaveChanges();

            return sanction.SanctionId;
        }

        public List<SanctionLetterDTO> fetchsanction()
        {
            var data = db.SanctionLetters.ToList();

            var mdata = mapper.Map<List<SanctionLetterDTO>>(data);

            return mdata;
        }

        public SanctionLetterDTO? fetchsanctionbyid(int sanctionid)
        {
            var data = db.SanctionLetters
                .FirstOrDefault(x => x.SanctionId == sanctionid);

            if (data != null)
            {
                var mdata = mapper.Map<SanctionLetterDTO>(data);

                return mdata;
            }

            return null;
            
        }
    }
}