// Models/Event.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Programming_7312_Part_1.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public string? Category { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public string? Location { get; set; }

        public string? ImagePath { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // For tracking user interest
        public int ViewCount { get; set; } = 0;

        // For recommendation system
        public List<string> Tags { get; set; } = new List<string>();
    }
}