using System;
using Glash.Blazor.Agent;
using Glash.Blazor.Agent.Core;
using Quick.LiteDB.Plus;
using System.Diagnostics;

Quick.Protocol.QpAllClients.RegisterUriSchema();
// Read dbFile path from environment variable, default to "Config.litedb" if not set
var dbFile = Environment.GetEnvironmentVariable("GLASH_DB_FILE_PATH") ?? "Config.litedb";
#if DEBUG
dbFile = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), dbFile);
#endif
ConfigDbContext.Init(dbFile, modelBuilder =>
{
    Global.Instance.OnModelCreating(modelBuilder);
});
ConfigDbContext.CacheContext.LoadCache();
GlashAgent.Core.LoginPasswordManager.Instance.Init();

// Read admin password from environment variable, set it if not empty
var adminPassword = Environment.GetEnvironmentVariable("GLASH_ADMIN_PASSWORD");
if (!string.IsNullOrEmpty(adminPassword))
{
    GlashAgent.Core.LoginPasswordManager.Instance.SetLoginPassword(adminPassword);
    Console.WriteLine("Admin password set from environment variable.");
}

GlashAgentManager.Instance.Init();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

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
