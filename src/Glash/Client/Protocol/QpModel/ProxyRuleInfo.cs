using System.ComponentModel.DataAnnotations;
using Glash.Core;

namespace Glash.Client.Protocol.QpModel
{
    public class ProxyRuleInfo:PropertyNotifyModel
    {
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
        public string ProxyType { get; set; }
        public string ProxyTypeConfig { get; set; }

        private bool _Enable = false;
        public bool Enable
        {
            get => _Enable;
            set => RaiseAndSetIfChanged(ref _Enable, value);
        }

        public override string ToString()
        {
            return $"ProxyRule[{Name}]";
        }
    }
}
