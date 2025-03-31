using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SportApp.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace SportApp.Services
{
    class ReportWebApiProxy
    {
        private HttpClient client;
        private JsonSerializerOptions jsonSerializerOptions;
        private string baseUrl;
        public static string BaseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5274/api/" : "http://localhost:5274/api/";
        public List<Report> events { get; set; }

        public ReportWebApiProxy(ClientHandler h)
        { 
            this.client = new HttpClient(h.handler, false);
            this.baseUrl = BaseAddress;
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<Report>> GetReportsAsync()
        {
            //Set URI to the specific function API
            string url = $"{this.baseUrl}Reports";
            try
            {
                var events = await client.GetFromJsonAsync<List<Report>>(url);
                return events; // Return the list of events
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
        public async Task<List<Report>> GetUserReportsAsync(int userid)
        {
            //Set URI to the specific function API
            string url = $"{this.baseUrl}Reports/user/{userid}";
            try
            {
                var events = await client.GetFromJsonAsync<List<Report>>(url);
                return events; // Return the list of events
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task<Report> GetReportAsync(int eventId)
        {
            string url = $"{this.baseUrl}Reports/{eventId}";
            try
            {
                var events = await client.GetFromJsonAsync<Report>(url);
                return events; // Return the list of events
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
        public async Task DeleteReportAsync(int eventId)
        {
            string url = $"{this.baseUrl}Reports/{eventId}";
            try
            {
                await client.DeleteAsync(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }
        public async Task<bool> PostUserAsync(Report events)
        {
            string url = $"{this.baseUrl}Reports";
            try
            {
                string json = JsonSerializer.Serialize(events, jsonSerializerOptions);
                //string json = JsonSerializer.Serialize(new{ Email=email,Password=password},jsonSerializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    return true; // Event successfully created
                }
                else
                {
                    return false; // Handle failure case
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (logging, rethrowing, etc.)
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false; // Return false on error
            }
        }
    }
}
