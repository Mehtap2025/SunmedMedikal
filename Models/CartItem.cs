using System.ComponentModel.DataAnnotations;

namespace SunmedMedikal.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        
        // SİHİR BURADA: Hangi kullanıcıya ait olduğunu kalıcı tutacak
        public string? Username { get; set; } 
        
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; } 
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsQuoteOnly { get; set; }
        
        public int MaxStock { get; set; } 
    }
}