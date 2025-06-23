using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly AppDbContext _context;

        public MankenController(IMankenService mankenService, AppDbContext context)
        {
            _mankenService = mankenService;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var mankens = await _context.Mankens.Where(m => m.IsActive).ToListAsync();
            return View(mankens);
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
                
                manken.UserId = user.Id;
                manken.CreatedAt = DateTime.Now;
                manken.IsActive = true;
                
                // Fotoğraf yükleme işlemi
                if (photo != null && photo.Length > 0)
                {
                    try
                    {
                        var fileService = HttpContext.RequestServices.GetRequiredService<IFileService>();
                        
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(photo.FileName).ToLowerInvariant();
                        
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            TempData["Warning"] = "Fotoğraf formatı desteklenmiyor. Sadece JPG, PNG ve GIF formatları kabul edilir.";
                        }
                        else if (photo.Length > 5 * 1024 * 1024) // 5MB limit
                        {
                            TempData["Warning"] = "Fotoğraf boyutu 5MB'dan büyük olamaz.";
                        }
                        else
                        {
                            var filePath = await fileService.UploadFileAsync(photo, "manken-photos");
                            
                            // Fotoğraf yolunu kaydet
                            manken.PhotoPath = "/" + filePath;
                            
                            TempData["Success"] = "Manken ve fotoğraf başarıyla eklendi.";
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["Warning"] = $"Manken eklendi ancak fotoğraf yüklenirken hata oluştu: {ex.Message}";
                    }
                }
                else
                {
                    TempData["Success"] = "Manken başarıyla eklendi.";
                }
                
                await _mankenService.CreateMankenAsync(manken);
                
                return RedirectToAction(nameof(Index));
            }
            
            // ModelState hatalarını detaylı olarak logla
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            var errorMessage = string.Join(", ", errors);
            TempData["Error"] = $"Manken eklenirken bir hata oluştu: {errorMessage}";
            
            return View(manken);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var manken = await _mankenService.GetMankenByIdAsync(id);
            if (manken == null)
            {
                return NotFound();
            }
            return View(manken);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Manken manken, IFormFile? photo)
        {
            if (id != manken.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingManken = await _mankenService.GetMankenByIdAsync(id);
                if (existingManken != null)
                {
                    manken.UserId = existingManken.UserId;
                    manken.CreatedAt = existingManken.CreatedAt;
                    manken.CreatedById = existingManken.CreatedById;
                    manken.PhotoPath = existingManken.PhotoPath; // Mevcut fotoğraf yolunu koru
                }
                
                // Yeni fotoğraf yükleme işlemi
                if (photo != null && photo.Length > 0)
                {
                    try
                    {
                        var fileService = HttpContext.RequestServices.GetRequiredService<IFileService>();
                        
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(photo.FileName).ToLowerInvariant();
                        
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            TempData["Warning"] = "Fotoğraf formatı desteklenmiyor. Sadece JPG, PNG ve GIF formatları kabul edilir.";
                        }
                        else if (photo.Length > 5 * 1024 * 1024) // 5MB limit
                        {
                            TempData["Warning"] = "Fotoğraf boyutu 5MB'dan büyük olamaz.";
                        }
                        else
                        {
                            // Eski fotoğrafı sil
                            if (!string.IsNullOrEmpty(existingManken?.PhotoPath))
                            {
                                fileService.DeleteFile(existingManken.PhotoPath);
                            }
                            
                            var filePath = await fileService.UploadFileAsync(photo, "manken-photos");
                            manken.PhotoPath = "/" + filePath;
                            
                            TempData["Success"] = "Manken ve fotoğraf başarıyla güncellendi.";
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["Warning"] = $"Manken güncellendi ancak fotoğraf yüklenirken hata oluştu: {ex.Message}";
                    }
                }
                else
                {
                    // Yeni fotoğraf yüklenmediyse mevcut fotoğraf yolunu koru
                    if (existingManken != null)
                    {
                        manken.PhotoPath = existingManken.PhotoPath;
                    }
                    TempData["Success"] = "Manken başarıyla güncellendi.";
                }
                
                await _mankenService.UpdateMankenAsync(manken);
                return RedirectToAction(nameof(Index));
            }
            
            // ModelState hatalarını detaylı olarak logla
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            var errorMessage = string.Join(", ", errors);
            TempData["Error"] = $"Manken güncellenirken bir hata oluştu: {errorMessage}";
            return View(manken);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var manken = await _mankenService.GetMankenByIdAsync(id);
            if (manken == null)
            {
                return NotFound();
            }
            return View(manken);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _mankenService.DeleteMankenAsync(id);
                TempData["Success"] = "Manken başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Manken silinirken hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var manken = await _context.Mankens.FirstOrDefaultAsync(m => m.Id == id);
                
            if (manken == null)
            {
                return NotFound();
            }
            return View(manken);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMankens()
        {
            var mankens = await _mankenService.GetAllMankensAsync();
            return Json(mankens.Select(m => new { m.Id, m.FirstName, m.LastName, m.Category }));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Search(string searchTerm, string gender, string category, int? minHeight, int? maxHeight)
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

            var query = _context.Mankens.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(m => m.FirstName.Contains(searchTerm) || m.LastName.Contains(searchTerm) || m.Email.Contains(searchTerm));
            }

            if (genderEnum.HasValue)
            {
                query = query.Where(m => m.Gender == genderEnum.Value);
            }

            if (categoryEnum.HasValue)
            {
                query = query.Where(m => m.Category == categoryEnum.Value);
            }

            if (minHeight.HasValue)
            {
                query = query.Where(m => m.Height >= minHeight.Value);
            }

            if (maxHeight.HasValue)
            {
                query = query.Where(m => m.Height <= maxHeight.Value);
            }

            var mankens = await query.ToListAsync();
            return View("Index", mankens);
        }
    }
} 