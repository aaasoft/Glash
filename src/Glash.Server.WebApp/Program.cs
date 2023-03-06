var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseWebSockets();
app.UseGlashServer("/glash", "123456");
app.MapGet("/", () =>
{
    return "Hello Glash.";
});
app.Run("http://127.0.0.1:18520");