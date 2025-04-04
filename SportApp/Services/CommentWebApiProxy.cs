﻿using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SportApp.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace SportApp.Services
{
    class CommentWebApiProxy
    {
        private HttpClient client;
        private JsonSerializerOptions jsonSerializerOptions;
        private string baseUrl;
        public static string BaseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5274/api/" : "http://localhost:5274/api/";
        public List<Comment> events { get; set; }

        public CommentWebApiProxy(ClientHandler h)
        {

            this.client = new HttpClient(h.handler, false);
            this.baseUrl = BaseAddress;
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<Comment>> GetCommentsAsync()
        {
            //Set URI to the specific function API
            string url = $"{this.baseUrl}Comments";
            try
            {
                var events = await client.GetFromJsonAsync<List<Comment>>(url);
                return events; // Return the list of events
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
        public async Task<List<Comment>> GetUserCommentsAsync(int userid)
        {
            //Set URI to the specific function API
            string url = $"{this.baseUrl}Comments/user/{userid}";
            try
            {
                var events = await client.GetFromJsonAsync<List<Comment>>(url);
                return events; // Return the list of events
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task<Comment> GetCommentAsync(int eventId)
        {
            string url = $"{this.baseUrl}Comments/{eventId}";
            try
            {
                var events = await client.GetFromJsonAsync<Comment>(url);
                return events; // Return the list of events
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> DeleteCommentAsync(int eventId)
        {
            string url = $"{this.baseUrl}Comments/{eventId}";
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
        public async Task<bool> PostUserAsync(Comment events)
        {
            string url = $"{this.baseUrl}Comments";
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
