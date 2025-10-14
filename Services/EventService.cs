
//************************************************* start of file **************************************************//


//----------------------- start of imports -------------------------------//

using Programming_7312_Part_1.Data;
using Programming_7312_Part_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
 
 //------------------------ end of im[ports --------------------------// 
namespace Programming_7312_Part_1.Services
{
    public class EventService
    {
        /*
         *
         * the below is the database context for the event service
         *
         * it is used to interact with the database 
         *
         * there is extensive use of advanced data structures vto meet poe requirements even though the bulk of this can just be done with arrays and lists
         *
         * 
         */
        private readonly ApplicationDbContext _context;

        // Sorted dictionary for organizing events by date
        public SortedDictionary<DateTime, HashSet<Event>> EventsByDate { get; } = new SortedDictionary<DateTime, HashSet<Event>>();

        // Dictionary for organizing events by category
        public Dictionary<string, LinkedList<Event>> EventsByCategory { get; } = new Dictionary<string, LinkedList<Event>>();

        // HashSet for unique categories
        public HashSet<string> UniqueCategories { get; } = new HashSet<string>();

        // HashSet for unique tags
        public HashSet<string> UniqueTags { get; } = new HashSet<string>();

        // Queue for recently added events (FIFO)
        public Queue<Event> RecentEvents { get; } = new Queue<Event>();

        // Stack for featured events (LIFO)
        public Stack<Event> FeaturedEvents { get; } = new Stack<Event>();

        // Priority queue for upcoming events (prioritized by date)
        public SortedDictionary<DateTime, HashSet<Event>> UpcomingEvents { get; } = new SortedDictionary<DateTime, HashSet<Event>>();

        // Dictionary for user search history
        public Dictionary<string, int> SearchHistory { get; } = new Dictionary<string, int>();

        public EventService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); // ensure context is not null
            
            InitializeDataStructures(); // load events 
        }
        /*
         *
         * the below method is used to initialize the data structures used in the event service
         *
         *
         * 
         */
        private void InitializeDataStructures()
        {
            // Load all events from database
            var allEvents = _context.Events.ToList();

            foreach (var eventItem in allEvents)
            {
                AddEventToDataStructures(eventItem); // populate data structures
            }

            // Populate UniqueTags from existing events
            foreach (var eventItem in allEvents)
            {
                if (eventItem.Tags != null)
                {
                    foreach (var tag in eventItem.Tags)
                    {
                        UniqueTags.Add(tag.ToLower()); // set to lover case to standardize the input 
                    }
                }
            }

            // If no events exist, seed sample events
            // the admin caan also add new events in the admin panel 
            // this is just a baackup incase someone cant get the system to work 
            if (!allEvents.Any())
            {
                SeedSampleEvents(); // seed sample events which have been hard coded into the sysem . 
            }
        }
        /*
         *
         * the below method is used to seed sample events into the database
         * 
         *
         * this is useful for testing and demonstration purposes
         *
         * it adds a few sample events to the database and populates the data structures 
         *
         * these are the events which will show up o the local events by default
         *
         * an admin member can add new events in the admin panel and delete them
         *
         * the images are kept in the images folder in wwwroot for the default 
         *
         *  when a admin adds an event the image is stored in the uploads folder in wwwroot 
         */
        private void SeedSampleEvents()
        {
            var sampleEvents = new List<Event>
            {
                new Event
                {
                    Title = "Pothole Patch-Up",
                    Description = "This community event is structured to putting an end to those pesky potholes.Join us to make our environment safer.",
                    Category = "Environment",
                    EventDate = DateTime.Now.AddDays(7), // event is 7 days  from now
                    Location = "Claremont",
                    Tags = new List<string> { "environment", "community", "volunteer" },
                    ImagePath = "/Images/pothole.jpg"
                },
                new Event
                {
                    Title = "Wild Life Conservation",
                    Description = "Enjoy the wildlife local to the Tokai mountains. Bring trail snacks and water",
                    Category = "Entertainment",
                    EventDate = DateTime.Now.AddDays(14), // event is 14 days from now
                    Location = "Tokai",
                    Tags = new List<string> { "outdoor", "family","environment" },
                    ImagePath = "/Images/newlands.jpg"
                },
                new Event
                {
                    Title = "Crime Talk",
                    Description = "Learn about common crimes and prevention",
                    Category = "Education",
                    EventDate = DateTime.Now.AddDays(21), // event is 21 days from now
                    Location = "Newlands Cricket Ground",
                    Tags = new List<string> {  "education", "innovation", "workshop" },
                    ImagePath = "/Images/crime.jpg"
                },
                new Event
                {
                    Title = "Health Awareness Event",
                    Description = "Join fellow residents for a Hike through the Newlands forrest",
                    Category = "Health",
                    EventDate = DateTime.Now.AddDays(10), // event is 10 days from now
                    Location = "Newlands Forrest",
                    Tags = new List<string> { "health", "wellness", "free", "community" }
                },
                new Event
                {
                    Title = "Youth Sports ",
                    Description = "Come and support the next generation of athletes",
                    Category = "Sports",
                    EventDate = DateTime.Now.AddDays(30), // event is 30 days from now
                    Location = "Newlands",
                    Tags = new List<string> { "sports", "youth", "tournament", "outdoor" },
                    ImagePath = "/Images/comunity.jpg"
                }
            };

            foreach (var eventItem in sampleEvents)
            {
                _context.Events.Add(eventItem); // add to database
            }
            _context.SaveChanges(); // saved to the database

            
                    foreach (var eventItem in sampleEvents)
                    {
                        AddEventToDataStructures(eventItem); // populate data structures
                    }
        }

        public void AddEvent(Event eventItem)
        {
            _context.Events.Add(eventItem); // add to database
            
            _context.SaveChanges(); // save to database

            AddEventToDataStructures(eventItem); // add event item to data structures
        }

        private void AddEventToDataStructures(Event eventItem)
        {
            // Add to EventsByDate
            var dateKey = eventItem.EventDate.Date;
            
            
            if (!EventsByDate.ContainsKey(dateKey)) // check if date exists
            {
                EventsByDate[dateKey] = new HashSet<Event>(); // initialize if date doesn't exist
            }
            EventsByDate[dateKey].Add(eventItem);

            // Add to EventsByCategory
            if (!EventsByCategory.ContainsKey(eventItem.Category))
            {
                EventsByCategory[eventItem.Category] = new LinkedList<Event>(); // initialize if category doesn't exist
            }
            EventsByCategory[eventItem.Category].AddLast(eventItem); // add to end of linked list

            // Add to UniqueCategories
            UniqueCategories.Add(eventItem.Category);

            // Add to UniqueTags
            if (eventItem.Tags != null)
            {
                foreach (var tag in eventItem.Tags)
                {
                    UniqueTags.Add(tag.ToLower()); // llowercase 
                }
            }

            // Add to RecentEvents (keep only last 5)
            RecentEvents.Enqueue(eventItem);
            if (RecentEvents.Count > 5)
            {
                RecentEvents.Dequeue(); // remove the old  ones 
            }

            // Add to FeaturedEvents 
            FeaturedEvents.Push(eventItem);
            if (FeaturedEvents.Count > 3) // max 3 featured events
            {
                var list = FeaturedEvents.ToList();
                list.RemoveAt(list.Count - 1); // remove bottom of stack
                FeaturedEvents.Clear();
                foreach(var item in list.AsEnumerable().Reverse()) // reverse to maintain order
                {
                    FeaturedEvents.Push(item); 
                }
            }

            // Add to UpcomingEvents
            if (!UpcomingEvents.ContainsKey(eventItem.EventDate))
            {
                UpcomingEvents[eventItem.EventDate] = new HashSet<Event>(); // initialize if date doesn't exist
            }
            UpcomingEvents[eventItem.EventDate].Add(eventItem); // add to set
        }
        /*
         *
         *
         *
         * the below method is used to get all events from the database
         *
         * they are ordered by the event date in growing order 
         *
         * 
         */
        public LinkedList<Event> GetAllEvents()
        {
            return new LinkedList<Event>(_context.Events.OrderBy(e => e.EventDate)); // 
        }
        /*
         *
         *  the below method is used to get events by category from the database
         */
        public List<Event> GetEventsByCategory(string category)
        {
            if (EventsByCategory.ContainsKey(category))
            {
                return EventsByCategory[category].OrderBy(e => e.EventDate).ToList();
            }
            return new List<Event>(); // return empty list if category not found
        }
        /*
         * the beloow method is used to get events by date range from the database
         * 
         */
        public LinkedList<Event> GetEventsByDateRange(DateTime startDate, DateTime endDate)
        {
            var result = new LinkedList<Event>();

            foreach (var dateKey in EventsByDate.Keys)
            {
                if (dateKey >= startDate.Date && dateKey <= endDate.Date) // check if date is within range
                {
                    foreach (var eventItem in EventsByDate[dateKey])
                    {
                        result.AddLast(eventItem); // add to result
                    }
                }
            }

            return new LinkedList<Event>(result.OrderBy(e => e.EventDate)); // order by event date
        }

        public LinkedList<Event> GetUpcomingEvents(int count = 5)
        {
            return new LinkedList<Event>(UpcomingEvents
                .Where(kv => kv.Key >= DateTime.Now)
                .SelectMany(kv => kv.Value)
                .OrderBy(e => e.EventDate)
                .Take(count));
        }

        public LinkedList<Event> GetRecentEvents(int count = 3) // default to 3
        {
            return new LinkedList<Event>(RecentEvents.Reverse().Take(count));
        }

        public LinkedList<Event> GetFeaturedEvents(int count = 3) // default to 3 
        {
            return new LinkedList<Event>(FeaturedEvents.Take(count)); // most recent featured events
        }

        public void RecordSearch(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return;

            searchTerm = searchTerm.ToLower().Trim(); // standardize input trim the trailing whitespaces 

            if (SearchHistory.ContainsKey(searchTerm))
            {
                SearchHistory[searchTerm]++; // increment count  ->> ++ 
            }
            else
            {
                SearchHistory[searchTerm] = 1; // initialize count
            }
        }

        public LinkedList<Event> SearchEvents(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) // check for empty or whitespace
            
                return GetAllEvents(); // return all events if search term is empty

            RecordSearch(searchTerm); // record the search term into the system 

            searchTerm = searchTerm.ToLower().Trim(); // lower case and remove white spaces ffor trailing and leading 

            var matchingEvents = _context.Events
                .AsEnumerable()
                .Where(e => e.Title.ToLower().Contains(searchTerm))
                .ToList(); // search in title

            // Increment SearchCount for matching events
            foreach (var eventItem in matchingEvents)
            {
                eventItem.SearchCount++; // increment search count
            }
            _context.SaveChanges(); // save 

            return new LinkedList<Event>(matchingEvents.OrderBy(e => e.EventDate));
        }

        public bool UpdateEvent(Event updatedEvent)
        {
            var existingEvent = _context.Events.FirstOrDefault(e => e.Id == updatedEvent.Id); // find existing event by id
            if (existingEvent == null)
            {
                return false;
            }

            // Store old values for cleanup
            var oldEventDate = existingEvent.EventDate;
            
            
            var oldCategory = existingEvent.Category;

            // Update properties
            // fields shall be updated / overwriteen with the new information 
            existingEvent.Title = updatedEvent.Title;
            
            existingEvent.Description = updatedEvent.Description;
            
            existingEvent.Category = updatedEvent.Category;
            
            existingEvent.EventDate = updatedEvent.EventDate;
            
            existingEvent.Location = updatedEvent.Location;
            
            existingEvent.ImagePath = updatedEvent.ImagePath;
            
            existingEvent.Tags = updatedEvent.Tags ?? new List<string>();

            _context.SaveChanges();

            // Update data structures
            UpdateEventInDataStructures(existingEvent, oldEventDate, oldCategory);

            return true;
        }

        private void UpdateEventInDataStructures(Event updatedEvent, DateTime oldEventDate, string oldCategory)
        {
            // If category changed, update EventsByCategory
            if (oldCategory != updatedEvent.Category)
            {
                // Remove from old category
                if (EventsByCategory.ContainsKey(oldCategory))
                {
                    var node = EventsByCategory[oldCategory].First; 
                    while (node != null)
                    {
                        var next = node.Next;
                        if (node.Value.Id == updatedEvent.Id)
                        {
                            EventsByCategory[oldCategory].Remove(node);
                            break;
                        }
                        node = next; // move to next node   
                    }
                    if (EventsByCategory[oldCategory].Count == 0)
                    {
                        EventsByCategory.Remove(oldCategory);  // remove category if empty
                        
                        UniqueCategories.Remove(oldCategory);
                    }
                }

                // Add to new category
                if (!EventsByCategory.ContainsKey(updatedEvent.Category))
                {
                    EventsByCategory[updatedEvent.Category] = new LinkedList<Event>(); // update if category doesn't exist
                }
                EventsByCategory[updatedEvent.Category].AddLast(updatedEvent);

                // Update UniqueCategories
                UniqueCategories.Add(updatedEvent.Category);

                // Update UniqueTags
                if (updatedEvent.Tags != null)
                {
                    foreach (var tag in updatedEvent.Tags)
                    {
                        UniqueTags.Add(tag.ToLower());
                    }
                }
            }

            // If date changed, update EventsByDate
            var newDateKey = updatedEvent.EventDate.Date;
            var oldDateKey = oldEventDate.Date;
            if (oldDateKey != newDateKey)
            {
                // Remove from old date
                if (EventsByDate.ContainsKey(oldDateKey))
                {
                    EventsByDate[oldDateKey].RemoveWhere(e => e.Id == updatedEvent.Id);
                    if (EventsByDate[oldDateKey].Count == 0)
                    {
                        EventsByDate.Remove(oldDateKey);  // remove old date 
                    }
                }

                // Add to new date
                if (!EventsByDate.ContainsKey(newDateKey))
                {
                    EventsByDate[newDateKey] = new HashSet<Event>();
                }
                EventsByDate[newDateKey].Add(updatedEvent);
            }

            // Update UpcomingEvents
            if (UpcomingEvents.ContainsKey(oldEventDate))
            {
                UpcomingEvents[oldEventDate].RemoveWhere(e => e.Id == updatedEvent.Id);
                if (UpcomingEvents[oldEventDate].Count == 0)
                {
                    UpcomingEvents.Remove(oldEventDate);
                }
            }
            if (!UpcomingEvents.ContainsKey(updatedEvent.EventDate)) 
            {
                UpcomingEvents[updatedEvent.EventDate] = new HashSet<Event>();
            }
            UpcomingEvents[updatedEvent.EventDate].Add(updatedEvent);
        }

        public Event GetEventById(int id)
        {
            return _context.Events.FirstOrDefault(e => e.Id == id); // retrive by id 
        }
        /*
         *
         * the below is for the upvote 
         *
         * 
         */

        public bool UpvoteEvent(int eventId)
        {
            var eventItem = GetEventById(eventId);
            if (eventItem != null)
            {
                eventItem.Upvotes++; // increment 
                _context.SaveChanges(); // save and update the database 
                
                return true;
            }
            return false;
        }
        /*
         * the below is for the event regarding downvoting 
         *
         * 
         */ 
        public bool DownvoteEvent(int eventId) // using event id which is unique 
        {
            var eventItem = GetEventById(eventId);
            if (eventItem != null)
            {
                eventItem.Downvotes++; // increment 
                _context.SaveChanges(); // update and save databasse 
                return true;
            }
            return false;
        }

        public LinkedList<Event> GetRecommendedEvents(int count = 3)
        {
            // Recommendation based on upvotes and search history
            var recommendedEvents = new LinkedList<Event>();

            // First, prioritize events with high upvotes
            var highVotedEvents = _context.Events
                .Where(e => e.Upvotes > 0)
                .OrderByDescending(e => e.Upvotes - e.Downvotes) // Net positive votes
                .ThenBy(e => e.EventDate)
                .Take(count)
                .ToList();

            foreach (var eventItem in highVotedEvents)
            {
                recommendedEvents.AddLast(eventItem); // event item added to recomended list in the last position 
            }

            // If we need more, use search history
            if (recommendedEvents.Count < count && SearchHistory.Count > 0)
            {
                var topSearches = SearchHistory
                    .OrderByDescending(kv => kv.Value)
                    .Take(3)
                    .Select(kv => kv.Key)
                    .ToList();

                var usedEventIds = new HashSet<int>(recommendedEvents.Select(e => e.Id));

                foreach (var search in topSearches)
                {
                    var events = _context.Events
                        .AsEnumerable()
                        .Where(e => !usedEventIds.Contains(e.Id) && (
                            e.Title.ToLower().Contains(search) ||
                            e.Description.ToLower().Contains(search) ||
                            e.Category.ToLower().Contains(search) ||
                            e.Location.ToLower().Contains(search) ||
                            (e.Tags != null && e.Tags.IndexOf(search.ToLower()) >= 0)))
                        .OrderByDescending(e => e.Upvotes - e.Downvotes)
                        .ThenBy(e => e.EventDate)
                        .Take(2) // Take up to 2 events per search term
                        .ToList();

                    foreach (var eventItem in events)
                    {
                        recommendedEvents.AddLast(eventItem); // move to last 
                        usedEventIds.Add(eventItem.Id);

                        if (recommendedEvents.Count >= count)
                            break;
                    }

                    if (recommendedEvents.Count >= count)
                        break;
                }
            }

            // If still not enough, fill with upcoming events
            if (recommendedEvents.Count < count)
            {
                var usedEventIds = new HashSet<int>(recommendedEvents.Select(e => e.Id));
                var upcoming = GetUpcomingEvents(count - recommendedEvents.Count)
                    .Where(e => !usedEventIds.Contains(e.Id))
                    .ToList();

                foreach (var eventItem in upcoming)
                {
                    recommendedEvents.AddLast(eventItem); // move event to last position 
                }
            }

            return new LinkedList<Event>(recommendedEvents.Take(count));
        }

        public LinkedList<Event> GetUpcomingEventsByCategory(string category, int count = 5)
        {
            return new LinkedList<Event>(UpcomingEvents
                .Where(kv => kv.Key >= DateTime.Now)
                .SelectMany(kv => kv.Value)
                .Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .Take(count));
        }

        public LinkedList<Event> GetFeaturedEventsByCategory(string category, int count = 3)
        {
            return new LinkedList<Event>(FeaturedEvents
                .Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .Take(count));
        }

        public LinkedList<Event> GetRecommendedEventsByCategory(string category, int count = 3)
        {
            if (string.IsNullOrEmpty(category))
            {
                return new LinkedList<Event>(); // new event 
            }

            // Recommendation based on upvotes and search history, filtered by category
            var recommendedEvents = new LinkedList<Event>();

            // First, prioritize events with high upvotes in this category
            var highVotedEvents = _context.Events
                .Where(e => e.Category.ToLower() == category.ToLower() && e.Upvotes > 0)
                .OrderByDescending(e => e.Upvotes - e.Downvotes) // Net positive votes
                .ThenBy(e => e.EventDate)
                .Take(count)
                .ToList();

            foreach (var eventItem in highVotedEvents)
            {
                recommendedEvents.AddLast(eventItem); 
            }

            // If we need more, use search history
            if (recommendedEvents.Count < count && SearchHistory.Count > 0)
            {
                var topSearches = SearchHistory
                    .OrderByDescending(kv => kv.Value)
                    .Take(3)
                    .Select(kv => kv.Key)
                    .ToList();
            /*
             * below is a use of a hash set 
             * 
             */ 
                var usedEventIds = new HashSet<int>(recommendedEvents.Select(e => e.Id));

                foreach (var search in topSearches) // going through the top searches through a loop
                {
                    var events = _context.Events
                        .AsEnumerable()
                        .Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase) &&
                                    !usedEventIds.Contains(e.Id) && (
                            e.Title.ToLower().Contains(search) ||
                            e.Description.ToLower().Contains(search) ||
                            e.Category.ToLower().Contains(search) ||
                            e.Location.ToLower().Contains(search) ||
                            (e.Tags != null && e.Tags.IndexOf(search.ToLower()) >= 0)))
                        .OrderByDescending(e => e.Upvotes - e.Downvotes)
                        .ThenBy(e => e.EventDate)
                        .Take(2) // Take up to 2 events per search term
                        .ToList();

                    foreach (var eventItem in events)
                    {
                        recommendedEvents.AddLast(eventItem);
                        usedEventIds.Add(eventItem.Id);

                        if (recommendedEvents.Count >= count)
                            break;
                    }

                    if (recommendedEvents.Count >= count)
                        break;
                }
            }

            // If not enough, fill with upcoming events in this category 
            
            // the limit is  5 
            if (recommendedEvents.Count < count)
            {
                var usedEventIds = new HashSet<int>(recommendedEvents.Select(e => e.Id));
                var upcoming = GetUpcomingEventsByCategory(category, count - recommendedEvents.Count)
                    .Where(e => !usedEventIds.Contains(e.Id))
                    .ToList();

                foreach (var eventItem in upcoming)
                {
                    recommendedEvents.AddLast(eventItem);
                }
            }

            return new LinkedList<Event>(recommendedEvents.Take(count));
        }

        public LinkedList<Event> GetPopularEvents(int count = 10)
        {
            return new LinkedList<Event>(_context.Events
                .OrderByDescending(e => e.SearchCount)
                .ThenByDescending(e => e.Upvotes)
                .Take(count));
        }
        /*
         * the below method is to delete an event
         *
         * this is done by removing the id and clearingthe data assosciated with the id 
         *
         *  the system shall then update with the removed id and thus the event shall be removed 
         */
        public bool DeleteEvent(int id)
        {
            var eventItem = _context.Events.FirstOrDefault(e => e.Id == id);
            if (eventItem == null)
            {
                return false;
            }

            // Store old values for cleanup
            var oldEventDate = eventItem.EventDate;
            var oldCategory = eventItem.Category;

            // Remove from database
            _context.Events.Remove(eventItem);
            _context.SaveChanges();

            // Update data structures
            RemoveEventFromDataStructures(eventItem, oldEventDate, oldCategory);

            return true;
        }
        /*
         *below is the action to remove the event from the ddata structures
         *
         * this is to ensure that even though it has been deleted from the database, it does not keep running in memory and actually updates the system
         *
         * this works by removing the date aand event key and thus invalidating it 
         * 
         */
        private void RemoveEventFromDataStructures(Event eventItem, DateTime oldEventDate, string oldCategory)
        {
            // Remove from EventsByDate
            var oldDateKey = oldEventDate.Date;
            if (EventsByDate.ContainsKey(oldDateKey))
            {
                EventsByDate[oldDateKey].RemoveWhere(e => e.Id == eventItem.Id);
                if (EventsByDate[oldDateKey].Count == 0)
                {
                    EventsByDate.Remove(oldDateKey);
                }
            }

            // Remove from EventsByCategory
            if (EventsByCategory.ContainsKey(oldCategory))
            {
                var node = EventsByCategory[oldCategory].First;
                while (node != null)
                {
                    var next = node.Next;
                    if (node.Value.Id == eventItem.Id)
                    {
                        EventsByCategory[oldCategory].Remove(node);
                        break;
                    }
                    node = next;
                }
                if (EventsByCategory[oldCategory].Count == 0)
                {
                    EventsByCategory.Remove(oldCategory);
                    UniqueCategories.Remove(oldCategory);
                }
            }

            // Remove from RecentEvents (Queue)
            var recentList = RecentEvents.Where(e => e.Id != eventItem.Id).ToList();
            RecentEvents.Clear();
            foreach (var e in recentList)
            {
                RecentEvents.Enqueue(e); // remove from data structure 
            }

            // Remove from FeaturedEvents (Stack)
            var featuredList = FeaturedEvents.Where(e => e.Id != eventItem.Id).ToList();
            FeaturedEvents.Clear();
            foreach (var e in featuredList.AsEnumerable().Reverse())
            {
                FeaturedEvents.Push(e);
            }

            // Remove from UpcomingEvents
            if (UpcomingEvents.ContainsKey(oldEventDate))
            {
                UpcomingEvents[oldEventDate].RemoveWhere(e => e.Id == eventItem.Id);
                if (UpcomingEvents[oldEventDate].Count == 0)
                {
                    UpcomingEvents.Remove(oldEventDate);
                }
            }
        }
    }
}
 //************************************************* end of file **************************************************//