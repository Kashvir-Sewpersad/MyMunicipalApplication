//--------------------------- start of file -----------------------------//
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

        public string? AttachedFilePath { get; set; } // optional attachment

        public string Status { get; set; } = "Pending";

        public DateTime ReportedDate { get; set; } // to be set to current date and time off submission

        // For voting system
        public int Upvotes { get; set; } = 0;

        public Issue()
        {
            ReportedDate = DateTime.Now; // set to current 
        }
    }
}
//------------------------------------ end of file --------------------------------//
