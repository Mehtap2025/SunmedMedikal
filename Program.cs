using Microsoft.EntityFrameworkCore;
using SunmedMedikal.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using SunmedMedikal.Services;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;

var builder = WebApplication.CreateBuilder(args);

// =======================================================
// 1. BÖLÜM: SERVİS KAYITLARI (app.Build'den önce)
// =======================================================

// --- SİHİR BURADA: ÇOKLU DİL MOTORU (LOCALIZATION) EKLENDİ ---
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();
// -------------------------------------------------------------

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<SmsService>();

// --- LOGO İŞBAŞI API BAĞLANTILARI ---
builder.Services.AddHttpClient(); 
builder.Services.AddScoped<LogoIsbasiService>(); 

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromHours(48); 
});

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "GuvenlikAnahtarlari")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; 
        options.AccessDeniedPath = "/Account/AccessDenied"; 
        options.ExpireTimeSpan = TimeSpan.FromDays(30); 
        options.SlidingExpiration = true; 
        options.Cookie.IsEssential = true; 
    });

var app = builder.Build();

// =======================================================
// 2. BÖLÜM: HTTP PIPELINE
// =======================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting(); 
app.UseStaticFiles();

// --- SİHİR BURADA: DESTEKLENEN DİLLER PİPELINE'A EKLENDİ ---
var supportedCultures = new[]
{
    new CultureInfo("tr-TR"), // Türkçe
    new CultureInfo("en-US"), // İngilizce
    new CultureInfo("fr-FR"), // Fransızca
    new CultureInfo("ar-SA")  // Arapça
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("tr-TR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});
// -------------------------------------------------------------

app.UseSession(); 
app.UseAuthentication(); 
app.UseAuthorization(); 
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// ==========================================
// VERİTABANI YOKSA OTOMATİK OLUŞTURMA SİHRİ
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SunmedMedikal.Data.ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
