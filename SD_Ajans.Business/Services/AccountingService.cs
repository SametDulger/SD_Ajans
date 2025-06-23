using SD_Ajans.Core.Entities;
using SD_Ajans.Core.Repositories;

namespace SD_Ajans.Business.Services
{
    public class AccountingService : IAccountingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<decimal> CalculateMankenFeeAsync(int mankenId, int organizationId, DateTime startDate, DateTime endDate)
        {
            var manken = await _unitOfWork.Mankens.GetByIdAsync(mankenId);
            var organization = await _unitOfWork.Organizations.GetByIdAsync(organizationId);
            
            if (manken == null || organization == null)
                return 0;

            var numberOfDays = (endDate - startDate).Days + 1;

            switch (manken.Category)
            {
                case MankenCategory.Category1:
                    return 40 * numberOfDays;
                case MankenCategory.Category2:
                    return 100 * numberOfDays;
                case MankenCategory.Category3:
                    return organization.TotalBudget * 0.20m / numberOfDays;
                default:
                    return 0;
            }
        }

        public Task<decimal> CalculateMealCostAsync(bool isOutOfCity, int numberOfDays)
        {
            if (isOutOfCity)
            {
                return Task.FromResult(40m * numberOfDays); // 2 öğün + konaklama
            }
            else
            {
                return Task.FromResult(10m * numberOfDays); // 1 öğün
            }
        }

        public Task<decimal> CalculateAccommodationCostAsync(bool isOutOfCity, int numberOfDays)
        {
            if (isOutOfCity)
            {
                return Task.FromResult(30m * numberOfDays); // Konaklama maliyeti
            }
            return Task.FromResult(0m);
        }

        public async Task<decimal> CalculateTotalPaymentAsync(Assignment assignment)
        {
            var manken = await _unitOfWork.Mankens.GetByIdAsync(assignment.MankenId);
            var organization = await _unitOfWork.Organizations.GetByIdAsync(assignment.OrganizationId);
            
            if (manken == null || organization == null)
                return 0;

            var baseFee = await CalculateMankenFeeAsync(assignment.MankenId, assignment.OrganizationId, assignment.StartTime, assignment.EndTime);
            var mealCost = await CalculateMealCostAsync(assignment.IncludesAccommodation, assignment.NumberOfDays);
            var accommodationCost = await CalculateAccommodationCostAsync(assignment.IncludesAccommodation, assignment.NumberOfDays);

            return baseFee + mealCost + accommodationCost;
        }

        public Task<decimal> CalculateOrganizationProfitAsync(int organizationId)
        {
            return Task.FromResult(0m); // Bu metod async olarak kullanılmıyor, basit bir implementasyon
        }

        public Task<Dictionary<string, decimal>> GetMonthlyRevenueAsync(int year)
        {
            return Task.FromResult(new Dictionary<string, decimal>()); // Bu metod async olarak kullanılmıyor, basit bir implementasyon
        }

        public Task<Dictionary<string, decimal>> GetMonthlyExpensesAsync(int year)
        {
            return Task.FromResult(new Dictionary<string, decimal>()); // Bu metod async olarak kullanılmıyor, basit bir implementasyon
        }

        public Task<FeeCalculationResult> CalculateFeeAsync(Manken manken, Organization organization, int numberOfDays, bool includesMeal, bool includesAccommodation)
        {
            var dailyRate = manken.Category switch
            {
                MankenCategory.Category1 => 40m,
                MankenCategory.Category2 => 100m,
                MankenCategory.Category3 => organization.TotalBudget * 0.20m / numberOfDays,
                _ => 0m
            };

            var mealCost = includesMeal ? (includesAccommodation ? 40m : 10m) * numberOfDays : 0m;
            var accommodationCost = includesAccommodation ? 30m * numberOfDays : 0m;
            var totalPayment = dailyRate * numberOfDays + mealCost + accommodationCost;
            var fee = totalPayment * 0.15m; // %15 komisyon
            var profit = totalPayment - fee;

            return Task.FromResult(new FeeCalculationResult
            {
                DailyRate = dailyRate,
                TotalPayment = totalPayment,
                MealCost = mealCost,
                AccommodationCost = accommodationCost,
                Fee = fee,
                Profit = profit
            });
        }

        public async Task<OrganizationProfitAnalysis> GetOrganizationProfitAsync(int organizationId)
        {
            var organization = await _unitOfWork.Organizations.GetByIdAsync(organizationId);
            if (organization == null)
                return new OrganizationProfitAnalysis();

            var assignments = await _unitOfWork.Assignments.GetAllAsync(a => a.OrganizationId == organizationId);
            var payments = await _unitOfWork.Payments.GetAllAsync(p => p.OrganizationId == organizationId && p.Status == PaymentStatus.Completed);

            var totalRevenue = payments.Sum(p => p.Amount);
            var totalExpenses = assignments.Sum(a => a.TotalPayment);
            var netProfit = totalRevenue - totalExpenses;
            var profitMargin = totalRevenue > 0 ? (netProfit / totalRevenue) * 100 : 0;

            return new OrganizationProfitAnalysis
            {
                TotalRevenue = totalRevenue,
                TotalExpenses = totalExpenses,
                NetProfit = netProfit,
                ProfitMargin = profitMargin,
                Assignments = assignments.ToList(),
                Payments = payments.ToList()
            };
        }

        public async Task<OrganizationCostAnalysis> GetOrganizationCostAnalysisAsync(int organizationId)
        {
            var organization = await _unitOfWork.Organizations.GetByIdAsync(organizationId);
            if (organization == null)
                return new OrganizationCostAnalysis();

            var assignments = await _unitOfWork.Assignments.GetAllAsync(a => a.OrganizationId == organizationId);
            var totalCosts = assignments.Sum(a => a.TotalPayment);
            var remainingBudget = organization.TotalBudget - totalCosts;
            var costPercentage = organization.TotalBudget > 0 ? (totalCosts / organization.TotalBudget) * 100 : 0;

            var costBreakdown = new Dictionary<string, decimal>
            {
                ["Manken Ücretleri"] = assignments.Sum(a => a.DailyRate * a.NumberOfDays),
                ["Yemek Maliyetleri"] = assignments.Sum(a => a.MealCost),
                ["Konaklama Maliyetleri"] = assignments.Sum(a => a.AccommodationCost)
            };

            return new OrganizationCostAnalysis
            {
                TotalBudget = organization.TotalBudget,
                TotalCosts = totalCosts,
                RemainingBudget = remainingBudget,
                CostPercentage = costPercentage,
                Assignments = assignments.ToList(),
                CostBreakdown = costBreakdown
            };
        }

        private string GetMonthName(int month)
        {
            return month switch
            {
                1 => "Ocak",
                2 => "Şubat",
                3 => "Mart",
                4 => "Nisan",
                5 => "Mayıs",
                6 => "Haziran",
                7 => "Temmuz",
                8 => "Ağustos",
                9 => "Eylül",
                10 => "Ekim",
                11 => "Kasım",
                12 => "Aralık",
                _ => "Bilinmeyen"
            };
        }
    }
} 