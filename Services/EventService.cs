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
                    Title = "Community Clean-Up Day",
                    Description = "Join us for a community-wide clean-up initiative to keep our neighborhoods beautiful.",
                    Category = "Environment",
                    EventDate = DateTime.Now.AddDays(7),
                    Location = "Central Park",
                    Tags = new List<string> { "environment", "community", "outdoor", "volunteer" }
                },
                new Event
                {
                    Id = 2,
                    Title = "Summer Music Festival",
                    Description = "Enjoy live performances from local artists at our annual summer music festival.",
                    Category = "Entertainment",
                    EventDate = DateTime.Now.AddDays(14),
                    Location = "City Amphitheater",
                    Tags = new List<string> { "music", "festival", "outdoor", "family" }
                },
                new Event
                {
                    Id = 3,
                    Title = "Tech Innovation Workshop",
                    Description = "Learn about the latest technological innovations and how they can improve municipal services.",
                    Category = "Education",
                    EventDate = DateTime.Now.AddDays(21),
                    Location = "Community Center",
                    Tags = new List<string> { "technology", "education", "innovation", "workshop" }
                },
                new Event
                {
                    Id = 4,
                    Title = "Health Awareness Campaign",
                    Description = "Free health screenings and wellness information for all residents.",
                    Category = "Health",
                    EventDate = DateTime.Now.AddDays(10),
                    Location = "City Hall",
                    Tags = new List<string> { "health", "wellness", "free", "community" }
                },
                new Event
                {
                    Id = 5,
                    Title = "Youth Sports Tournament",
                    Description = "Annual tournament for youth teams in various sports.",
                    Category = "Sports",
                    EventDate = DateTime.Now.AddDays(30),
                    Location = "Sports Complex",
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

        public List<Event> GetRecommendedEvents(int count = 3)
        {
            // Simple recommendation based on most searched terms
            if (SearchHistory.Count == 0)
            {
                // If no search history, return upcoming events
                return GetUpcomingEvents(count);
            }

            var topSearches = SearchHistory
                .OrderByDescending(kv => kv.Value)
                .Take(3)
                .Select(kv => kv.Key)
                .ToList();

            var recommendedEvents = new List<Event>();
            var usedEventIds = new HashSet<int>();

            foreach (var search in topSearches)
            {
                var events = _allEvents
                    .Where(e => !usedEventIds.Contains(e.Id) && (
                        e.Title.ToLower().Contains(search) ||
                        e.Description.ToLower().Contains(search) ||
                        e.Category.ToLower().Contains(search) ||
                        e.Location.ToLower().Contains(search) ||
                        e.Tags.Any(tag => tag.ToLower().Contains(search))))
                    .OrderBy(e => e.EventDate)
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

            // If we don't have enough recommendations, fill with upcoming events
            if (recommendedEvents.Count < count)
            {
                var upcoming = GetUpcomingEvents(count - recommendedEvents.Count)
                    .Where(e => !usedEventIds.Contains(e.Id))
                    .ToList();

                recommendedEvents.AddRange(upcoming);
            }

            return recommendedEvents.Take(count).ToList();
        }
    }
}


