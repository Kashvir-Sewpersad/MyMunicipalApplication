
//***************************************************************** Start Of file ****************************************************//

//------------------------- start of imports -------------------------//

using Microsoft.AspNetCore.Mvc;
using Programming_7312_Part_1.Models;
using Programming_7312_Part_1.Services;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

//----------------------- end of imports -----------------------------//
namespace Programming_7312_Part_1.Controllers
{
    public class AdminController : Controller
    {
        private readonly IssueStorage _issueStorage;
        private readonly EventService _eventService;
        private readonly AnnouncementService _announcementService;

        public AdminController(IssueStorage issueStorage, EventService eventService, AnnouncementService announcementService)
        {
            _issueStorage = issueStorage;
            _eventService = eventService;
            _announcementService = announcementService;
        }

        // GET: Admin/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Admin/Login
        [HttpPost]
        public IActionResult Login(string password)
        {
            // Hash table for admin credentials
            var adminCredentials = new Dictionary<string, string>
            {
                { "password", "1234" } // hard coded password for simple login. 
            };

            if (adminCredentials.ContainsValue(password))
            {
                // Simple session-based auth
                HttpContext.Session.SetString("AdminLoggedIn", "true");
                return RedirectToAction("Dashboard"); // opens admin dash 
            }
            else
            {
                ViewBag.Error = "Invalid password."; // error message 
                return View(); // returns back to login 
            }
        }

        // GET: Admin/Dashboard
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            ViewBag.Issues = _issueStorage.GetAllIssues();
            ViewBag.Events = _eventService.GetAllEvents(); // opens up the events 
            ViewBag.Announcements = _announcementService.GetAllAnnouncements(); // opens up all stored announcements 
            return View(); // displays on the admin view 
        }

        // GET: Admin/EditEvent
        public IActionResult EditEvent(int id)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            var eventItem = _eventService.GetEventById(id);
            if (eventItem == null)
            {
                return NotFound();
            }

            ViewBag.TagsInput = string.Join(", ", eventItem.Tags ?? new List<string>());
            ViewBag.Categories = _eventService.UniqueCategories.ToList();
            ViewBag.Tags = _eventService.UniqueTags.ToList();
            return View(eventItem);
        }

        // GET: Admin/CreateEvent
        public IActionResult CreateEvent()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            ViewBag.Categories = _eventService.UniqueCategories.ToList();
            ViewBag.Tags = _eventService.UniqueTags.ToList();
            return View();
        }

        // POST: Admin/CreateEvent
        /*
         *the below method is used to cpature the process of creaing an event 
         * 
         */
        [HttpPost]
        public async Task<IActionResult> CreateEvent(Event model, string tagsInput, IFormFile imageFile) // fields 
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            try
            {
                // Validate category
                var validCategories = _eventService.UniqueCategories.ToList();
                if (!validCategories.Contains(model.Category))
                {
                    ModelState.AddModelError("Category", "Please select a valid category from the dropdown."); // error message  
                    ViewBag.Categories = validCategories;
                    ViewBag.Tags = _eventService.UniqueTags.ToList();
                    return View(model);
                }

                if (!ModelState.IsValid) // error if not valid 
                {
                    ViewBag.Categories = validCategories;
                    ViewBag.Tags = _eventService.UniqueTags.ToList();
                    return View(model); // return model 
                }

                // Handle image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads"); // image path is in  the root images folder 
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder); // locatoion 
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    model.ImagePath = "/uploads/" + uniqueFileName;
                }

                // Parse tags
                if (!string.IsNullOrWhiteSpace(tagsInput))
                {
                    model.Tags = tagsInput.Split(',')
                        .Select(t => t.Trim())
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .ToList();
                }
                else
                {
                    model.Tags = new List<string>();
                }

                // Generate new ID
                var allEvents = _eventService.GetAllEvents();
                model.Id = allEvents.Any() ? allEvents.Max(e => e.Id) + 1 : 1; // previous id + 1 

                _eventService.AddEvent(model); // added to the model  

                return RedirectToAction("Dashboard"); // takes the admin back to the dash 
            }
            // exception handling added  
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the event. Please try again."); // error message 
                ViewBag.Categories = _eventService.UniqueCategories.ToList();
                ViewBag.Tags = _eventService.UniqueTags.ToList();
                return View(model);
            }
        }

        // POST: Admin/EditEvent
        [HttpPost]
        public IActionResult EditEvent(Event model, string tagsInput)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            try
            {
                // Validate category
                var validCategories = _eventService.UniqueCategories.ToList();
                if (!validCategories.Contains(model.Category))
                {
                    ModelState.AddModelError("Category", "Please select a valid category from the dropdown."); // error message 
                    ViewBag.Categories = validCategories;
                    ViewBag.Tags = _eventService.UniqueTags.ToList();
                    ViewBag.TagsInput = string.Join(", ", model.Tags ?? new List<string>());
                    return View(model); // retuen 
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = validCategories;
                    ViewBag.Tags = _eventService.UniqueTags.ToList();
                    ViewBag.TagsInput = string.Join(", ", model.Tags ?? new List<string>());
                    return View(model);
                }

                // Parse tags
                if (!string.IsNullOrWhiteSpace(tagsInput))
                {
                    model.Tags = tagsInput.Split(',')
                        .Select(t => t.Trim())
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .ToList();
                }
                else
                {
                    model.Tags = new List<string>();
                }

                var success = _eventService.UpdateEvent(model);
                if (!success)
                {
                    ModelState.AddModelError("", "Failed to update event.");
                    ViewBag.Categories = validCategories;
                    ViewBag.Tags = _eventService.UniqueTags.ToList();
                    ViewBag.TagsInput = string.Join(", ", model.Tags ?? new List<string>());
                    return View(model);
                }

                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the event. Please try again.");
                ViewBag.Categories = _eventService.UniqueCategories.ToList();
                ViewBag.Tags = _eventService.UniqueTags.ToList();
                ViewBag.TagsInput = string.Join(", ", model.Tags ?? new List<string>());
                return View(model);
            }
        }

        // POST: Admin/DeleteEvent
        /*
         * the below method is to capture and update the functionality for deleting an event 
         *
         * 
         */
        [HttpPost]
        public IActionResult DeleteEvent(int id)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            var success = _eventService.DeleteEvent(id);
            if (!success)
            {
                TempData["Error"] = "Failed to delete event."; // error message 
            }
            else
            {
                TempData["Success"] = "Event deleted successfully."; // sucess message 
            }

            return RedirectToAction("Dashboard"); // back to dash 
        }

        // GET: Admin/CreateAnnouncement
        
        /*
         *
         * the below method contains the functionaity for capturing and storing the new announcements added by the admin staff 
         *
         * 
         */
        public IActionResult CreateAnnouncement()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            ViewBag.Categories = _announcementService.UniqueCategories.ToList();
            return View();
        }

        // POST: Admin/CreateAnnouncement
        [HttpPost]
        public IActionResult CreateAnnouncement(Announcement model)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = _announcementService.UniqueCategories.ToList();
                    return View(model);
                }

                // Generate new ID
                var allAnnouncements = _announcementService.GetAllAnnouncements();
                model.Id = allAnnouncements.Any() ? allAnnouncements.Max(a => a.Id) + 1 : 1;

                _announcementService.AddAnnouncement(model);

                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the announcement. Please try again."); // error message 
                ViewBag.Categories = _announcementService.UniqueCategories.ToList();
                return View(model); // return 
            }
        }

        // POST: Admin/DeleteAnnouncementfunctionality to delete and capture the change for deleting an announcement 
        
        /*
         *
         *
         * the below method contains the 
         *
         * 
         */
        [HttpPost]
        public IActionResult DeleteAnnouncement(int id)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            var success = _announcementService.DeleteAnnouncement(id);
            if (!success)
            {
                TempData["Error"] = "Failed to delete announcement.";
            }
            else
            {
                TempData["Success"] = "Announcement deleted successfully.";
            }

            return RedirectToAction("Dashboard");
        }
        
        // POST: Admin/ApproveIssue
        [HttpPost]
        public IActionResult ApproveIssue(int issueId, string comments)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            var success = _issueStorage.ApproveIssue(issueId, comments);
            if (!success)
            {
                TempData["Error"] = "Failed to approve issue.";
            }
            else
            {
                TempData["Success"] = "Issue approved successfully.";
            }

            return RedirectToAction("Dashboard");
        }

        // POST: Admin/RejectIssue
        [HttpPost]
        public IActionResult RejectIssue(int issueId, string comments)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            var success = _issueStorage.RejectIssue(issueId, comments);
            if (!success)
            {
                TempData["Error"] = "Failed to reject issue.";
            }
            else
            {
                TempData["Success"] = "Issue rejected successfully.";
            }

            return RedirectToAction("Dashboard");
        }

        // POST: Admin/DeleteIssue
        [HttpPost]
        public IActionResult DeleteIssue(int issueId, string comments)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            var success = _issueStorage.DeleteIssue(issueId, comments);
            if (!success)
            {
                TempData["Error"] = "Failed to delete issue.";
            }
            else
            {
                TempData["Success"] = "Issue deleted successfully.";
            }

            return RedirectToAction("Dashboard");
        }

        // POST: Admin/Logout

        /*
         *
         * the below method contains the functionality for logging and admin out
         *
         * this comes in the form of a simple session end
         *
         * it is not the safest but works
         *
         *
         */
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminLoggedIn");
            return RedirectToAction("Index", "Home"); //  admin is directed back to home page
        }
    }
}
//***************************************************************** End Of file ****************************************************//