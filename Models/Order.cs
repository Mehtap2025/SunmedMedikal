using System;
using System.Collections.Generic;

namespace SunmedMedikal.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string? OrderNotes { get; set; }
        
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        
        // Siparişin durumu: "Beklemede", "Onaylandı", "Teklif Hazırlanıyor" vb.
        public string Status { get; set; } = "Beklemede";
    }
}