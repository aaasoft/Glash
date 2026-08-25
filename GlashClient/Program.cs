using Glash.Blazor.Client;
using Quick.LiteDB.Plus;

Quick.Protocol.WebSocket.Client.QpWebSocketClientOptions.RegisterUriSchema();
Quick.Protocol.Http.Client.QpHttpClientOptions.RegisterUriSchema();

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
Glash.Blazor.Client.Core.ConnectionContextManager.Instance.Init();
GlashClient.Core.LoginPasswordManager.Instance.Init();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapRazorComponents<GlashClient.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();