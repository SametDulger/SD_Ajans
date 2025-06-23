using System.ComponentModel.DataAnnotations;

namespace SD_Ajans.Core.Entities
{
    public class Payment : BaseEntity
    {
        public int OrganizationId { get; set; }
        public virtual Organization? Organization { get; set; }
        
        public int? AssignmentId { get; set; }
        public virtual Assignment? Assignment { get; set; }
        
        public PaymentType PaymentType { get; set; }
        
        public decimal Amount { get; set; }
        
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        
        public DateTime PaymentDate { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [StringLength(500)]
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