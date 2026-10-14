using LoanApp.Application.DTOs;

namespace GoLoan.Application.Interfaces
{
    public interface ICibilService
    {
        CibilReportDto GenerateCibilReport(int id);

        CibilReportViewDto GetCibilReport(int id);
    }
}
