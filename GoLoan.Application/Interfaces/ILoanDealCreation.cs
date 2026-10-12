using GoLoan.Application.DTO;
using GoLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.Interfaces
{
    public  interface ILoanDealCreation
    {
        void addloan(ApplyLoanDTO app);

        List<ApplyLoanDTO> fetchdetails();

        ApplyLoanDTO? fetchloanbyid(int customerid);
    }
}
