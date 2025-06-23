using SD_Ajans.Core.Entities;

namespace SD_Ajans.Business.Services
{
    public interface IPaymentService
    {
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();
        Task<Payment?> GetPaymentByIdAsync(int id);
        Task<IEnumerable<Payment>> GetPaymentsByOrganizationAsync(int organizationId);
        Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Payment>> GetPaymentsByTypeAsync(PaymentType type);
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment> UpdatePaymentAsync(Payment payment);
        Task DeletePaymentAsync(int id);
        Task<decimal> CalculateTotalIncomeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> CalculateTotalExpenseAsync(DateTime startDate, DateTime endDate);
        Task<decimal> CalculateNetProfitAsync(DateTime startDate, DateTime endDate);
    }
} 