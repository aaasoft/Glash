using Quick.EntityFrameworkCore.Plus;
using Quick.Localize;
using Quick.Protocol;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glash.Client.Razor.Model
{
    [ModelMeta("ProxyRule")]
    public class ProxyRule : BaseModel, IHasDependcyRelation
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
        [Required]
        public string ProfileId { get; set; }
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

        public ModelDependcyInfo[] GetDependcyRelation()
        {
            return new ModelDependcyInfo[]
            {
                new ModelDependcyInfo<ProxyRule, Profile>
                (
                    source => source.ProfileId == null ? null : new Profile(source.ProfileId),
                    target => t => t.ProfileId == target.Id
                )
            };
        }
    }
}
