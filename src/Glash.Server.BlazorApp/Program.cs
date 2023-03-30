using Glash.Server.BlazorApp;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Quick.EntityFrameworkCore.Plus.SQLite;
using Quick.EntityFrameworkCore.Plus;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;

ConfigDbContext.Init(new SQLiteDbContextConfigHandler(SQLiteDbContextConfigHandler.CONFIG_DB_FILE), modelBuilder =>
{
    Global.Instance.OnModelCreating(modelBuilder);
});
using (var dbContext = new ConfigDbContext())
    dbContext.EnsureDatabaseCreatedAndUpdated(t => Debug.Print(t));
ConfigDbContext.CacheContext.LoadCache();
Global.Instance.Init();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();
app.UseWebSockets();
app.UseGlashServer("/glash", Global.Instance.ConnectionPassword);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();