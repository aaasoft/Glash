using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Blazor.Client.ProxyTypes.RDP
{
    [ProxyType(typeof(Texts), nameof(Texts.ProxyTypeName))]
    public class ProxyType : AbstractProxyType<UI>
    {
        public enum Texts
        {
            ProxyTypeName,
            User,
            Password
        }

        public string User { get; set; }
        public string Password { get; set; }
    }
}
