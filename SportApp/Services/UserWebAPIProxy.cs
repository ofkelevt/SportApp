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
                UserSend user = new UserSend(events);
                var form = new MultipartFormDataContent();

                // Add simple string values
                form.Add(new StringContent(user.FirstName), nameof(user.FirstName));
                form.Add(new StringContent(user.LastName), nameof(user.LastName));
                form.Add(new StringContent(user.Username), nameof(user.Username));
                form.Add(new StringContent(user.Password), nameof(user.Password));
                form.Add(new StringContent(user.PhoneNum ?? ""), nameof(user.PhoneNum));
                form.Add(new StringContent(user.HomeNum ?? ""), nameof(user.HomeNum));
                form.Add(new StringContent(user.StreetName ?? ""), nameof(user.StreetName));
                form.Add(new StringContent(user.CityName ?? ""), nameof(user.CityName));
                form.Add(new StringContent(user.Urank.ToString()), nameof(user.Urank));
                form.Add(new StringContent(user.Description ?? ""), nameof(user.Description));

                // Add file (PictureUrl)
                if (user.PictureUrl != null)
                {
                    form.Add(user.PictureUrl, "PictureUrl", "profile.jpg"); // name must match the parameter in controller
                }

                // Send request
                var response = await client.PostAsync(url, form);
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
