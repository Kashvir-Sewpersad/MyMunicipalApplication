

//********************************************************* start of file ***********************************************************//

//------------------------------ start of imports ---------------------------------//

using System;
using System.ComponentModel.DataAnnotations;

//---------------------------------- end of imports --------------------------------//

namespace Programming_7312_Part_1.Models
{
    public class Issue
    {
        public int Id { get; set; } // primary key getter and setter

        [Required]
        public string? Location { get; set; } // location getter and setter variable 

        [Required]
        public string? Category { get; set; }  // category getter and setter variable

        [Required]
        public string? Description { get; set; }  // description getter and setter variable

        public string? AttachedFilePath { get; set; } // optional attachment

        public string Status { get; set; } = "Pending"; // default status is pending 

        public DateTime ReportedDate { get; set; } // to be set to current date and time off submission

        // For voting system
        
        /*
         *
         * below method is used to track the number of upvotes an issue has had
         *
         * this output from the data set to this is used to determine how popular a search is
         *
         * this will be used ti recommend issues based on what is popular
         *
         * 
         */
        public int Upvotes { get; set; } = 0; // default set to zero 

        public Issue()
        {
            ReportedDate = DateTime.Now; // set to current 
        }
    }
}
//**************************************************** end of file ***********************************************************//
