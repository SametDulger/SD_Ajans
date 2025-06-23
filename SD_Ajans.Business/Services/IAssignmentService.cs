using SD_Ajans.Core.Entities;

namespace SD_Ajans.Business.Services
{
    public interface IAssignmentService
    {
        Task<IEnumerable<Assignment>> GetAllAssignmentsAsync();
        Task<Assignment?> GetAssignmentByIdAsync(int id);
        Task<IEnumerable<Assignment>> GetAssignmentsByOrganizationAsync(int organizationId);
        Task<IEnumerable<Assignment>> GetAssignmentsByMankenAsync(int mankenId);
        Task<Assignment> CreateAssignmentAsync(Assignment assignment);
        Task<Assignment> UpdateAssignmentAsync(Assignment assignment);
        Task DeleteAssignmentAsync(int id);
        Task<decimal> CalculateAssignmentPaymentAsync(int assignmentId);
        Task<IEnumerable<Assignment>> GetAssignmentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> AssignMankenToOrganizationAsync(int mankenId, int organizationId, int numberOfDays = 1);
    }
} 