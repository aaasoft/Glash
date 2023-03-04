using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Core.Client
{
    public class ProxyPortContext
    {
        public ProxyPortInfo Config { get; private set; }
        private TcpListener tcpListener;
        private UdpClient udpListener;

        public ProxyPortContext(ProxyPortInfo config)
        {
            Config = config;
        }

        public void Start()
        {
            switch (Config.ProtocolType)
            {
                case Model.ProtocolType.TCP:
                    tcpListener = new TcpListener(IPAddress.Parse(Config.LocalIPAddress), Config.LocalPort);                    
                    break;
                case Model.ProtocolType.UDP:
                    
                    break;
            }
        }

        public void Stop()
        {

        }
    }
}
