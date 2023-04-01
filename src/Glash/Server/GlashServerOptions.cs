using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Server
{
    public class GlashServerOptions
    {
        public int MaxTunnelCount { get; set; } = 100;
        public Func<RegisterValidationInfo, bool> AgentRegisterValidator { get; set; }
        public Func<RegisterValidationInfo, bool> ClientRegisterValidator { get; set; }
        public Func<string, string[]> GetClientRelateAgentsFunc { get; set; }
        public Func<string, string, bool> IsClientRelateAgentFunc { get; set; }
    }
}
