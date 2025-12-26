using System;
using System.Threading;
using System.Threading.Tasks;
using Glash.Agent;

namespace GlashAgentWorker;

public class Program
{
    private static CancellationTokenSource cts;
    private static GlashAgent glashAgent;

    public static async Task Main(string[] args)
    {
        Quick.Protocol.QpAllClients.RegisterUriSchema();
        // Read configuration from environment variables
        var serverUrl = Environment.GetEnvironmentVariable("GLASH_SERVER_URL") ?? throw new ArgumentNullException("GLASH_SERVER_URL", "GLASH_SERVER_URL environment variable is required.");
        var agentName = Environment.GetEnvironmentVariable("GLASH_AGENT_NAME") ?? throw new ArgumentNullException("GLASH_AGENT_NAME", "GLASH_AGENT_NAME environment variable is required.");
        var agentPassword = Environment.GetEnvironmentVariable("GLASH_AGENT_PASSWORD") ?? throw new ArgumentNullException("GLASH_AGENT_PASSWORD", "GLASH_AGENT_PASSWORD environment variable is required.");

        Console.WriteLine($"Starting Glash Agent Worker...");
        Console.WriteLine($"Server URL: {serverUrl}");
        Console.WriteLine($"Agent Name: {agentName}");

        cts = new CancellationTokenSource();
        
        // Initialize GlashAgent
        glashAgent = new GlashAgent(serverUrl, agentName, agentPassword);
        glashAgent.LogPushed += GlashAgent_LogPushed;
        glashAgent.Disconnected += GlashAgent_Disconnected;

        // Start connecting
        await beginConnect(cts.Token);

        // Wait for cancellation
        Console.WriteLine("Glash Agent Worker started. Press Ctrl+C to stop.");
        await Task.Delay(Timeout.Infinite, cts.Token);
    }

    private static void GlashAgent_LogPushed(object sender, string e)
    {
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {e}");
    }

    private static void GlashAgent_Disconnected(object sender, EventArgs e)
    {
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Disconnected from server. Will reconnect in 5 seconds...");
        _ = delayToConnect(cts.Token);
    }

    private static async Task beginConnect(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Connecting to server...");
                await glashAgent.ConnectAsync();
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Connected to server successfully.");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Connection failed: {ex.Message}");
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    private static async Task delayToConnect(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        await Task.Delay(5000, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;

        await beginConnect(cancellationToken);
    }
}
