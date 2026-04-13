using CaseInvestigationManagementSystem.Hubs;
using CaseInvestigationManagementSystem.Repositories;
<<<<<<< HEAD
using CaseInvestigationManagementSystem.Services;
=======
using Microsoft.EntityFrameworkCore;
using CaseInvestigationManagementSystem.Data;
>>>>>>> f47e5b5dd25d827ff896aa5617cada90325c82ef

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<ICaseRepository,CaseRepository>();
builder.Services.AddScoped<IDocumentRepository,DocumentRepository>();
builder.Services.AddScoped<ICommentRepository,CommentRepository>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();

builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddScoped<GeminiService>();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSignalR();
builder.Services.AddSession(o =>
{
   o.IdleTimeout = TimeSpan.FromMinutes(30);
   o.Cookie.HttpOnly = true;
   o.Cookie.IsEssential = true; 
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();
// In containers (and some Azure configurations) HTTPS may not be configured.
// Only enable HTTPS redirection when an HTTPS port is explicitly provided.
if (!string.IsNullOrWhiteSpace(app.Configuration["ASPNETCORE_HTTPS_PORT"]))
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Index}/{id?}");

app.MapHub<NotificationHub>("/notificationHub");
app.Run();
