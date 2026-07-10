using Microsoft.AspNetCore.Mvc;
using SunmedMedikal.Models;
using SunmedMedikal.Data;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks; 

namespace SunmedMedikal.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return RedirectToAction("Index", "Home");

            q = q.ToLower().Trim();

            var products = _context.Products.Where(p => 
                (p.Name != null && p.Name.ToLower().Contains(q)) ||
                (p.Category != null && p.Category.ToLower().Contains(q)) ||
                (p.SubCategory != null && p.SubCategory.ToLower().Contains(q)) ||
                (p.SubSubCategory != null && p.SubSubCategory.ToLower().Contains(q)) ||
                (p.SubSubSubCategory != null && p.SubSubSubCategory.ToLower().Contains(q)) ||
                (p.SubSubSubSubCategory != null && p.SubSubSubSubCategory.ToLower().Contains(q)) || 
                (p.Brand != null && p.Brand.ToLower().Contains(q)) ||
                (p.ProductCode != null && p.ProductCode.ToLower().Contains(q)) ||
                (p.Tags != null && p.Tags.ToLower().Contains(q)) 
            ).ToList();

            ViewBag.CategoryName = $"Arama Sonuçları: '{q}'";
            return View("Chemicals", products);
        }

        // ==========================================================
        // DİNAMİK KATEGORİ YÖNLENDİRİCİSİ (MARKA VE TÜRKÇE KARAKTER DESTEĞİ EKLENDİ)
        // ==========================================================
        public IActionResult Category(string main, string sub, string subsub, string subsubsub, string subsubsubsub, string brand)
        {
            if (string.IsNullOrEmpty(main)) return RedirectToAction("Index", "Home");

            var query = _context.Products.AsQueryable();

            // TÜRKÇE KARAKTER "ı" ve "i" UYUŞMAZLIĞINI ÇÖZEN KISIM
            if (main == "Referans Standartları" || main == "Referans Standartlari")
            {
                query = query.Where(p => p.Category == "Referans Standartları" || p.Category == "Referans Standartlari");
            }
            else
            {
                query = query.Where(p => p.Category == main);
            }

            if (!string.IsNullOrEmpty(sub))
            {
                query = query.Where(p => p.SubCategory == sub);
                ViewBag.CategoryName = $"{main} > {sub}"; 
                
                if (!string.IsNullOrEmpty(subsub))
                {
                    query = query.Where(p => p.SubSubCategory == subsub);
                    ViewBag.CategoryName = $"{main} > {sub} > {subsub}";

                    if (!string.IsNullOrEmpty(subsubsub))
                    {
                        query = query.Where(p => p.SubSubSubCategory == subsubsub);
                        ViewBag.CategoryName = $"{main} > {sub} > {subsub} > {subsubsub}";

                        if (!string.IsNullOrEmpty(subsubsubsub))
                        {
                            query = query.Where(p => p.SubSubSubSubCategory == subsubsubsub);
                            ViewBag.CategoryName = $"{main} > {sub} > {subsub} > {subsubsub} > {subsubsubsub}";
                        }
                    }
                }
            }
            else
            {
                ViewBag.CategoryName = main;
            }

            // EĞER MENÜDEN "LGC" GİBİ BİR MARKA SEÇİLDİYSE LİSTEYİ ONA GÖRE FİLTRELE
            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(p => p.Brand == brand);
                ViewBag.CategoryName += $" - {brand} Ürünleri";
            }

            var products = query.ToList();

            switch (main)
            {
                case "Kimyasallar": return View("Chemicals", products);
                case "Cam Malzemeler": return View("Glassware", products);
                case "Sarf Malzemeler": return View("Consumables", products);
                case "Laboratuvar Cihazları": return View("Devices", products);
                case "Referans Standartları": 
                case "Referans Standartlari": 
                    return View("Standards", products);
                default: return View("Chemicals", products); 
            }
        }

        public IActionResult Chemicals() { return RedirectToAction("Category", new { main = "Kimyasallar" }); }
        public IActionResult Glassware() { return RedirectToAction("Category", new { main = "Cam Malzemeler" }); }
        public IActionResult Consumables() { return RedirectToAction("Category", new { main = "Sarf Malzemeler" }); }
        public IActionResult Devices() { return RedirectToAction("Category", new { main = "Laboratuvar Cihazları" }); }
        public IActionResult Standards() { return RedirectToAction("Category", new { main = "Referans Standartları" }); }

        public IActionResult Details(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound("Ürün bulunamadı."); 
            return View(product);
        }
    }
}