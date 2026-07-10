using System.ComponentModel.DataAnnotations;

namespace SunmedMedikal.Models
{
    public class SubSubSubSubCategory
    {
        [Key]
        public int Id { get; set; }
        
        public string MainCategory { get; set; }
        public string SubCategory { get; set; }
        public string SubSubCategory { get; set; }
        public string SubSubSubCategory { get; set; }
        
        // 5. Seviye Kategorinin Kendi Adı
        public string Name { get; set; } 
    }
}