using Quick.Protocol;
using System.ComponentModel;
using System.Text.Json.Serialization.Metadata;

namespace Glash.Client.Protocol.QpCommands.CreateTunnel
{
    [DisplayName("Create Tunnel")]
    public class Request : AbstractQpSerializer<Request>, IQpCommandRequest<Request, Response>
    {
        protected override JsonTypeInfo<Request> GetTypeInfo() => CreateTunnelCommandSerializerContext.Default.Request;
        public string ProxyRuleId { get; set; }
        /// <summary>
        /// 服务器发给客户端的通道包类型
        /// </summary>
        public byte ServerToClientTunnelPackageType { get; set; }
    }
}
