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
    class UserWebAPIProxy
    {
        private HttpClient client;
        private JsonSerializerOptions jsonSerializerOptions;
        private string baseUrl;
        public static string BaseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5171/api/" : "http://localhost:5171/api/";
        public List<Users> events { get; set; }

        public UserWebAPIProxy()
        {
            //Set client handler to support cookies!!
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = new System.Net.CookieContainer();

            this.client = new HttpClient(handler, true);
            this.baseUrl = BaseAddress;
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<Users>> GetEventsAsync()
        {
            //Set URI to the specific function API
            string url = $"{this.baseUrl}User";
            try
            {
                var events = await client.GetFromJsonAsync<List<Users>>(url);
                return events; // Return the list of events
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task<Users> GetEventAsync(int eventId)
        {
            string url = $"{this.baseUrl}User/{eventId}";
            try
            {
                var events = await client.GetFromJsonAsync<Users>(url);
                return events; // Return the list of events
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
        public async void DeleteEventAsync(int eventId)
        {
            string url = $"{this.baseUrl}User/{eventId}";
            try
            {
                await client.DeleteAsync(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }
        public async Task<bool> PostEventAsync(Users events)
        {
            string url = $"{this.baseUrl}User";
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
