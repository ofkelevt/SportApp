using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportApp.Models
{
    internal class EventSend
    {
        public int EventId { get; set; }
        public string? HomeNum { get; set; }

        public string? StreetName { get; set; }

        public string? CityName { get; set; }

        public ByteArrayContent? PictureUrl { get; set; }

        public string Sport { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? Description { get; set; }

        public DateTime EndsAt { get; set; }

        public string EventName { get; set; }

        public int CratorId { get; set; }
        public EventSend(Event events)
        {
            this.EventId = events.EventId;
            this.HomeNum = events.HomeNum;
            this.StreetName = events.StreetName;
            this.CityName = events.CityName;
            this.PictureUrl = new ByteArrayContent(events.PictureUrl);
            this.Sport = events.Sport;
            this.CreatedAt = events.CreatedAt;
            this.Description = events.Description;
            this.EndsAt = events.EndsAt;
            this.EventName = events.EventName;
            this.CratorId = events.CratorId;
        }
    }
}
