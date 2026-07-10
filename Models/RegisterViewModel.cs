using System.ComponentModel.DataAnnotations;

namespace SunmedMedikal.Models 
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Hesap türü seçimi zorunludur.")]
        public string AccountType { get; set; } = "Bireysel";

        [Required(ErrorMessage = "Adı Soyadı / Yetkili Adı alanı zorunludur.")]
        public string FullName { get; set; }

        public string? TCKimlikNo { get; set; } // Zorunlu değil

        [Required(ErrorMessage = "E-Posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir E-Posta adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon numarası zorunludur.")]
        [RegularExpression(@"^\+905\d{9}$", ErrorMessage = "Lütfen +905XXXXXXXXX formatında giriniz.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Sisteme giriş adınız zorunludur.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [RegularExpression(@"^(?=.*[a-zA-ZçÇğĞıİöÖşŞüÜ])(?=.*[0-9]).{6,}$", ErrorMessage = "Şifreniz en az 6 karakter olmalı, harf ve sayı içermelidir.")]
        public string Password { get; set; }

        // --- KURUMSAL ÖZEL ALANLAR ---
        // (Zorunluluk kontrollerini Controller'da "Eğer Kurumsalsa" diyerek yapacağız)
        public string? CompanyTitle { get; set; } 
        public string? CompanyName { get; set; }  
        public string? TaxOffice { get; set; }    
        public string? TaxNumber { get; set; }    
    }
}