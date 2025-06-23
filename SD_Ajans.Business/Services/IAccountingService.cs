using SD_Ajans.Core.Entities;

namespace SD_Ajans.Business.Services
{
    public interface IAccountingService
    {
        Task<decimal> CalculateMankenFeeAsync(int mankenId, int organizationId, DateTime startDate, DateTime endDate);
        Task<decimal> CalculateMealCostAsync(bool isOutOfCity, int numberOfDays);
        Task<decimal> CalculateAccommodationCostAsync(bool isOutOfCity, int numberOfDays);
        Task<decimal> CalculateTotalPaymentAsync(Assignment assignment);
        Task<decimal> CalculateOrganizationProfitAsync(int organizationId);
        Task<Dictionary<string, decimal>> GetMonthlyRevenueAsync(int year);
        Task<Dictionary<string, decimal>> GetMonthlyExpensesAsync(int year);
        
        // Yeni eklenen metotlar
        Task<FeeCalculationResult> CalculateFeeAsync(Manken manken, Organization organization, int numberOfDays, bool includesMeal, bool includesAccommodation);
        Task<OrganizationProfitAnalysis> GetOrganizationProfitAsync(int organizationId);
        Task<OrganizationCostAnalysis> GetOrganizationCostAnalysisAsync(int organizationId);
    }

    public class FeeCalculationResult
    {
        public decimal DailyRate { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal MealCost { get; set; }
        public decimal AccommodationCost { get; set; }
        public decimal Fee { get; set; }
        public decimal Profit { get; set; }
    }

    public class OrganizationProfitAnalysis
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public decimal ProfitMargin { get; set; }
        public List<Assignment> Assignments { get; set; } = new();
        public List<Payment> Payments { get; set; } = new();
    }

    public class OrganizationCostAnalysis
    {
        public decimal TotalBudget { get; set; }
        public decimal TotalCosts { get; set; }
        public decimal RemainingBudget { get; set; }
        public decimal CostPercentage { get; set; }
        public List<Assignment> Assignments { get; set; } = new();
        public Dictionary<string, decimal> CostBreakdown { get; set; } = new();
    }
} 