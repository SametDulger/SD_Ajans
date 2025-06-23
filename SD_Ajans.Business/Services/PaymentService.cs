using SD_Ajans.Core.Entities;
using SD_Ajans.Core.Repositories;

namespace SD_Ajans.Business.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            return await _unitOfWork.Repository<Payment>().GetAllAsync();
        }

        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            return await _unitOfWork.Repository<Payment>().GetByIdAsync(id);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByOrganizationAsync(int organizationId)
        {
            return await _unitOfWork.Repository<Payment>().FindAsync(p => p.OrganizationId == organizationId);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _unitOfWork.Repository<Payment>().FindAsync(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByTypeAsync(PaymentType type)
        {
            return await _unitOfWork.Repository<Payment>().FindAsync(p => p.PaymentType == type);
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            await _unitOfWork.Repository<Payment>().AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> UpdatePaymentAsync(Payment payment)
        {
            payment.UpdatedAt = DateTime.Now;
            await _unitOfWork.Repository<Payment>().UpdateAsync(payment);
            await _unitOfWork.SaveChangesAsync();
            return payment;
        }

        public async Task DeletePaymentAsync(int id)
        {
            var payment = await _unitOfWork.Repository<Payment>().GetByIdAsync(id);
            if (payment != null)
            {
                payment.IsActive = false;
                payment.UpdatedAt = DateTime.Now;
                await _unitOfWork.Repository<Payment>().UpdateAsync(payment);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<decimal> CalculateTotalIncomeAsync(DateTime startDate, DateTime endDate)
        {
            var payments = await _unitOfWork.Repository<Payment>().FindAsync(p => 
                p.PaymentType == PaymentType.Cash && 
                p.CreatedAt >= startDate && 
                p.CreatedAt <= endDate && 
                p.Status == PaymentStatus.Completed);

            return payments.Sum(p => p.Amount);
        }

        public async Task<decimal> CalculateTotalExpenseAsync(DateTime startDate, DateTime endDate)
        {
            var payments = await _unitOfWork.Repository<Payment>().FindAsync(p => 
                p.PaymentType == PaymentType.BankTransfer && 
                p.CreatedAt >= startDate && 
                p.CreatedAt <= endDate && 
                p.Status == PaymentStatus.Completed);

            return payments.Sum(p => p.Amount);
        }

        public async Task<decimal> CalculateNetProfitAsync(DateTime startDate, DateTime endDate)
        {
            var totalIncome = await CalculateTotalIncomeAsync(startDate, endDate);
            var totalExpense = await CalculateTotalExpenseAsync(startDate, endDate);
            return totalIncome - totalExpense;
        }
    }
} 