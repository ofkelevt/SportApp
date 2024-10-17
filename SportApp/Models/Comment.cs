using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportApp.Models
{

    [Table("comment")]
    public class Comment
    {
        [Key]
        [Column("comment_id")]
        public int CommentId { get; set; }

        [ForeignKey("User")]
        [Column("commenter_id")]
        public int CommenterId { get; set; }


        [ForeignKey("User")]
        [Column("commented_on_id")]
        public int CommentedOnId { get; set; }


        [Column("comment")]
        public string? CommentText { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Range(1, 5)]
        [Column("rating")]
        public int Rating { get; set; }

        // Navigation Properties
        public Users Commenter { get; set; }
        public Users CommentedOn { get; set; }
    }

}
