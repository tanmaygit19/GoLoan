using AutoMapper;
using GoLoan.Application.DTO;
using GoLoan.Domain.Entities;

namespace GoLoan.Application
{
    public class MappingData : Profile
    {
        public MappingData()
        {
            CreateMap<ApplyLoanDTO, LoanDeal>();
            CreateMap<LoanDeal, ApplyLoanDTO>();

            CreateMap<SanctionLetterDTO, SanctionLetter>();
            CreateMap<SanctionLetter, SanctionLetterDTO>();
            CreateMap<DisbursementDTO, Disbursement>();
            CreateMap<Disbursement, DisbursementDTO2>();
        }
    }
}