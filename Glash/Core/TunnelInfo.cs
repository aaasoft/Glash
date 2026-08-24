namespace Glash.Core
{
    public class TunnelInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 客户端到服务端通道包类型
        /// </summary>
        public byte ClientToServerTunnelPackageType { get; set; }
        /// <summary>
        /// 服务端到客户端通道包类型
        /// </summary>
        public byte ServerToClientTunnelPackageType { get; set; }
        /// <summary>
        /// 代理端到服务端通道包类型
        /// </summary>
        public byte AgentToServerTunnelPackageType { get; set; }
        /// <summary>
        /// 服务端到代理端通道包类型
        /// </summary>
        public byte ServerToAgentTunnelPackageType { get; set; }

        public string Agent { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}