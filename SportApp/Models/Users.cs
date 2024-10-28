using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SportApp.Models
{
    public class Users
    {
        //when passing along set UserId to null
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string? PictureUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNum { get; set; }
        public string? HomeNum { get; set; }
        public string? StreetName { get; set; }
        public string? CityName { get; set; }
        public int Urank { get; set; }
        public string? Description { get; set; }
        [JsonIgnore]
        public bool IsCheck { get; set; } = false;
        [JsonIgnore]
        public int Rating { get; set; }
        [JsonIgnore]
        public int ReportCount { get; set; }
    }

}
