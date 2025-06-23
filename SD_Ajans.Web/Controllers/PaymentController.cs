using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SD_Ajans.Business.Services;
using SD_Ajans.Core.Entities;
using SD_Ajans.Data;

namespace SD_Ajans.Web.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrganizationService _organizationService;
        private readonly IAssignmentService _assignmentService;
        private readonly AppDbContext _context;

        public PaymentController(IPaymentService paymentService, IOrganizationService organizationService, IAssignmentService assignmentService, AppDbContext context)
        {
            _paymentService = paymentService;
            _organizationService = organizationService;
            _assignmentService = assignmentService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var payments = await _context.Payments
                .Include(p => p.Assignment)
                .ThenInclude(a => a!.Manken)
                .Include(p => p.Assignment)
                .ThenInclude(a => a!.Organization)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(payments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Assignment)
                .ThenInclude(a => a!.Manken)
                .Include(p => p.Assignment)
                .ThenInclude(a => a!.Organization)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
            
            if (payment == null)
            {
                return NotFound();
            }
            return View(payment);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Assignments = await _assignmentService.GetAllAssignmentsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment payment)
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

                // OrganizationId'yi seçilen Assignment üzerinden otomatik doldur
                if (payment.AssignmentId.HasValue)
                {
                    var assignment = await _assignmentService.GetAssignmentByIdAsync(payment.AssignmentId.Value);
                    if (assignment != null)
                    {
                        payment.OrganizationId = assignment.OrganizationId;
                    }
                }

                payment.CreatedById = user.Id;
                payment.ProcessedById = user.Id;
                payment.CreatedAt = DateTime.Now;
                payment.IsActive = true;

                await _paymentService.CreatePaymentAsync(payment);
                TempData["Success"] = "Ödeme başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            var errorMessage = string.Join(", ", errors);
            TempData["Error"] = $"Ödeme eklenirken bir hata oluştu: {errorMessage}";
            ViewBag.Assignments = await _assignmentService.GetAllAssignmentsAsync();
            return View(payment);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Assignment)
                .ThenInclude(a => a!.Manken)
                .Include(p => p.Assignment)
                .ThenInclude(a => a!.Organization)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (payment == null)
            {
                return NotFound();
            }
            ViewBag.Assignments = await _assignmentService.GetAllAssignmentsAsync();
            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Payment payment)
        {
            if (id != payment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingPayment = await _paymentService.GetPaymentByIdAsync(id);
                if (existingPayment != null)
                {
                    payment.CreatedById = existingPayment.CreatedById;
                    payment.CreatedAt = existingPayment.CreatedAt;
                }

                // OrganizationId'yi seçilen Assignment üzerinden otomatik doldur
                if (payment.AssignmentId.HasValue)
                {
                    var assignment = await _assignmentService.GetAssignmentByIdAsync(payment.AssignmentId.Value);
                    if (assignment != null)
                    {
                        payment.OrganizationId = assignment.OrganizationId;
                    }
                }

                await _paymentService.UpdatePaymentAsync(payment);
                TempData["Success"] = "Ödeme başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            var errorMessage = string.Join(", ", errors);
            TempData["Error"] = $"Ödeme güncellenirken bir hata oluştu: {errorMessage}";
            ViewBag.Assignments = await _assignmentService.GetAllAssignmentsAsync();
            return View(payment);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Assignment)
                .ThenInclude(a => a!.Manken)
                .Include(p => p.Assignment)
                .ThenInclude(a => a!.Organization)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
            
            if (payment == null)
            {
                return NotFound();
            }
            return View(payment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _paymentService.DeletePaymentAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reports()
        {
            var startDate = DateTime.Now.AddMonths(-1);
            var endDate = DateTime.Now;

            var totalIncome = await _paymentService.CalculateTotalIncomeAsync(startDate, endDate);
            var totalExpense = await _paymentService.CalculateTotalExpenseAsync(startDate, endDate);
            var netProfit = await _paymentService.CalculateNetProfitAsync(startDate, endDate);

            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpense = totalExpense;
            ViewBag.NetProfit = netProfit;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View();
        }
    }
} 