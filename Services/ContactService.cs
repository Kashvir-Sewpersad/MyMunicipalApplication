
//*************************************************** start of file **************************************************//

//------------------------------ start of imports ---------------------------------//
using Programming_7312_Part_1.Data;
using Programming_7312_Part_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

//---------------------------------- end of imports --------------------------------//

//=========================================================== references ====================================//
/*
 *
 *

         Brevo.com. (2025). Brevo | Email Marketing Software, Automation & CRM. [online] Available at: https://www.brevo.com/?gad_source=1&gad_campaignid=23086271243&gbraid=0AAAAAp4YiPJM7xOPrXsppAUZnGIAu-LvQ&gclid=Cj0KCQjw6bfHBhDNARIsAIGsqLjfE68ZXXIQRwSdE_GX3qLjhs0mMFeMj1Idn_ShXWA3lIatJ3mTXQQaArjtEALw_wcB [Accessed 11 Oct. 2025].

         SendGrid. (2024). Email API - Start for Free | SendGrid. [online] Available at: https://sendgrid.com/en-us/solutions/email-api.
 * 
 */
//=============================================================================================================//
namespace Programming_7312_Part_1.Services
{
    public class ContactService
    {
        private readonly ApplicationDbContext _context;

        // Queue for managing contact messages (FIFO - oldest first)
        public Queue<Contact> ContactQueue { get; } = new Queue<Contact>();  // max size 20

        // Dictionary for storing contacts by category
        public Dictionary<string, List<Contact>> ContactsByCategory { get; } = new Dictionary<string, List<Contact>>(); // max size 10

        // HashSet for unique contact categories
        public HashSet<string> UniqueCategories { get; } = new HashSet<string>(); // max size 10

        // SortedDictionary for contacts by creation date (most recent first)
        public SortedDictionary<DateTime, List<Contact>> ContactsByDate { get; } = new SortedDictionary<DateTime, List<Contact>>();

        public ContactService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            
            
            InitializeDataStructures(); // Load existing contacts into data structures
        }

        private void InitializeDataStructures()
        {
            // Load all contacts from database
            var allContacts = _context.Contacts.ToList(); 

            foreach (var contact in allContacts)
            {
                AddContactToDataStructures(contact); // Populate data structures with the contacts 
            }
        }
        /*
         *
         * the below method is used to add a contact to the database
         *
         * it also adds the contact to the relevant data structures for efficient retrieval
         *
         * 
         */
        public void AddContact(Contact contact)
        {
            _context.Contacts.Add(contact); // add 
            _context.SaveChanges(); // save 

            AddContactToDataStructures(contact);
        }
        
        /*
         *
         * the below method is used to add a contact to the relevant data structures
         *
         * it ensures that the data structures are kept up to date with the latest contact information
         *
         *  this information here is captured threough the contact form on the contact us page in the home location 
         */

        private void AddContactToDataStructures(Contact contact)
        {
            // Add to ContactQueue (keep only last 20)
            ContactQueue.Enqueue(contact); // add to queue
            
            if (ContactQueue.Count > 20) // max 20 people 
            {
                ContactQueue.Dequeue(); // remove oldest
            }

            // Add to ContactsByCategory
            if (!string.IsNullOrEmpty(contact.Category))
            {
                if (!ContactsByCategory.ContainsKey(contact.Category))
                {
                    ContactsByCategory[contact.Category] = new List<Contact>(); // initialize list if category doesn't exist
                }
                ContactsByCategory[contact.Category].Add(contact);

                // Add to UniqueCategories
                UniqueCategories.Add(contact.Category);
            }

            // Add to ContactsByDate
            var dateKey = contact.CreatedDate.Date;
            if (!ContactsByDate.ContainsKey(dateKey)) // if date doesn't exist
            {
                ContactsByDate[dateKey] = new List<Contact>(); // initialize list
            }
            ContactsByDate[dateKey].Add(contact); // add contact to date 
        }
        /*
         *
         * the below method is used to get all contacts from the database
         *
         * it orders them by the created date in descending order (most recent first)
         *
         * display all contacts in the admin panel
         * 
         */
        public List<Contact> GetAllContacts()
        {
            return _context.Contacts
                .OrderByDescending(c => c.CreatedDate)
                .ToList(); 
        }
        /*
         * the below method is used to get all unread contacts from the database
         *
         * it orders them by the created date in descending order (most recent first)
         * 
         */ 
        public List<Contact> GetUnreadContacts()
        {
            return _context.Contacts
                .Where(c => !c.IsRead)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();
        }
        /*
         * the below method is used to get contacts by category from the database
         *
         * it orders them by the created date in descending order 
         * 
         */
        public List<Contact> GetContactsByCategory(string category)
        {
            if (ContactsByCategory.ContainsKey(category))
            {
                return ContactsByCategory[category]
                    .OrderByDescending(c => c.CreatedDate)
                    .ToList();
            }
            return new List<Contact>();
        }

        public List<Contact> GetRecentContacts(int count = 5) // default to 5 
        {
            return ContactQueue.Reverse().Take(count).ToList(); // most recent first
        }

        public Contact GetContactById(int id)
        {
            return _context.Contacts.FirstOrDefault(c => c.Id == id); // get by id
        }

        public bool MarkAsRead(int id)
        {
            var contact = _context.Contacts.FirstOrDefault(c => c.Id == id); // find contact by id
            if (contact == null)
            {
                return false;
            }

            contact.IsRead = true;
            _context.SaveChanges(); // save 

            return true;
        }

        public bool MarkAsResponded(int id)
        {
            var contact = _context.Contacts.FirstOrDefault(c => c.Id == id); // find contact by id
            if (contact == null)
            {
                return false;
            }

            contact.IsResponded = true;
            _context.SaveChanges(); // save 

            return true;
        }
        /*
         *
         * the below method is used to delete a contact from the database
         *
         * it also removes the contact from the relevant data structures
         *
         * ensures that the data structures are kept up to date after a deletion
         *
         *
         *
         * 
         */
        public bool DeleteContact(int id)
        {
            var contact = _context.Contacts.FirstOrDefault(c => c.Id == id);
            if (contact == null)
            {
                return false;
            }

            // Store old values for cleanup
            var oldCategory = contact.Category;

            // Remove from database
            _context.Contacts.Remove(contact);
            _context.SaveChanges(); // save 

            // Update data structures
            RemoveContactFromDataStructures(contact, oldCategory);

            return true; // deletion successful
        }

        private void RemoveContactFromDataStructures(Contact contact, string oldCategory)
        {
            // Remove from ContactsByCategory
            if (!string.IsNullOrEmpty(oldCategory) && ContactsByCategory.ContainsKey(oldCategory)) // if category exists
            {
                ContactsByCategory[oldCategory].RemoveAll(c => c.Id == contact.Id);
                if (ContactsByCategory[oldCategory].Count == 0)
                {
                    ContactsByCategory.Remove(oldCategory); // remove category if empty
                    UniqueCategories.Remove(oldCategory); // also remove from unique categories
                }
            }

            // Remove from ContactQueue
            var queueList = ContactQueue.Where(c => c.Id != contact.Id).ToList();
            ContactQueue.Clear();
            foreach (var c in queueList)
            {
                ContactQueue.Enqueue(c);
            }

            // Remove from ContactsByDate
            var dateKey = contact.CreatedDate.Date;
            if (ContactsByDate.ContainsKey(dateKey)) // if date exists
            {
                ContactsByDate[dateKey].RemoveAll(c => c.Id == contact.Id);
                if (ContactsByDate[dateKey].Count == 0) // if no contacts left for that date
                {
                    ContactsByDate.Remove(dateKey); // remove date if empty
                }
            }
        }
    }
}

//*************************************************** end of file **************************************************//