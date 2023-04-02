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
        public Func<LoginValidationInfo, bool> AgentLoginValidator { get; set; }
        public Func<LoginValidationInfo, bool> ClientLoginValidator { get; set; }
        public Func<string, string[]> GetClientRelateAgentsFunc { get; set; }
        public Func<string, string, bool> IsClientRelateAgentFunc { get; set; }
    }
}
