using AutoMapper;
using GoLoan.Application.DTO.Dashboard;
using GoLoan.Domain.Entities;

namespace GoLoan.Application.Mapper
{
    public class DashboardMappingProfile : Profile
    {
        public DashboardMappingProfile()
        {
            CreateMap<LoanDeal, LoanApplicationDTO>()
            .ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(src => src.DealId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.CurrentStatus));

        CreateMap<LoanAccount, ActiveLoanDTO>()
            .ForMember(dest => dest.LoanType, opt => opt.MapFrom(src => src.LoanDeal != null ? src.LoanDeal.LoanType : string.Empty));

            CreateMap<LoanPayment, RecentPaymentDTO>();

            CreateMap<SanctionLetter, LoanDocumentDTO>()
                .ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => src.SanctionId))
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => "Sanction Letter"))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt));
        }
    }

}
