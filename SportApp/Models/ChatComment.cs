using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportApp.Models
{
    [Table("chat_comment")]
    public class ChatComment
    {
        [Key]
        [Column("comment_id")]
        public int CommentId { get; set; }

        [ForeignKey("User")]
        [Column("commenter_id")]
        public int CommenterId { get; set; }

        [ForeignKey("Event")]
        [Column("event_id")]
        public int EventId { get; set; }

        [Column("comment")]
        public string? CommentText { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        // Navigation Properties
        public Users Commenter { get; set; }
        public Event Event { get; set; }
    }

}
