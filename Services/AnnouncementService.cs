//**************************************************** start of file **********************************************************//

//------------------------------ start of imports ---------------------------------//
using Programming_7312_Part_1.Data;

using Programming_7312_Part_1.Models;

using System;
using System.Collections.Generic;
using System.Linq;
//---------------------------------- end of imports --------------------------------//

namespace Programming_7312_Part_1.Services
{
    /*
     *
     * the following class  is used to manage the announcement system 
     *
     * this class shall  shall contian the data srucures and functionalityy for the process of adding an announcement
     *
     * this includes seeded data for testing aand defualt startup  
     *
     * 
     */
    public class AnnouncementService
    {
        
        /*
         *the below is the main data structures used to manage the announcements
         *
         * this includes the queues dictionaries sets sorted dictionaries and more. 
         * 
         */
        private readonly ApplicationDbContext _context;

        // Queue for managing announcements (FIFO - oldest first)
        public Queue<Announcement> AnnouncementQueue { get; } = new Queue<Announcement>();

        // Dictionary for storing announcements by category
        public Dictionary<string, List<Announcement>> AnnouncementsByCategory { get; } = new Dictionary<string, List<Announcement>>();

        // HashSet for unique announcement categories
        public HashSet<string> UniqueCategories { get; } = new HashSet<string>();

        // SortedDictionary for announcements by priority (higher priority first)
        public SortedDictionary<int, List<Announcement>> AnnouncementsByPriority { get; } = new SortedDictionary<int, List<Announcement>>();

        public AnnouncementService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            InitializeDataStructures();
        }
        
        /*
         * 
         * thee below method is used to initialize the data structures for the announcements 
         *
         *
         * this will include the seedeed data 
         * 
         */
        private void InitializeDataStructures()
        {
            // Load all announcements from database
            var allAnnouncements = _context.Announcements.ToList();

            foreach (var announcement in allAnnouncements)
            {
                AddAnnouncementToDataStructures(announcement); // add to data structures
            }

            /*
             *
             * If no announcements exist, seed sample announcements
             * 
             */
            if (!allAnnouncements.Any())
            {
                SeedSampleAnnouncements(); // method call 
            }
        }
        /*
         * the below method is used to seed sample announcements
         *
         * this is to ensure that the system has some data to work with on initial startup
         *
         * this is hard coded in to the sysytem 
         *
         * this is a fail safe should someone not be able to add an announcement
         *
         * cuurently there are 2 sample announcements
         *
         * 
         */
        private void SeedSampleAnnouncements()
        {
            var sampleAnnouncements = new List<Announcement>
            {
                new Announcement
                {
                    Title = "Water Maintenance Notice",
                    Content = "There will be no water in the area of Upper Newlands on 18 November 2025 from 08:00 to 17:00 due to planned maintenance of water infrastructure - Municipality.",
                    Category = "Maintenance",
                    Priority = 3,
                    ExpiryDate = DateTime.Now.AddDays(30)
                },
                new Announcement
                {
                    Title = "Community Meeting",
                    Content = "Join us for the monthly community meeting to discuss local issues and improvements.",
                    Category = "Community",
                    Priority = 2,
                    ExpiryDate = DateTime.Now.AddDays(7)
                }
            };

            foreach (var announcement in sampleAnnouncements)
            {
                _context.Announcements.Add(announcement);
            }
            _context.SaveChanges(); // saving the changes to the system 

            /*
             *
             * th the below forloop is to add the datatstructure to the seeded data 
             *
             * 
             */
            foreach (var announcement in sampleAnnouncements)
            {
                AddAnnouncementToDataStructures(announcement);
            }
        }
        /*
         *
         * the below method is to add an annooucement
         *
         * the changes are saved to the system
         *
         *
         * 
         */
        public void AddAnnouncement(Announcement announcement)
        {
            _context.Announcements.Add(announcement);
            _context.SaveChanges(); // save action 

            AddAnnouncementToDataStructures(announcement);
        }
        /*
         * the below method contains the logic to add an announcement to the data structures
         *
         * this includes limmiting on how many announcements caan be stored in the system 
         * 
         */
        private void AddAnnouncementToDataStructures(Announcement announcement)
        {
            // Add to AnnouncementQueue  this will allow a miximuum of 10 announcements to be stored int the system 
            AnnouncementQueue.Enqueue(announcement);
            if (AnnouncementQueue.Count > 10)
            {
                AnnouncementQueue.Dequeue();
            }

            // the below method is to add the announcement to the category dictionary 
            if (!string.IsNullOrEmpty(announcement.Category))
            {
                if (!AnnouncementsByCategory.ContainsKey(announcement.Category))
                {
                    AnnouncementsByCategory[announcement.Category] = new List<Announcement>(); // adding a new list if the category does not exist
                }
                AnnouncementsByCategory[announcement.Category].Add(announcement); // adding the announcement to the category list

                // Add to UniqueCategories
                UniqueCategories.Add(announcement.Category);
            }

            // Add to AnnouncementsByPriority
            if (!AnnouncementsByPriority.ContainsKey(announcement.Priority)) // checking if the priority exists
            {
                AnnouncementsByPriority[announcement.Priority] = new List<Announcement>(); // adding a new list if the priority does not exist
            }
            AnnouncementsByPriority[announcement.Priority].Add(announcement); // adding the announcement to the priority list
        }
        /*
         * the below method is used to get all announcements from the system
         *
         *  this includes filtering out inactive or expired announcements
         *
         *  the announcements are ordered by priority and created date
         *
         * the priority is used to determine how important an announcement is and the priority is set by the admin when creating the announcement 
         * 
         */
        public List<Announcement> GetAllAnnouncements()
        {
            return _context.Announcements
                .Where(a => a.IsActive && (a.ExpiryDate == null || a.ExpiryDate > DateTime.Now)) 
                .OrderByDescending(a => a.Priority) // most important first  followed by the lest recent
                .ThenByDescending(a => a.CreatedDate)
                .ToList();
        }
        /* the below method is used to get active announcements
         * 
         * this includes a count to limit the number of announcements returned
         *
         * the default count is 5
         * 
         */
        public List<Announcement> GetActiveAnnouncements(int count = 5) // set to  5 as default 
        {
            return GetAllAnnouncements().Take(count).ToList(); // take the top 'count' announcements 
        }
        
        
        /*
         *
         * the below is to get the announcements by category
         *
         *  this includes filtering out inactive or expired announcements
         */

        public List<Announcement> GetAnnouncementsByCategory(string category)
        {
            if (AnnouncementsByCategory.ContainsKey(category)) // checking if the category exists
            {
                // filtering out inactive or expired announcements
                return AnnouncementsByCategory[category]
                    .Where(a => a.IsActive && (a.ExpiryDate == null || a.ExpiryDate > DateTime.Now))
                    .OrderByDescending(a => a.CreatedDate)
                    .ToList();
            }
            return new List<Announcement>(); // return empty list if category not found
        }

        public List<Announcement> GetRecentAnnouncements(int count = 3) // default to 3 recent announcements
        {
            return AnnouncementQueue.Reverse().Take(count).ToList(); // take the most recent 'count' announcements
        }
        

        public Announcement GetAnnouncementById(int id) // get announcement by id
        {
            return _context.Announcements.FirstOrDefault(a => a.Id == id); // return null if not found
        }

        public bool UpdateAnnouncement(Announcement updatedAnnouncement) // update an existing announcement
        {
            var existingAnnouncement = _context.Announcements.FirstOrDefault(a => a.Id == updatedAnnouncement.Id);
            if (existingAnnouncement == null)
            {
                return false; // announcement not found
            }

            // Store old values for cleanup
            var oldCategory = existingAnnouncement.Category; // Store the old category 
            var oldPriority = existingAnnouncement.Priority; // Store the old priority

            // Update properties
            existingAnnouncement.Title = updatedAnnouncement.Title; // update title
            
            existingAnnouncement.Content = updatedAnnouncement.Content; // update content
            
             existingAnnouncement.Category = updatedAnnouncement.Category; // update category
            
            existingAnnouncement.ExpiryDate = updatedAnnouncement.ExpiryDate; // update expiry date
            
            existingAnnouncement.IsActive = updatedAnnouncement.IsActive; // update active status
            
            existingAnnouncement.Priority = updatedAnnouncement.Priority; // update priority

            _context.SaveChanges();

            // Update data structures
            UpdateAnnouncementInDataStructures(existingAnnouncement, oldCategory, oldPriority); // Pass old priority

            return true;
        }
        
        /*
         *
         * the below method is used to update the announcement in the data structures
         *
         * this includes checking if the category or priority has changed
         *
         * if the category has changed the announcement is removed from the old category and added to the new one
         *
         * 
         */

        private void UpdateAnnouncementInDataStructures(Announcement updatedAnnouncement, string oldCategory, int oldPriority)
        {
            // If category changed, update AnnouncementsByCategory
            if (oldCategory != updatedAnnouncement.Category)
            {
                // Remove from old category
                if (!string.IsNullOrEmpty(oldCategory) && AnnouncementsByCategory.ContainsKey(oldCategory))
                {
                    AnnouncementsByCategory[oldCategory].RemoveAll(a => a.Id == updatedAnnouncement.Id); // remove from old category
                    if (AnnouncementsByCategory[oldCategory].Count == 0) // if no more announcements in the old category
                    {
                        AnnouncementsByCategory.Remove(oldCategory); // deletw 
                        
                        UniqueCategories.Remove(oldCategory); // delete 
                    }
                }

                // Add to new category
                if (!string.IsNullOrEmpty(updatedAnnouncement.Category))
                {
                    if (!AnnouncementsByCategory.ContainsKey(updatedAnnouncement.Category)) // if new category does not exist
                    {
                        AnnouncementsByCategory[updatedAnnouncement.Category] = new List<Announcement>(); // create new list
                    }
                    AnnouncementsByCategory[updatedAnnouncement.Category].Add(updatedAnnouncement);

                    // Update UniqueCategories
                    UniqueCategories.Add(updatedAnnouncement.Category);
                }
            }

            // If priority changed, update AnnouncementsByPriority
            if (oldPriority != updatedAnnouncement.Priority)
            {
                // Remove from old priority
                if (AnnouncementsByPriority.ContainsKey(oldPriority)) // if old priority exists
                {
                    AnnouncementsByPriority[oldPriority].RemoveAll(a => a.Id == updatedAnnouncement.Id); // remove from old priority
                    if (AnnouncementsByPriority[oldPriority].Count == 0)
                    {
                        AnnouncementsByPriority.Remove(oldPriority); // remove if no more announcements in that priority
                    }
                }
            }

            // Add to new priority (or update in existing)
            if (!AnnouncementsByPriority.ContainsKey(updatedAnnouncement.Priority))
            {
                AnnouncementsByPriority[updatedAnnouncement.Priority] = new List<Announcement>(); // create new list if priority does not exist
            }
            // Ensure the announcement is not duplicated if priority hasn't changed
            AnnouncementsByPriority[updatedAnnouncement.Priority].RemoveAll(a => a.Id == updatedAnnouncement.Id);
            
            AnnouncementsByPriority[updatedAnnouncement.Priority].Add(updatedAnnouncement); // add to new priority
        }

        public bool DeleteAnnouncement(int id)
        {
            var announcement = _context.Announcements.FirstOrDefault(a => a.Id == id);// find announcement by id
            if (announcement == null) // if not found
            {
                return false;
            }

            // Store old values for cleanup
            var oldCategory = announcement.Category;

            // Remove from database
            _context.Announcements.Remove(announcement); // remove from db
            
            _context.SaveChanges(); // update 

            // Update data structures
            RemoveAnnouncementFromDataStructures(announcement, oldCategory);

            return true;
        }

        private void RemoveAnnouncementFromDataStructures(Announcement announcement, string oldCategory)
        {
            // Remove from AnnouncementsByCategory
            if (!string.IsNullOrEmpty(oldCategory) && AnnouncementsByCategory.ContainsKey(oldCategory))
            {
                AnnouncementsByCategory[oldCategory].RemoveAll(a => a.Id == announcement.Id); // remove from old category
                if (AnnouncementsByCategory[oldCategory].Count == 0) // if no more announcements in that category
                {
                    AnnouncementsByCategory.Remove(oldCategory);
                    UniqueCategories.Remove(oldCategory); // remove from unique categories as well
                }
            }

            // Remove from AnnouncementQueue
            var queueList = AnnouncementQueue.Where(a => a.Id != announcement.Id).ToList(); // filter out the announcement to be removed
            AnnouncementQueue.Clear(); // clear the queue
            foreach (var a in queueList)
            {
                AnnouncementQueue.Enqueue(a); // re-enqueue remaining announcements
            }

            // Remove from AnnouncementsByPriority
            foreach (var priorityList in AnnouncementsByPriority.Values)
            {
                priorityList.RemoveAll(a => a.Id == announcement.Id); // remove from each priority list
            }
        }
    }
}

//**************************************************** end of file **********************************************************//