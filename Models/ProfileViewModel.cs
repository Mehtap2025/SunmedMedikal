using System.ComponentModel.DataAnnotations;

namespace SunmedMedikal.Models
{
    public class ProfileViewModel
    {
        public string? CustomerType { get; set; } // Bireysel mi Kurumsal mı? (Ekranda gizli tutacağız)
        public string? Username { get; set; } // Kullanıcı Adı (Sadece okuma amaçlı gösterebiliriz)

        [Required(ErrorMessage = "Ad Soyadı / Yetkili alanı zorunludur.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "E-Posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir E-Posta giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon numarası zorunludur.")]
        public string PhoneNumber { get; set; }

        // --- BİREYSEL MÜŞTERİ ALANLARI ---
        public string? TcNo { get; set; }

        // --- KURUMSAL MÜŞTERİ ALANLARI ---
        public string? CompanyTitle { get; set; }
        public string? CompanyName { get; set; }
        public string? TaxOffice { get; set; }
        public string? TaxNumber { get; set; }

        // --- ŞİFRE DEĞİŞTİRMEK İSTERSE ---
        public string? CurrentPassword { get; set; } 
        public string? NewPassword { get; set; }  

        // --- ADRES BİLGİLERİ ---
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Address { get; set; }   
    }
}