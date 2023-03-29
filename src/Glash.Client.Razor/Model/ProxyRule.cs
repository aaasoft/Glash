﻿using Glash.Core.Client;
using Quick.EntityFrameworkCore.Plus;
using Quick.Localize;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glash.Client.Razor.Model
{
    [ModelMeta("ProxyRule")]
    public class ProxyRule : BaseModel, IProxyRule, IHasDependcyRelation
    {
        [TextResource]
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
        [NotMapped]
        public bool Enable { get; set; } = false;

        public override string ToString()
        {
            return $"{Global.Instance.TextManager.GetText(Texts.ModelName)}[{Name}]";
        }

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
