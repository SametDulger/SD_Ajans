using SD_Ajans.Core.Entities;

namespace SD_Ajans.Business.Services
{
    public interface IMankenService
    {
        Task<IEnumerable<Manken>> GetAllMankensAsync();
        Task<Manken?> GetMankenByIdAsync(int id);
        Task<IEnumerable<Manken>> SearchMankensAsync(string searchTerm, Gender? gender = null, MankenCategory? category = null, int? minHeight = null, int? maxHeight = null);
        Task<IEnumerable<Manken>> GetAvailableMankensForDateAsync(DateTime date);
        Task<Manken> CreateMankenAsync(Manken manken);
        Task<Manken> UpdateMankenAsync(Manken manken);
        Task DeleteMankenAsync(int id);
        Task<bool> IsMankenAvailableAsync(int mankenId, DateTime date);
        Task<IEnumerable<Manken>> GetMankensByCategoryAsync(MankenCategory category);
    }
} 