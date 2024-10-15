using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies; // Add this for cookie authentication

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // Add session services
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Configure authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultSignInScheme = "Cookies";
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Redirect to login page if not authenticated
    options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect to Access Denied page if unauthorized
});


// Add authorization policies if needed
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireProgrammeCoordinatorRole", policy => policy.RequireRole("ProgrammeCoordinator"));
    options.AddPolicy("RequireAcademicManagerRole", policy => policy.RequireRole("AcademicManager"));
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

app.UseSession(); // Enable session middleware
app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization(); // Enable authorization middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
