using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace SportApp.Models
{
    [Table("users")]
    public class Users
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("username")]
        public string Username { get; set; }

        [MaxLength(100)]
        [Column("password")]
        public string? Password { get; set; }  // Sensitive data nullable

        [Column("picture_url")]
        public string? PictureUrl { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("first_name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("last_name")]
        public string LastName { get; set; }

        [MaxLength(15)]
        [Column("phone_num")]
        public string? PhoneNum { get; set; }

        [MaxLength(10)]
        [Column("home_num")]
        public string? HomeNum { get; set; }

        [MaxLength(100)]
        [Column("street_name")]
        public string? StreetName { get; set; }

        [MaxLength(50)]
        [Column("city_name")]
        public string? CityName { get; set; }

        [Column("urank")]
        public int? Urank { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        // Navigation Properties
        public ICollection<Event> CreatedEvents { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<ChatComment> ChatComments { get; set; }
        public ICollection<Report> Reports { get; set; }
        public ICollection<UserToEvent> UserEvents { get; set; }
    }

}
