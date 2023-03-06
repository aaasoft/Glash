using Glash.Core.Server;
using Quick.Protocol;

namespace Microsoft.AspNetCore.Builder
{
    public static class GlashServerMiddlewareExtensions
    {
        private static GlashServer glashServer;
        private static Quick.Protocol.WebSocket.Server.AspNetCore.QpWebSocketServer qpServer;

        public static IApplicationBuilder UseGlashServer(this IApplicationBuilder app, string path, string password)
        {
            var serverOptions = new Quick.Protocol.WebSocket.Server.AspNetCore.QpWebSocketServerOptions()
            {
                Path = path,
                Password = password,
                ServerProgram = "Glash.Server"
            };
            glashServer = new GlashServer();
            glashServer.HandleServerOptions(serverOptions);

            app.UseQuickProtocol(serverOptions, out qpServer);
            qpServer.Start();
            return app;
        }
    }
}
