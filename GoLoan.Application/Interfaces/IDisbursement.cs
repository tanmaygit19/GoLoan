using GoLoan.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.Interfaces
{
    public interface IDisbursement
    {
        void adddisbursement(DisbursementDTO dis);

        List<DisbursementDTO2> fetchdisbursement();

        DisbursementDTO2 fetchdisbursementbyid(int id);
    }
}
