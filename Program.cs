

//********************************************************* start of file and entry into the program *******************************************//

//-------------------------------- start of imports -------------//
using Microsoft.EntityFrameworkCore;
using Programming_7312_Part_1.Data;
using Programming_7312_Part_1.Services;
//---------------------------------- end of imports -----------//

//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ REFERENCES, AI DECLERATION AND CURRENT ISSUES @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@//

 ///////////////////////////////////////////////////////////// REFEREMCES USED FOR THIS PROJECT //////////////////////////////////////////////////////////////////
/*
 
 *Brevo.com. (2025). Brevo | Email Marketing Software, Automation & CRM. [online] Available at: https://www.brevo.com/?gad_source=1&gad_campaignid=23086271243&gbraid=0AAAAAp4YiPJM7xOPrXsppAUZnGIAu-LvQ&gclid=Cj0KCQjw6bfHBhDNARIsAIGsqLjfE68ZXXIQRwSdE_GX3qLjhs0mMFeMj1Idn_ShXWA3lIatJ3mTXQQaArjtEALw_wcB [Accessed 11 Oct. 2025].

 * GeeksforGeeks (2024). Queue Data Structure. [online] GeeksforGeeks. Available at: https://www.geeksforgeeks.org/dsa/queue-data-structure/.

 *GeeksforGeeks (2024). Stack Data Structure. [online] GeeksforGeeks. Available at: https://www.geeksforgeeks.org/dsa/stack-data-structure/.


 *Caleb Curry (2024). SQLite Introduction - Beginners Guide to SQL and Databases. [online] YouTube. Available at: https://www.youtube.com/watch?v=8Xyn8R9eKB8.


 *Sajjaad Khader (2025). Data Structures Explained for Beginners - How I Wish I was Taught. [online] YouTube. Available at: https://www.youtube.com/watch?v=O9v10jQkm5c.


 * W3Schools (2019). AJAX Introduction. [online] W3schools.com. Available at: https://www.w3schools.com/xml/ajax_intro.asp.


 *SendGrid. (2024). Email API - Start for Free | SendGrid. [online] Available at: https://sendgrid.com/en-us/solutions/email-api.


 *Code With RaiGenics (2021). Sorted list and sorted dictionary in C# | C# Collection Part 8. [online] YouTube. Available at: https://www.youtube.com/watch?v=QGw9ozFgSw0.


 *Tech With RGenics (2021). C# List with example | List in C# | C# Collection Part 5. [online] YouTube. Available at: https://www.youtube.com/watch?v=bJEVvhcW8GU [Accessed 8 Oct. 2025].


 *Tech With RGenics (2021). Stack in C# with real time example | C# Collection Part 10. [online] YouTube. Available at: https://www.youtube.com/watch?v=lzfNSP0DYeE&list=PL_xlJum5pRdDC-SCtuLI0D_m4KJcIsCLq&index=10 [Accessed 8 Oct. 2025].


 *Tech With RGenics (2021). C# Collection Best Practices | Collection in C# | C# Collection part 15. [online] YouTube. Available at: https://www.youtube.com/watch?v=Ks0dDWNVURc&list=PL_xlJum5pRdDC-SCtuLI0D_m4KJcIsCLq&index=16 [Accessed 2 Oct. 2025].


 *Tech With RGenics (2021). C# Dictionary with example | Dictionary in C# | C# Collection Part 6. [online] YouTube. Available at: https://www.youtube.com/watch?v=tx_JNYL0Img&list=PL_xlJum5pRdDC-SCtuLI0D_m4KJcIsCLq&index=6 [Accessed 7 Oct. 2025].


 */

/*
 *
 **************************************************************** AI DECLERATION ****************************************************************
 * I Kashvir Sewpersad, the student , ST10257503,  hereby declare that I have made use of artificial intelligence in the creation of this project .
 * The nature of my usage has primarily been for debugging issues, either logical or syntaxes based, improving functionality, assistance with Ui elements (front end) and overall improving the quality of the work I produce .
 * I have also used AI to gain a better understanding of concepts, specifically the data structures and algorithms implemented,  AI was used heavily to get the email feature working which makes use of API to send email to the user.
 * I copied the logic from the contact section over to the report issues section to implement the tracking functionality.
 * I at no stage used AI to do the assignment, however I did use it to help me improve the features and implementation.
 * Topics were researched prior to implementation and requesting AI assistance. 
 * 
 *OpenAI. 2025. Chat-GPT (Version 5.0). [Large language model]. Available at:
https://chatgpt.com/share/68ee7ec7-2df8-8010-ac96-18b83ad84849  [Accessed: 1
November 2025] 
 

OpenAI. 2025. Chat-GPT (Version 5.0). [Large language model]. Available at:
https://chatgpt.com/share/68ee805a-b258-8010-b34b-a8a98b18cf4b  [Accessed:4  
November 2025]

OpenAI. 2025. Chat-GPT (Version 5.0). [Large language model]. Available at:
https://chatgpt.com/share/68ee814e-ddfc-8010-b2fa-da3e1cbcbd9f  [Accessed: 6
November 2025]



OpenAI. 2025. Chat-GPT (Version 5.0). [Large language model]. Available at:
https://chatgpt.com/share/68ee83d0-6d94-8010-995f-c50d8a2bc85b  [Accessed: 8
November 2025]

OpenAI. 2025. Chat-GPT (Version 5.0). [Large language model]. Available at:
https://chatgpt.com/share/6911020a-f5b4-8010-9ab0-aa97e036661d  [Accessed: 8 
November 2025]

OpenAI. 2025. Chat-GPT (Version 5.0). [Large language model]. Available at:
https://chatgpt.com/share/6911d34e-5ae4-8010-8d54-0fb84fc289e5  [Accessed: 9
November 2025]

OpenAI. 2025. Chat-GPT (Version 5.0). [Large language model]. Available at:
https://chatgpt.com/share/6911d3be-08d0-8010-882a-0cda861ab809  [Accessed: 10
November 2025]

 */

//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@// 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// Register DbContext
// im using a sqlite db to store all of the events added by an admin. 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register our services
builder.Services.AddScoped<IssueStorage>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<AnnouncementService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

//**************************************************** end of program and program **************************************************//
