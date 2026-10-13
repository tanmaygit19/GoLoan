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

        void approvedisbursement(int disbursementid);

        void rejectdisbursement(int disbursementid);

        List<DisbursementDTO2> fetchdisbursementsbystatus(string status);

    }
}
