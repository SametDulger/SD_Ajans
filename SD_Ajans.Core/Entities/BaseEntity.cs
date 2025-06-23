using System.ComponentModel.DataAnnotations;

namespace SD_Ajans.Core.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CreatedById { get; set; }
        public virtual User? CreatedBy { get; set; }
    }
} 