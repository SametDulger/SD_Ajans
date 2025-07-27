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
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IAssignmentService _assignmentService;
        private readonly ILogger<PaymentController> _logger;
        private readonly AppDbContext _context;

        public PaymentController(
            IPaymentService paymentService, 
            IAssignmentService assignmentService,
            ILogger<PaymentController> logger,
            AppDbContext context)
        {
            _paymentService = paymentService;
            _assignmentService = assignmentService;
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var payments = await _paymentService.GetAllPaymentsAsync();
                return View(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ödeme listesi alınırken hata oluştu");
                TempData["Error"] = "Ödeme listesi alınırken bir hata oluştu.";
                return View(new List<Payment>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByIdAsync(id);
                if (payment == null)
                {
                    TempData["Error"] = "Ödeme bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ödeme detay sayfası açılırken hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Ödeme bilgileri alınırken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Assignments = await _assignmentService.GetAllAssignmentsAsync();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ödeme oluşturma sayfası açılırken hata oluştu");
                TempData["Error"] = "Sayfa yüklenirken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment payment)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    TempData["Error"] = $"Validation hataları: {string.Join(", ", errors)}";
                    ViewBag.Assignments = await _assignmentService.GetAllAssignmentsAsync();
                    return View(payment);
                }

                // Mevcut kullanıcıyı al
                var currentUser = await _context.Users.FirstOrDefaultAsync();
                if (currentUser != null)
                {
                    payment.CreatedById = currentUser.Id;
                    payment.ProcessedById = currentUser.Id;
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

                payment.CreatedAt = DateTime.Now;
                payment.IsActive = true;

                await _paymentService.CreatePaymentAsync(payment);
                TempData["Success"] = "Ödeme başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ödeme oluşturma sırasında hata oluştu");
                TempData["Error"] = "Ödeme eklenirken beklenmeyen bir hata oluştu.";
                ViewBag.Assignments = await _assignmentService.GetAllAssignmentsAsync();
                return View(payment);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByIdAsync(id);
                if (payment == null)
                {
                    TempData["Error"] = "Ödeme bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Assignments = await _assignmentService.GetAllAssignmentsAsync();
                return View(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ödeme düzenleme sayfası açılırken hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Ödeme bilgileri alınırken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Payment payment)
        {
            try
            {
                if (id != payment.Id)
                {
                    TempData["Error"] = "Geçersiz ödeme ID'si.";
                    return RedirectToAction(nameof(Index));
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    TempData["Error"] = $"Validation hataları: {string.Join(", ", errors)}";
                    ViewBag.Assignments = await _assignmentService.GetAllAssignmentsAsync();
                    return View(payment);
                }

                await _paymentService.UpdatePaymentAsync(payment);
                TempData["Success"] = "Ödeme başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ödeme güncelleme sırasında hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Ödeme güncellenirken beklenmeyen bir hata oluştu.";
                ViewBag.Assignments = await _assignmentService.GetAllAssignmentsAsync();
                return View(payment);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByIdAsync(id);
                if (payment == null)
                {
                    TempData["Error"] = "Ödeme bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ödeme silme sayfası açılırken hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Ödeme bilgileri alınırken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _paymentService.DeletePaymentAsync(id);
                TempData["Success"] = "Ödeme başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ödeme silme sırasında hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Ödeme silinirken beklenmeyen bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Reports()
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ödeme raporları sayfası açılırken hata oluştu");
                TempData["Error"] = "Raporlar yüklenirken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 