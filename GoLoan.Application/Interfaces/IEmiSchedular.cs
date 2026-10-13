using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.Interfaces
{
    public interface IEmiSchedular
    {
        Task GenerateEmiSchedule(int loanAccountId);
        Task ForeClosureEmiPayment(int loanAccountId, int numberOfEmis);
    }
}
