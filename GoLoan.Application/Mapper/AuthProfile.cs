using AutoMapper;
using GoLoan.Application.DTO.Auth;
using GoLoan.Domain.Entities;

namespace GoLoan.Application.Mapper
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterDTO, User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.IsTwoFaEnabled, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.AuthProvider, opt => opt.MapFrom(_ => "Local"));

            CreateMap<User, UserProfileDTO>()
                .ForMember(dest => dest.RoleName,
                    opt => opt.MapFrom(src => src.Role != null ? src.Role.RoleName : "Customer"));
        }
    }
}
