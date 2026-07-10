using Microsoft.AspNetCore.Mvc;
using SunmedMedikal.Data;
using SunmedMedikal.Models;
using System.Linq;
using System.Collections.Generic;

namespace SunmedMedikal.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string query)
        {
            ViewBag.SearchQuery = query;

            // Eğer arama kutusu boşsa, sayfaya boş bir liste gönder
            if (string.IsNullOrEmpty(query))
            {
                return View(new List<Product>());
            }

            // SQL'e git: Ürün adında VEYA açıklamasında bu kelime geçenleri bul ve getir (Büyük/küçük harf duyarlılığını kaldırarak)
            var results = _context.Products
                .Where(p => p.Name.Contains(query) || p.Description.Contains(query))
                .ToList();

            return View(results);
        }
    }
}