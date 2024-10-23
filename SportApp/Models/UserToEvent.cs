using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace SportApp.Models
{
    public class UserToEvent
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int TableId { get; set; }
        public int UserId { get; set; }

        public int EventId { get; set; }
        public string RealtionshipType { get; set; }
        public UserToEvent ChangeStatus()
        {
            if (RealtionshipType == "attend")
                RealtionshipType = "waiting";
            else RealtionshipType = "attend";
            return this;
        }
    }
}
