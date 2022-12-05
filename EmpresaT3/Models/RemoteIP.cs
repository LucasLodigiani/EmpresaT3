using System.Text.Json;
using System.Text.Json.Serialization;

namespace EmpresaT3.Models
{
    public class RemoteIP
    {
        [JsonPropertyName("ip")]
        public string IP { get; set; }
        [JsonPropertyName("geo-ip")]
        public string GeoIp { get; set; }
        [JsonPropertyName("API Help")]
        public string ApiHelp { get; set; }

        static public string GetIP()
        {
            HttpClient client = new HttpClient();
            var result = client.GetStringAsync("https://jsonip.com/").Result;
            var ip = JsonSerializer.Deserialize<RemoteIP>(result.ToString()).IP;
            return ip;
        }

    }


}