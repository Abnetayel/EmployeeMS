using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation(); // Enable runtime compilation

// Add Razor Pages (if needed)
builder.Services.AddRazorPages();

// Configure Authentication with Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdminOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "SuperAdmin"));

    options.AddPolicy("AdminOrHigher", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin", "SuperAdmin"));

    options.AddPolicy("UserOrHigher", policy =>
        policy.RequireClaim(ClaimTypes.Role,"Admin", "SuperAdmin"));
    options.AddPolicy("User", policy =>
       policy.RequireClaim(ClaimTypes.Role, "User"));
});

// Configure DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services (optional - if you want to use Identity features)
//builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
//{
//    // Configure identity options if needed
//    options.Password.RequireDigit = true;
//    options.Password.RequiredLength = 8;
//    options.Password.RequireNonAlphanumeric = false;
//    options.Password.RequireUppercase = true;
//    options.Password.RequireLowercase = true;
//})
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");


app.MapRazorPages();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = new PasswordHasher<User>(); // For manual hashing

        // 1. Ensure database is created/migrated
        context.Database.Migrate();

        // 2. Seed default roles directly in Users table
        var superAdminEmail = "abnetayele694@gmail.com";
        var adminEmail = "eliasaynekulu694@gmail.com";

        // Check if users already exist
        if (!context.Users.Any(u => u.Role == "SuperAdmin"))
        {
            context.Users.Add(new User
            {
                UserName = "AbnetAyele",
                Email = superAdminEmail,
                Password = passwordHasher.HashPassword(null, "abnetayele@694"),
                Role = "SuperAdmin"
            });
        }

        if (!context.Users.Any(u => u.Role == "Admin"))
        {
            context.Users.Add(new User
            {
                UserName = "EliasAynekulu",
                Email = adminEmail,
                Password = passwordHasher.HashPassword(null, "eliasaynekulu@694"),
                Role = "Admin"
            });
        }

        // 3. Save all changes
        await context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Seeding failed");
    }
}

app.Run();