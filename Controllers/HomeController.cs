using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SunmedMedikal.Models;
using SunmedMedikal.Data;
using System.Linq;
using Microsoft.Extensions.Logging;
using SunmedMedikal.Services; 
using System.Threading.Tasks; 
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization; 

namespace SunmedMedikal.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context; 
    private readonly EmailService _emailService; 

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, EmailService emailService)
    {
        _logger = logger;
        _context = context;
        _emailService = emailService;
    }

    // =======================================================
    // ÇOKLU DİL DEĞİŞTİRME MOTORU
    // =======================================================
    [HttpPost]
    public IActionResult ChangeLanguage(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        return LocalRedirect(returnUrl);
    }
    // =======================================================

    public IActionResult Index()
    {
        var sonUrunler = _context.Products.OrderByDescending(p => p.Id).Take(4).ToList();
        var markalar = _context.Brands.ToList();

        ViewBag.SonUrunler = sonUrunler;
        ViewBag.Markalar = markalar;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // Zaten var olan Hakkımızda (Biz Kimiz) Sayfası
    public IActionResult About()
    {
        return View();
    }

    // İletişim Sayfası
    public IActionResult Contact()
    {
        return View();
    }

    // =======================================================
    // YENİ EKLENEN KURUMSAL SAYFALARIN KOMUTLARI
    // =======================================================
    
    public IActionResult VizyonMisyon()
    {
        return View();
    }

    public IActionResult KalitePolitikasi()
    {
        return View();
    }

    public IActionResult Sss()
    {
        return View();
    }

    public IActionResult KargoTakip()
    {
        return View();
    }

    public IActionResult NasilSiparisVerilir()
    {
        return View();
    }

    public IActionResult Kvkk()
    {
        return View();
    }

    public IActionResult CerezTercihleri()
    {
        return View();
    }

    public IActionResult KullanimKosullari()
    {
        return View();
    }
    // =======================================================

    [HttpPost]
    public async Task<IActionResult> Contact(string name, string company, string email, string subject, string message)
    {
        string toEmail = "info@sunmedikal.com"; 
        string mailSubject = $"Siteden Yeni İletişim Formu: {subject}";

        string mailBody = $@"
            <div style='font-family: Arial; padding: 20px; border: 1px solid #ddd; border-top: 4px solid #dc3545;'>
                <h2 style='color: #dc3545;'>Yeni İletişim Mesajı</h2>
                <p><strong>Gönderen:</strong> {name}</p>
                <p><strong>Firma:</strong> {(string.IsNullOrEmpty(company) ? "Belirtilmedi" : company)}</p>
                <p><strong>Müşteri E-Posta:</strong> {email}</p>
                <p><strong>Konu:</strong> {subject}</p>
                <hr>
                <p><strong>Mesaj Detayı:</strong></p>
                <p style='background-color: #f9f9f9; padding: 15px;'>{message.Replace("\n", "<br>")}</p>
            </div>";

        try
        {
            await _emailService.SendEmailAsync(toEmail, mailSubject, mailBody);
            TempData["SuccessMessage"] = "Mesajınız başarıyla gönderildi. Ekibimiz en kısa sürede size dönüş yapacaktır.";
        }
        catch (Exception ex)
        {
            _logger.LogError("İletişim formu mail hatası: " + ex.Message);
            TempData["ErrorMessage"] = "Mesaj gönderilirken bir hata oluştu. Lütfen bilgilerinizi kontrol edip tekrar deneyin.";
        }

        return RedirectToAction("Contact");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // ==========================================================
    // KURUMSAL SÖZLEŞMELER VE POLİTİKALAR
    // ==========================================================

    public IActionResult GizlilikPolitikası()
    {
        return View();
    }

    public IActionResult MesafeliSatis()
    {
        return View();
    }

    public IActionResult IadeSartlari()
    {
        return View();
    }

    public IActionResult OdemeTeslimat()
    {
        return View();
    }

    public IActionResult CerezPolitikası()
    {
        return View();
    }
}