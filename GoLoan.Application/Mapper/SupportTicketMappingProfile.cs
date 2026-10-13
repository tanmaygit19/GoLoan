using AutoMapper;
using GoLoan.Domain.Entities;
using GoLoan.Application.DTO.SupportTicket;

namespace GoLoan.Application.Mapper
{
     
    public class SupportTicketMappingProfile : Profile
    { 
        public SupportTicketMappingProfile()
        {
           
            CreateMap<SupportTicket, SupportTicketDTO>();
 
            CreateMap<SupportTicket, SupportTicketDetailsDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src =>
                    src.Customer != null
                        ? src.Customer.FirstName + " " + src.Customer.LastName
                        : string.Empty))
                .ForMember(dest => dest.LoanAccountNo, opt => opt.MapFrom(src =>
                    src.LoanAccount != null
                        ? src.LoanAccount.LoanAccountNo
                        : string.Empty));

           CreateMap<SupportTicket, SupportTicketListDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src =>
                 src.Customer != null
                     ? src.Customer.FirstName + " " + src.Customer.LastName
                     : string.Empty))
                .ForMember(dest => dest.LoanAccountNo, opt => opt.MapFrom(src =>
                     src.LoanAccount != null
                     ? src.LoanAccount.LoanAccountNo
                     : string.Empty));
        }
    }
}

