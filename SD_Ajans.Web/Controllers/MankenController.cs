using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SD_Ajans.Business.Services;
using SD_Ajans.Core.Entities;
using SD_Ajans.Core.Repositories;
using SD_Ajans.Data;
using SD_Ajans.Web.Services;

namespace SD_Ajans.Web.Controllers
{
    [Authorize]
    public class MankenController : Controller
    {
        private readonly IMankenService _mankenService;
        private readonly IFileService _fileService;
        private readonly ILogger<MankenController> _logger;
        private readonly AppDbContext _context;

        public MankenController(
            IMankenService mankenService, 
            IFileService fileService,
            ILogger<MankenController> logger,
            AppDbContext context)
        {
            _mankenService = mankenService;
            _fileService = fileService;
            _logger = logger;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var mankens = await _mankenService.GetAllMankensAsync();
                return View(mankens);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Manken listesi alınırken hata oluştu");
                TempData["Error"] = "Manken listesi alınırken bir hata oluştu.";
                return View(new List<Manken>());
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
        public async Task<IActionResult> Create(Manken manken, IFormFile? photo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    TempData["Error"] = $"Validation hataları: {string.Join(", ", errors)}";
                    return View(manken);
                }

                // Fotoğraf validasyonu
                if (photo != null && !_fileService.IsValidFile(photo))
                {
                    TempData["Error"] = $"Geçersiz dosya. Maksimum boyut: {_fileService.GetFileSizeInMB(5 * 1024 * 1024)}MB, İzin verilen formatlar: JPG, PNG, GIF, WEBP";
                    return View(manken);
                }

                // Fotoğraf yükleme işlemi
                if (photo != null && photo.Length > 0)
                {
                    try
                    {
                        var filePath = await _fileService.UploadFileAsync(photo, "manken-photos");
                        manken.PhotoPath = "/" + filePath;
                        _logger.LogInformation("Manken fotoğrafı yüklendi: {FilePath}", filePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Fotoğraf yükleme sırasında hata oluştu");
                        TempData["Error"] = $"Fotoğraf yüklenirken hata oluştu: {ex.Message}";
                        return View(manken);
                    }
                }

                await _mankenService.CreateMankenAsync(manken);
                TempData["Success"] = "Manken başarıyla eklendi.";
                
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return View(manken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Manken oluşturma sırasında hata oluştu");
                TempData["Error"] = "Manken eklenirken beklenmeyen bir hata oluştu.";
                return View(manken);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var manken = await _mankenService.GetMankenByIdAsync(id);
                if (manken == null)
                {
                    TempData["Error"] = "Manken bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(manken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Manken düzenleme sayfası açılırken hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Manken bilgileri alınırken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Manken manken, IFormFile? photo)
        {
            try
            {
                if (id != manken.Id)
                {
                    TempData["Error"] = "Geçersiz manken ID'si.";
                    return RedirectToAction(nameof(Index));
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    TempData["Error"] = $"Validation hataları: {string.Join(", ", errors)}";
                    return View(manken);
                }

                var existingManken = await _mankenService.GetMankenByIdAsync(id);
                if (existingManken == null)
                {
                    TempData["Error"] = "Manken bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                // Fotoğraf validasyonu
                if (photo != null && !_fileService.IsValidFile(photo))
                {
                    TempData["Error"] = $"Geçersiz dosya. Maksimum boyut: {_fileService.GetFileSizeInMB(5 * 1024 * 1024)}MB, İzin verilen formatlar: JPG, PNG, GIF, WEBP";
                    return View(manken);
                }

                // Yeni fotoğraf yükleme işlemi
                if (photo != null && photo.Length > 0)
                {
                    try
                    {
                        // Eski fotoğrafı sil
                        if (!string.IsNullOrEmpty(existingManken.PhotoPath))
                        {
                            _fileService.DeleteFile(existingManken.PhotoPath.TrimStart('/'));
                        }

                        var filePath = await _fileService.UploadFileAsync(photo, "manken-photos");
                        manken.PhotoPath = "/" + filePath;
                        _logger.LogInformation("Manken fotoğrafı güncellendi: {FilePath}", filePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Fotoğraf güncelleme sırasında hata oluştu");
                        TempData["Error"] = $"Fotoğraf güncellenirken hata oluştu: {ex.Message}";
                        return View(manken);
                    }
                }
                else
                {
                    // Yeni fotoğraf yüklenmediyse mevcut fotoğraf yolunu koru
                    manken.PhotoPath = existingManken.PhotoPath;
                }

                await _mankenService.UpdateMankenAsync(manken);
                TempData["Success"] = "Manken başarıyla güncellendi.";
                
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return View(manken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Manken güncelleme sırasında hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Manken güncellenirken beklenmeyen bir hata oluştu.";
                return View(manken);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var manken = await _mankenService.GetMankenByIdAsync(id);
                if (manken == null)
                {
                    TempData["Error"] = "Manken bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(manken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Manken silme sayfası açılırken hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Manken bilgileri alınırken hata oluştu.";
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
                var manken = await _mankenService.GetMankenByIdAsync(id);
                if (manken != null && !string.IsNullOrEmpty(manken.PhotoPath))
                {
                    // Fotoğrafı sil
                    _fileService.DeleteFile(manken.PhotoPath.TrimStart('/'));
                }

                await _mankenService.DeleteMankenAsync(id);
                TempData["Success"] = "Manken başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Manken silme sırasında hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Manken silinirken beklenmeyen bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var manken = await _mankenService.GetMankenByIdAsync(id);
                if (manken == null)
                {
                    TempData["Error"] = "Manken bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(manken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Manken detay sayfası açılırken hata oluştu. Id: {Id}", id);
                TempData["Error"] = "Manken bilgileri alınırken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMankens()
        {
            try
            {
                var mankens = await _mankenService.GetAllMankensAsync();
                return Json(mankens.Select(m => new { m.Id, m.FirstName, m.LastName, m.Category }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllMankens API'si çalışırken hata oluştu");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Search(string searchTerm, string gender, string category, int? minHeight, int? maxHeight)
        {
            try
            {
                Gender? genderEnum = null;
                if (!string.IsNullOrEmpty(gender) && Enum.TryParse<Gender>(gender, out var g))
                {
                    genderEnum = g;
                }

                MankenCategory? categoryEnum = null;
                if (!string.IsNullOrEmpty(category) && Enum.TryParse<MankenCategory>(category, out var c))
                {
                    categoryEnum = c;
                }

                var mankens = await _mankenService.SearchMankensAsync(searchTerm, genderEnum, categoryEnum, minHeight, maxHeight);
                return View("Index", mankens);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Manken arama sırasında hata oluştu");
                TempData["Error"] = "Arama sırasında hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 