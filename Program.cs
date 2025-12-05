using Guc_Uni_System.Models;
using Guc_Uni_System.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// 1. Add Session
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

// 2. Add Database
builder.Services.AddDbContext<UniversityHrManagementSystemContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbConnection")));

// 3. Add Auth Service
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStatusCodePagesWithReExecute("/NotFound");
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// 4. Enable Session
app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.Run();