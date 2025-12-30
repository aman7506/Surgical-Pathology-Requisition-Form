using Microsoft.EntityFrameworkCore;
using PathologyFormApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// Add DbContext
builder.Services.AddDbContext<PathologyContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<PathologyContext>()
.AddDefaultTokenProviders();

// Configure Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireDoctorRole", policy =>
        policy.RequireRole("Doctor"));
    
    options.AddPolicy("RequireNurseRole", policy =>
        policy.RequireRole("Nurse"));
});

// Configure Authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "PathologyFormApp.Auth";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

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

// Use standard authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Set the default route to Pathology/Index, but authentication will redirect unauthenticated users to login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pathology}/{action=Index}/{id?}");

app.Run();