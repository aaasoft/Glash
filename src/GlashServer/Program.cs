using System;
using Glash.Blazor.Server;
using Quick.LiteDB.Plus;

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
GlashServer.Core.LoginPasswordManager.Instance.Init();

// Read connection password from environment variable, set it if not empty
var connectionPassword = Environment.GetEnvironmentVariable("GLASH_CONNECTION_PASSWORD");
if (!string.IsNullOrEmpty(connectionPassword))
{
    Global.Instance.ConnectionPassword = connectionPassword;
}

// Read admin password from environment variable, set it if not empty
var adminPassword = Environment.GetEnvironmentVariable("GLASH_ADMIN_PASSWORD");
if (!string.IsNullOrEmpty(adminPassword))
{
    GlashServer.Core.LoginPasswordManager.Instance.SetLoginPassword(adminPassword);
    Console.WriteLine("Admin password set from environment variable.");
}

// Read Glash server path from environment variable, set it if not empty
var glashServerPath = Environment.GetEnvironmentVariable("GLASH_SERVER_PATH");
if (!string.IsNullOrEmpty(glashServerPath))
{
    Global.Instance.GlashServerPath = glashServerPath;
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();
app.UseWebSockets();
// Use configured Glash server path
app.UseGlashServer(Global.Instance.GlashServerPath, Global.Instance.ConnectionPassword);

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
