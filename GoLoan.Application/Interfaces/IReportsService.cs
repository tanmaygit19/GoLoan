using GoLoan.Application.DTO.Reports;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.Interfaces
{
    public interface IReportsService
    {
        Task<ReportsDashboardDTO> GetReportsDashboardAsync(DateTime? fromDate, DateTime? toDate);
    }
}
