using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SportApp.Models;
namespace SportApp.Services
{
    public class LoginDemoWebAPIProxy
    {
        private HttpClient client;
        private JsonSerializerOptions jsonSerializerOptions;
        private string baseUrl;
        public static string BaseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5171/api/" : "http://localhost:5171/api/";

        public Users LoggedInUser { get; set; }

        public LoginDemoWebAPIProxy()
        {
            //Set client handler to support cookies!!
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = new System.Net.CookieContainer();

            this.client = new HttpClient(handler, true);
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
                    //Extract the content as string
                    string resContent = await response.Content.ReadAsStringAsync();
                    //Desrialize result
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    Users result = JsonSerializer.Deserialize<Users>(resContent, options);
                    return result;
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
                    return resContent;
                }
                else
                {
                    return "User is not logged in!";
                }
            }
            catch (Exception ex)
            {

                return "FAILED WITH EXCEPTION!";
            }
        }

    }
}
