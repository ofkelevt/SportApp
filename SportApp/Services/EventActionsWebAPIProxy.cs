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
    class EventActionsWebAPIProxy
    {
        private HttpClient client;
        private JsonSerializerOptions jsonSerializerOptions;
        private string baseUrl;
        public static string BaseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5274/api/" : "http://localhost:5274/api/";

        public List<Event> events { get; set;}

        public EventActionsWebAPIProxy(ClientHandler h)
        {
            //Set client handler to support cookies!!
            this.client = new HttpClient(h.handler, false);
            this.baseUrl = BaseAddress;
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<Event>> GetEventsAsync()
        {
            //Set URI to the specific function API
            string url = $"{this.baseUrl}Events";
            try
            {
                var events = await client.GetFromJsonAsync<List<Event>>(url);
                return events; // Return the list of events
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
        public async Task<string> status()
        {
            string url = $"{this.baseUrl}Events/status";
            try
            {
                var events = await client.GetFromJsonAsync<string>(url);
                return events;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
        public async Task<Event> GetEventAsync(int eventId)
        {
            string url = $"{this.baseUrl}Events/{eventId}";
            try
            {
                var events = await client.GetFromJsonAsync<Event>(url);
                return events; // Return the list of events
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> putEventAsync(Event updatedEvent)
        {
            try
            {
                // Serialize the updated object to JSON
                var content = JsonContent.Create(updatedEvent);

                // Send the PUT request to the appropriate endpoint (e.g., /events/{id})
                var response = await client.PutAsync($"{baseUrl}UserToEvents/{updatedEvent.EventId}", content);

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
            string url = $"{this.baseUrl}Events/{eventId}";
            try
            {
                await client.DeleteAsync(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }
        public async Task<bool> PostEventAsync(Event events)
        {
            string url = $"{this.baseUrl}Events";
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
