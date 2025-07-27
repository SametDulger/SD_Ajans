using System.ComponentModel.DataAnnotations;

namespace SD_Ajans.Core.Entities
{
    public class Assignment : BaseEntity
    {
        [Required(ErrorMessage = "Manken seçimi zorunludur.")]
        public int MankenId { get; set; }
        public virtual Manken? Manken { get; set; }
        
        [Required(ErrorMessage = "Organizasyon seçimi zorunludur.")]
        public int OrganizationId { get; set; }
        public virtual Organization? Organization { get; set; }
        
        [Required(ErrorMessage = "Başlangıç zamanı zorunludur.")]
        public DateTime StartTime { get; set; }
        
        [Required(ErrorMessage = "Bitiş zamanı zorunludur.")]
        public DateTime EndTime { get; set; }
        
        [Required(ErrorMessage = "Ücret zorunludur.")]
        [Range(0, double.MaxValue, ErrorMessage = "Ücret 0'dan büyük olmalıdır.")]
        public decimal Fee { get; set; }
        
        public AssignmentStatus Status { get; set; } = AssignmentStatus.Scheduled;
        
        [Required(ErrorMessage = "Günlük ücret zorunludur.")]
        [Range(0, double.MaxValue, ErrorMessage = "Günlük ücret 0'dan büyük olmalıdır.")]
        public decimal DailyRate { get; set; }
        
        [Required(ErrorMessage = "Gün sayısı zorunludur.")]
        [Range(1, 365, ErrorMessage = "Gün sayısı 1-365 arasında olmalıdır.")]
        public int NumberOfDays { get; set; } = 1;
        
        public bool IncludesMeal { get; set; }
        
        public bool IncludesAccommodation { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Yemek maliyeti 0'dan büyük olmalıdır.")]
        public decimal MealCost { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Konaklama maliyeti 0'dan büyük olmalıdır.")]
        public decimal AccommodationCost { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Toplam ödeme 0'dan büyük olmalıdır.")]
        public decimal TotalPayment { get; set; }
        
        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir.")]
        public string? Notes { get; set; }
        
        public DateTime? CompletedAt { get; set; }
        
        // Computed properties
        public TimeSpan Duration => EndTime - StartTime;
        public bool IsOverlapping(DateTime start, DateTime end) => 
            (StartTime <= end && EndTime >= start);
    }
    
    public enum AssignmentStatus
    {
        Scheduled,
        InProgress,
        Completed,
        Cancelled
    }
} 