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
        public static string BaseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5274/api/" : "http://localhost:5274/api/";
        public List<Users> events { get; set; }

        public UserWebAPIProxy(ClientHandler h)
        {

            this.client = new HttpClient(h.handler, false);
            this.baseUrl = BaseAddress;
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<Users>> GetUsersAsync()
        {
            //Set URI to the specific function API
            string url = $"{this.baseUrl}Users";
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

        public async Task<Users> GetUserAsync(int eventId)
        {
            string url = $"{this.baseUrl}Users/{eventId}";
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
        public async Task<bool> DeleteUserAsync(int eventId)
        {
            string url = $"{this.baseUrl}Users/{eventId}";
            try
            {
                await client.DeleteAsync(url);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }

        }
        public async Task<bool> PostUserAsync(Users events)
        {
            string url = $"{this.baseUrl}Users";
            try
            {
                UserSend u = new UserSend(events);
                string json = JsonSerializer.Serialize(u, jsonSerializerOptions);
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
        public async Task<bool> PutUserAsync(Users user)
        {
            UserSend u = new UserSend(user);
            string url = $"{this.baseUrl}Users/{user.UserId}"; // Assuming the user ID is part of the URL for the PUT request
            try
            {
                string json = JsonSerializer.Serialize(u, jsonSerializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(url, content);
                return response.IsSuccessStatusCode;
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
