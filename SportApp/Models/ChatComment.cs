using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SportApp.Models
{
    public class ChatComment
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int CommentId { get; set; }
        public int CommenterId { get; set; }
        public int EventId { get; set; }
        public string? CommentText { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [JsonIgnore]
        public Users Commenter { get; set; }
        [JsonIgnore]
        public Event Event { get; set; }
    }

}
