using SD_Ajans.Core.Entities;
using SD_Ajans.Core.Repositories;

namespace SD_Ajans.Business.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrganizationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Organization>> GetAllOrganizationsAsync()
        {
            return await _unitOfWork.Repository<Organization>().GetAllAsync();
        }

        public async Task<Organization?> GetOrganizationByIdAsync(int id)
        {
            return await _unitOfWork.Repository<Organization>().GetByIdAsync(id);
        }

        public async Task<IEnumerable<Organization>> GetOrganizationsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _unitOfWork.Repository<Organization>().FindAsync(o => o.Date >= startDate && o.Date <= endDate);
        }

        public async Task<IEnumerable<Organization>> GetOrganizationsByStatusAsync(OrganizationStatus status)
        {
            return await _unitOfWork.Repository<Organization>().FindAsync(o => o.Status == status);
        }

        public async Task<Organization> CreateOrganizationAsync(Organization organization)
        {
            await _unitOfWork.Repository<Organization>().AddAsync(organization);
            await _unitOfWork.SaveChangesAsync();
            return organization;
        }

        public async Task<Organization> UpdateOrganizationAsync(Organization organization)
        {
            organization.UpdatedAt = DateTime.Now;
            await _unitOfWork.Repository<Organization>().UpdateAsync(organization);
            await _unitOfWork.SaveChangesAsync();
            return organization;
        }

        public async Task DeleteOrganizationAsync(int id)
        {
            var organization = await _unitOfWork.Repository<Organization>().GetByIdAsync(id);
            if (organization != null)
            {
                // Soft delete related assignments
                var assignments = await _unitOfWork.Repository<Assignment>().FindAsync(a => a.OrganizationId == id);
                foreach (var assignment in assignments)
                {
                    assignment.IsActive = false;
                    assignment.UpdatedAt = DateTime.Now;
                    await _unitOfWork.Repository<Assignment>().UpdateAsync(assignment);
                }

                // Soft delete related payments
                var payments = await _unitOfWork.Repository<Payment>().FindAsync(p => p.OrganizationId == id);
                foreach (var payment in payments)
                {
                    payment.IsActive = false;
                    payment.UpdatedAt = DateTime.Now;
                    await _unitOfWork.Repository<Payment>().UpdateAsync(payment);
                }

                // Soft delete the organization
                organization.IsActive = false;
                organization.UpdatedAt = DateTime.Now;
                await _unitOfWork.Repository<Organization>().UpdateAsync(organization);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<decimal> CalculateOrganizationProfitAsync(int organizationId)
        {
            var organization = await _unitOfWork.Repository<Organization>().GetByIdAsync(organizationId);
            if (organization == null) return 0;

            var assignments = await _unitOfWork.Repository<Assignment>().FindAsync(a => a.OrganizationId == organizationId);
            var payments = await _unitOfWork.Repository<Payment>().FindAsync(p => p.OrganizationId == organizationId);

            var totalExpenses = assignments.Sum(a => a.TotalPayment);
            var totalIncome = payments.Where(p => p.PaymentType == PaymentType.Cash).Sum(p => p.Amount);
            var totalExpensePayments = payments.Where(p => p.PaymentType == PaymentType.BankTransfer).Sum(p => p.Amount);

            return totalIncome - totalExpenses - totalExpensePayments;
        }

        public async Task<IEnumerable<Organization>> GetOrganizationsByTypeAsync(OrganizationType type)
        {
            return await _unitOfWork.Repository<Organization>().FindAsync(o => o.Type == type);
        }
    }
} 