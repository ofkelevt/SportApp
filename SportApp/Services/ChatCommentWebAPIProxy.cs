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
    class ChatCommentWebAPIProxy
    {
        private HttpClient client;
        private JsonSerializerOptions jsonSerializerOptions;
        private string baseUrl;
        public static string BaseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5274/api/" : "http://localhost:5274/api/";
        public ChatCommentWebAPIProxy()
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

        public async Task<List<ChatComment>> GetChatCommentsAsync()
        {
            //Set URI to the specific function API
            string url = $"{this.baseUrl}ChatComment";
            try
            {
                var chatComments = await client.GetFromJsonAsync<List<ChatComment>>(url);
                return chatComments; // Return the list of events
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
        public async Task<List<ChatComment>> GetEventChatCommentsAsync(int eventId)
        {
            //Set URI to the specific function API
            string url = $"{this.baseUrl}ChatComment/event/{eventId}";
            try
            {
                var chatComments = await client.GetFromJsonAsync<List<ChatComment>>(url);
                return chatComments; // Return the list of events
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
        public async Task<ChatComment> GetChatCommentAsync(int chatCommentId)
        {
            string url = $"{this.baseUrl}ChatComment/{chatCommentId}";
            try
            {
                var chatComments = await client.GetFromJsonAsync<ChatComment>(url);
                return chatComments; // Return the list of events
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> PutChatCommentAsync(ChatComment updatedchatComment)
        {
            try
            {
                // Serialize the updated object to JSON
                var content = JsonContent.Create(updatedchatComment);

                // Send the PUT request to the appropriate endpoint (e.g., /events/{id})
                var response = await client.PutAsync($"{baseUrl}ChatComment/{updatedchatComment.CommentId}", content);

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
        public async Task DeleteChatCommentAsync(int chatCommentId)
        {
            string url = $"{this.baseUrl}ChatComment/{chatCommentId}";
            try
            {
                await client.DeleteAsync(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }
        public async Task<bool> PostChatCommentAsync(ChatComment chatComments)
        {
            string url = $"{this.baseUrl}ChatComment";
            try
            {
                string json = JsonSerializer.Serialize(chatComments, jsonSerializerOptions);
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
