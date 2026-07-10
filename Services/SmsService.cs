using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Web;

namespace SunmedMedikal.Services
{
    public class SmsService
    {
        private readonly IConfiguration _config;

        public SmsService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            var apiUrl = _config["SmsSettings:ApiUrl"];
            var user = _config["SmsSettings:Username"];
            var pass = _config["SmsSettings:Password"];
            var header = _config["SmsSettings:Header"];

            // 📌 Telefonu düzelt (çok önemli)
            if (phoneNumber.StartsWith("+90"))
                phoneNumber = phoneNumber.Substring(3);
            else if (phoneNumber.StartsWith("0"))
                phoneNumber = phoneNumber.Substring(1);

            using (var client = new HttpClient())
            {
                var url = $"{apiUrl}?usercode={user}&password={pass}&gsmno={phoneNumber}&message={HttpUtility.UrlEncode(message)}&msgheader={header}";

                var response = await client.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();

                Console.WriteLine("SMS SONUÇ: " + result);
            }
        }
    }
}