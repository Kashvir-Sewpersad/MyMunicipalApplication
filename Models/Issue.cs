//====================================================== START OF FILE ===================================//

//------------------- START OF IMPORTS -------------//
using System;
using System.ComponentModel.DataAnnotations;
//--------------------- END OF IMPORTS --------------//
namespace Programming_7312_Part_1.Models
{
    public class Issue
    {
            // get and set for the issue ID
        public int Id { get; set; }

            // get and set for the issue location
        [Required]
        public string? Location { get; set; }

            // get and set for the issue category
        [Required]
        public string? Category { get; set; }

                // get and set for the issue description
        [Required]
        public string? Description { get; set; }

            // get and set for the issue email
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        // get and set for the issue attached file path
        public string? AttachedFilePath { get; set; }

            // get and set for the issue status
        public string Status { get; set; } = "Pending"; // Pending, In Progress, Resolved

        // get and set for the issue reported date
        public DateTime ReportedDate { get; set; }

            // get and set for the issue upvotes
        public int Upvotes { get; set; } = 0;

        // get and set for the issue downvotes
        public int Downvotes { get; set; } = 0;

            // get and set for the issue user ID
        // New: User ID for tracking
        public string? UserId { get; set; } = string.Empty;

            // get and set for the issue admin response
        // Admin response fields
        public string? AdminResponse { get; set; } // "Approved", "Rejected", "Deleted"
            // get and set for the issue admin comments
        public string? AdminComments { get; set; }
        // get and set for the issue response date
        public DateTime? ResponseDate { get; set; }

        public Issue()
        {
            ReportedDate = DateTime.Now;
        }
    }
}
//====================================================== END OF FILE ===================================//
