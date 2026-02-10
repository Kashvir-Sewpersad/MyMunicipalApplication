# Municipal Services Web Application  
**Programming 7312 POE – Final Submission**

A **C# ASP.NET MVC** web application for reporting municipal issues, submitting service requests, viewing events/announcements, and managing them via an admin dashboard. Built to meet POE requirements with additional features like email notifications, tracking numbers, and advanced in-memory data structures.

## Table of Contents
- [Project Overview](#project-overview)
- [Key Features](#key-features)
- [Technologies & Data Structures](#technologies--data-structures)
- [Prerequisites](#prerequisites)
- [Setup Instructions](#setup-instructions)
- [Important Notes](#important-notes)
- [Demonstration Videos](#demonstration-videos)
- [References](#references)
- [AI Declaration](#ai-declaration)

## Project Overview
This is a full-stack **ASP.NET MVC** web application developed for the Programming 7312 POE (Final Submission). It allows citizens to:
- Report municipal issues / submit service requests
- Track request status using a unique tracking number
- View community events and announcements
- Contact the municipality

Administrators can manage reported issues, events, and announcements through a secure dashboard.

The system uses **SQLite** for persistent storage and advanced **in-memory data structures** for efficient runtime operations (sorting, prioritization, searching, etc.).

## Key Features
- User-friendly issue reporting with automatic **tracking number** generation
- **Email confirmation** sent to users (using Brevo API) containing tracking number
- Service request / issue status tracking via search bar on "Service Status" page
- Admin dashboard (hardcoded password: **1234**)
- Community events listing with category/tag filtering
- Announcements with priority queuing
- Responsive front-end design

## Technologies & Data Structures
**Backend & Framework**
- ASP.NET MVC (.NET 6.0)
- C#
- SQLite (configured in `appsettings.json`)
- Brevo API (for transactional emails – credentials removed for security)

**Advanced Data Structures** (in-memory – for runtime efficiency)

**IssueStorage.cs**
- `BinarySearchTree<Issue>` – fast lookup by ID
- `AVLTree<Issue>` – self-balancing tree sorted by date
- `SortedSet<Issue>` (Red-Black tree) – category-based ordering & filtering
- `PriorityQueue<Issue>` (max-heap) – prioritize by community votes
- `ServiceRequestGraph` (adjacency list) – models dependencies, supports BFS & MST (Minimum Spanning Tree for resource optimization)

**EventService.cs**
- `SortedDictionary<DateTime, HashSet<Event>>` – events by date
- `Dictionary<string, LinkedList<Event>>` – events by category
- `Queue<Event>` – recent events (FIFO, limited to 5)
- `Stack<Event>` – featured events (LIFO, limited to 3)
- `HashSet<string>` – unique categories & tags
- `Dictionary<string, int>` – search history for recommendations

**AnnouncementService.cs**
- `Queue<Announcement>` – FIFO queue (max 10)
- `Dictionary<string, List<Announcement>>` – by category
- `SortedDictionary<int, List<Announcement>>` – by priority (high first)

**Other**
- `LinkedList<Issue>` – all reported issues (fast access & upvoting)

Seeded data is loaded on first startup. User/admin actions persist to SQLite.

## Prerequisites
- **Operating System**: Windows 10/11 (primary), macOS/Linux possible
- **.NET SDK**: 6.0 or later → [Download here](https://dotnet.microsoft.com/download)
- **IDE** (recommended):
  - Visual Studio 2022 (Community Edition – free)
  - JetBrains Rider
- **Browser**: Google Chrome (preferred)

## Setup Instructions

### Option 1: Clone from GitHub (recommended)
1. Copy the repository URL
2. In your IDE:
   - Visual Studio → File → Clone Repository…
   - Rider → VCS → Get from Version Control…
3. Paste URL and clone

### Option 2: Download ZIP
1. Download ZIP from GitHub
2. Extract to a folder (e.g. `C:\Projects\MunicipalApp` or Desktop)
3. Open the `.sln` file in Visual Studio / Rider

### Run the Project
1. Open the solution (`.sln`)
2. Right-click the **solution** → **Restore NuGet Packages**
3. In Solution Explorer, right-click the **web project** (usually the one with MVC folders) → **Set as Startup Project**
4. Press **F5** or click **Run** / green play button

The app should launch in your default browser (SQLite DB is created automatically).

## Important Notes
- **Admin access**: Use password **1234** (hardcoded – for demonstration only)
- **Email functionality**: Uses Brevo API – configuration keys removed from source code for security
- Emails are sent when:
  - User submits contact form
  - User reports an issue / service request (includes tracking number)
- **Tracking**: Use the unique tracking number (sent via email) on the **Service Status** page
- Database: SQLite – file created in project folder on first run

## Demonstration Videos
Watch both videos for the complete POE demonstration:

- [POE Overview & Functionality](https://youtu.be/CbJI8qQRKsE)
- [Additional Features & Tracking](https://youtu.be/ksuvHasZ940)
- Earlier demo: [https://youtu.be/7nXOtbyj1Ks](https://youtu.be/7nXOtbyj1Ks)

## References
- Brevo – Email API: https://www.brevo.com/
- GeeksforGeeks – Queue / Stack / Data Structures
- YouTube tutorials: Caleb Curry (SQLite), Sajjad Khader (Data Structures), Tech with Organics (C# Collections), Code with Argenis (Sorted collections)
- W3Schools – AJAX
- SendGrid (alternative email reference)

## AI Declaration
I, **Kashvir Sewpersad** (Student No: ST10257503), declare that I used artificial intelligence (primarily ChatGPT 5.0) in this project for:
- Debugging logical & syntax errors
- Improving functionality & code quality
- UI element suggestions (front-end)
- Understanding & implementing data structures/algorithms
- Assistance with email API integration & tracking logic

AI was **not** used to complete the assignment outright. All core concepts were researched first, and I manually implemented and verified the work.

ChatGPT conversation links (examples):  
- https://chatgpt.com/share/68ee7ec7-2df8-8010-ac96-18b83ad84849  
- https://chatgpt.com/share/68ee805a-b258-8010-b34b-a8a98b18cf4b  
(and others dated Nov 2025)

---
Made with 💻 for Programming 7312 – Final POE  
© Kashvir Sewpersad 2025/2026
