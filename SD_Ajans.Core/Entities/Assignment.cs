using System.ComponentModel.DataAnnotations;

namespace SD_Ajans.Core.Entities
{
    public class Assignment : BaseEntity
    {
        public int MankenId { get; set; }
        public virtual Manken? Manken { get; set; }
        
        public int OrganizationId { get; set; }
        public virtual Organization? Organization { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }
        
        public decimal Fee { get; set; }
        
        public AssignmentStatus Status { get; set; } = AssignmentStatus.Scheduled;
        
        public decimal DailyRate { get; set; }
        
        public int NumberOfDays { get; set; } = 1;
        
        public bool IncludesMeal { get; set; }
        
        public bool IncludesAccommodation { get; set; }
        
        public decimal MealCost { get; set; }
        
        public decimal AccommodationCost { get; set; }
        
        public decimal TotalPayment { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public DateTime? CompletedAt { get; set; }
    }
    
    public enum AssignmentStatus
    {
        Scheduled,
        InProgress,
        Completed,
        Cancelled
    }
} 