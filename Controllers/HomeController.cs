// Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using Programming_7312_Part_1.Models;
using Programming_7312_Part_1.Services;
using System.IO;

namespace Programming_7312_Part_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IssueStorage _issueStorage;
        private readonly EventService _eventService;

        public HomeController(IssueStorage issueStorage, EventService eventService)
        {
            _issueStorage = issueStorage ?? throw new ArgumentNullException(nameof(issueStorage));
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }

        // home  
        public IActionResult Index()
        {
            return View();
        }

        // privacy 
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ReportIssues()
        {
            ViewBag.Categories = new[] { "Sanitation", "Roads", "Utilities", "Other" }; // categories 
            return View(new Issue());
        }

        [HttpPost]
        public async Task<IActionResult> ReportIssues(Issue model, IFormFile attachment)
        {
            ViewBag.Categories = new[] { "Sanitation", "Roads", "Utilities", "Other" };

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Handle file upload
            if (attachment != null && attachment.Length > 0)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads"); // stored in upload folder

                // Create directory if it doesn't exist
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                // Generate unique filename to avoid conflicts
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(attachment.FileName);
                var filePath = Path.Combine(uploadsDir, fileName);

                // Save file asynchronously
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await attachment.CopyToAsync(stream);
                }

                model.AttachedFilePath = "/uploads/" + fileName;
            }

            // Store issues
            _issueStorage.AddIssue(model);

            ViewBag.SuccessMessage = "Issue reported successfully!";
            ViewBag.EngagementMessage = "Your reports make our community better!";

            ModelState.Clear();
            return View(new Issue());
        }

        public IActionResult LocalEvents(string category = "", string searchTerm = "")
        {
            ViewBag.Categories = _eventService.UniqueCategories.ToList();

            List<Event> events;

            if (!string.IsNullOrEmpty(category))
            {
                events = _eventService.GetEventsByCategory(category);
                ViewBag.SelectedCategory = category;

                // Filter other sections by category as well
                ViewBag.UpcomingEvents = _eventService.GetUpcomingEventsByCategory(category, 3);
                ViewBag.FeaturedEvents = _eventService.GetFeaturedEventsByCategory(category, 3);
                ViewBag.RecommendedEvents = _eventService.GetRecommendedEventsByCategory(category, 3);
            }
            else if (!string.IsNullOrEmpty(searchTerm))
            {
                events = _eventService.SearchEvents(searchTerm);
                ViewBag.SearchTerm = searchTerm;

                ViewBag.UpcomingEvents = _eventService.GetUpcomingEvents(3);
                ViewBag.FeaturedEvents = _eventService.GetFeaturedEvents(3);
                ViewBag.RecommendedEvents = _eventService.GetRecommendedEvents(3);
            }
            else
            {
                events = _eventService.GetAllEvents();

                ViewBag.UpcomingEvents = _eventService.GetUpcomingEvents(3);
                ViewBag.FeaturedEvents = _eventService.GetFeaturedEvents(3);
                ViewBag.RecommendedEvents = _eventService.GetRecommendedEvents(3);
            }

            return View(events);
        }

        [HttpPost]
        public IActionResult UpvoteEvent(int eventId)
        {
            var success = _eventService.UpvoteEvent(eventId);
            if (success)
            {
                var eventItem = _eventService.GetEventById(eventId);
                return Json(new { success = true, upvotes = eventItem.Upvotes, downvotes = eventItem.Downvotes });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult DownvoteEvent(int eventId)
        {
            var success = _eventService.DownvoteEvent(eventId);
            if (success)
            {
                var eventItem = _eventService.GetEventById(eventId);
                return Json(new { success = true, upvotes = eventItem.Upvotes, downvotes = eventItem.Downvotes });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult UpvoteIssue(int issueId)
        {
            var success = _issueStorage.UpvoteIssue(issueId);
            if (success)
            {
                var issue = _issueStorage.GetIssueById(issueId);
                return Json(new { success = true, upvotes = issue.Upvotes });
            }
            return Json(new { success = false });
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}