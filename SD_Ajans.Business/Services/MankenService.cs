using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SD_Ajans.Core.Entities;
using SD_Ajans.Core.Repositories;
using System.Linq.Expressions;

namespace SD_Ajans.Business.Services
{
    public class MankenService : IMankenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MankenService> _logger;

        public MankenService(IUnitOfWork unitOfWork, ILogger<MankenService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<Manken>> GetAllMankensAsync()
        {
            try
            {
                return await _unitOfWork.Repository<Manken>().FindAsync(m => m.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllMankensAsync metodu çalışırken hata oluştu");
                throw;
            }
        }

        public async Task<Manken?> GetMankenByIdAsync(int id)
        {
            try
            {
                return await _unitOfWork.Repository<Manken>().GetAsync(m => m.Id == id && m.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMankenByIdAsync metodu çalışırken hata oluştu. Id: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Manken>> SearchMankensAsync(string searchTerm, Gender? gender = null, MankenCategory? category = null, int? minHeight = null, int? maxHeight = null)
        {
            try
            {
                var query = _unitOfWork.Repository<Manken>().Query()
                    .Where(m => m.IsActive);

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.ToLowerInvariant();
                    query = query.Where(m => m.FirstName.ToLower().Contains(searchTerm) || 
                                           m.LastName.ToLower().Contains(searchTerm) || 
                                           m.Email.ToLower().Contains(searchTerm));
                }

                if (gender.HasValue)
                    query = query.Where(m => m.Gender == gender.Value);

                if (category.HasValue)
                    query = query.Where(m => m.Category == category.Value);

                if (minHeight.HasValue)
                    query = query.Where(m => m.Height >= minHeight.Value);

                if (maxHeight.HasValue)
                    query = query.Where(m => m.Height <= maxHeight.Value);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchMankensAsync metodu çalışırken hata oluştu. SearchTerm: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<Manken>> GetAvailableMankensForDateAsync(DateTime date)
        {
            try
            {
                var allMankens = await _unitOfWork.Repository<Manken>().FindAsync(m => m.IsActive && m.IsAvailable);
                
                // Belirtilen tarihte görevlendirilmiş mankenleri bul
                var assignedMankens = await _unitOfWork.Repository<Assignment>().FindAsync(a => 
                    a.IsActive && 
                    a.Status != AssignmentStatus.Cancelled &&
                    a.StartTime.Date <= date.Date && 
                    a.EndTime.Date >= date.Date);
                
                var assignedMankenIds = assignedMankens.Select(a => a.MankenId).ToHashSet();
                
                return allMankens.Where(m => !assignedMankenIds.Contains(m.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAvailableMankensForDateAsync metodu çalışırken hata oluştu. Date: {Date}", date);
                throw;
            }
        }

        public async Task<Manken> CreateMankenAsync(Manken manken)
        {
            try
            {
                // Email benzersizlik kontrolü
                var existingManken = await _unitOfWork.Repository<Manken>().GetAsync(m => m.Email == manken.Email);
                if (existingManken != null)
                {
                    throw new InvalidOperationException("Bu e-posta adresi zaten kullanılıyor.");
                }

                manken.CreatedAt = DateTime.Now;
                manken.IsActive = true;
                manken.IsAvailable = true;

                await _unitOfWork.Repository<Manken>().AddAsync(manken);
                await _unitOfWork.SaveChangesAsync();
                
                _logger.LogInformation("Yeni manken oluşturuldu. Id: {Id}, Ad: {FirstName} {LastName}", 
                    manken.Id, manken.FirstName, manken.LastName);
                
                return manken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateMankenAsync metodu çalışırken hata oluştu");
                throw;
            }
        }

        public async Task<Manken> UpdateMankenAsync(Manken manken)
        {
            try
            {
                var existing = await _unitOfWork.Repository<Manken>().GetByIdAsync(manken.Id);
                if (existing == null)
                    throw new InvalidOperationException("Manken bulunamadı.");

                // Email benzersizlik kontrolü (kendi email'i hariç)
                var existingWithEmail = await _unitOfWork.Repository<Manken>().GetAsync(m => 
                    m.Email == manken.Email && m.Id != manken.Id);
                if (existingWithEmail != null)
                {
                    throw new InvalidOperationException("Bu e-posta adresi başka bir manken tarafından kullanılıyor.");
                }

                // Sadece güncellenebilir alanları set et
                existing.FirstName = manken.FirstName;
                existing.LastName = manken.LastName;
                existing.Email = manken.Email;
                existing.Phone = manken.Phone;
                existing.BirthDate = manken.BirthDate;
                existing.Gender = manken.Gender;
                existing.Height = manken.Height;
                existing.Weight = manken.Weight;
                existing.HairColor = manken.HairColor;
                existing.EyeColor = manken.EyeColor;
                existing.City = manken.City;
                existing.Category = manken.Category;
                existing.Description = manken.Description;
                existing.IsAvailable = manken.IsAvailable;
                existing.PhotoPath = manken.PhotoPath;
                existing.UpdatedAt = DateTime.Now;

                await _unitOfWork.Repository<Manken>().UpdateAsync(existing);
                await _unitOfWork.SaveChangesAsync();
                
                _logger.LogInformation("Manken güncellendi. Id: {Id}, Ad: {FirstName} {LastName}", 
                    existing.Id, existing.FirstName, existing.LastName);
                
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateMankenAsync metodu çalışırken hata oluştu. Id: {Id}", manken.Id);
                throw;
            }
        }

        public async Task DeleteMankenAsync(int id)
        {
            try
            {
                var manken = await _unitOfWork.Repository<Manken>().GetByIdAsync(id);
                if (manken == null)
                    throw new InvalidOperationException("Manken bulunamadı.");

                // Aktif görevlendirmesi var mı kontrol et
                var activeAssignments = await _unitOfWork.Repository<Assignment>().FindAsync(a => 
                    a.MankenId == id && 
                    a.IsActive && 
                    a.Status != AssignmentStatus.Cancelled);
                
                if (activeAssignments.Any())
                {
                    throw new InvalidOperationException("Bu mankenin aktif görevlendirmeleri bulunduğu için silinemez.");
                }

                manken.IsActive = false;
                manken.UpdatedAt = DateTime.Now;

                await _unitOfWork.Repository<Manken>().UpdateAsync(manken);
                await _unitOfWork.SaveChangesAsync();
                
                _logger.LogInformation("Manken silindi. Id: {Id}, Ad: {FirstName} {LastName}", 
                    manken.Id, manken.FirstName, manken.LastName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteMankenAsync metodu çalışırken hata oluştu. Id: {Id}", id);
                throw;
            }
        }

        public async Task<bool> IsMankenAvailableAsync(int mankenId, DateTime date)
        {
            try
            {
                var assignments = await _unitOfWork.Repository<Assignment>().FindAsync(a => 
                    a.MankenId == mankenId && 
                    a.IsActive &&
                    a.Status != AssignmentStatus.Cancelled &&
                    a.StartTime.Date <= date.Date && 
                    a.EndTime.Date >= date.Date);
                
                return !assignments.Any();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IsMankenAvailableAsync metodu çalışırken hata oluştu. MankenId: {MankenId}, Date: {Date}", 
                    mankenId, date);
                throw;
            }
        }

        public async Task<IEnumerable<Manken>> GetMankensByCategoryAsync(MankenCategory category)
        {
            try
            {
                return await _unitOfWork.Repository<Manken>().FindAsync(m => m.IsActive && m.Category == category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMankensByCategoryAsync metodu çalışırken hata oluştu. Category: {Category}", category);
                throw;
            }
        }
    }
} 