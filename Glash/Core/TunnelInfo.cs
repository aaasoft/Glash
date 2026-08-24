namespace Glash.Core
{
    public class TunnelInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 客户端通道包类型
        /// </summary>
        public byte ClientTunnelPackageType { get; set; }
        /// <summary>
        /// 代理端通道包类型
        /// </summary>
        public byte AgentTunnelPackageType { get; set; }

        public string Agent { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}