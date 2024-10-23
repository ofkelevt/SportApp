using System;
using System.Collections.Generic;
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
    class EventToUserWebApiProxy
    {
        private HttpClient client;
        private JsonSerializerOptions jsonSerializerOptions;
        private string baseUrl;
        public static string BaseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5274/api/" : "http://localhost:5274/api/";
        public List<UserToEvent> events { get; set; }

        public EventToUserWebApiProxy()
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

        public async Task<List<UserToEvent>> GetEventsAsync()
        {
            //Set URI to the specific function API
            string url = $"{this.baseUrl}UserToEvent";
            try
            {
                var events = await client.GetFromJsonAsync<List<UserToEvent>>(url);
                return events; // Return the list of events
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task<UserToEvent> GetEventAsync(int eventId)
        {
            string url = $"{this.baseUrl}UserToEvent/{eventId}";
            try
            {
                var events = await client.GetFromJsonAsync<UserToEvent>(url);
                return events; // Return the list of events
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> putUserToEventAsync(UserToEvent updatedUserToEvent)
        {
            try
            {
                // Serialize the updated object to JSON
                var content = JsonContent.Create(updatedUserToEvent);

                // Send the PUT request to the appropriate endpoint (e.g., /events/{id})
                var response = await client.PutAsync($"{baseUrl}UserToEvent/{updatedUserToEvent.TableId}", content);

                // Check if the request was successful (HTTP 2xx)
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log the error and return false to indicate failure
                Console.WriteLine($"Error updating UserToEvent: {ex.Message}");
                return false;
            }
        }

        public async Task DeleteEventAsync(int eventId)
        {
            string url = $"{this.baseUrl}UserToEvent/{eventId}";
            try
            {
                await client.DeleteAsync(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }
        public async Task<bool> PostEventAsync(UserToEvent events)
        {
            string url = $"{this.baseUrl}UserToEvent";
            try
            {
                string json = JsonSerializer.Serialize(events, jsonSerializerOptions);
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
