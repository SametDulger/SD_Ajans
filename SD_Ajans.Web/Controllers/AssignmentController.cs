using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly AppDbContext _context;

        public AssignmentController(IAssignmentService assignmentService, IMankenService mankenService, IOrganizationService organizationService, AppDbContext context)
        {
            _assignmentService = assignmentService;
            _mankenService = mankenService;
            _organizationService = organizationService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var assignments = await _context.Assignments
                .Include(a => a.Manken)
                .Include(a => a.Organization)
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
            return View(assignments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Manken)
                .Include(a => a.Organization)
                .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);
            
            if (assignment == null)
            {
                return NotFound();
            }
            return View(assignment);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Mankens = await _mankenService.GetAllMankensAsync();
            ViewBag.Organizations = await _organizationService.GetAllOrganizationsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                // Geçici olarak ilk User'ı kullan veya oluştur
                var user = await _context.Users.FirstOrDefaultAsync();
                if (user == null)
                {
                    user = new User
                    {
                        UserName = "admin@sdajans.com",
                        Email = "admin@sdajans.com",
                        FirstName = "Admin",
                        LastName = "User",
                        Role = UserRole.Admin
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
                
                assignment.CreatedById = user.Id;
                assignment.CreatedAt = DateTime.Now;
                assignment.IsActive = true;
                
                await _assignmentService.CreateAssignmentAsync(assignment);
                TempData["Success"] = "Görevlendirme başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            var errorMessage = string.Join(", ", errors);
            TempData["Error"] = $"Görevlendirme eklenirken bir hata oluştu: {errorMessage}";
            ViewBag.Mankens = await _mankenService.GetAllMankensAsync();
            ViewBag.Organizations = await _organizationService.GetAllOrganizationsAsync();
            return View(assignment);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Manken)
                .Include(a => a.Organization)
                .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);
            
            if (assignment == null)
            {
                return NotFound();
            }
            ViewBag.Mankens = await _mankenService.GetAllMankensAsync();
            ViewBag.Organizations = await _organizationService.GetAllOrganizationsAsync();
            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Assignment assignment)
        {
            if (id != assignment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingAssignment = await _assignmentService.GetAssignmentByIdAsync(id);
                if (existingAssignment != null)
                {
                    assignment.CreatedById = existingAssignment.CreatedById;
                    assignment.CreatedAt = existingAssignment.CreatedAt;
                }
                
                await _assignmentService.UpdateAssignmentAsync(assignment);
                TempData["Success"] = "Görevlendirme başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            var errorMessage = string.Join(", ", errors);
            TempData["Error"] = $"Görevlendirme güncellenirken bir hata oluştu: {errorMessage}";
            ViewBag.Mankens = await _mankenService.GetAllMankensAsync();
            ViewBag.Organizations = await _organizationService.GetAllOrganizationsAsync();
            return View(assignment);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Manken)
                .Include(a => a.Organization)
                .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);
            
            if (assignment == null)
            {
                return NotFound();
            }
            return View(assignment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _assignmentService.DeleteAssignmentAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AssignManken(int mankenId, int organizationId, int numberOfDays = 1)
        {
            var result = await _assignmentService.AssignMankenToOrganizationAsync(mankenId, organizationId, numberOfDays);
            if (result)
            {
                TempData["Success"] = "Manken başarıyla organizasyona atandı.";
            }
            else
            {
                TempData["Error"] = "Manken atanamadı. Tarih çakışması olabilir.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Status(int id)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Manken)
                .Include(a => a.Organization)
                .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);
            
            if (assignment == null)
            {
                return NotFound();
            }
            
            ViewBag.Statuses = Enum.GetValues<AssignmentStatus>();
            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Status(int id, AssignmentStatus newStatus)
        {
            var assignment = await _assignmentService.GetAssignmentByIdAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            assignment.Status = newStatus;
            assignment.UpdatedAt = DateTime.Now;
            
            if (newStatus == AssignmentStatus.Completed)
            {
                assignment.CompletedAt = DateTime.Now;
            }

            await _assignmentService.UpdateAssignmentAsync(assignment);
            TempData["Success"] = "Görevlendirme durumu başarıyla güncellendi.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
} 