using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SD_Ajans.Core.Entities
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        
        public UserRole Role { get; set; }
        
        public virtual ICollection<Manken>? Mankens { get; set; }
        public virtual ICollection<Organization>? CreatedOrganizations { get; set; }
    }
    
    public enum UserRole
    {
        Admin
    }
} 