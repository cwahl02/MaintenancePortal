using MaintenancePortal.Data;
using MaintenancePortal.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure cookie-based authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
        options.LoginPath = "/User/Login"; // Redirect to this path if not authenticated
        options.LoginPath = "/User/Login"; // Redirect to this path if not authenticated
        //options.AccessDeniedPath = "/User/AccessDenied"; // Redirect to this path if access is denied
    });

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


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Call the seed method
    await SeedDatabase(userManager, roleManager);
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

app.UseAuthorization();
app.UseAuthentication();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();

async Task SeedDatabase(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
{
    // Seed default roles
    string[] roleNames = { "Admin", "User" };
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Seed a default admin user
    var defaultUser = await userManager.FindByEmailAsync("admin@example.com");
    if (defaultUser == null)
    {
        defaultUser = new User
        {
            UserName = "admin",
            Email = "admin@example.com",
            FirstName = "Admin",
            LastName = "User"
        };
        var result = await userManager.CreateAsync(defaultUser, "Admin@1234");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(defaultUser, "Admin");
        }
    }
}