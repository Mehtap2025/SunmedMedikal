using Microsoft.AspNetCore.Mvc;
using SunmedMedikal.Data;
using SunmedMedikal.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using System.IO;
using SunmedMedikal.Services;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;

namespace SunmedMedikal.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        private readonly SmsService _smsService;    

        public CartController(ApplicationDbContext context, EmailService emailService, SmsService smsService)
        {
            _context = context;
            _emailService = emailService;
            _smsService = smsService;
        }


        // --- YENİ KALICI SEPET MOTORU ---
        // Kullanıcı giriş yaptıysa kendi ismini, yapmadıysa bilgisayarında kalıcı bir çerez ID'si alır
        private string GetCartUserId()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return User.Identity.Name;

            var guestId = Request.Cookies["GuestCartId"];
            if (string.IsNullOrEmpty(guestId))
            {
                guestId = Guid.NewGuid().ToString();
                Response.Cookies.Append("GuestCartId", guestId, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) });
            }
            return guestId;
        }


        // Sepeti artık geçici Session'dan değil, SQL Veritabanından çekiyoruz (ASLA SİLİNMEZ)
        private List<CartItem> GetCart()
        {
            string userName = GetCartUserId();
            return _context.CartItems.Where(c => c.Username == userName).ToList();
        }


        // Sepeti güncellerken SQL Veritabanına kalıcı olarak yazıyoruz
        private void SaveCart(List<CartItem> cart)
        {
            string userName = GetCartUserId();
            
            var existingItems = _context.CartItems.Where(c => c.Username == userName).ToList();
            if (existingItems.Any())
            {
                _context.CartItems.RemoveRange(existingItems);
                _context.SaveChanges();
            }

            if (cart != null && cart.Any())
            {
                foreach (var item in cart)
                {
                    item.Id = 0;
                    item.Username = userName;
                }
                _context.CartItems.AddRange(cart);
                _context.SaveChanges();
            }
        }


        public IActionResult Index()
        {
            return View(GetCart());
        }


        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = _context.Products.Find(productId);
            if (product == null) return NotFound();

            var cart = GetCart();

            // =========================================================================
            // SİHİRLİ DOKUNUŞ: FİYAT 0 İSE VERİTABANINA BAKMADAN ZORLA TEKLİFE AT
            // =========================================================================
            bool isQuote = product.IsQuoteOnly || product.Price == 0 || product.Price == 0.00m;

            int currentNormalQty = cart.Where(c => c.ProductId == productId && !c.IsQuoteOnly).Sum(c => c.Quantity);
            int totalRequested = currentNormalQty + quantity;

            if (isQuote || totalRequested <= product.StockQuantity)
            {
                var existingItem = cart.FirstOrDefault(c => c.ProductId == productId && c.IsQuoteOnly == isQuote);
                if (existingItem != null) existingItem.Quantity += quantity;
                else cart.Add(new CartItem {
                    ProductId = product.Id, Name = product.Name, Price = product.Price, Currency = product.Currency,
                    ImageUrl = product.ImageUrl, IsQuoteOnly = isQuote, Quantity = quantity, MaxStock = isQuote ? 99999 : product.StockQuantity
                });
            }
            else
            {
                int excessQty = totalRequested - product.StockQuantity;
                
                if (product.StockQuantity > 0)
                {
                    var normalItem = cart.FirstOrDefault(c => c.ProductId == productId && !c.IsQuoteOnly);
                    if (normalItem != null) normalItem.Quantity = product.StockQuantity;
                    else cart.Add(new CartItem {
                        ProductId = product.Id, Name = product.Name, Price = product.Price, Currency = product.Currency,
                        ImageUrl = product.ImageUrl, IsQuoteOnly = false, Quantity = product.StockQuantity, MaxStock = product.StockQuantity
                    });
                }
                
                var quoteItem = cart.FirstOrDefault(c => c.ProductId == productId && c.IsQuoteOnly);
                if (quoteItem != null) quoteItem.Quantity += quantity;
                else cart.Add(new CartItem {
                    ProductId = product.Id, Name = product.Name + " (Stok Aşımı - Teklif)", Price = product.Price, Currency = product.Currency,
                    ImageUrl = product.ImageUrl, IsQuoteOnly = true, Quantity = excessQty, MaxStock = 99999
                });

                TempData["CartWarning"] = $"İstediğiniz {totalRequested} adetin {product.StockQuantity} kadarı sepete, kalan {excessQty} adeti ise özel fiyat/temin için 'Teklif Listesine' eklendi.";
            }

            SaveCart(cart);
            return RedirectToAction("Index", "Cart");
        }


        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int newQuantity, bool isQuote)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId && c.IsQuoteOnly == isQuote);

            if (item != null)
            {
                if (!isQuote && newQuantity > item.MaxStock) {
                    return Json(new { success = false, message = $"En fazla {item.MaxStock} adet (Stokta Kalan) alabilirsiniz. Fazlası için ürün sayfasından tekrar ekleyip teklif isteyiniz." });
                }
                item.Quantity = newQuantity < 1 ? 1 : newQuantity;
                SaveCart(cart);
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Ürün bulunamadı!" });
        }


        public IActionResult RemoveFromCart(int id, bool isQuote)
        {
            var cart = GetCart();
            var itemToRemove = cart.FirstOrDefault(c => c.ProductId == id && c.IsQuoteOnly == isQuote);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }


        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var cart = GetCart();
            if (cart == null || !cart.Any(x => !x.IsQuoteOnly)) return RedirectToAction("Index");

            // CANLI KUR ÇEKME (API)
            decimal usdKur = 33.00m; 
            decimal eurKur = 36.00m;
            try {
                using var client = new HttpClient();
                var response = await client.GetFromJsonAsync<JsonElement>("https://api.exchangerate-api.com/v4/latest/TRY");
                usdKur = 1 / response.GetProperty("rates").GetProperty("USD").GetDecimal();
                eurKur = 1 / response.GetProperty("rates").GetProperty("EUR").GetDecimal();
            } catch { /* API Çökse bile yedek kurlarla devam eder */ }

            decimal araToplam = cart.Where(x => !x.IsQuoteOnly).Sum(item => {
                decimal p = item.Price;
                if (item.Currency == "USD") p *= usdKur;
                else if (item.Currency == "EUR") p *= eurKur;
                return p * item.Quantity;
            });

            ViewBag.UsdKur = usdKur;
            ViewBag.EurKur = eurKur;
            ViewBag.AraToplam = araToplam;
            
            // KARGO GÜNCELLEMESİ (2000 TL ÜZERİ BEDAVA, DEĞİLSE 200 TL)
            ViewBag.KargoUcreti = (araToplam >= 2000 || araToplam == 0) ? 0 : 200.00m; 
            ViewBag.GenelToplam = araToplam + (decimal)ViewBag.KargoUcreti;

            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == User.Identity.Name || u.FullName == User.Identity.Name);
            ViewBag.CurrentUser = currentUser;

            return View();
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PlaceOrder(string FullName, string Phone, string Address, string City, string District, string CityManual, string DistrictManual, string OrderNotes, string PaymentMethod, string TcNo, string TaxOffice, string TaxNumber, IFormFile ReceiptFile)
        {
            try
            {
                var cart = GetCart();
                if (cart == null || !cart.Any(x => !x.IsQuoteOnly)) return RedirectToAction("Index");

                // HAVALE SEÇİLDİYSE DEKONT YÜKLEME ZORUNLULUĞU KONTROLÜ
                if (PaymentMethod == "Havale/EFT" && (ReceiptFile == null || ReceiptFile.Length == 0))
                {
                    TempData["CartWarning"] = "Havale/EFT ile ödeme yapabilmek için lütfen dekont yükleyiniz!";
                    return RedirectToAction("Checkout");
                }

                if (City == "Diğer" && !string.IsNullOrWhiteSpace(CityManual)) City = CityManual;
                if (District == "Diğer" && !string.IsNullOrWhiteSpace(DistrictManual)) District = DistrictManual;

                string urunListesiText = string.Join("\n", cart.Where(x => !x.IsQuoteOnly).Select(x => $"• {x.Quantity} Adet - {x.Name} ({x.Price:N2} {x.Currency})"));
                string startingStatus = PaymentMethod == "Havale/EFT" ? "Ödeme Bekleniyor (Dekont İncelemede)" : "Onay Bekliyor";
                string safeOrderNotes = OrderNotes ?? "Müşteri not eklemedi.";
                string receiptFilePath = "";

                // DEKONTU SUNUCUYA KAYDETME İŞLEMİ
                if (PaymentMethod == "Havale/EFT" && ReceiptFile != null && ReceiptFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "dekontlar");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ReceiptFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ReceiptFile.CopyToAsync(fileStream);
                    }
                    receiptFilePath = "/uploads/dekontlar/" + uniqueFileName;
                }

                // Vergi Dairesi ve Vergi No Formdan Alınarak Kaydedilecek
                string taxInfo = "Belirtilmedi";
                if (!string.IsNullOrWhiteSpace(TaxNumber)) {
                    taxInfo = $"Vergi Dairesi: {TaxOffice ?? "-"} / Vergi No: {TaxNumber}";
                } else if (!string.IsNullOrWhiteSpace(TcNo)) {
                    taxInfo = $"TC Kimlik No: {TcNo}";
                }

                var newOrder = new Order
                {
                    CustomerName = User.Identity.Name ?? "Bilinmiyor", 
                    Phone = Phone ?? "", 
                    Address = Address ?? "", 
                    City = City ?? "", 
                    District = District ?? "",
                    OrderNotes = $"Müşteri Adı: {FullName}\nFatura Bilgisi: {taxInfo}\nMüşteri Notu: {safeOrderNotes}\nÖdeme Yöntemi: {PaymentMethod}\n\nSİPARİŞ İÇERİĞİ:\n{urunListesiText}",
                    Status = startingStatus,
                    OrderDate = DateTime.Now
                };

                // --- SİHİR BURADA: CANLI KUR ÇEKME EKLENDİ ---
                decimal usdKur = 33.00m; 
                decimal eurKur = 36.00m;
                try {
                    using var client = new System.Net.Http.HttpClient();
                    var response = await System.Net.Http.Json.HttpClientJsonExtensions.GetFromJsonAsync<JsonElement>(client, "https://api.exchangerate-api.com/v4/latest/TRY");
                    usdKur = 1 / response.GetProperty("rates").GetProperty("USD").GetDecimal();
                    eurKur = 1 / response.GetProperty("rates").GetProperty("EUR").GetDecimal();
                } catch { }

                newOrder.TotalAmount = cart.Where(x => !x.IsQuoteOnly).Sum(item => {
                    decimal p = item.Price;
                    if (item.Currency == "USD") p *= usdKur; else if (item.Currency == "EUR") p *= eurKur;
                    return p * item.Quantity;
                });

                decimal kargoUcreti = (newOrder.TotalAmount >= 2000 || newOrder.TotalAmount == 0) ? 0 : 200.00m;
                newOrder.TotalAmount += kargoUcreti;

                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();

                cart.RemoveAll(x => !x.IsQuoteOnly);
                SaveCart(cart);

                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == User.Identity.Name || u.FullName == User.Identity.Name);
                string customerEmail = currentUser != null ? currentUser.Email : "satis@sunmedikal.com";

                string subject = "Siparişiniz Alındı - SUNMED Medikal";
                
                string dekontLinkHtml = "";
                if (!string.IsNullOrEmpty(receiptFilePath))
                {
                    string fileNameOnly = Path.GetFileName(receiptFilePath);
                    dekontLinkHtml = $"<p><b>Müşteri Dekontu:</b> <a href='https://sunmedikal.com/Cart/ViewReceipt?fileName={fileNameOnly}'>Görüntülemek İçin Tıklayın</a></p>";
                }

                string mailBody = $@"
                    <div style='font-family: Arial; padding: 20px; border: 1px solid #eee;'>
                        <h2 style='color: #dc3545;'>Teşekkürler!</h2>
                        <p>Siparişiniz başarıyla sistemimize ulaşmıştır. <b>#{newOrder.Id}</b> numaralı siparişinizin ödeme ve onay sürecini sitemizden takip edebilirsiniz.</p>
                        <p><b>Sipariş Özeti:</b><br>{newOrder.OrderNotes.Replace("\n", "<br>")}</p>
                        <p><b>Toplam Tutar:</b> {newOrder.TotalAmount.ToString("N2")} TL</p>
                        {dekontLinkHtml}
                        <hr>
                        <p>SUNMED Medikal - İyi çalışmalar dileriz.</p>
                    </div>";
                string smsBody = $"Sayin {FullName}, #{newOrder.Id} nolu siparisiniz basariyla alinmistir. Bizi tercih ettiginiz icin tesekkur ederiz. SUNMED MEDIKAL";

                try {
                    await _emailService.SendEmailAsync(customerEmail, subject, mailBody);
                    await _smsService.SendSmsAsync(Phone, smsBody);
                }
                catch (Exception ex) { Console.WriteLine("CART MAIL/SMS HATA: " + ex.Message); }

                TempData["OrderId"] = newOrder.Id.ToString();
                TempData["OrderTotal"] = newOrder.TotalAmount.ToString("N2");
                TempData["OrderPayment"] = PaymentMethod ?? "Havale/EFT";
                
                return RedirectToAction("Success");
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                if (ex.InnerException != null) errorMsg += " | Detay: " + ex.InnerException.Message;
                
                TempData["CartWarning"] = "Siparişiniz kaydedilirken bir hata oluştu: " + errorMsg;
                return RedirectToAction("Checkout");
            }
        }


        public IActionResult Success() 
        { 
            if(TempData["OrderId"] == null) return RedirectToAction("Index", "Home");
            return View(); 
        }


        [Authorize] 
        public IActionResult ViewReceipt(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return BadRequest();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "dekontlar", fileName);
            
            if (!System.IO.File.Exists(path)) return NotFound("Dekont dosyası sunucuda bulunamadı!");

            string contentType = "application/pdf"; 
            string ext = Path.GetExtension(fileName).ToLower();
            
            if (ext == ".jpg" || ext == ".jpeg") contentType = "image/jpeg";
            else if (ext == ".png") contentType = "image/png";

            var fileBytes = System.IO.File.ReadAllBytes(path);

            return File(fileBytes, contentType);
        }
    }
}