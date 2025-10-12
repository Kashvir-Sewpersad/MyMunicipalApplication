// Models/Event.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Programming_7312_Part_1.Models
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Category { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Location { get; set; }

        [MaxLength(500)]
        public string? ImagePath { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // For tracking user interest
        public int ViewCount { get; set; } = 0;

        // For voting system
        public int Upvotes { get; set; } = 0;
        public int Downvotes { get; set; } = 0;

        // For recommendation system
        public List<string> Tags { get; set; } = new List<string>();
    }
}