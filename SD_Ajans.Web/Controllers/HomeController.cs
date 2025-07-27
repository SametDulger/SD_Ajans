using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SD_Ajans.Web.Models;
using System.Diagnostics;

namespace SD_Ajans.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ana sayfa yüklenirken hata oluştu");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = statusCode
            };

            switch (statusCode)
            {
                case 404:
                    errorViewModel.Message = "Sayfa bulunamadı.";
                    break;
                case 403:
                    errorViewModel.Message = "Bu sayfaya erişim yetkiniz yok.";
                    break;
                case 500:
                    errorViewModel.Message = "Sunucu hatası oluştu.";
                    break;
                default:
                    errorViewModel.Message = "Beklenmeyen bir hata oluştu.";
                    break;
            }

            _logger.LogWarning("HTTP {StatusCode} hatası: {Message}", statusCode, errorViewModel.Message);
            return View("Error", errorViewModel);
        }
    }
}
