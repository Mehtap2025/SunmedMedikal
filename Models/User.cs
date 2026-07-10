namespace SunmedMedikal.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        
        // --- YENİ EKLENEN ORTAK BİLGİLER ---
        public string Email { get; set; } 
        public string PhoneNumber { get; set; } 

        // --- BİREYSEL / KURUMSAL AYRIMI ---
        public string? CustomerType { get; set; } // "Bireysel" veya "Kurumsal"
        public string? TcNo { get; set; }
        
        // --- KURUMSAL ÖZEL ---
        public string? CompanyTitle { get; set; } // Firma Ünvanı (Opsiyonel)
        public string? CompanyName { get; set; }  // Firma Adı
        public string? TaxOffice { get; set; }    // Vergi Dairesi
        public string? TaxNumber { get; set; }    // Vergi Numarası

        // --- ADRES BİLGİLERİ ---
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Address { get; set; }
    }
}