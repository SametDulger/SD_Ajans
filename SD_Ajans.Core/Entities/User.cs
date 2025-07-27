using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SD_Ajans.Core.Entities
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        public UserRole Role { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual ICollection<Manken>? Mankens { get; set; }
        public virtual ICollection<Organization>? CreatedOrganizations { get; set; }
        public virtual ICollection<Payment>? ProcessedPayments { get; set; }
    }
    
    public enum UserRole
    {
        Admin,
        Manager,
        User
    }
} 