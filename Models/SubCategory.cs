namespace SunmedMedikal.Models
{
    public class SubCategory
    {
        public int Id { get; set; }
        
        // Hangi ana kategoriye ait? (Örn: "Kimyasallar")
        public string MainCategory { get; set; } 
        
        // Alt kategorinin kendi adı (Örn: "Analitik Kimyasallar")
        public string Name { get; set; } 
    }
}