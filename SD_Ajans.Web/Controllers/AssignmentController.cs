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
    public class AssignmentController : Controller
    {
        private readonly IAssignmentService _assignmentService;
        private readonly IMankenService _mankenService;
        private readonly IOrganizationService _organizationService;
        private readonly ILogger<AssignmentController> _logger;
        private readonly AppDbContext _context;

        public AssignmentController(
            IAssignmentService assignmentService, 
            IMankenService mankenService, 
            IOrganizationService organizationService,
            ILogger<AssignmentController> logger,
            AppDbContext context)
        {
            _assignmentService = assignmentService;
            _mankenService = mankenService;
            _organizationService = organizationService;
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var assignments = await _assignmentService.GetAllAssignmentsAsync();
                return View(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Görevlendirme listesi alınırken hata oluştu");
                TempData["Error"] = "Görevlendirme listesi alınırken bir hata oluştu.";
                return View(new List<Assignment>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var assignment = await _assignmentService.GetAssignmentByIdAsync(id);
                if (assignment == null)
                {
                    TempData["Error"] = "Görevlendirme bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Görevlendirme detay sayfası açılırken hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Görevlendirme bilgileri alınırken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Mankens = await _mankenService.GetAllMankensAsync();
                ViewBag.Organizations = await _organizationService.GetAllOrganizationsAsync();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Görevlendirme oluşturma sayfası açılırken hata oluştu");
                TempData["Error"] = "Sayfa yüklenirken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Assignment assignment)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    TempData["Error"] = $"Validation hataları: {string.Join(", ", errors)}";
                    ViewBag.Mankens = await _mankenService.GetAllMankensAsync();
                    ViewBag.Organizations = await _organizationService.GetAllOrganizationsAsync();
                    return View(assignment);
                }

                // Mevcut kullanıcıyı al
                var currentUser = await _context.Users.FirstOrDefaultAsync();
                if (currentUser != null)
                {
                    assignment.CreatedById = currentUser.Id;
                }

                assignment.CreatedAt = DateTime.Now;
                assignment.IsActive = true;

                await _assignmentService.CreateAssignmentAsync(assignment);
                TempData["Success"] = "Görevlendirme başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Görevlendirme oluşturma sırasında hata oluştu");
                TempData["Error"] = "Görevlendirme eklenirken beklenmeyen bir hata oluştu.";
                ViewBag.Mankens = await _mankenService.GetAllMankensAsync();
                ViewBag.Organizations = await _organizationService.GetAllOrganizationsAsync();
                return View(assignment);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var assignment = await _assignmentService.GetAssignmentByIdAsync(id);
                if (assignment == null)
                {
                    TempData["Error"] = "Görevlendirme bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Mankens = await _mankenService.GetAllMankensAsync();
                ViewBag.Organizations = await _organizationService.GetAllOrganizationsAsync();
                return View(assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Görevlendirme düzenleme sayfası açılırken hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Görevlendirme bilgileri alınırken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Assignment assignment)
        {
            try
            {
                if (id != assignment.Id)
                {
                    TempData["Error"] = "Geçersiz görevlendirme ID'si.";
                    return RedirectToAction(nameof(Index));
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    TempData["Error"] = $"Validation hataları: {string.Join(", ", errors)}";
                    ViewBag.Mankens = await _mankenService.GetAllMankensAsync();
                    ViewBag.Organizations = await _organizationService.GetAllOrganizationsAsync();
                    return View(assignment);
                }

                await _assignmentService.UpdateAssignmentAsync(assignment);
                TempData["Success"] = "Görevlendirme başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Görevlendirme güncelleme sırasında hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Görevlendirme güncellenirken beklenmeyen bir hata oluştu.";
                ViewBag.Mankens = await _mankenService.GetAllMankensAsync();
                ViewBag.Organizations = await _organizationService.GetAllOrganizationsAsync();
                return View(assignment);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var assignment = await _assignmentService.GetAssignmentByIdAsync(id);
                if (assignment == null)
                {
                    TempData["Error"] = "Görevlendirme bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Görevlendirme silme sayfası açılırken hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Görevlendirme bilgileri alınırken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _assignmentService.DeleteAssignmentAsync(id);
                TempData["Success"] = "Görevlendirme başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Görevlendirme silme sırasında hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Görevlendirme silinirken beklenmeyen bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Status(int id)
        {
            try
            {
                var assignment = await _assignmentService.GetAssignmentByIdAsync(id);
                if (assignment == null)
                {
                    TempData["Error"] = "Görevlendirme bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Statuses = Enum.GetValues<AssignmentStatus>();
                return View(assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Görevlendirme durum sayfası açılırken hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Görevlendirme bilgileri alınırken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, AssignmentStatus status)
        {
            try
            {
                var assignment = await _assignmentService.GetAssignmentByIdAsync(id);
                if (assignment == null)
                {
                    TempData["Error"] = "Görevlendirme bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                assignment.Status = status;
                if (status == AssignmentStatus.Completed)
                {
                    assignment.CompletedAt = DateTime.Now;
                }

                await _assignmentService.UpdateAssignmentAsync(assignment);
                TempData["Success"] = "Görevlendirme durumu başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Görevlendirme durumu güncellenirken hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Görevlendirme durumu güncellenirken beklenmeyen bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 