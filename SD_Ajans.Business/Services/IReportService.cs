using SD_Ajans.Core.Entities;

namespace SD_Ajans.Business.Services
{
    public interface IReportService
    {
        Task<byte[]> GenerateOrganizationReportAsync(int organizationId);
        Task<byte[]> GenerateMankenReportAsync(int mankenId);
        Task<byte[]> GenerateMonthlyReportAsync(int year, int month);
        Task<byte[]> GenerateFinancialReportAsync(int year);
        Task<byte[]> GenerateAssignmentReportAsync(DateTime startDate, DateTime endDate);
    }
} 