using MaintenancePortal.Data;
using MaintenancePortal.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register AppDbContext with ASP.NET Core DI container
// Connect to SQL Server using "DefaultConnection" from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register ASP.NET Core Identity with custom User and IdentityRole
builder.Services.AddIdentity<User, IdentityRole>()
    // Tells Identity to use Entity Framework Core with AppDbContext for storing user data
    .AddEntityFrameworkStores<AppDbContext>()
    // Enable default token providers for password reset, email confirmation, etc.
    .AddDefaultTokenProviders();

// Add authorization services
builder.Services.AddAuthorization();

// Configure cookie-based authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
        options.LoginPath = "/User/Login"; // Redirect to this path if not authenticated
        options.LogoutPath = "/User/Logout"; // Redirect to this path if not authenticated
        //options.AccessDeniedPath = "/User/AccessDenied"; // Redirect to this path if access is denied
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>(); // Add logger for debugging
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();

    try
    {
        // Apply any pending migrations
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied.");

        // Call the SeedData method to seed users and tickets
        logger.LogInformation("Starting data seeding...");
        await SeedData.InitializeAsync(context, userManager);
        logger.LogInformation("Seeding completed successfully.");
    }
    catch (Exception ex)
    {
        // Log any errors that happen during seeding
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();


app.UseAuthentication(); // Must come before UseAuthorization, otherwise auth won't work
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();