using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SportApp.Models;
using SportApp.ViewModels;
namespace SportApp.Services
{
    public class LoginDemoWebAPIProxy
    {
        private static readonly CookieContainer cookieContainer = new CookieContainer();
        private static readonly HttpClient client = new HttpClient(new HttpClientHandler { CookieContainer = cookieContainer, UseCookies = true }, true);
        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly string baseUrl;
        public static string BaseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5274/api/" : "http://localhost:5274/api/";

        public Users LoggedInUser { get; set; }

        public LoginDemoWebAPIProxy()
        {
            //Set client handler to support cookies!!
            var handler = new HttpClientHandler { CookieContainer = cookieContainer, UseCookies = true };
            this.baseUrl = BaseAddress;
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<Users> LoginAsync(string userName, string password)
        {
            //Set URI to the specific function API
            string url = $"{this.baseUrl}login";
            try
            {
                //Call the server API
                LoginInfo info = new LoginInfo() { UserName = userName, Password = password };
                string json = JsonSerializer.Serialize(info, jsonSerializerOptions);
                //string json = JsonSerializer.Serialize(new{ Email=email,Password=password},jsonSerializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);
                //Check status
                if (response.IsSuccessStatusCode)
                {
                    var shellViewModel = (AppShellViewModel)App.Current.MainPage.BindingContext;
                    shellViewModel.IsLoggedIn = true;
                    
                    //Extract the content as string
                    string resContent = await response.Content.ReadAsStringAsync();
                    //Desrialize result
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    LoggedInUser = JsonSerializer.Deserialize<Users>(resContent, options);
                    if (LoggedInUser.UserId == 2)
                        shellViewModel.IsAdmin = true;
                    return LoggedInUser;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> CheckAsync()
        {
            //Set URI to the specific function API
            string url = $"{this.baseUrl}check";
            try
            {
                //Call the server API
                HttpResponseMessage response = await client.GetAsync(url);
                //Check status
                if (response.IsSuccessStatusCode)
                {
                    //Extract the content as string
                    string resContent = await response.Content.ReadAsStringAsync();
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    LoggedInUser = JsonSerializer.Deserialize<Users>(resContent, options);
                    var shellViewModel = (AppShellViewModel)App.Current.MainPage.BindingContext;
                    shellViewModel.IsLoggedIn = true;
                    if (LoggedInUser.UserId == 2)
                        shellViewModel.IsAdmin = true;
                    return resContent;
                }
                else
                {
                    LoggedInUser = null;
                    var shellViewModel = (AppShellViewModel)App.Current.MainPage.BindingContext;
                    shellViewModel.IsLoggedIn = false;
                    return "User is not logged in!";
                }
            }
            catch (Exception ex)
            {
                var shellViewModel = (AppShellViewModel)App.Current.MainPage.BindingContext;
                shellViewModel.IsLoggedIn = false;
                return "FAILED WITH EXCEPTION!";
            }
        }
        public async Task<bool> Logout()
        {
            string url = $"{this.baseUrl}logout";

            try
            {
                // Send a POST request without any body content.
                var response = await client.PostAsync(url, null);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"An error occurred: {response.ReasonPhrase}");
                    return false;
                }

                // Deserialize the response.
                var result = await response.Content.ReadFromJsonAsync<LogoutResponse>();

                if (result == null || !result.Success)
                {
                    Console.WriteLine($"An error occurred: {result?.Message ?? "Unknown error"}");
                    return false;
                }

                LoggedInUser = null;
                var shellViewModel = (AppShellViewModel)App.Current.MainPage.BindingContext;
                shellViewModel.IsLoggedIn = false;
                shellViewModel.IsAdmin = false;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        // Class to match the JSON structure from the server response.
        public class LogoutResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }
    }

}
