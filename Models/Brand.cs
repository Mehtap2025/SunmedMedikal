using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
namespace SunmedMedikal.Models

{
    public class Brand
    {
        public int Id { get; set; }
        
        // Markanın Adı (Örn: MERCK)
        public required string Name { get; set; } 
        
        // Markanın Logosunun Yolu (İşte hata veren kısım buydu, şimdi ekledik!)
        public string? ImageUrl { get; set; } 

        //Bilgisayardan seçilen dosyanın arka plana taşınması için (Veritabanına yansımaz)
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}