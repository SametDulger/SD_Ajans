using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SD_Ajans.Business.Services;
using SD_Ajans.Core.Entities;
using SD_Ajans.Data;

namespace SD_Ajans.Web.Controllers
{
    [Authorize]
    public class OrganizationController : Controller
    {
        private readonly IOrganizationService _organizationService;
        private readonly ILogger<OrganizationController> _logger;
        private readonly AppDbContext _context;

        public OrganizationController(IOrganizationService organizationService, ILogger<OrganizationController> logger, AppDbContext context)
        {
            _organizationService = organizationService;
            _logger = logger;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var organizations = await _organizationService.GetAllOrganizationsAsync();
                return View(organizations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Organizasyon listesi alınırken hata oluştu");
                TempData["Error"] = "Organizasyon listesi alınırken bir hata oluştu.";
                return View(new List<Organization>());
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Organization organization)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    TempData["Error"] = $"Validation hataları: {string.Join(", ", errors)}";
                    return View(organization);
                }

                // Mevcut kullanıcıyı al
                var currentUser = await _context.Users.FirstOrDefaultAsync();
                if (currentUser != null)
                {
                    organization.CreatedById = currentUser.Id;
                }

                organization.CreatedAt = DateTime.Now;
                organization.IsActive = true;

                await _organizationService.CreateOrganizationAsync(organization);
                TempData["Success"] = "Organizasyon başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Organizasyon oluşturma sırasında hata oluştu");
                TempData["Error"] = "Organizasyon eklenirken beklenmeyen bir hata oluştu.";
                return View(organization);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var organization = await _organizationService.GetOrganizationByIdAsync(id);
                if (organization == null)
                {
                    TempData["Error"] = "Organizasyon bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(organization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Organizasyon düzenleme sayfası açılırken hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Organizasyon bilgileri alınırken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Organization organization)
        {
            try
            {
                if (id != organization.Id)
                {
                    TempData["Error"] = "Geçersiz organizasyon ID'si.";
                    return RedirectToAction(nameof(Index));
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    TempData["Error"] = $"Validation hataları: {string.Join(", ", errors)}";
                    return View(organization);
                }

                await _organizationService.UpdateOrganizationAsync(organization);
                TempData["Success"] = "Organizasyon başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Organizasyon güncelleme sırasında hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Organizasyon güncellenirken beklenmeyen bir hata oluştu.";
                return View(organization);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var organization = await _organizationService.GetOrganizationByIdAsync(id);
                if (organization == null)
                {
                    TempData["Error"] = "Organizasyon bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(organization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Organizasyon silme sayfası açılırken hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Organizasyon bilgileri alınırken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _organizationService.DeleteOrganizationAsync(id);
                TempData["Success"] = "Organizasyon başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Organizasyon silme sırasında hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Organizasyon silinirken beklenmeyen bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var organization = await _organizationService.GetOrganizationByIdAsync(id);
                if (organization == null)
                {
                    TempData["Error"] = "Organizasyon bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(organization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Organizasyon detay sayfası açılırken hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Organizasyon bilgileri alınırken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrganizations()
        {
            try
            {
                var organizations = await _organizationService.GetAllOrganizationsAsync();
                return Json(organizations.Select(o => new { o.Id, o.Name }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllOrganizations API'si çalışırken hata oluştu");
                return Json(new List<object>());
            }
        }

        public async Task<IActionResult> Profit(int id)
        {
            try
            {
                var organization = await _organizationService.GetOrganizationByIdAsync(id);
                if (organization == null)
                {
                    TempData["Error"] = "Organizasyon bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                var profit = await _organizationService.CalculateOrganizationProfitAsync(id);
                return View(profit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Organizasyon kar analizi hesaplanırken hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Kar analizi hesaplanırken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 