using System;
using System.ComponentModel.DataAnnotations;

namespace Programming_7312_Part_1.Models
{
    public class Issue
    {
        public int Id { get; set; }

        [Required]
        public string? Location { get; set; }

        [Required]
        public string? Category { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public string? AttachedFilePath { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, In Progress, Resolved

        public DateTime ReportedDate { get; set; }

        public int Upvotes { get; set; } = 0;

        public int Downvotes { get; set; } = 0;

        // New: User ID for tracking
        public string? UserId { get; set; } = string.Empty;

        // Admin response fields
        public string? AdminResponse { get; set; } // "Approved", "Rejected", "Deleted"
        public string? AdminComments { get; set; }
        public DateTime? ResponseDate { get; set; }

        public Issue()
        {
            ReportedDate = DateTime.Now;
        }
    }
}
