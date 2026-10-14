using GoLoan.Application.DTO.Dashboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.Interfaces
{
    public interface ICustomerDashboardService
    {
        Task<DashboardDTO> GetDashboardAsync(int customerId);
    }
}
