public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; } // Hangi siparişe ait?
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}