using System.ComponentModel.DataAnnotations;

namespace SD_Ajans.Core.Entities
{
    public class Manken : BaseEntity
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string LastName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir.")]
        public string Email { get; set; } = string.Empty;
        
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [StringLength(20, ErrorMessage = "Telefon numarası en fazla 20 karakter olabilir.")]
        public string? Phone { get; set; }
        
        [Required(ErrorMessage = "Doğum tarihi zorunludur.")]
        public DateTime BirthDate { get; set; }
        
        [Required(ErrorMessage = "Cinsiyet seçimi zorunludur.")]
        public Gender Gender { get; set; }
        
        [Required(ErrorMessage = "Boy bilgisi zorunludur.")]
        [Range(100, 250, ErrorMessage = "Boy 100-250 cm arasında olmalıdır.")]
        public int Height { get; set; }
        
        [Required(ErrorMessage = "Kilo bilgisi zorunludur.")]
        [Range(30, 200, ErrorMessage = "Kilo 30-200 kg arasında olmalıdır.")]
        public int Weight { get; set; }
        
        [StringLength(50, ErrorMessage = "Saç rengi en fazla 50 karakter olabilir.")]
        public string? HairColor { get; set; }
        
        [StringLength(50, ErrorMessage = "Göz rengi en fazla 50 karakter olabilir.")]
        public string? EyeColor { get; set; }
        
        [StringLength(100, ErrorMessage = "Şehir en fazla 100 karakter olabilir.")]
        public string? City { get; set; }
        
        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        public MankenCategory Category { get; set; }
        
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Description { get; set; }
        
        public bool IsAvailable { get; set; } = true;
        
        [StringLength(255, ErrorMessage = "Fotoğraf yolu en fazla 255 karakter olabilir.")]
        public string? PhotoPath { get; set; }
        
        // Navigation properties
        public virtual ICollection<Assignment>? Assignments { get; set; }
        
        // Computed property
        public string FullName => $"{FirstName} {LastName}";
        public int Age => DateTime.Now.Year - BirthDate.Year - (DateTime.Now.DayOfYear < BirthDate.DayOfYear ? 1 : 0);
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