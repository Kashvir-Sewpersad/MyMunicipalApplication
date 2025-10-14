

//********************************************************* start of file and entry into the program *******************************************//

//-------------------------------- start of imports -------------//
using Microsoft.EntityFrameworkCore;
using Programming_7312_Part_1.Data;
using Programming_7312_Part_1.Services;
//---------------------------------- end of imports -----------//
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// Register DbContext
// im using a sqlite db to store all of the events added by an admin. 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register our services
builder.Services.AddSingleton<IssueStorage>();
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