using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SD_Ajans.Business.Services;
using SD_Ajans.Core.Entities;

namespace SD_Ajans.Web.Controllers
{
    [Authorize]
    public class AccountingController : Controller
    {
        private readonly IAccountingService _accountingService;
        private readonly IOrganizationService _organizationService;
        private readonly IMankenService _mankenService;
        private readonly IAssignmentService _assignmentService;

        public AccountingController(IAccountingService accountingService, 
            IOrganizationService organizationService, 
            IMankenService mankenService, 
            IAssignmentService assignmentService)
        {
            _accountingService = accountingService;
            _organizationService = organizationService;
            _mankenService = mankenService;
            _assignmentService = assignmentService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var currentYear = DateTime.Now.Year;
                var monthlyRevenue = await _accountingService.GetMonthlyRevenueAsync(currentYear);
                var monthlyExpenses = await _accountingService.GetMonthlyExpensesAsync(currentYear);

                ViewBag.MonthlyRevenue = monthlyRevenue ?? new Dictionary<string, decimal>();
                ViewBag.MonthlyExpenses = monthlyExpenses ?? new Dictionary<string, decimal>();
                ViewBag.CurrentYear = currentYear;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.MonthlyRevenue = new Dictionary<string, decimal>();
                ViewBag.MonthlyExpenses = new Dictionary<string, decimal>();
                ViewBag.CurrentYear = DateTime.Now.Year;
                TempData["Error"] = $"Muhasebe verileri yüklenirken hata oluştu: {ex.Message}";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CalculateFee(int mankenId, int organizationId, int numberOfDays, bool includesMeal, bool includesAccommodation)
        {
            try
            {
                var manken = await _mankenService.GetMankenByIdAsync(mankenId);
                var organization = await _organizationService.GetOrganizationByIdAsync(organizationId);

                if (manken == null || organization == null)
                {
                    return Json(new { success = false, message = "Manken veya organizasyon bulunamadı." });
                }

                var feeCalculation = await _accountingService.CalculateFeeAsync(manken, organization, numberOfDays, includesMeal, includesAccommodation);

                return Json(new
                {
                    success = true,
                    dailyRate = feeCalculation.DailyRate,
                    totalPayment = feeCalculation.TotalPayment,
                    mealCost = feeCalculation.MealCost,
                    accommodationCost = feeCalculation.AccommodationCost,
                    fee = feeCalculation.Fee,
                    profit = feeCalculation.Profit
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OrganizationProfit(int organizationId)
        {
            try
            {
                var organization = await _organizationService.GetOrganizationByIdAsync(organizationId);
                if (organization == null)
                {
                    return NotFound();
                }

                var profitAnalysis = await _accountingService.GetOrganizationProfitAsync(organizationId);
                ViewBag.Organization = organization;
                ViewBag.ProfitAnalysis = profitAnalysis;

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Kar analizi yüklenirken hata oluştu: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> FinancialSummary(int year)
        {
            try
            {
                var monthlyRevenue = await _accountingService.GetMonthlyRevenueAsync(year);
                var monthlyExpenses = await _accountingService.GetMonthlyExpensesAsync(year);

                var totalRevenue = monthlyRevenue?.Values.Sum() ?? 0;
                var totalExpenses = monthlyExpenses?.Values.Sum() ?? 0;
                var netProfit = totalRevenue - totalExpenses;

                ViewBag.Year = year;
                ViewBag.TotalRevenue = totalRevenue;
                ViewBag.TotalExpenses = totalExpenses;
                ViewBag.NetProfit = netProfit;
                ViewBag.MonthlyRevenue = monthlyRevenue ?? new Dictionary<string, decimal>();
                ViewBag.MonthlyExpenses = monthlyExpenses ?? new Dictionary<string, decimal>();

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Finansal özet yüklenirken hata oluştu: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> CostAnalysis(int organizationId)
        {
            try
            {
                var organization = await _organizationService.GetOrganizationByIdAsync(organizationId);
                if (organization == null)
                {
                    return NotFound();
                }

                var costAnalysis = await _accountingService.GetOrganizationCostAnalysisAsync(organizationId);
                ViewBag.Organization = organization;
                ViewBag.CostAnalysis = costAnalysis;

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Maliyet analizi yüklenirken hata oluştu: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
} 