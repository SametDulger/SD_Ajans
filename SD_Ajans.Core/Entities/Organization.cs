using System.ComponentModel.DataAnnotations;

namespace SD_Ajans.Core.Entities
{
    public class Organization : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Location { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public OrganizationType Type { get; set; }
        
        public decimal TotalBudget { get; set; }
        
        public OrganizationStatus Status { get; set; } = OrganizationStatus.Planned;
        
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