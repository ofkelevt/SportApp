using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportApp.Services
{
    public class ClientHandler
    {
        public HttpClientHandler handler;
        public ClientHandler()
        {
            handler = new HttpClientHandler();
            handler.CookieContainer = new System.Net.CookieContainer();
            handler.UseCookies = true;
        }
    }
}
