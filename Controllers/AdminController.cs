using Microsoft.AspNetCore.Mvc;
using SunmedMedikal.Data;
using SunmedMedikal.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunmedMedikal.Services; 
using Microsoft.EntityFrameworkCore; 

namespace SunmedMedikal.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env; 
        private readonly EmailService _emailService;
        private readonly SmsService _smsService;
        private readonly LogoIsbasiService _logoIsbasiService;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment env, EmailService emailService, SmsService smsService, LogoIsbasiService logoIsbasiService)
        {
            _context = context;
            _env = env;
            _emailService = emailService;
            _smsService = smsService;
            _logoIsbasiService = logoIsbasiService;
        }

        // --- ARAMA VE ÇİFT YÖNLÜ SIRALAMA MOTORU ---
        public IActionResult Index(string? searchString = null, string? sortOrder = null)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;

            ViewData["IdSortParam"] = string.IsNullOrEmpty(sortOrder) ? "id_asc" : "";
            ViewData["CodeSortParam"] = sortOrder == "code_asc" ? "code_desc" : "code_asc";
            ViewData["BrandSortParam"] = sortOrder == "brand_asc" ? "brand_desc" : "brand_asc";
            ViewData["NameSortParam"] = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewData["StockSortParam"] = sortOrder == "stock_asc" ? "stock_desc" : "stock_asc";
            ViewData["PriceSortParam"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";

            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                products = products.Where(p => (p.Name != null && p.Name.Contains(searchString)) || 
                                               (p.ProductCode != null && p.ProductCode.Contains(searchString)));
            }

            switch (sortOrder)
            {
                case "id_asc":
                    products = products.OrderBy(p => p.Id);
                    break;
                case "code_asc":
                    products = products.OrderBy(p => p.ProductCode);
                    break;
                case "code_desc":
                    products = products.OrderByDescending(p => p.ProductCode);
                    break;
                case "brand_asc":
                    products = products.OrderBy(p => p.Brand);
                    break;
                case "brand_desc":
                    products = products.OrderByDescending(p => p.Brand);
                    break;
                case "name_asc":
                    products = products.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.Name);
                    break;
                case "stock_asc":
                    products = products.OrderBy(p => p.StockQuantity);
                    break;
                case "stock_desc":
                    products = products.OrderByDescending(p => p.StockQuantity);
                    break;
                case "price_asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                default:
                    products = products.OrderByDescending(p => p.Id);
                    break;
            }

            return View(products.ToList());
        }

        public IActionResult Create()
        {
            ViewBag.MainCategories = _context.MainCategories.OrderBy(m => m.Name).ToList();
            ViewBag.SubCategories = _context.SubCategories.ToList();
            ViewBag.SubSubCategories = _context.SubSubCategories.ToList();
            ViewBag.SubSubSubCategories = _context.SubSubSubCategories.ToList();
            ViewBag.SubSubSubSubCategories = _context.SubSubSubSubCategories.ToList(); 
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ImageFiles != null && product.ImageFiles.Count > 0)
                {
                    string uploadFolder = Path.Combine(_env.WebRootPath, "images", "products");
                    if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);
                    List<string> uploadedFiles = new List<string>();

                    foreach (var file in product.ImageFiles)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        string filePath = Path.Combine(uploadFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create)) { file.CopyTo(fileStream); }
                        uploadedFiles.Add("/images/products/" + uniqueFileName);
                    }

                    product.ImageUrl = uploadedFiles.First();
                    if (uploadedFiles.Count > 1) { product.AdditionalImageUrls = string.Join(",", uploadedFiles.Skip(1)); }
                }
                else { product.ImageUrl = "https://via.placeholder.com/300x200/343a40/ffffff?text=Gorsel+Yok"; }

                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MainCategories = _context.MainCategories.OrderBy(m => m.Name).ToList();
            ViewBag.SubCategories = _context.SubCategories.ToList();
            ViewBag.SubSubCategories = _context.SubSubCategories.ToList();
            ViewBag.SubSubSubCategories = _context.SubSubSubCategories.ToList();
            ViewBag.SubSubSubSubCategories = _context.SubSubSubSubCategories.ToList(); 
            return View(product);
        }

        public IActionResult Delete(int id, string? returnUrl = null)
        {
            var product = _context.Products.Find(id);
            if (product != null) 
            { 
                string redirectTarget = "";

                if (!string.IsNullOrEmpty(product.Category))
                {
                    redirectTarget = $"/Product/Category?main={Uri.EscapeDataString(product.Category)}";
                    if (!string.IsNullOrEmpty(product.SubCategory))
                    {
                        redirectTarget += $"&sub={Uri.EscapeDataString(product.SubCategory)}";
                    }
                }

                _context.Products.Remove(product); 
                _context.SaveChanges(); 

                if (!string.IsNullOrEmpty(redirectTarget))
                {
                    return Redirect(redirectTarget);
                }
            }
            
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index");
        }

        // --- DÜZENLEME SAYFASINA GİDERKEN ARAMAYI VEYA DÖNÜLECEK SAYFAYI TAŞIYORUZ ---
        public IActionResult Edit(int id, string? searchString = null, string? returnUrl = null)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound("Düzenlenecek ürün bulunamadı.");

            TempData["LastSearch"] = searchString;
            TempData["ReturnUrl"] = returnUrl;

            ViewBag.MainCategories = _context.MainCategories.OrderBy(m => m.Name).ToList();
            ViewBag.SubCategories = _context.SubCategories.ToList();
            ViewBag.SubSubCategories = _context.SubSubCategories.ToList();
            ViewBag.SubSubSubCategories = _context.SubSubSubCategories.ToList();
            ViewBag.SubSubSubSubCategories = _context.SubSubSubSubCategories.ToList(); 
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                if (product.ImageFiles != null && product.ImageFiles.Count > 0)
                {
                    string uploadFolder = Path.Combine(_env.WebRootPath, "images", "products");
                    if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);
                    List<string> uploadedFiles = new List<string>();

                    foreach (var file in product.ImageFiles)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        string filePath = Path.Combine(uploadFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create)) { file.CopyTo(fileStream); }
                        uploadedFiles.Add("/images/products/" + uniqueFileName);
                    }

                    if (string.IsNullOrEmpty(product.ImageUrl) || product.ImageUrl.Contains("placeholder"))
                    {
                        product.ImageUrl = uploadedFiles.First();
                        if (uploadedFiles.Count > 1) { product.AdditionalImageUrls = string.Join(",", uploadedFiles.Skip(1)); }
                    }
                    else
                    {
                        var currentAdditionals = string.IsNullOrEmpty(product.AdditionalImageUrls) ? new List<string>() : product.AdditionalImageUrls.Split(',').ToList();
                        currentAdditionals.AddRange(uploadedFiles);
                        product.AdditionalImageUrls = string.Join(",", currentAdditionals);
                    }
                }

                _context.Products.Update(product); 
                _context.SaveChanges(); 

                // Kategori sayfasına ve ürün hizasına yönlendirme
                if (!string.IsNullOrEmpty(product.Category))
                {
                    string categoryRedirectUrl = $"/Product/Category?main={Uri.EscapeDataString(product.Category)}";
                    
                    if (!string.IsNullOrEmpty(product.SubCategory))
                    {
                        categoryRedirectUrl += $"&sub={Uri.EscapeDataString(product.SubCategory)}";
                    }
                    if (!string.IsNullOrEmpty(product.SubSubCategory))
                    {
                        categoryRedirectUrl += $"&subsub={Uri.EscapeDataString(product.SubSubCategory)}";
                    }
                    if (!string.IsNullOrEmpty(product.SubSubSubCategory))
                    {
                        categoryRedirectUrl += $"&subsubsub={Uri.EscapeDataString(product.SubSubSubCategory)}";
                    }
                    if (!string.IsNullOrEmpty(product.SubSubSubSubCategory))
                    {
                        categoryRedirectUrl += $"&subsubsubsub={Uri.EscapeDataString(product.SubSubSubSubCategory)}";
                    }

                    categoryRedirectUrl += "#urun-" + product.Id;

                    return Redirect(categoryRedirectUrl);
                }

                string? targetUrl = returnUrl ?? TempData["ReturnUrl"] as string;
                if (!string.IsNullOrEmpty(targetUrl))
                {
                    return Redirect(targetUrl);
                }

                string? lastSearch = TempData["LastSearch"] as string;
                string redirectUrl = "/Admin/Index";

                if (!string.IsNullOrEmpty(lastSearch)) {
                    redirectUrl += "?searchString=" + Uri.EscapeDataString(lastSearch);
                }
                
                redirectUrl += "#urun-" + product.Id;

                return Redirect(redirectUrl); 
            }

            if (TempData.ContainsKey("LastSearch")) { TempData.Keep("LastSearch"); }
            if (TempData.ContainsKey("ReturnUrl")) { TempData.Keep("ReturnUrl"); }

            ViewBag.MainCategories = _context.MainCategories.OrderBy(m => m.Name).ToList();
            ViewBag.SubCategories = _context.SubCategories.ToList();
            ViewBag.SubSubCategories = _context.SubSubCategories.ToList();
            ViewBag.SubSubSubCategories = _context.SubSubSubCategories.ToList();
            ViewBag.SubSubSubSubCategories = _context.SubSubSubSubCategories.ToList(); 
            return View(product);
        }

        // =========================================================
        // TEKİL RESİM SİLME İŞLEMİ
        // =========================================================
        [HttpPost]
        public IActionResult DeleteProductImage(int id, string imageUrl)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            try {
                var filePath = Path.Combine(_env.WebRootPath, imageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            } catch { }

            if (product.ImageUrl == imageUrl) {
                if (!string.IsNullOrEmpty(product.AdditionalImageUrls)) {
                    var additionalList = product.AdditionalImageUrls.Split(',').ToList();
                    product.ImageUrl = additionalList.First(); 
                    additionalList.RemoveAt(0); 
                    product.AdditionalImageUrls = additionalList.Any() ? string.Join(",", additionalList) : null;
                } else {
                    product.ImageUrl = "https://via.placeholder.com/300x200/343a40/ffffff?text=Gorsel+Yok";
                }
            } else if (!string.IsNullOrEmpty(product.AdditionalImageUrls)) {
                var additionalList = product.AdditionalImageUrls.Split(',').ToList();
                additionalList.Remove(imageUrl);
                product.AdditionalImageUrls = additionalList.Any() ? string.Join(",", additionalList) : null;
            }

            _context.SaveChanges();
            return RedirectToAction("Edit", new { id = product.Id });
        }

        // =========================================================
        // SİPARİŞ İŞLEMLERİ (LOGO, MAİL VE SMS ENTEGRASYONLU)
        // =========================================================
        public async Task<IActionResult> Orders() 
        { 
            return View(await _context.Orders.OrderByDescending(o => o.OrderDate).ToListAsync()); 
        }

        public async Task<IActionResult> OrderDetails(int id) 
        { 
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id); 
            if (order == null) return NotFound(); 
            return View(order); 
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status, bool notifyCustomer = false, string returnPage = "OrderDetails") 
        { 
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Sipariş bulunamadı.";
                return RedirectToAction(returnPage == "Orders" ? "Orders" : "OrderDetails", new { id = orderId });
            }

            // 1. Sipariş Durumunu Güncelle ve Kaydet
            order.Status = status;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            // 2. LOGO İŞBAŞI ENTEGRASYONU (Durum 'Hazırlanıyor' yapıldığında tetiklenir)
            if (status == "Hazırlanıyor")
            {
                try
                {
                    // Telefon numarası veya ada göre eşleşen kayıtlı müşteriyi buluyoruz
                    var cleanPhone = order.Phone?.Replace(" ", "").Trim();
                    var customer = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == cleanPhone || u.FullName == order.CustomerName);
                    
                    // Eğer kullanıcı sistemde kayıtlı değilse fatura için sipariş bilgilerinden geçici profil oluşturuyoruz
                    if (customer == null)
                    {
                        customer = new User
                        {
                            FullName = order.CustomerName,
                            PhoneNumber = order.Phone,
                            CustomerType = "Bireysel"
                        };
                    }

                    bool logoResult = await _logoIsbasiService.CreateInvoiceAsync(order, customer);
                    if (logoResult)
                    {
                        Console.WriteLine($"[LOGO BAŞARILI] #{order.Id} numaralı sipariş Logo İşbaşı sistemine aktarıldı.");
                    }
                    else
                    {
                        Console.WriteLine($"[LOGO UYARI] #{order.Id} Logo İşbaşı aktarımı başarısız oldu.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[LOGO HATA] " + ex.Message);
                }
            }

            // 3. siparis@sunmedikal.com ADRESİNE HER DURUM DEĞİŞİKLİĞİNDE BİLGİ MAİLİ
            try
            {
                string adminMailKonu = $"[Sipariş Durumu Güncellendi] #{order.Id} - {status}";
                string adminMailIcerik = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px; background-color: #f8f9fa;'>
                        <div style='background-color: #ffffff; padding: 20px; border-radius: 8px; border: 1px solid #dee2e6;'>
                            <h2 style='color: #dc3545; margin-top: 0;'>SUNMED Medikal - Sipariş Bilgilendirmesi</h2>
                            <p><strong>Sipariş No:</strong> #{order.Id}</p>
                            <p><strong>Müşteri:</strong> {order.CustomerName}</p>
                            <p><strong>Telefon:</strong> {order.Phone}</p>
                            <p><strong>Tutar:</strong> {order.TotalAmount:N2} TL</p>
                            <p><strong>Yeni Durum:</strong> <span style='background-color: #ffc107; padding: 3px 8px; border-radius: 4px; font-weight: bold;'>{status}</span></p>
                            <p><strong>İşlem Tarihi:</strong> {DateTime.Now:dd.MM.yyyy HH:mm}</p>
                            <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
                            <p style='color: #6c757d; font-size: 12px;'>Bu e-posta SUNMED Medikal yönetim paneli tarafından otomatik olarak gönderilmiştir.</p>
                        </div>
                    </div>";

                await _emailService.SendEmailAsync("siparis@sunmedikal.com", adminMailKonu, adminMailIcerik);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[YÖNETİCİ MAİL HATA] " + ex.Message);
            }

            // 4. MÜŞTERİYE SMS BİLDİRİMİ GÖNDERME
            try
            {
                if (!string.IsNullOrEmpty(order.Phone))
                {
                    string smsMesaji = $"Sayin {order.CustomerName}, #{order.Id} numarali siparisinizin durumu '{status}' olarak guncellenmistir. SUNMED Medikal";
                    await _smsService.SendSmsAsync(order.Phone, smsMesaji);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[SMS HATA] " + ex.Message);
            }

            TempData["SuccessMessage"] = $"#{order.Id} numaralı sipariş durumu başarıyla '{status}' olarak güncellendi.";

            if (returnPage == "Orders")
            {
                return RedirectToAction("Orders");
            }

            return RedirectToAction("OrderDetails", new { id = orderId }); 
        }

        // =========================================================
        // KATEGORİ YÖNETİMLERİ
        // =========================================================
        public IActionResult MainCategories() { return View(_context.MainCategories.OrderBy(m => m.Name).ToList()); }
        [HttpPost] public IActionResult AddMainCategory(MainCategory mainCategory) { if (ModelState.IsValid) { _context.MainCategories.Add(mainCategory); _context.SaveChanges(); } return RedirectToAction("MainCategories"); }
        public IActionResult DeleteMainCategory(int id) { var main = _context.MainCategories.Find(id); if(main != null) { _context.MainCategories.Remove(main); _context.SaveChanges(); } return RedirectToAction("MainCategories"); }
        
        public IActionResult SubCategories() { ViewBag.MainCategories = _context.MainCategories.OrderBy(m => m.Name).ToList(); return View(_context.SubCategories.OrderBy(s => s.MainCategory).ThenBy(s => s.Name).ToList()); }
        [HttpPost] public IActionResult AddSubCategory(SubCategory subCategory) { if (ModelState.IsValid) { _context.SubCategories.Add(subCategory); _context.SaveChanges(); } return RedirectToAction("SubCategories"); }
        public IActionResult DeleteSubCategory(int id) { var sub = _context.SubCategories.Find(id); if(sub != null) { _context.SubCategories.Remove(sub); _context.SaveChanges(); } return RedirectToAction("SubCategories"); }

        public IActionResult SubSubCategories() { ViewBag.MainCategories = _context.MainCategories.OrderBy(m => m.Name).ToList(); ViewBag.SubCategories = _context.SubCategories.OrderBy(s => s.Name).ToList(); return View(_context.SubSubCategories.OrderBy(s => s.MainCategory).ThenBy(s => s.SubCategory).ThenBy(s => s.Name).ToList()); }
        [HttpPost] public IActionResult AddSubSubCategory(SubSubCategory subSubCategory) { if (ModelState.IsValid) { _context.SubSubCategories.Add(subSubCategory); _context.SaveChanges(); } return RedirectToAction("SubSubCategories"); }
        public IActionResult DeleteSubSubCategory(int id) { var subSub = _context.SubSubCategories.Find(id); if(subSub != null) { _context.SubSubCategories.Remove(subSub); _context.SaveChanges(); } return RedirectToAction("SubSubCategories"); }

        // 4. SEVİYE ALT-ALT-ALT KATEGORİ YÖNETİMİ
        public IActionResult SubSubSubCategories()
        {
            ViewBag.MainCategories = _context.MainCategories.OrderBy(m => m.Name).ToList();
            ViewBag.SubCategories = _context.SubCategories.OrderBy(s => s.Name).ToList();
            ViewBag.SubSubCategories = _context.SubSubCategories.OrderBy(s => s.Name).ToList();
            return View(_context.SubSubSubCategories.OrderBy(s => s.MainCategory).ThenBy(s => s.Name).ToList());
        }

        [HttpPost]
        public IActionResult AddSubSubSubCategory(SubSubSubCategory subSubSubCategory)
        {
            if (ModelState.IsValid) { _context.SubSubSubCategories.Add(subSubSubCategory); _context.SaveChanges(); }
            return RedirectToAction("SubSubSubCategories");
        }

        public IActionResult DeleteSubSubSubCategory(int id)
        {
            var item = _context.SubSubSubCategories.Find(id);
            if(item != null) { _context.SubSubSubCategories.Remove(item); _context.SaveChanges(); }
            return RedirectToAction("SubSubSubCategories");
        }

        [HttpGet]
        public IActionResult EditSubSubSubCategory(int id)
        {
            var category = _context.SubSubSubCategories.Find(id);
            if (category == null) return NotFound();

            ViewBag.MainCategories = _context.MainCategories.OrderBy(m => m.Name).ToList();
            ViewBag.SubCategories = _context.SubCategories.OrderBy(s => s.Name).ToList();
            ViewBag.SubSubCategories = _context.SubSubCategories.OrderBy(s => s.Name).ToList();

            return View(category);
        }

        [HttpPost]
        public IActionResult EditSubSubSubCategory(SubSubSubCategory model)
        {
            if (ModelState.IsValid)
            {
                _context.SubSubSubCategories.Update(model);
                _context.SaveChanges();
                return RedirectToAction("SubSubSubCategories");
            }

            ViewBag.MainCategories = _context.MainCategories.OrderBy(m => m.Name).ToList();
            ViewBag.SubCategories = _context.SubCategories.OrderBy(s => s.Name).ToList();
            ViewBag.SubSubCategories = _context.SubSubCategories.OrderBy(s => s.Name).ToList();
            return View(model);
        }

        // =========================================================
        // 5. SEVİYE KATEGORİ YÖNETİMİ (SubSubSubSubCategory)
        // =========================================================
        public IActionResult SubSubSubSubCategories()
        {
            ViewBag.MainCategories = _context.MainCategories.OrderBy(m => m.Name).ToList();
            ViewBag.SubCategories = _context.SubCategories.OrderBy(s => s.Name).ToList();
            ViewBag.SubSubCategories = _context.SubSubCategories.OrderBy(s => s.Name).ToList();
            ViewBag.SubSubSubCategories = _context.SubSubSubCategories.OrderBy(s => s.Name).ToList();
            return View(_context.SubSubSubSubCategories.OrderBy(s => s.MainCategory).ThenBy(s => s.Name).ToList());
        }

        [HttpPost]
        public IActionResult AddSubSubSubSubCategory(SubSubSubSubCategory subSubSubSubCategory)
        {
            if (ModelState.IsValid) 
            { 
                _context.SubSubSubSubCategories.Add(subSubSubSubCategory); 
                _context.SaveChanges(); 
            }
            return RedirectToAction("SubSubSubSubCategories");
        }

        public IActionResult DeleteSubSubSubSubCategory(int id)
        {
            var item = _context.SubSubSubCategories.Find(id);
            if(item != null) { _context.SubSubSubCategories.Remove(item); _context.SaveChanges(); }
            return RedirectToAction("SubSubSubSubCategories");
        }
        
        [HttpGet]
        public IActionResult EditSubSubSubSubCategory(int id)
        {
            var category = _context.SubSubSubSubCategories.Find(id);
            if (category == null) return NotFound();

            ViewBag.MainCategories = _context.MainCategories.OrderBy(m => m.Name).ToList();
            ViewBag.SubCategories = _context.SubCategories.OrderBy(s => s.Name).ToList();
            ViewBag.SubSubCategories = _context.SubSubCategories.OrderBy(s => s.Name).ToList();
            ViewBag.SubSubSubCategories = _context.SubSubSubCategories.OrderBy(s => s.Name).ToList();

            return View(category);
        }

        [HttpPost]
        public IActionResult EditSubSubSubSubCategory(SubSubSubSubCategory model)
        {
            if (ModelState.IsValid)
            {
                _context.SubSubSubSubCategories.Update(model);
                _context.SaveChanges();
                return RedirectToAction("SubSubSubSubCategories");
            }

            ViewBag.MainCategories = _context.MainCategories.OrderBy(m => m.Name).ToList();
            ViewBag.SubCategories = _context.SubCategories.OrderBy(s => s.Name).ToList();
            ViewBag.SubSubCategories = _context.SubSubCategories.OrderBy(s => s.Name).ToList();
            ViewBag.SubSubSubCategories = _context.SubSubSubCategories.OrderBy(s => s.Name).ToList();
            return View(model);
        }

        // =========================================================
        // KATEGORİ DÜZENLEME (EDİT) İŞLEMLERİ (1, 2 VE 3. SEVİYE)
        // =========================================================

        // --- 1. ANA KATEGORİ DÜZENLEME ---
        [HttpGet]
        public IActionResult EditMainCategory(int id)
        {
            var category = _context.MainCategories.Find(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public IActionResult EditMainCategory(MainCategory model)
        {
            if (ModelState.IsValid) { _context.MainCategories.Update(model); _context.SaveChanges(); return RedirectToAction("MainCategories"); }
            return View(model);
        }

        // --- 2. ALT KATEGORİ DÜZENLEME ---
        [HttpGet]
        public IActionResult EditSubCategory(int id)
        {
            var category = _context.SubCategories.Find(id);
            if (category == null) return NotFound();
            ViewBag.MainCategories = _context.MainCategories.OrderBy(m => m.Name).ToList();
            return View(category);
        }

        [HttpPost]
        public IActionResult EditSubCategory(SubCategory model)
        {
            if (ModelState.IsValid) { _context.SubCategories.Update(model); _context.SaveChanges(); return RedirectToAction("SubCategories"); }
            ViewBag.MainCategories = _context.MainCategories.OrderBy(m => m.Name).ToList();
            return View(model);
        }

        // --- 3. ALT-ALT KATEGORİ DÜZENLEME ---
        [HttpGet]
        public IActionResult EditSubSubCategory(int id)
        {
            var category = _context.SubSubCategories.Find(id);
            if (category == null) return NotFound();
            ViewBag.MainCategories = _context.MainCategories.OrderBy(m => m.Name).ToList();
            ViewBag.SubCategories = _context.SubCategories.OrderBy(s => s.Name).ToList();
            return View(category);
        }

        [HttpPost]
        public IActionResult EditSubSubCategory(SubSubCategory model)
        {
            if (ModelState.IsValid) { _context.SubSubCategories.Update(model); _context.SaveChanges(); return RedirectToAction("SubSubCategories"); }
            ViewBag.MainCategories = _context.MainCategories.OrderBy(m => m.Name).ToList();
            ViewBag.SubCategories = _context.SubCategories.OrderBy(s => s.Name).ToList();
            return View(model);
        }
    }
}