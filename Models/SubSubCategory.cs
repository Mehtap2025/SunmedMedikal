using System.ComponentModel.DataAnnotations;

namespace SunmedMedikal.Models
{
    public class SubSubCategory
    {
        public int Id { get; set; }
        
        [Required]
        public string MainCategory { get; set; } // Hangi Ana Kategoriye ait? (Örn: Cam Malzemeler)
        
        [Required]
        public string SubCategory { get; set; }  // Hangi Alt Kategoriye ait? (Örn: Beherler)
        
        [Required]
        public string Name { get; set; }         // Kendisi ne? (Örn: Cam Beherler)
    }
}