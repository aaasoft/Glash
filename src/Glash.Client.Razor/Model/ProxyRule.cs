using Quick.EntityFrameworkCore.Plus;
using Quick.Localize;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glash.Client.Razor.Model
{
    public class ProxyRule : BaseModel
    {
        [TextResource]
        public enum Texts
        {
            Name,
            Agent,
            LocalIPAddress,
            LocalPort,
            RemoteHost,
            RemotePort
        }

        public string Name { get; set; }
        public string Agent { get; set; }
        public string LocalIPAddress { get; set; }
        public int LocalPort { get; set; }
        public string RemoteHost { get; set; }
        public int RemotePort { get; set; }
    }
}
