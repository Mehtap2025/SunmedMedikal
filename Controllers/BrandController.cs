using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SunmedMedikal.Data;
using SunmedMedikal.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;

namespace SunmedMedikal.Controllers
{
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env; // Klasör yollarını bulmak için eklendi

        public BrandController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // 1. HERKESİN GÖRDÜĞÜ MARKALAR GALERİSİ
        public IActionResult Index()
        {
            var brands = _context.Brands.ToList();
            return View(brands);
        }

        // ================= AŞAĞISI SADECE ADMİN İÇİNDİR =================

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // 3. FORMDAN GELEN BİLGİLERİ VE RESMİ KAYDET
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Brand brand)
        {
            if (ModelState.IsValid)
            {
                // YENİ: RESİM YÜKLEME KODU
                if (brand.ImageFile != null && brand.ImageFile.Length > 0)
                {
                    string uploadFolder = Path.Combine(_env.WebRootPath, "images", "brands");
                    if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);
                    
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + brand.ImageFile.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create)) 
                    { 
                        await brand.ImageFile.CopyToAsync(fileStream); 
                    }
                    
                    // Dosyanın yolunu veritabanına yazılmak üzere hazırlıyoruz
                    brand.ImageUrl = "/images/brands/" + uniqueFileName;
                }
                else
                {
                    brand.ImageUrl = "https://via.placeholder.com/300x200/343a40/ffffff?text=Logo+Yok";
                }

                _context.Brands.Add(brand);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return NotFound();
            return View(brand);
        }

        // 5. DÜZENLENEN BİLGİLERİ KAYDET (Resim değiştirilirse diye burayı da güncelledim)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Brand brand)
        {
            if (id != brand.Id) return NotFound();
            
            if (ModelState.IsValid)
            {
                // DÜZENLEMEDE YENİ RESİM YÜKLENİRSE
                if (brand.ImageFile != null && brand.ImageFile.Length > 0)
                {
                    string uploadFolder = Path.Combine(_env.WebRootPath, "images", "brands");
                    if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);
                    
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + brand.ImageFile.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create)) 
                    { 
                        await brand.ImageFile.CopyToAsync(fileStream); 
                    }
                    brand.ImageUrl = "/images/brands/" + uniqueFileName;
                }

                _context.Update(brand);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand != null)
            {
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}