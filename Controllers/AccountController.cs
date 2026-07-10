using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using System.Collections.Generic;
using SunmedMedikal.Data;
using System.Linq;
using SunmedMedikal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Http;

namespace SunmedMedikal.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SunmedMedikal.Services.EmailService _emailService;
        private readonly SunmedMedikal.Services.SmsService _smsService;

        public AccountController(ApplicationDbContext context, SunmedMedikal.Services.EmailService emailService, SunmedMedikal.Services.SmsService smsService)
        {
            _context = context;
            _emailService = emailService;
            _smsService = smsService;
        }

        public IActionResult Login()
        {
            // Kapılara özel hafızaları HTML'e gönder
            ViewBag.SavedBireysel = Request.Cookies["SavedUsername_Bireysel"];
            ViewBag.SavedPassBireysel = Request.Cookies["SavedPassword_Bireysel"];

            ViewBag.SavedKurumsal = Request.Cookies["SavedUsername_Kurumsal"];
            ViewBag.SavedPassKurumsal = Request.Cookies["SavedPassword_Kurumsal"];

            ViewBag.SavedAdmin = Request.Cookies["SavedUsername_Admin"];
            ViewBag.SavedPassAdmin = Request.Cookies["SavedPassword_Admin"];

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string loginType, bool rememberMe = false)
        {
            // Veri boş gelirse uyar
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Kullanıcı adı ve şifre boş olamaz!";
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => (u.Username == username || u.Email == username) && u.Password == password);

            if (user != null)
            {
                // KAPI KONTROLÜ (Güvenlik Duvarı)
                bool isBireysel = user.CustomerType != null && user.CustomerType.Equals("Bireysel", StringComparison.OrdinalIgnoreCase);
                bool isKurumsal = user.CustomerType != null && user.CustomerType.Equals("Kurumsal", StringComparison.OrdinalIgnoreCase);
                bool isAdmin = user.Role != null && user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);

                if (loginType == "Admin" && !isAdmin)
                {
                    ViewBag.ErrorMessage = "Bu alandan sadece yöneticiler giriş yapabilir!";
                    return View();
                }
                
                if (loginType == "Bireysel" && !isBireysel)
                {
                    ViewBag.ErrorMessage = "Lütfen Kurumsal Müşteri panelinden giriş yapınız.";
                    return View();
                }

                if (loginType == "Kurumsal" && !isKurumsal)
                {
                    ViewBag.ErrorMessage = "Lütfen Bireysel Müşteri panelinden giriş yapınız.";
                    return View();
                }

                // HAFIZAYA KAYDET (Sadece giriş yaptığı kapı için)
                string cookieNameUser = "SavedUsername_" + loginType;
                string cookieNamePass = "SavedPassword_" + loginType; 

                if (rememberMe)
                {
                    CookieOptions options = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) };
                    Response.Cookies.Append(cookieNameUser, username, options);
                    Response.Cookies.Append(cookieNamePass, password, options); 
                }
                else
                {
                    Response.Cookies.Delete(cookieNameUser);
                    Response.Cookies.Delete(cookieNamePass);
                }

                // SİSTEME GİRİŞ YAP
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("FullName", user.FullName)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = false }; // Tarayıcı kapanınca çıkış yapar

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, 
                    new ClaimsPrincipal(claimsIdentity), 
                    authProperties);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (model.AccountType == "Kurumsal")
            {
                if (string.IsNullOrWhiteSpace(model.CompanyName))
                    ModelState.AddModelError("CompanyName", "Firma Adı zorunludur.");
                
                if (string.IsNullOrWhiteSpace(model.TaxOffice))
                    ModelState.AddModelError("TaxOffice", "Vergi Dairesi zorunludur.");
                    
                if (string.IsNullOrWhiteSpace(model.TaxNumber))
                    ModelState.AddModelError("TaxNumber", "Vergi Numarası zorunludur.");
            }

            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                ViewBag.ErrorMessage = "Kayıt Başarısız! Lütfen düzeltin: " + string.Join(" | ", errorList);
                return View(model);
            }

            bool userExists = _context.Users.Any(u => u.Username == model.Username || u.Email == model.Email);
            if (userExists)
            {
                ViewBag.ErrorMessage = "Bu E-Posta veya Kullanıcı Adı zaten sistemde kayıtlı!";
                return View(model);
            }

            try 
            {
                var newUser = new User
                {
                    CustomerType = model.AccountType,
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Username = model.Username,
                    Password = model.Password, 
                    Role = "Customer",
                    TcNo = model.AccountType == "Bireysel" ? model.TCKimlikNo : null,
                    CompanyTitle = model.AccountType == "Kurumsal" ? model.CompanyTitle : null,
                    CompanyName = model.AccountType == "Kurumsal" ? model.CompanyName : null,
                    TaxOffice = model.AccountType == "Kurumsal" ? model.TaxOffice : null,
                    TaxNumber = model.AccountType == "Kurumsal" ? model.TaxNumber : null
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Sistemsel bir SQL hatası oluştu: " + ex.Message;
                return View(model);
            }
        }
        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult MyOrders()
        {
            var currentUserName = User.Identity.Name; 
    
            var orders = _context.Orders
                .Where(o => o.CustomerName == currentUserName)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        [Authorize]
        public async Task<IActionResult> OrderDetails(int id) 
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.CustomerName == User.Identity.Name);
    
            if (order == null) return NotFound();

            return View(order);
        }

        [Authorize]
        public async Task<IActionResult> Invoice(int id)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.CustomerName == User.Identity.Name);
        
            if (order == null) return NotFound();
    
            return View(order);
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string emailOrUsername)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == emailOrUsername || u.Username == emailOrUsername);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Sistemde bu bilgilere ait bir kayıt bulunamadı.";
                return View();
            }

            var random = new Random();
            string tempPassword = "Sunmed" + random.Next(1000, 9999).ToString(); 

            user.Password = tempPassword;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            string subject = "Geçici Şifreniz - SUNMED Medikal";
            string mailBody = $@"
                <div style='font-family: Arial; padding: 20px; border: 1px solid #eee;'>
                    <h2 style='color: #dc3545;'>Şifreniz Sıfırlandı</h2>
                    <p>Sayın {user.FullName},</p>
                    <p>Sistemimize giriş yapabilmeniz için geçici şifreniz oluşturulmuştur.</p>
                    <p><b>Kullanıcı Adınız:</b> {user.Username}</p>
                    <p><b>Yeni Şifreniz:</b> <span style='color: #dc3545; font-size: 18px; font-weight: bold;'>{tempPassword}</span></p>
                    <hr>
                    <p>Güvenliğiniz için sisteme giriş yaptıktan sonra şifrenizi değiştirmenizi öneririz.</p>
                </div>";

            try 
            {
                await _emailService.SendEmailAsync(user.Email, subject, mailBody);
                ViewBag.SuccessMessage = "Yeni şifreniz e-posta adresinize (ve SMS olarak) gönderilmiştir.";
            } 
            catch 
            {
                ViewBag.ErrorMessage = "Şifreniz sıfırlandı ancak e-posta gönderilirken bir hata oluştu. Lütfen müşteri hizmetleri ile iletişime geçin.";
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> MyProfile()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FullName == User.Identity.Name || u.Username == User.Identity.Name);
            
            if (user == null) return RedirectToAction("Login");

            // Veritabanındaki tüm bilgileri formumuza doldurup HTML'e gönderiyoruz
            var model = new ProfileViewModel
            {
                CustomerType = user.CustomerType,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                TcNo = user.TcNo,
                CompanyTitle = user.CompanyTitle,
                CompanyName = user.CompanyName,
                TaxOffice = user.TaxOffice,
                TaxNumber = user.TaxNumber,
                City = user.City,
                District = user.District,
                Address = user.Address
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MyProfile(ProfileViewModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FullName == User.Identity.Name || u.Username == User.Identity.Name);
            if (user == null) return RedirectToAction("Login");

            // 1. Temel Bilgileri Güncelle
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            // 2. Müşteri Tipine Göre Fatura Bilgilerini Güncelle
            if (user.CustomerType == "Kurumsal")
            {
                user.CompanyName = model.CompanyName;
                user.CompanyTitle = model.CompanyTitle;
                user.TaxOffice = model.TaxOffice;
                user.TaxNumber = model.TaxNumber;
                // YENİ EKLENEN ADRES KAYIT İŞLEMİ
                user.City = model.City;
                user.District = model.District;
                user.Address = model.Address;
            }
            else
            {
                user.TcNo = model.TcNo;
            }

            // 3. Şifre Değiştirme Talebi Varsa
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(model.CurrentPassword) || user.Password != model.CurrentPassword)
                {
                    ViewBag.ErrorMessage = "Şifrenizi değiştirmek için mevcut şifrenizi doğru girmelisiniz!";
                    return View(model);
                }
                
                user.Password = model.NewPassword;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            ViewBag.SuccessMessage = "Bilgileriniz başarıyla güncellenmiştir!";
            
            // Başarılı kayıttan sonra yeni bilgileri (Username vs) tekrar forma basıyoruz ki ekran boşalmasın
            model.Username = user.Username; 
            return View(model);
        }
    
    // YENİ EKLENEN: SİPARİŞ İPTAL ETME KODU
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CancelOrder(int id)
        {
            // Sadece kendi siparişini bulsun
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id && o.CustomerName == User.Identity.Name);
            
            // Eğer kargolanmadıysa veya tamamlanmadıysa iptal edilebilir
            if (order != null && (order.Status == "Onay Bekliyor" || order.Status == "Ödeme Bekleniyor (Havale)"))
            {
                order.Status = "İptal Edildi";
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = $"#{order.Id} numaralı siparişiniz başarıyla iptal edilmiştir.";
            }
            else
            {
                TempData["ErrorMessage"] = "Bu sipariş şu anki aşaması sebebiyle iptal edilemez.";
            }
            
            return RedirectToAction("MyOrders");
        }
    }
}