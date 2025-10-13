
//************************************************** start of file ************************************************//

//------------------------------ start of imports ---------------------//
using Microsoft.AspNetCore.Mvc;
using Programming_7312_Part_1.Models;
using Programming_7312_Part_1.Services;
using System.IO;
using System.Collections.Generic;
using System.Linq;

//----------------------------- end of imports ------------------------//

namespace Programming_7312_Part_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IssueStorage _issueStorage;
        private readonly EventService _eventService;
        private readonly AnnouncementService _announcementService;
        private readonly ContactService _contactService;
        private readonly EmailService _emailService;

        public HomeController(IssueStorage issueStorage, EventService eventService, AnnouncementService announcementService, ContactService contactService, EmailService emailService)
        {
            _issueStorage = issueStorage ?? throw new ArgumentNullException(nameof(issueStorage));
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _announcementService = announcementService ?? throw new ArgumentNullException(nameof(announcementService));
            _contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        // home  page 
        public IActionResult Index()
        {
            return View();
        }

        // privacy page 
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ReportIssues()
        {
            ViewBag.Categories = new[] { "Sanitation", "Roads", "Utilities", "Other" }; // categories 
            return View(new Issue()); // new issue 
        }

        [HttpPost]
        public async Task<IActionResult> ReportIssues(Issue model, IFormFile attachment)
        {
            ViewBag.Categories = new[] { "Sanitation", "Roads", "Utilities", "Other" }; // catergories 

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

        public IActionResult LocalEvents(string category = "", string searchTerm = "", DateTime? startDate = null, DateTime? endDate = null)
        {
            ViewBag.Categories = _eventService.UniqueCategories.ToList();
            ViewBag.Announcements = _announcementService.GetActiveAnnouncements(5);

            // Set default start date to current date if not provided
            if (!startDate.HasValue)
            {
                startDate = DateTime.Now.Date; // current date
            }

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            IEnumerable<Event> events;

            if (!string.IsNullOrEmpty(category))
            {
                if (category.ToLower() == "popular")
                {
                    events = _eventService.GetPopularEvents(20); // Show more popular events
                    ViewBag.SelectedCategory = category;

                    // For popular, show general sections
                    ViewBag.UpcomingEvents = _eventService.GetUpcomingEvents(3);
                    ViewBag.FeaturedEvents = _eventService.GetFeaturedEvents(3);
                    ViewBag.RecommendedEvents = _eventService.GetRecommendedEvents(3);
                }
                else
                {
                    events = _eventService.GetEventsByCategory(category);
                    ViewBag.SelectedCategory = category;

                    // Filter other sections by category as well
                    ViewBag.UpcomingEvents = _eventService.GetUpcomingEventsByCategory(category, 3);
                    ViewBag.FeaturedEvents = _eventService.GetFeaturedEventsByCategory(category, 3);
                    ViewBag.RecommendedEvents = _eventService.GetRecommendedEventsByCategory(category, 3);
                }
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

            // Apply date filtering if dates are provided
            if (startDate.HasValue || endDate.HasValue)
            {
                events = events.Where(e =>
                {
                    bool matchesStart = !startDate.HasValue || e.EventDate.Date >= startDate.Value.Date;
                    bool matchesEnd = !endDate.HasValue || e.EventDate.Date <= endDate.Value.Date;
                    return matchesStart && matchesEnd;
                }).ToList();

                // Also filter the section events by date
                if (ViewBag.UpcomingEvents != null)
                {
                    ViewBag.UpcomingEvents = ((IEnumerable<Event>)ViewBag.UpcomingEvents).Where(e =>
                    {
                        bool matchesStart = !startDate.HasValue || e.EventDate.Date >= startDate.Value.Date;
                        bool matchesEnd = !endDate.HasValue || e.EventDate.Date <= endDate.Value.Date;
                        return matchesStart && matchesEnd;
                    }).ToList();
                }

                if (ViewBag.FeaturedEvents != null)
                {
                    ViewBag.FeaturedEvents = ((IEnumerable<Event>)ViewBag.FeaturedEvents).Where(e =>
                    {
                        bool matchesStart = !startDate.HasValue || e.EventDate.Date >= startDate.Value.Date;
                        bool matchesEnd = !endDate.HasValue || e.EventDate.Date <= endDate.Value.Date;
                        return matchesStart && matchesEnd;
                    }).ToList();
                }

                if (ViewBag.RecommendedEvents != null)
                {
                    ViewBag.RecommendedEvents = ((IEnumerable<Event>)ViewBag.RecommendedEvents).Where(e =>
                    {
                        bool matchesStart = !startDate.HasValue || e.EventDate.Date >= startDate.Value.Date;
                        bool matchesEnd = !endDate.HasValue || e.EventDate.Date <= endDate.Value.Date;
                        return matchesStart && matchesEnd;
                    }).ToList();
                }
            }

            return View(events.ToList());
        }
        /*
         *
         * the below method is to display the upvotes 
         *
         *
         * 
         */
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
        public IActionResult DownvoteEvent(int eventId) // downvotes based on id 
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
        /*
         *
         *
         * the below is for the contact 
         */
        public IActionResult Contact()
        {
            ViewBag.Categories = new[] { "General Inquiry", "Technical Support", "Feedback", "Complaint", "Other" };
            return View(new Contact());
        }

        [HttpPost]
        public async Task<IActionResult> Contact(Contact model)
        {
            ViewBag.Categories = new[] { "General Inquiry", "Technical Support", "Feedback", "Complaint", "Other" }; // options 

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Store contact message
            _contactService.AddContact(model);

            // Send confirmation email
            await _emailService.SendContactConfirmationAsync(model.Email, model.Name, model.Subject, model.Message);

            ViewBag.SuccessMessage = "Your message has been sent successfully! A confirmation email has been sent to your email address.";
            ViewBag.EngagementMessage = "Thank you for contacting us.";

            ModelState.Clear();
            return View(new Contact());
        }

        public IActionResult Error()
        {
            return View(); // return static view 
        }
    }
}
//************************************** end of file **********************************************************//