using Microsoft.AspNetCore.Mvc;
using Programming_7312_Part_1.Models;
using Programming_7312_Part_1.Services;
using System.Linq;

namespace Programming_7312_Part_1.Controllers
{
    public class AdminController : Controller
    {
        private readonly IssueStorage _issueStorage;
        private readonly EventService _eventService;

        public AdminController(IssueStorage issueStorage, EventService eventService)
        {
            _issueStorage = issueStorage;
            _eventService = eventService;
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
            if (password == "1234")
            {
                // Simple session-based auth
                HttpContext.Session.SetString("AdminLoggedIn", "true");
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.Error = "Invalid password.";
                return View();
            }
        }

        // GET: Admin/Dashboard
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            ViewBag.Issues = _issueStorage.ReportedIssues.ToList();
            ViewBag.Events = _eventService.GetAllEvents();
            return View();
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
            return View(eventItem);
        }

        // POST: Admin/EditEvent
        [HttpPost]
        public IActionResult EditEvent(Event model, string tagsInput)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
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
                return View(model);
            }

            return RedirectToAction("Dashboard");
        }

        // POST: Admin/Logout
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminLoggedIn");
            return RedirectToAction("Index", "Home");
        }
    }
}