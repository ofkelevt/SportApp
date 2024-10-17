using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SportApp.Models
{
    [Table("event")]
    public class Event
    {
        [Key]
        [Column("event_id")]
        public int EventId { get; set; }

        [MaxLength(10)]
        [Column("home_num")]
        public string? HomeNum { get; set; }

        [MaxLength(100)]
        [Column("street_name")]
        public string? StreetName { get; set; }

        [MaxLength(50)]
        [Column("city_name")]
        public string? CityName { get; set; }

        [Column("picture_url")]
        public string? PictureUrl { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("sport")]
        public string Sport { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("description")]
        public string? Description { get; set; }

        [Column("ends_at")]
        public DateTime? EndsAt { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("event_name")]
        public string EventName { get; set; }

        //[ForeignKey("User")]
        [Column("crator_id")]
        public int CratorId { get; set; }

        public Users Crator { get; set; }
        [Required]
        public ICollection<UserToEvent> Participants { get; set; }
        [Required]
        public ICollection<ChatComment> ChatComments { get; set; }
    }


}
