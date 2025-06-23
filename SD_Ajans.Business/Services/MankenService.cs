using Microsoft.EntityFrameworkCore;
using SD_Ajans.Core.Entities;
using SD_Ajans.Core.Repositories;
using System.Linq.Expressions;

namespace SD_Ajans.Business.Services
{
    public class MankenService : IMankenService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MankenService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Manken>> GetAllMankensAsync()
        {
            return await _unitOfWork.Repository<Manken>().GetAllAsync();
        }

        public async Task<Manken?> GetMankenByIdAsync(int id)
        {
            return await _unitOfWork.Repository<Manken>().GetByIdAsync(id);
        }

        public async Task<IEnumerable<Manken>> SearchMankensAsync(string searchTerm, Gender? gender = null, MankenCategory? category = null, int? minHeight = null, int? maxHeight = null)
        {
            var query = _unitOfWork.Repository<Manken>().Query();

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(m => (m.FirstName != null && m.FirstName.Contains(searchTerm)) || (m.LastName != null && m.LastName.Contains(searchTerm)) || (m.Email != null && m.Email.Contains(searchTerm)));

            if (gender.HasValue)
                query = query.Where(m => m.Gender == gender.Value);

            if (category.HasValue)
                query = query.Where(m => m.Category == category.Value);

            if (minHeight.HasValue)
                query = query.Where(m => m.Height >= minHeight.Value);

            if (maxHeight.HasValue)
                query = query.Where(m => m.Height <= maxHeight.Value);

            query = query.Where(m => m.IsActive);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Manken>> GetAvailableMankensForDateAsync(DateTime date)
        {
            var allMankens = await _unitOfWork.Repository<Manken>().FindAsync(m => m.IsActive && m.IsAvailable);
            var assignedMankens = await _unitOfWork.Repository<Assignment>().FindAsync(a => a.Organization != null && a.Organization.Date == date && a.Status != AssignmentStatus.Cancelled);
            
            var assignedMankenIds = assignedMankens.Select(a => a.MankenId).ToHashSet();
            return allMankens.Where(m => !assignedMankenIds.Contains(m.Id));
        }

        public async Task<Manken> CreateMankenAsync(Manken manken)
        {
            await _unitOfWork.Repository<Manken>().AddAsync(manken);
            await _unitOfWork.SaveChangesAsync();
            return manken;
        }

        public async Task<Manken> UpdateMankenAsync(Manken manken)
        {
            var existing = await _unitOfWork.Repository<Manken>().GetByIdAsync(manken.Id);
            if (existing == null)
                throw new Exception("Manken bulunamadı.");

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
            return existing;
        }

        public async Task DeleteMankenAsync(int id)
        {
            var manken = await _unitOfWork.Repository<Manken>().GetByIdAsync(id);
            if (manken != null)
            {
                manken.IsActive = false;
                manken.UpdatedAt = DateTime.Now;
                await _unitOfWork.Repository<Manken>().UpdateAsync(manken);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> IsMankenAvailableAsync(int mankenId, DateTime date)
        {
            var assignments = await _unitOfWork.Repository<Assignment>().FindAsync(a => a.MankenId == mankenId && a.Organization!.Date == date && a.Status != AssignmentStatus.Cancelled);
            return !assignments.Any();
        }

        public async Task<IEnumerable<Manken>> GetMankensByCategoryAsync(MankenCategory category)
        {
            return await _unitOfWork.Repository<Manken>().FindAsync(m => m.IsActive && m.Category == category);
        }
    }
} 