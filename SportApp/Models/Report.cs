using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace SportApp.Models
{

    public class Report
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int ReportId { get; set; }
        public int ReporterId { get; set; }
        public int TargetId { get; set; }
        public string? CommentText { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Users Reporter { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Users Target { get; set; }
    }

}
