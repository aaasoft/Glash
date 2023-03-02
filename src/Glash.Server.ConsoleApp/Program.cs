using System.Net;

var qpServerOptions = new Quick.Protocol.Tcp.QpTcpServerOptions()
{
    Address = IPAddress.Any,
    Port = 3011,
    Password = "123456"
};
var qpServer = new Quick.Protocol.Tcp.QpTcpServer(qpServerOptions);
var glashServer = new Glash.Core.Server.GlashServer();
glashServer.Init(qpServer);
glashServer.Start();
Console.WriteLine("Start finish.");
Console.ReadLine();
