using SD_Ajans.Core.Entities;

namespace SD_Ajans.Business.Services
{
    public interface IOrganizationService
    {
        Task<IEnumerable<Organization>> GetAllOrganizationsAsync();
        Task<Organization?> GetOrganizationByIdAsync(int id);
        Task<IEnumerable<Organization>> GetOrganizationsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Organization>> GetOrganizationsByStatusAsync(OrganizationStatus status);
        Task<Organization> CreateOrganizationAsync(Organization organization);
        Task<Organization> UpdateOrganizationAsync(Organization organization);
        Task DeleteOrganizationAsync(int id);
        Task<decimal> CalculateOrganizationProfitAsync(int organizationId);
        Task<IEnumerable<Organization>> GetOrganizationsByTypeAsync(OrganizationType type);
    }
} 