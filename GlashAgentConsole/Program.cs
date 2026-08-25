using Glash.Agent;

// Read environment variables
var serverUrl = Environment.GetEnvironmentVariable("GLASH_SERVER_URL");
var agentName = Environment.GetEnvironmentVariable("GLASH_AGENT_NAME");
var agentPassword = Environment.GetEnvironmentVariable("GLASH_AGENT_PASSWORD");

// Validate required environment variables
if (string.IsNullOrEmpty(serverUrl))
{
    Console.WriteLine("Error: GLASH_SERVER_URL environment variable is required.");
    Console.WriteLine("Example: GLASH_SERVER_URL=ws://your-server:6000/glash");
    return 1;
}
if (string.IsNullOrEmpty(agentName))
{
    Console.WriteLine("Error: GLASH_AGENT_NAME environment variable is required.");
    return 1;
}
if (string.IsNullOrEmpty(agentPassword))
{
    Console.WriteLine("Error: GLASH_AGENT_PASSWORD environment variable is required.");
    return 1;
}

Console.WriteLine($"Glash Agent Console");
Console.WriteLine($"Server URL: {serverUrl}");
Console.WriteLine($"Agent Name: {agentName}");
Console.WriteLine();

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
    Console.WriteLine("Shutting down...");
};

var glashAgent = new GlashAgent(serverUrl, agentName, agentPassword);
glashAgent.LogPushed += (s, msg) => Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {msg}");
glashAgent.Disconnected += (s, e) =>
{
    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Disconnected. Reconnecting in 5 seconds...");
    _ = Task.Run(async () =>
    {
        await Task.Delay(5000, cts.Token);
        await ConnectAsync(glashAgent, cts.Token);
    });
};

// Initial connection
await ConnectAsync(glashAgent, cts.Token);

// Keep running until cancellation
try
{
    await Task.Delay(Timeout.Infinite, cts.Token);
}
catch (OperationCanceledException) { }

glashAgent.Dispose();
Console.WriteLine("Glash Agent Console stopped.");
return 0;

static async Task ConnectAsync(GlashAgent glashAgent, CancellationToken token)
{
    try
    {
        await glashAgent.ConnectAsync();
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Connected to server.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Connection failed: {ex.Message}");
        if (!token.IsCancellationRequested)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(5000, token);
                await ConnectAsync(glashAgent, token);
            });
        }
    }
}
