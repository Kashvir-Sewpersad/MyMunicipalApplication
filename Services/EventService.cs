// Services/EventService.cs
using Programming_7312_Part_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Programming_7312_Part_1.Services
{
    public class EventService
    {
        // Sorted dictionary for organizing events by date
        public SortedDictionary<DateTime, List<Event>> EventsByDate { get; } = new SortedDictionary<DateTime, List<Event>>();

        // Dictionary for organizing events by category
        public Dictionary<string, List<Event>> EventsByCategory { get; } = new Dictionary<string, List<Event>>();

        // HashSet for unique categories
        public HashSet<string> UniqueCategories { get; } = new HashSet<string>();

        // Queue for recently added events (FIFO)
        public Queue<Event> RecentEvents { get; } = new Queue<Event>();

        // Stack for featured events (LIFO)
        public Stack<Event> FeaturedEvents { get; } = new Stack<Event>();

        // Priority queue for upcoming events (prioritized by date)
        public SortedDictionary<DateTime, Event> UpcomingEvents { get; } = new SortedDictionary<DateTime, Event>();

        // Dictionary for user search history
        public Dictionary<string, int> SearchHistory { get; } = new Dictionary<string, int>();

        // List to store all events
        private List<Event> _allEvents = new List<Event>();

        public EventService()
        {
            // Initialize with some sample events
            InitializeSampleEvents();
        }

        private void InitializeSampleEvents()
        {
            var sampleEvents = new List<Event>
            {
                new Event
                {
                    Id = 1,
                    Title = "Pothole Patch-Up",
                    Description = "This community event is structured to putting an end to those pesky potholes.Join us to make our environment safer.",
                    Category = "Environment",
                    EventDate = DateTime.Now.AddDays(7),
                    Location = "Claremont",
                    Tags = new List<string> { "environment", "community", "volunteer" }
                },
                new Event
                {
                    Id = 2,
                    Title = "Wild Life Conservation",
                    Description = "Enjoy the wildlife local to the Tokai mountains. Bring trail snacks and water",
                    Category = "Entertainment",
                    EventDate = DateTime.Now.AddDays(14),
                    Location = "Tokai",
                    Tags = new List<string> { "outdoor", "family","environment" }
                },
                new Event
                {
                    Id = 3,
                    Title = "Crime Talk",
                    Description = "Learn about common crimes and prevention",
                    Category = "Education",
                    EventDate = DateTime.Now.AddDays(21),
                    Location = "Newlands Cricket Ground",
                    Tags = new List<string> {  "education", "innovation", "workshop" }
                },
                new Event
                {
                    Id = 4,
                    Title = "Health Awareness Event",
                    Description = "Join fellow residents for a Hike through the Newlands forrest",
                    Category = "Health",
                    EventDate = DateTime.Now.AddDays(10),
                    Location = "Newlands Forrest",
                    Tags = new List<string> { "health", "wellness", "free", "community" }
                },
                new Event
                {
                    Id = 5,
                    Title = "Youth Sports ",
                    Description = "Come and support the next generation of athletes",
                    Category = "Sports",
                    EventDate = DateTime.Now.AddDays(30),
                    Location = "Newlands",
                    Tags = new List<string> { "sports", "youth", "tournament", "outdoor" }
                }
            };

            foreach (var eventItem in sampleEvents)
            {
                AddEvent(eventItem);
            }
        }

        public void AddEvent(Event eventItem)
        {
            _allEvents.Add(eventItem);

            // Add to EventsByDate
            var dateKey = eventItem.EventDate.Date;
            if (!EventsByDate.ContainsKey(dateKey))
            {
                EventsByDate[dateKey] = new List<Event>();
            }
            EventsByDate[dateKey].Add(eventItem);

            // Add to EventsByCategory
            if (!EventsByCategory.ContainsKey(eventItem.Category))
            {
                EventsByCategory[eventItem.Category] = new List<Event>();
            }
            EventsByCategory[eventItem.Category].Add(eventItem);

            // Add to UniqueCategories
            UniqueCategories.Add(eventItem.Category);

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
                FeaturedEvents.Pop();
            }

            // Add to UpcomingEvents
            UpcomingEvents[eventItem.EventDate] = eventItem;
        }

        public List<Event> GetAllEvents()
        {
            return _allEvents.OrderBy(e => e.EventDate).ToList();
        }

        public List<Event> GetEventsByCategory(string category)
        {
            if (EventsByCategory.ContainsKey(category))
            {
                return EventsByCategory[category].OrderBy(e => e.EventDate).ToList();
            }
            return new List<Event>();
        }

        public List<Event> GetEventsByDateRange(DateTime startDate, DateTime endDate)
        {
            var result = new List<Event>();

            foreach (var dateKey in EventsByDate.Keys)
            {
                if (dateKey >= startDate.Date && dateKey <= endDate.Date)
                {
                    result.AddRange(EventsByDate[dateKey]);
                }
            }

            return result.OrderBy(e => e.EventDate).ToList();
        }

        public List<Event> GetUpcomingEvents(int count = 5)
        {
            return UpcomingEvents
                .Where(kv => kv.Key >= DateTime.Now)
                .Take(count)
                .Select(kv => kv.Value)
                .ToList();
        }

        public List<Event> GetRecentEvents(int count = 3)
        {
            return RecentEvents.Reverse().Take(count).ToList();
        }

        public List<Event> GetFeaturedEvents(int count = 3)
        {
            return FeaturedEvents.Take(count).ToList();
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

        public List<Event> SearchEvents(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllEvents();

            RecordSearch(searchTerm);

            searchTerm = searchTerm.ToLower().Trim();

            return _allEvents
                .Where(e =>
                    e.Title.ToLower().Contains(searchTerm) ||
                    e.Description.ToLower().Contains(searchTerm) ||
                    e.Category.ToLower().Contains(searchTerm) ||
                    e.Location.ToLower().Contains(searchTerm) ||
                    e.Tags.Any(tag => tag.ToLower().Contains(searchTerm)))
                .OrderBy(e => e.EventDate)
                .ToList();
        }

        public bool UpdateEvent(Event updatedEvent)
        {
            var existingEvent = _allEvents.FirstOrDefault(e => e.Id == updatedEvent.Id);
            if (existingEvent == null)
            {
                return false;
            }

            // Store old values for cleanup
            var oldDateKey = existingEvent.EventDate.Date;
            var oldCategory = existingEvent.Category;

            // Update properties
            existingEvent.Title = updatedEvent.Title;
            existingEvent.Description = updatedEvent.Description;
            existingEvent.Category = updatedEvent.Category;
            existingEvent.EventDate = updatedEvent.EventDate;
            existingEvent.Location = updatedEvent.Location;
            existingEvent.ImagePath = updatedEvent.ImagePath;
            existingEvent.Tags = updatedEvent.Tags ?? new List<string>();

            // If category changed, update EventsByCategory
            if (oldCategory != updatedEvent.Category)
            {
                // Remove from old category
                if (EventsByCategory.ContainsKey(oldCategory))
                {
                    EventsByCategory[oldCategory].Remove(existingEvent);
                    if (EventsByCategory[oldCategory].Count == 0)
                    {
                        EventsByCategory.Remove(oldCategory);
                    }
                }

                // Add to new category
                if (!EventsByCategory.ContainsKey(updatedEvent.Category))
                {
                    EventsByCategory[updatedEvent.Category] = new List<Event>();
                }
                EventsByCategory[updatedEvent.Category].Add(existingEvent);

                // Update UniqueCategories
                UniqueCategories.Remove(oldCategory);
                UniqueCategories.Add(updatedEvent.Category);
            }

            // If date changed, update EventsByDate
            var newDateKey = updatedEvent.EventDate.Date;
            if (oldDateKey != newDateKey)
            {
                // Remove from old date
                if (EventsByDate.ContainsKey(oldDateKey))
                {
                    EventsByDate[oldDateKey].Remove(existingEvent);
                    if (EventsByDate[oldDateKey].Count == 0)
                    {
                        EventsByDate.Remove(oldDateKey);
                    }
                }

                // Add to new date
                if (!EventsByDate.ContainsKey(newDateKey))
                {
                    EventsByDate[newDateKey] = new List<Event>();
                }
                EventsByDate[newDateKey].Add(existingEvent);
            }

            // Update UpcomingEvents
            UpcomingEvents.Remove(oldDateKey);
            UpcomingEvents[updatedEvent.EventDate] = existingEvent;

            return true;
        }

        public Event GetEventById(int id)
        {
            return _allEvents.FirstOrDefault(e => e.Id == id);
        }

        public bool UpvoteEvent(int eventId)
        {
            var eventItem = GetEventById(eventId);
            if (eventItem != null)
            {
                eventItem.Upvotes++;
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
                return true;
            }
            return false;
        }

        public List<Event> GetRecommendedEvents(int count = 3)
        {
            // Recommendation based on upvotes and search history
            var recommendedEvents = new List<Event>();

            // First, prioritize events with high upvotes
            var highVotedEvents = _allEvents
                .Where(e => e.Upvotes > 0)
                .OrderByDescending(e => e.Upvotes - e.Downvotes) // Net positive votes
                .ThenBy(e => e.EventDate)
                .Take(count)
                .ToList();

            recommendedEvents.AddRange(highVotedEvents);

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
                    var events = _allEvents
                        .Where(e => !usedEventIds.Contains(e.Id) && (
                            e.Title.ToLower().Contains(search) ||
                            e.Description.ToLower().Contains(search) ||
                            e.Category.ToLower().Contains(search) ||
                            e.Location.ToLower().Contains(search) ||
                            e.Tags.Any(tag => tag.ToLower().Contains(search))))
                        .OrderByDescending(e => e.Upvotes - e.Downvotes)
                        .ThenBy(e => e.EventDate)
                        .Take(2) // Take up to 2 events per search term
                        .ToList();

                    foreach (var eventItem in events)
                    {
                        recommendedEvents.Add(eventItem);
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

                recommendedEvents.AddRange(upcoming);
            }

            return recommendedEvents.Take(count).ToList();
        }

        public List<Event> GetUpcomingEventsByCategory(string category, int count = 5)
        {
            return UpcomingEvents
                .Where(kv => kv.Key >= DateTime.Now && kv.Value.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .Take(count)
                .Select(kv => kv.Value)
                .ToList();
        }

        public List<Event> GetFeaturedEventsByCategory(string category, int count = 3)
        {
            return FeaturedEvents
                .Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .Take(count)
                .ToList();
        }

        public List<Event> GetRecommendedEventsByCategory(string category, int count = 3)
        {
            // Recommendation based on upvotes and search history, filtered by category
            var recommendedEvents = new List<Event>();

            // First, prioritize events with high upvotes in this category
            var highVotedEvents = _allEvents
                .Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase) && e.Upvotes > 0)
                .OrderByDescending(e => e.Upvotes - e.Downvotes) // Net positive votes
                .ThenBy(e => e.EventDate)
                .Take(count)
                .ToList();

            recommendedEvents.AddRange(highVotedEvents);

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
                    var events = _allEvents
                        .Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase) &&
                                   !usedEventIds.Contains(e.Id) && (
                            e.Title.ToLower().Contains(search) ||
                            e.Description.ToLower().Contains(search) ||
                            e.Category.ToLower().Contains(search) ||
                            e.Location.ToLower().Contains(search) ||
                            e.Tags.Any(tag => tag.ToLower().Contains(search))))
                        .OrderByDescending(e => e.Upvotes - e.Downvotes)
                        .ThenBy(e => e.EventDate)
                        .Take(2) // Take up to 2 events per search term
                        .ToList();

                    foreach (var eventItem in events)
                    {
                        recommendedEvents.Add(eventItem);
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

                recommendedEvents.AddRange(upcoming);
            }

            return recommendedEvents.Take(count).ToList();
        }
    }
}


