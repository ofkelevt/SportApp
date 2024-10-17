using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SportApp.Models
{
    [Table("user_to_event")]
    public class UserToEvent
    {
        [Key]
        [Column("table_id")]
        public int TableId { get; set; }

        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey("Event")]
        [Column("event_id")]
        public int EventId { get; set; }

        [MaxLength(8)]
        [Column("realtionship_type")]
        public string? RealtionshipType { get; set; }

        // Navigation Properties
        public Users User { get; set; }
        public Event Event { get; set; }
    }

}
