using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SportApp.Models
{

    public class Comment
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int CommentId { get; set; }
        public int CommenterId { get; set; }
        public int CommentedOnId { get; set; }
        public string? CommentText { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int Rating { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Users Commenter { get; set; }
        [JsonIgnore]
        public bool IsCommenter { get; set; }
    }

}
