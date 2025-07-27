using System.ComponentModel.DataAnnotations;

namespace SD_Ajans.Core.Entities
{
    public class Organization : BaseEntity
    {
        [Required(ErrorMessage = "Organizasyon adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Organizasyon adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Tarih zorunludur.")]
        public DateTime Date { get; set; }
        
        [Required(ErrorMessage = "Konum zorunludur.")]
        [StringLength(200, ErrorMessage = "Konum en fazla 200 karakter olabilir.")]
        public string Location { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Organizasyon tipi zorunludur.")]
        public OrganizationType Type { get; set; }
        
        [Required(ErrorMessage = "Toplam bütçe zorunludur.")]
        [Range(0, double.MaxValue, ErrorMessage = "Bütçe 0'dan büyük olmalıdır.")]
        public decimal TotalBudget { get; set; }
        
        public OrganizationStatus Status { get; set; } = OrganizationStatus.Planned;
        
        // Navigation properties
        public virtual ICollection<Assignment>? Assignments { get; set; }
        public virtual ICollection<Payment>? Payments { get; set; }
    }
    
    public enum OrganizationType
    {
        CosmeticPromotion,
        CeremonyHostess,
        HolidayAnimation,
        ConcertEvent,
        Other
    }
    
    public enum OrganizationStatus
    {
        Planned,
        InProgress,
        Completed,
        Cancelled
    }
} 