using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using GoLoan.Application.DTO;
using GoLoan.Domain.Entities;

namespace GoLoan.Application.Mapper
{
    public class LoanAccountMapper : Profile
    {
        public LoanAccountMapper()
        {
            CreateMap<LoanAccount, LoanAccountDTO>();
        }
    }
}
