
//************************************************** start of file ************************************************//

//------------------------------ start of imports ---------------------//
using Microsoft.AspNetCore.Mvc;
using Programming_7312_Part_1.Models;
using Programming_7312_Part_1.Services;
using Programming_7312_Part_1.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;

//----------------------------- end of imports ------------------------//

namespace Programming_7312_Part_1.Controllers
{
   
    /// The HomeController class manages user interactions for the home section of the application.
    /// It provides actions for viewing the home page, reporting issues, checking service status, browsing local events, and submitting contact forms.
    ////the other use of the home controller is to make pages visible or not 
    public class HomeController : Controller
    {
        private readonly IssueStorage _issueStorage;
        private readonly EventService _eventService;
        private readonly AnnouncementService _announcementService;
        private readonly ContactService _contactService;
        private readonly EmailService _emailService;
        private readonly ApplicationDbContext _context;

        
        /// Initializes a new instance of the HomeController with dependency-injected services.
   
        /// "issueStorage"----->Service for managing issue storage and retrieval.
        /// "eventService"---------->Service for handling event-related action
        /// "announcementService"----->Service for managing announcement
        /// "contactService"--------------->Service for contact form subs
        /// "emailService"-------------------->Service for sending emails.
        /// "context"----------------------------->Database context for the application.
        public HomeController(IssueStorage issueStorage, EventService eventService, AnnouncementService announcementService, ContactService contactService, EmailService emailService, ApplicationDbContext context)
        {
            _issueStorage = issueStorage ?? throw new ArgumentNullException(nameof(issueStorage));
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _announcementService = announcementService ?? throw new ArgumentNullException(nameof(announcementService));
            _contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

       
        /// Action method that returns the home page view.
        
        /// The Index view shall be returned and thus made visible .
        // home  page
        public IActionResult Index()
        {
            return View();
        }

        
        /// Displays the service status page for the current user, showing their reported issues.
        /// 
        /// If no user ID is found, displays a message to report an issue first.
       
        ///The ServiceStatus view with user's issues shall be made visible 
        // New: Service Status Action
        public IActionResult ServiceStatus()
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            if (string.IsNullOrEmpty(userId))
            {
                ViewBag.Message = "No issues reported. Please report an issue first.";
                ViewBag.Issues = new List<Issue>();
            }
            else
            {
                var issues = _issueStorage.GetUserIssues(userId);
                ViewBag.Issues = issues;
                ViewBag.UserId = userId;
            }
            return View();
        }

        
        /// Displays the public service status page, showing all issues sorted by upvotes.
       
        /// The ServiceStatus view with all issues.
        //  Public Service Status Action (shows all issues sorted by upvotes)
        public IActionResult PublicServiceStatus()
        {
            var issues = _issueStorage.GetAllIssues();
            ViewBag.Issues = issues;
            return View("ServiceStatus");
        }

        
        /// Action method that returns the privacy policy page view.
        
        /// The Privacy view.
        // privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        
        /// Displays the report issues page with available categories.
        /// the below are hard coded to allow for a user to select from some prepopulated responses / catergoriess
        /// this is done to help with the user experience
        /// overall a nice feature and follows other government apps such as the city of cape town
        /// The ReportIssues view with a new Issue model.
        public IActionResult ReportIssues()
        {
            ViewBag.Categories = new[] {
                "Sanitation", "Roads", "Utilities", "Other",
                "animal carcass removal",
                "blocked stormwater drains",
                "electricity outage/ unscheduled outages",
                "graffiti and stolen or vandalised City property",
                "uncollected refuse or waste",
                "tree removal and cutting",
                "tree root, trimming, pruning",
                "illegal dumping",
                "leaking water pipes",
                "maintenance of open spaces, parks and public spaces",
                "mowing of vegetation",
                "noise nuisances",
                "problem buildings (abandoned and derelict buildings)",
                "public toilets/ ablution facilities unhygienic",
                "speeding and traffic offences",
                "traffic calming (speed bumps), new or damaged street signs",
                "unruly or inappropriate behaviour",
                "unscheduled water supply disruptions",
                "vagrants and illegal squatting"
            }; // categories
            return View(new Issue()); // new issue
        }

      
        /// Handles the POST request for reporting a new issue, including file upload and email confirmation.
       
        /// The Issue model containing the report details
        /// attachment">Optional file attachment for the issue.
        /// The ReportIssues view with success message or validation errors.
        ///
        ///
        ///
        /// the below are hard coded to allow for a user to select from some prepopulated responses / catergoriess
        /// this is done to help with the user experience
        /// overall a nice feature and follows other government apps such as the city of cape town
        [HttpPost]
        public async Task<IActionResult> ReportIssues(Issue model, IFormFile attachment)
        {
            ViewBag.Categories = new[] {
                "Sanitation", "Roads", "Utilities", "Other",
                "animal carcass removal",
                "blocked stormwater drains",
                "electricity outage/ unscheduled outages",
                "graffiti and stolen or vandalised City property",
                "uncollected refuse or waste",
                "tree removal and cutting",
                "tree root, trimming, pruning",
                "illegal dumping",
                "leaking water pipes",
                "maintenance of open spaces, parks and public spaces",
                "mowing of vegetation",
                "noise nuisances",
                "problem buildings (abandoned and derelict buildings)",
                "public toilets/ ablution facilities unhygienic",
                "speeding and traffic offences",
                "traffic calming (speed bumps), new or damaged street signs",
                "unruly or inappropriate behaviour",
                "unscheduled water supply disruptions",
                "vagrants and illegal squatting"
            }; 

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Handle file upload if an attachment is provided
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

            // New: Set UserId cookie
            var userId = HttpContext.Request.Cookies["UserId"];
            if (string.IsNullOrEmpty(userId))
            {
                userId = Guid.NewGuid().ToString();
                Response.Cookies.Append("UserId", userId, new CookieOptions { Expires = DateTime.Now.AddYears(1) });
            }
            model.UserId = userId;

            // Store issues
            _issueStorage.AddIssue(model); // Now updates advanced structures

            // Send confirmation email
            await _emailService.SendIssueConfirmationAsync(model.Email, model.Id, model.Location, model.Category, model.Description);

            ViewBag.SuccessMessage = "Issue reported successfully! A confirmation email has been sent to your email address.";
            ViewBag.EngagementMessage = "Your reports make our community better!";

            ModelState.Clear();
            return View(new Issue());
        }

       
        /// Displays local events based on category, search term, or date filters.
        /// Also populates ViewBag with announcements and categorized event sections.
        
        /// "category">Optional category filter for events
        /// "searchTerm">Optional search term for events
        /// "startDate">Optional start date for filtering events.
        /// "endDate">Optional end date for filtering events.
        /// The LocalEvents view with filtered events.
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

                // Get recommended events excluding the searched events
                var searchedEventIds = events.Select(e => e.Id).ToHashSet();
                var allRecommended = _eventService.GetRecommendedEvents(10); // Get more to filter
                ViewBag.RecommendedEvents = allRecommended.Where(e => !searchedEventIds.Contains(e.Id)).Take(3).ToList();
            }
            else
            {
                events = _eventService.GetAllEvents();

                ViewBag.UpcomingEvents = _eventService.GetUpcomingEvents(3);
                ViewBag.FeaturedEvents = _eventService.GetFeaturedEvents(3);
                ViewBag.RecommendedEvents = _eventService.GetRecommendedEvents(3);
            }

            // Apply date filtering to events and section events if dates are provided
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

        ///
        /// Handles downvoting an event via AJAX POST request.
        ///
        /// "eventId">The ID of the event to downvote.
        /// JSON response with success status and updated vote counts.
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

        /// <summary>
        /// Handles upvoting an issue via AJAX POST request.
        /// </summary>
        /// "issueId">The ID of the issue to upvote.
        /// JSON response with success status and updated vote counts.
        [HttpPost]
        public IActionResult UpvoteIssue(int issueId)
        {
            var success = _issueStorage.UpvoteIssue(issueId);
            if (success)
            {
                var issue = _issueStorage.GetIssueById(issueId);
                return Json(new { success = true, upvotes = issue.Upvotes, downvotes = issue.Downvotes });
            }
            return Json(new { success = false });
        }

        /// <summary>
        /// Handles downvoting an issue via AJAX POST request.
        /// </summary>
        /// "issueId">The ID of the issue to downvote.
        /// JSON response with success status and updated vote counts.
        [HttpPost]
        public IActionResult DownvoteIssue(int issueId)
        {
            var success = _issueStorage.DownvoteIssue(issueId);
            if (success)
            {
                var issue = _issueStorage.GetIssueById(issueId);
                return Json(new { success = true, upvotes = issue.Upvotes, downvotes = issue.Downvotes });
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

        /// <summary>
        /// Handles the POST request for submitting a contact form, storing the message and sending confirmation email.
        /// </summary>
        /// "model">The Contact model containing the form details.
        /// The Contact view with success message or validation errors.
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

            //*
            //
            // the below code is used to send the confirmation message. 
            //
            //**/
            await _emailService.SendContactConfirmationAsync(model.Email, model.Name, model.Subject, model.Message);

            ViewBag.SuccessMessage = "Your message has been sent successfully! A confirmation email has been sent to your email address.";
            ViewBag.EngagementMessage = "Thank you for contacting us.";

            ModelState.Clear();
            return View(new Contact());
        }

        /// <summary>
        /// Displays the error page.
        /// </summary>
        /// The Error view.
        public IActionResult Error()
        {
            return View(); // return static view 
        }
    }
}
//************************************** end of file **********************************************************//