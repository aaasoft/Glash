using Glash.Core;
using Glash.Server;
using Quick.EntityFrameworkCore.Plus;
using Quick.Protocol;
using Quick.Protocol.WebSocket.Server.AspNetCore;

namespace Microsoft.AspNetCore.Builder
{
    public static class GlashServerMiddlewareExtensions
    {
        private static QpWebSocketServer qpServer;
        public static GlashServer GlashServer { get; private set; }        
        public static QpServerOptions ServerOptions { get; private set; }

        public static IApplicationBuilder UseGlashServer(this IApplicationBuilder app, string path, string password, int maxTunnelCount = 100)
        {
            var serverOptions = new QpWebSocketServerOptions()
            {
                Path = path,
                Password = password,
                ServerProgram = "Glash.Server"
            };
            ServerOptions = serverOptions;
            GlashServer = new GlashServer(new GlashServerOptions()
            {
                MaxTunnelCount = maxTunnelCount,
                AgentRegisterValidator = rvi =>
                {
                    var model = ConfigDbContext.CacheContext
                        .Find(new Glash.Blazor.Server.Model.AgentInfo(rvi.Name));
                    if (model == null)
                        return false;
                    var answer = CryptoUtils.GetAnswer(rvi.Question, model.Password);
                    return answer == rvi.Answer;
                },
                ClientRegisterValidator = rvi =>
                {
                    var model = ConfigDbContext.CacheContext
                        .Find(new Glash.Blazor.Server.Model.ClientInfo(rvi.Name));
                    if (model == null)
                        return false;
                    var answer = CryptoUtils.GetAnswer(rvi.Question, model.Password);
                    return answer == rvi.Answer;
                },
                GetClientRelateAgentsFunc = clientName =>
                {
                    return ConfigDbContext.CacheContext
                        .Query<Glash.Blazor.Server.Model.ClientAgentRelation>()
                        .Where(t => t.ClientName == clientName)
                        .Select(t => t.AgentName)
                        .ToArray();
                },
                IsClientRelateAgentFunc = (client, agent) =>
                {
                    var model = ConfigDbContext.CacheContext.Find(new Glash.Blazor.Server.Model.ClientAgentRelation()
                    {
                        ClientName = client,
                        AgentName = agent
                    });
                    return model != null;
                }
            });
            GlashServer.AgentConnected += GlashServer_AgentConnected;
            GlashServer.AgentDisconnected += GlashServer_AgentDisconnected;
            GlashServer.ClientConnected += GlashServer_ClientConnected;
            GlashServer.ClientDisconnected += GlashServer_ClientDisconnected;

            GlashServer.HandleServerOptions(serverOptions);
            app.UseQuickProtocol(serverOptions, out qpServer);
            qpServer.Start();
            return app;
        }


        private static void GlashServer_AgentConnected(object sender, GlashAgentContext e)
        {
            var agentInfo = ConfigDbContext.CacheContext.Find(new Glash.Blazor.Server.Model.AgentInfo(e.Name));
            if (agentInfo == null)
                return;
            agentInfo.Context = e;
        }

        private static void GlashServer_AgentDisconnected(object sender, GlashAgentContext e)
        {
            var agentInfo = ConfigDbContext.CacheContext.Find(new Glash.Blazor.Server.Model.AgentInfo(e.Name));
            if (agentInfo == null)
                return;
            agentInfo.Context = null;
        }

        private static void GlashServer_ClientConnected(object sender, GlashClientContext e)
        {
            var clientInfo = ConfigDbContext.CacheContext.Find(new Glash.Blazor.Server.Model.ClientInfo(e.Name));
            if (clientInfo == null)
                return;
            clientInfo.Context = e;
        }

        private static void GlashServer_ClientDisconnected(object sender, GlashClientContext e)
        {
            var clientInfo = ConfigDbContext.CacheContext.Find(new Glash.Blazor.Server.Model.ClientInfo(e.Name));
            if (clientInfo == null)
                return;
            clientInfo.Context = null;
        }
    }
}