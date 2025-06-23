using Microsoft.AspNetCore.Mvc;
using SD_Ajans.Business.Services;
using System.Diagnostics;
using SD_Ajans.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace SD_Ajans.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMankenService _mankenService;
        private readonly IOrganizationService _organizationService;
        private readonly IAssignmentService _assignmentService;
        private readonly IPaymentService _paymentService;

        public HomeController(IMankenService mankenService, IOrganizationService organizationService, 
            IAssignmentService assignmentService, IPaymentService paymentService)
        {
            _mankenService = mankenService;
            _organizationService = organizationService;
            _assignmentService = assignmentService;
            _paymentService = paymentService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var mankens = await _mankenService.GetAllMankensAsync();
            var organizations = await _organizationService.GetAllOrganizationsAsync();
            var assignments = await _assignmentService.GetAllAssignmentsAsync();
            
            var startDate = DateTime.Now.AddMonths(-1);
            var endDate = DateTime.Now;
            var totalIncome = await _paymentService.CalculateTotalIncomeAsync(startDate, endDate);
            var totalExpense = await _paymentService.CalculateTotalExpenseAsync(startDate, endDate);
            var netProfit = await _paymentService.CalculateNetProfitAsync(startDate, endDate);

            ViewBag.TotalMankens = mankens.Count();
            ViewBag.TotalOrganizations = organizations.Count();
            ViewBag.TotalAssignments = assignments.Count();
            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpense = totalExpense;
            ViewBag.NetProfit = netProfit;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
