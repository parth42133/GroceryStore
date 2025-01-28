using GroceryStore.Models.DataLayer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
string projectRoot = Directory.GetParent(baseDirectory).Parent.Parent.Parent.FullName; // Adjust the number of .Parent as needed
string dataDirectory = Path.Combine(projectRoot, "Database");
AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMvc()
        .AddSessionStateTempDataProvider();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<GroceryContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("GroceryContext"));
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
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
