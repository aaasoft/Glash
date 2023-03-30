using Glash.Core.Server;

namespace Microsoft.AspNetCore.Builder
{
    public static class GlashServerMiddlewareExtensions
    {
        private static GlashServer glashServer;
        private static Quick.Protocol.WebSocket.Server.AspNetCore.QpWebSocketServer qpServer;

        public static IApplicationBuilder UseGlashServer(this IApplicationBuilder app, string path, string password, int maxTunnelCount = 100)
        {
            var serverOptions = new Quick.Protocol.WebSocket.Server.AspNetCore.QpWebSocketServerOptions()
            {
                Path = path,
                Password = password,
                ServerProgram = "Glash.Server"
            };
            glashServer = new GlashServer(maxTunnelCount);
            glashServer.HandleServerOptions(serverOptions);

            app.UseQuickProtocol(serverOptions, out qpServer);
            qpServer.Start();
            return app;
        }
    }
}