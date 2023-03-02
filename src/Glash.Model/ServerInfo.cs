using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Model
{
    public class ServerInfo
    {
        public int AgentCount { get; set; }
        public int ClientCount { get; set; }
        public int TunnelCount { get; set; }
        public string UploadSpeed { get; set; }
        public string DownloadSpeed { get; set; }
    }
}
