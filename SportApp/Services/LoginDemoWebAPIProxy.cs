using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private static readonly HttpClient client = new HttpClient(new HttpClientHandler { CookieContainer = cookieContainer, UseCookies = true },true);
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
                 PropertyNameCaseInsensitive= true
            };  
        }

        public async Task<Users> LoginAsync(string userName,string password)
        {
            //Set URI to the specific function API
            string url = $"{this.baseUrl}login";
            try
            {
                //Call the server API
                LoginInfo info =new LoginInfo() { UserName = userName, Password = password };
                string json= JsonSerializer.Serialize(info,jsonSerializerOptions);
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
                    LoggedInUser = JsonSerializer.Deserialize<Users>(resContent, jsonSerializerOptions); 
                    var shellViewModel = (AppShellViewModel)App.Current.MainPage.BindingContext;
                    shellViewModel.IsLoggedIn = true;
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

    }
}
