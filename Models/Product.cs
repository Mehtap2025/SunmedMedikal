using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunmedMedikal.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        public string? ProductCode { get; set; }
        public string? Brand { get; set; }
        
        public string Name { get; set; }
        public string Description { get; set; }

        public string? ProductInfo { get; set; }     // Ürün Bilgisi
        public string? ProductDetail { get; set; }   // Ürün Detayı
        public string? Tags { get; set; }            // Gizli Etiketler

        public decimal Price { get; set; }
        public string Currency { get; set; } = "TL"; 
        
        // KATEGORİ ALANLARI
        public string Category { get; set; } = "Kimyasallar";
        public string? SubCategory { get; set; }
        public string? SubSubCategory { get; set; }
        public string? SubSubSubCategory { get; set; } 
        public string? SubSubSubSubCategory { get; set; } // YENİ: 5. SEVİYE ALT KATEGORİ
        
        public string? ImageUrl { get; set; } 
        public string? AdditionalImageUrls { get; set; }
        
        public int StockQuantity { get; set; } = 100; 
        public bool IsQuoteOnly { get; set; }

        [NotMapped] 
        public List<IFormFile>? ImageFiles { get; set; }
    }
}