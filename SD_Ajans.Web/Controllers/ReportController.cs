using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SD_Ajans.Business.Services;
using SD_Ajans.Core.Entities;

namespace SD_Ajans.Web.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IMankenService _mankenService;
        private readonly IOrganizationService _organizationService;

        public ReportController(IReportService reportService, IMankenService mankenService, IOrganizationService organizationService)
        {
            _reportService = reportService;
            _mankenService = mankenService;
            _organizationService = organizationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> DownloadOrganizationReport(int organizationId)
        {
            try
            {
                var reportData = await _reportService.GenerateOrganizationReportAsync(organizationId);
                if (reportData.Length == 0)
                {
                    TempData["Error"] = "Organizasyon bulunamadı.";
                    return RedirectToAction("Index", "Organization");
                }

                var organization = await _organizationService.GetOrganizationByIdAsync(organizationId);
                var fileName = $"Organizasyon_Raporu_{organization?.Name?.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.xlsx";

                return File(reportData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Rapor oluşturulurken hata oluştu: {ex.Message}";
                return RedirectToAction("Index", "Organization");
            }
        }

        public async Task<IActionResult> DownloadMankenReport(int mankenId)
        {
            try
            {
                var reportData = await _reportService.GenerateMankenReportAsync(mankenId);
                if (reportData.Length == 0)
                {
                    TempData["Error"] = "Manken bulunamadı.";
                    return RedirectToAction("Index", "Manken");
                }

                var manken = await _mankenService.GetMankenByIdAsync(mankenId);
                var fileName = $"Manken_Raporu_{manken?.FirstName}_{manken?.LastName}_{DateTime.Now:yyyyMMdd}.xlsx";

                return File(reportData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Rapor oluşturulurken hata oluştu: {ex.Message}";
                return RedirectToAction("Index", "Manken");
            }
        }

        public async Task<IActionResult> DownloadMonthlyReport(int year, int month)
        {
            try
            {
                var reportData = await _reportService.GenerateMonthlyReportAsync(year, month);
                var fileName = $"Aylik_Rapor_{year}_{month:D2}_{DateTime.Now:yyyyMMdd}.xlsx";

                return File(reportData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Rapor oluşturulurken hata oluştu: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> DownloadFinancialReport(int year)
        {
            try
            {
                var reportData = await _reportService.GenerateFinancialReportAsync(year);
                var fileName = $"Finansal_Rapor_{year}_{DateTime.Now:yyyyMMdd}.xlsx";

                return File(reportData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Rapor oluşturulurken hata oluştu: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> DownloadAssignmentReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                var reportData = await _reportService.GenerateAssignmentReportAsync(startDate, endDate);
                var fileName = $"Gorevlendirme_Raporu_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx";

                return File(reportData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Rapor oluşturulurken hata oluştu: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
} 