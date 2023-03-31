using Glash.Core.Server;
using Glash.Core.Utils;
using Quick.EntityFrameworkCore.Plus;
using Quick.Protocol;
using Quick.Protocol.WebSocket.Server.AspNetCore;

namespace Microsoft.AspNetCore.Builder
{
    public static class GlashServerMiddlewareExtensions
    {
        private static GlashServer glashServer;
        private static QpWebSocketServer qpServer;
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
            glashServer = new GlashServer(new GlashServerOptions()
            {
                MaxTunnelCount = maxTunnelCount,
                AgentRegisterValidator = rvi =>
                {
                    var model = ConfigDbContext.CacheContext
                        .Find(new Glash.Server.BlazorApp.Model.AgentInfo(rvi.Name));
                    if (model == null)
                        return false;
                    var answer = CryptoUtils.GetAnswer(rvi.Question, model.Password);
                    return answer == rvi.Answer;
                },
                ClientRegisterValidator = rvi =>
                {
                    var model = ConfigDbContext.CacheContext
                        .Find(new Glash.Server.BlazorApp.Model.ClientInfo(rvi.Name));
                    if (model == null)
                        return false;
                    var answer = CryptoUtils.GetAnswer(rvi.Question, model.Password);
                    return answer == rvi.Answer;
                },
                GetClientRelateAgentsFunc = clientName =>
                {
                    return ConfigDbContext.CacheContext
                        .Query<Glash.Server.BlazorApp.Model.ClientAgentRelation>()
                        .Where(t => t.ClientName == clientName)
                        .Select(t => t.AgentName)
                        .ToArray();
                },
                IsClientRelateAgentFunc = (client, agent) =>
                {
                    var model = ConfigDbContext.CacheContext.Find(new Glash.Server.BlazorApp.Model.ClientAgentRelation()
                    {
                        ClientName = client,
                        AgentName = agent
                    });
                    return model != null;
                }
            });
            glashServer.HandleServerOptions(serverOptions);
            app.UseQuickProtocol(serverOptions, out qpServer);
            qpServer.Start();
            return app;
        }
    }
}