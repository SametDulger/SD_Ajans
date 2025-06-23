using SD_Ajans.Core.Entities;
using SD_Ajans.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace SD_Ajans.Business.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssignmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Assignment>> GetAllAssignmentsAsync()
        {
            return await _unitOfWork.Repository<Assignment>().Query()
                .Include(a => a.Manken)
                .Include(a => a.Organization)
                .Where(a => a.IsActive)
                .ToListAsync();
        }

        public async Task<Assignment?> GetAssignmentByIdAsync(int id)
        {
            return await _unitOfWork.Repository<Assignment>().GetByIdAsync(id);
        }

        public async Task<IEnumerable<Assignment>> GetAssignmentsByOrganizationAsync(int organizationId)
        {
            return await _unitOfWork.Repository<Assignment>().FindAsync(a => a.OrganizationId == organizationId);
        }

        public async Task<IEnumerable<Assignment>> GetAssignmentsByMankenAsync(int mankenId)
        {
            return await _unitOfWork.Repository<Assignment>().FindAsync(a => a.MankenId == mankenId);
        }

        public async Task<Assignment> CreateAssignmentAsync(Assignment assignment)
        {
            assignment.TotalPayment = await CalculateAssignmentPaymentAsync(assignment);
            await _unitOfWork.Repository<Assignment>().AddAsync(assignment);
            await _unitOfWork.SaveChangesAsync();
            return assignment;
        }

        public async Task<Assignment> UpdateAssignmentAsync(Assignment assignment)
        {
            assignment.UpdatedAt = DateTime.Now;
            assignment.TotalPayment = await CalculateAssignmentPaymentAsync(assignment);
            await _unitOfWork.Repository<Assignment>().UpdateAsync(assignment);
            await _unitOfWork.SaveChangesAsync();
            return assignment;
        }

        public async Task DeleteAssignmentAsync(int id)
        {
            var assignment = await _unitOfWork.Repository<Assignment>().GetByIdAsync(id);
            if (assignment != null)
            {
                assignment.IsActive = false;
                assignment.UpdatedAt = DateTime.Now;
                await _unitOfWork.Repository<Assignment>().UpdateAsync(assignment);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<decimal> CalculateAssignmentPaymentAsync(int assignmentId)
        {
            var assignment = await _unitOfWork.Repository<Assignment>().GetByIdAsync(assignmentId);
            if (assignment == null) return 0;

            return await CalculateAssignmentPaymentAsync(assignment);
        }

        private async Task<decimal> CalculateAssignmentPaymentAsync(Assignment assignment)
        {
            var manken = await _unitOfWork.Repository<Manken>().GetByIdAsync(assignment.MankenId);
            if (manken == null) return 0;

            decimal dailyRate = 0;
            switch (manken.Category)
            {
                case MankenCategory.Category1:
                    dailyRate = 40;
                    break;
                case MankenCategory.Category2:
                    dailyRate = 100;
                    break;
                case MankenCategory.Category3:
                    var organization = await _unitOfWork.Repository<Organization>().GetByIdAsync(assignment.OrganizationId);
                    if (organization != null)
                    {
                        var totalAssignments = await _unitOfWork.Repository<Assignment>().CountAsync(a => a.OrganizationId == assignment.OrganizationId);
                        dailyRate = totalAssignments > 0 ? (organization.TotalBudget * 0.2m) / totalAssignments : 0;
                    }
                    break;
            }

            decimal totalPayment = dailyRate * assignment.NumberOfDays;

            if (assignment.IncludesMeal)
            {
                totalPayment += assignment.MealCost;
            }

            if (assignment.IncludesAccommodation)
            {
                totalPayment += assignment.AccommodationCost;
            }

            return totalPayment;
        }

        public async Task<IEnumerable<Assignment>> GetAssignmentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _unitOfWork.Repository<Assignment>().FindAsync(a => a.Organization!.Date >= startDate && a.Organization!.Date <= endDate);
        }

        public async Task<bool> AssignMankenToOrganizationAsync(int mankenId, int organizationId, int numberOfDays = 1)
        {
            var isAvailable = await CheckMankenAvailabilityAsync(mankenId, organizationId);
            if (!isAvailable) return false;

            var assignment = new Assignment
            {
                MankenId = mankenId,
                OrganizationId = organizationId,
                NumberOfDays = numberOfDays,
                Status = AssignmentStatus.Scheduled
            };

            await CreateAssignmentAsync(assignment);
            return true;
        }

        private async Task<bool> CheckMankenAvailabilityAsync(int mankenId, int organizationId)
        {
            var organization = await _unitOfWork.Repository<Organization>().GetByIdAsync(organizationId);
            if (organization == null) return false;

            var existingAssignments = await _unitOfWork.Repository<Assignment>().FindAsync(a => 
                a.MankenId == mankenId && 
                a.Organization!.Date == organization.Date && 
                a.Status != AssignmentStatus.Cancelled);

            return !existingAssignments.Any();
        }
    }
} 