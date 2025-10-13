// Services/EventService.cs
using Programming_7312_Part_1.Data;
using Programming_7312_Part_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Programming_7312_Part_1.Services
{
    public class EventService
    {
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
            _context = context ?? throw new ArgumentNullException(nameof(context));
            InitializeDataStructures();
        }

        private void InitializeDataStructures()
        {
            // Load all events from database
            var allEvents = _context.Events.ToList();

            foreach (var eventItem in allEvents)
            {
                AddEventToDataStructures(eventItem);
            }

            // Populate UniqueTags from existing events
            foreach (var eventItem in allEvents)
            {
                if (eventItem.Tags != null)
                {
                    foreach (var tag in eventItem.Tags)
                    {
                        UniqueTags.Add(tag.ToLower());
                    }
                }
            }

            // If no events exist, seed sample events
            if (!allEvents.Any())
            {
                SeedSampleEvents();
            }
        }

        private void SeedSampleEvents()
        {
            var sampleEvents = new List<Event>
            {
                new Event
                {
                    Title = "Pothole Patch-Up",
                    Description = "This community event is structured to putting an end to those pesky potholes.Join us to make our environment safer.",
                    Category = "Environment",
                    EventDate = DateTime.Now.AddDays(7),
                    Location = "Claremont",
                    Tags = new List<string> { "environment", "community", "volunteer" },
                    ImagePath = "/Images/pothole.jpg"
                },
                new Event
                {
                    Title = "Wild Life Conservation",
                    Description = "Enjoy the wildlife local to the Tokai mountains. Bring trail snacks and water",
                    Category = "Entertainment",
                    EventDate = DateTime.Now.AddDays(14),
                    Location = "Tokai",
                    Tags = new List<string> { "outdoor", "family","environment" },
                    ImagePath = "/Images/newlands.jpg"
                },
                new Event
                {
                    Title = "Crime Talk",
                    Description = "Learn about common crimes and prevention",
                    Category = "Education",
                    EventDate = DateTime.Now.AddDays(21),
                    Location = "Newlands Cricket Ground",
                    Tags = new List<string> {  "education", "innovation", "workshop" },
                    ImagePath = "/Images/crime.jpg"
                },
                new Event
                {
                    Title = "Health Awareness Event",
                    Description = "Join fellow residents for a Hike through the Newlands forrest",
                    Category = "Health",
                    EventDate = DateTime.Now.AddDays(10),
                    Location = "Newlands Forrest",
                    Tags = new List<string> { "health", "wellness", "free", "community" }
                },
                new Event
                {
                    Title = "Youth Sports ",
                    Description = "Come and support the next generation of athletes",
                    Category = "Sports",
                    EventDate = DateTime.Now.AddDays(30),
                    Location = "Newlands",
                    Tags = new List<string> { "sports", "youth", "tournament", "outdoor" },
                    ImagePath = "/Images/comunity.jpg"
                }
            };

            foreach (var eventItem in sampleEvents)
            {
                _context.Events.Add(eventItem);
            }
            _context.SaveChanges();

            // Now add to data structures
            foreach (var eventItem in sampleEvents)
            {
                AddEventToDataStructures(eventItem);
            }
        }

        public void AddEvent(Event eventItem)
        {
            _context.Events.Add(eventItem);
            _context.SaveChanges();

            AddEventToDataStructures(eventItem);
        }

        private void AddEventToDataStructures(Event eventItem)
        {
            // Add to EventsByDate
            var dateKey = eventItem.EventDate.Date;
            if (!EventsByDate.ContainsKey(dateKey))
            {
                EventsByDate[dateKey] = new HashSet<Event>();
            }
            EventsByDate[dateKey].Add(eventItem);

            // Add to EventsByCategory
            if (!EventsByCategory.ContainsKey(eventItem.Category))
            {
                EventsByCategory[eventItem.Category] = new LinkedList<Event>();
            }
            EventsByCategory[eventItem.Category].AddLast(eventItem);

            // Add to UniqueCategories
            UniqueCategories.Add(eventItem.Category);

            // Add to UniqueTags
            if (eventItem.Tags != null)
            {
                foreach (var tag in eventItem.Tags)
                {
                    UniqueTags.Add(tag.ToLower());
                }
            }

            // Add to RecentEvents (keep only last 5)
            RecentEvents.Enqueue(eventItem);
            if (RecentEvents.Count > 5)
            {
                RecentEvents.Dequeue();
            }

            // Add to FeaturedEvents (keep only 3)
            FeaturedEvents.Push(eventItem);
            if (FeaturedEvents.Count > 3)
            {
                var list = FeaturedEvents.ToList();
                list.RemoveAt(list.Count - 1); // remove bottom of stack
                FeaturedEvents.Clear();
                foreach(var item in list.AsEnumerable().Reverse())
                {
                    FeaturedEvents.Push(item);
                }
            }

            // Add to UpcomingEvents
            if (!UpcomingEvents.ContainsKey(eventItem.EventDate))
            {
                UpcomingEvents[eventItem.EventDate] = new HashSet<Event>();
            }
            UpcomingEvents[eventItem.EventDate].Add(eventItem);
        }

        public LinkedList<Event> GetAllEvents()
        {
            return new LinkedList<Event>(_context.Events.OrderBy(e => e.EventDate));
        }

        public List<Event> GetEventsByCategory(string category)
        {
            if (EventsByCategory.ContainsKey(category))
            {
                return EventsByCategory[category].OrderBy(e => e.EventDate).ToList();
            }
            return new List<Event>();
        }

        public LinkedList<Event> GetEventsByDateRange(DateTime startDate, DateTime endDate)
        {
            var result = new LinkedList<Event>();

            foreach (var dateKey in EventsByDate.Keys)
            {
                if (dateKey >= startDate.Date && dateKey <= endDate.Date)
                {
                    foreach (var eventItem in EventsByDate[dateKey])
                    {
                        result.AddLast(eventItem);
                    }
                }
            }

            return new LinkedList<Event>(result.OrderBy(e => e.EventDate));
        }

        public LinkedList<Event> GetUpcomingEvents(int count = 5)
        {
            return new LinkedList<Event>(UpcomingEvents
                .Where(kv => kv.Key >= DateTime.Now)
                .SelectMany(kv => kv.Value)
                .OrderBy(e => e.EventDate)
                .Take(count));
        }

        public LinkedList<Event> GetRecentEvents(int count = 3)
        {
            return new LinkedList<Event>(RecentEvents.Reverse().Take(count));
        }

        public LinkedList<Event> GetFeaturedEvents(int count = 3)
        {
            return new LinkedList<Event>(FeaturedEvents.Take(count));
        }

        public void RecordSearch(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return;

            searchTerm = searchTerm.ToLower().Trim();

            if (SearchHistory.ContainsKey(searchTerm))
            {
                SearchHistory[searchTerm]++;
            }
            else
            {
                SearchHistory[searchTerm] = 1;
            }
        }

        public LinkedList<Event> SearchEvents(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllEvents();

            RecordSearch(searchTerm);

            searchTerm = searchTerm.ToLower().Trim();

            return new LinkedList<Event>(_context.Events
                .AsEnumerable()
                .Where(e => e.Title.ToLower().Contains(searchTerm))
                .OrderBy(e => e.EventDate));
        }

        public bool UpdateEvent(Event updatedEvent)
        {
            var existingEvent = _context.Events.FirstOrDefault(e => e.Id == updatedEvent.Id);
            if (existingEvent == null)
            {
                return false;
            }

            // Store old values for cleanup
            var oldEventDate = existingEvent.EventDate;
            var oldCategory = existingEvent.Category;

            // Update properties
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
                        node = next;
                    }
                    if (EventsByCategory[oldCategory].Count == 0)
                    {
                        EventsByCategory.Remove(oldCategory);
                        UniqueCategories.Remove(oldCategory);
                    }
                }

                // Add to new category
                if (!EventsByCategory.ContainsKey(updatedEvent.Category))
                {
                    EventsByCategory[updatedEvent.Category] = new LinkedList<Event>();
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
                        EventsByDate.Remove(oldDateKey);
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
            return _context.Events.FirstOrDefault(e => e.Id == id);
        }

        public bool UpvoteEvent(int eventId)
        {
            var eventItem = GetEventById(eventId);
            if (eventItem != null)
            {
                eventItem.Upvotes++;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DownvoteEvent(int eventId)
        {
            var eventItem = GetEventById(eventId);
            if (eventItem != null)
            {
                eventItem.Downvotes++;
                _context.SaveChanges();
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
                        recommendedEvents.AddLast(eventItem);
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
                    recommendedEvents.AddLast(eventItem);
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
                return new LinkedList<Event>();
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

                var usedEventIds = new HashSet<int>(recommendedEvents.Select(e => e.Id));

                foreach (var search in topSearches)
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

            // If still not enough, fill with upcoming events in this category
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
                RecentEvents.Enqueue(e);
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
