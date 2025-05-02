using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportApp.Models
{
    internal class UserSend
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ByteArrayContent? PictureUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNum { get; set; }
        public string? HomeNum { get; set; }
        public string? StreetName { get; set; }
        public string? CityName { get; set; }
        public int Urank { get; set; }
        public string? Description { get; set; }
        public UserSend(Users users)
        {
            this.UserId = users.UserId;
            this.Username = users.Username;
            this.Password = users.Password;
            this.PictureUrl = new ByteArrayContent(users.PictureUrl);
            this.FirstName = users.FirstName; 
            this.LastName  = users.LastName;
            this.PhoneNum  = users.PhoneNum;
            this.HomeNum  = users.HomeNum;
            this.StreetName = users.StreetName;
            this.CityName = users.CityName;
            this.Urank = users.Urank;
            this.Description = users.Description;
        }
    }
}
