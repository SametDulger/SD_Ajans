using System.ComponentModel.DataAnnotations;

namespace SD_Ajans.Core.Entities
{
    public class Payment : BaseEntity
    {
        [Required(ErrorMessage = "Organizasyon seçimi zorunludur.")]
        public int OrganizationId { get; set; }
        public virtual Organization? Organization { get; set; }
        
        public int? AssignmentId { get; set; }
        public virtual Assignment? Assignment { get; set; }
        
        [Required(ErrorMessage = "Ödeme tipi zorunludur.")]
        public PaymentType PaymentType { get; set; }
        
        [Required(ErrorMessage = "Tutar zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tutar 0'dan büyük olmalıdır.")]
        public decimal Amount { get; set; }
        
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        
        [Required(ErrorMessage = "Ödeme tarihi zorunludur.")]
        public DateTime PaymentDate { get; set; }
        
        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir.")]
        public string? Notes { get; set; }
        
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Description { get; set; }
        
        public string? ProcessedById { get; set; }
        public virtual User? ProcessedBy { get; set; }
    }
    
    public enum PaymentType
    {
        Cash,
        BankTransfer,
        CreditCard
    }
    
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Cancelled
    }
} 