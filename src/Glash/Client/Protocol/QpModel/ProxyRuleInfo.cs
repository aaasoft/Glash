using Glash.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glash.Client.Protocol.QpModel
{
    public class ProxyRuleInfo : IProxyRule
    {
        public enum Texts
        {
            ModelName,
            Name,
            Agent,
            LocalIPAddress,
            LocalPort,
            RemoteHost,
            RemotePort
        }

        [Key]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Agent { get; set; }
        [Required]
        public string LocalIPAddress { get; set; }
        [Required]
        [Range(0, 65535)]
        public int LocalPort { get; set; }
        [Required]
        public string RemoteHost { get; set; }
        [Required]
        [Range(0, 65535)]
        public int RemotePort { get; set; }
        [NotMapped]
        public bool Enable { get; set; } = false;
    }
}
