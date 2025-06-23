using System.ComponentModel.DataAnnotations;

namespace SD_Ajans.Core.Entities
{
    public class Manken : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Email { get; set; }
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        public DateTime BirthDate { get; set; }
        
        public Gender Gender { get; set; }
        
        public int Height { get; set; }
        
        public int Weight { get; set; }
        
        [StringLength(50)]
        public string? HairColor { get; set; }
        
        [StringLength(50)]
        public string? EyeColor { get; set; }
        
        [StringLength(100)]
        public string? City { get; set; }
        
        public MankenCategory Category { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public bool IsAvailable { get; set; } = true;
        
        [StringLength(255)]
        public string? PhotoPath { get; set; }
        
        public string? UserId { get; set; }
        public virtual User? User { get; set; }
        
        public virtual ICollection<Assignment>? Assignments { get; set; }
    }
    
    public enum Gender
    {
        Male,
        Female,
        Child
    }
    
    public enum MankenCategory
    {
        Category1 = 1,
        Category2 = 2,
        Category3 = 3
    }
} 