using GoLoan.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.Interfaces
{
    public interface Isanctionletter
    {
        int generatesanction(int loandealid);
        List<SanctionLetterDTO> fetchsanction();
        SanctionLetterDTO? fetchsanctionbyid(int sanctionid);
        byte[]? downloadsanction(int sanctionid);
    }
}
