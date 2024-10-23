using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace SportApp.Models
{
    public class Event
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int EventId { get; set; }
        public string? HomeNum { get; set; }

        public string? StreetName { get; set; }

        public string? CityName { get; set; }

        public string? PictureUrl { get; set; }

        public string Sport { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? Description { get; set; }

        public DateTime EndsAt { get; set; }

        public string EventName { get; set; }

        public int CratorId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Users Crator { get; set; }
    }


}
