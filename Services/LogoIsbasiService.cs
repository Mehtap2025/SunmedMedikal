using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SunmedMedikal.Models;

namespace SunmedMedikal.Services
{
    public class LogoIsbasiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private static string _accessToken = string.Empty;
        private static string _tenantId = string.Empty;

        public LogoIsbasiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<bool> AuthenticateAsync()
        {
            try
            {
                var loginData = new { username = _config["LogoIsbasi:Username"], password = _config["LogoIsbasi:Password"] };
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("ApiKey", _config["LogoIsbasi:ApiKey"]);

                var response = await _httpClient.PostAsync($"{_config["LogoIsbasi:ApiUrl"]}/user/integrationLogin", 
                    new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json"));
                
                var body = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode) return false;

                using var doc = JsonDocument.Parse(body);
                var data = doc.RootElement.GetProperty("data");
                _accessToken = data.GetProperty("accessToken").GetString();
                
                if (data.TryGetProperty("tenant", out JsonElement t))
                    _tenantId = t.GetProperty("id").GetString();

                return true;
            }
            catch { return false; }
        }

        public async Task<bool> CreateInvoiceAsync(Order order, User customer)
        {
            await AuthenticateAsync();

            try
            {
                bool isCorporate = customer.CustomerType == "Kurumsal";
                
                // 🔥 LOGO'NUN "TUTAR SIFIR" DEMESİNİ ENGELLEYEN FİYAT BOMBARDIMANI
                var item = new
                {
                    productId = 670, 
                    itemType = 1, 
                    productDetail = new { 
                        id = 670, 
                        itemType = 1, 
                        type = 1,
                        name = "E-Ticaret Satışı" 
                    },
                    quantity = 1.0,
                    // BÜTÜN İHTİMALLERİ GÖNDERİYORUZ Kİ BİRİNİ KESİN YAKALASIN!
                    price = (double)order.TotalAmount,       
                    amount = (double)order.TotalAmount,      
                    unitPrice = (double)order.TotalAmount,   
                    vatRate = 20.0,
                    unitName = "Adet",
                    description = "Sunmed Sipariş #" + order.Id
                };

                var invoiceRequest = new
                {
                    invoiceId = 0,
                    customer = new {
                        isPerson = !isCorporate,
                        name = isCorporate ? customer.CompanyName : null,
                        firstName = !isCorporate ? (customer.FullName ?? "Müşteri") : null,
                        lastName = !isCorporate ? "." : null,
                        tcknVkn = isCorporate ? customer.TaxNumber : (customer.TcNo ?? "11111111111"),
                        address = string.IsNullOrWhiteSpace(order.Address) ? "İstanbul Merkez" : order.Address,
                        city = string.IsNullOrWhiteSpace(order.City) ? "İstanbul" : order.City,
                        district = string.IsNullOrWhiteSpace(order.District) ? "Kadıköy" : order.District,
                        postalCode = "34000", 
                        country = "Türkiye",
                        email = string.IsNullOrWhiteSpace(customer.Email) ? "info@sunmedikal.com" : customer.Email
                    },
                    invoiceDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    currency = "TRY",
                    exchangeRate = 1.0,
                    description = "Sunmed Online Satış",
                    vatIncluded = true,
                    salesInvoiceDetails = new[] { item } 
                };

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var jsonPayload = JsonSerializer.Serialize(invoiceRequest, options);

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("ApiKey", _config["LogoIsbasi:ApiKey"]);
                _httpClient.DefaultRequestHeaders.Add("tenantId", _tenantId);
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

                var response = await _httpClient.PostAsync($"{_config["LogoIsbasi:ApiUrl"]}/invoices/integrationInvoices", 
                    new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
                
                var responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine("\n========================================");
                Console.WriteLine($"🚀 GÖNDERİLEN JSON: {jsonPayload}");
                Console.WriteLine($"🔍 LOGO FİNAL CEVABI: {responseBody}");
                Console.WriteLine("========================================\n");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("🚨 HATA: " + ex.Message);
                return false;
            }
        }
    }
}