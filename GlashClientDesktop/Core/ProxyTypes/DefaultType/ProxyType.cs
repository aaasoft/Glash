using Avalonia.Controls;
using Glash.Client;
using Quick.Localize;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace GlashClientDesktop.Core.ProxyTypes.DefaultType
{
    [JsonSerializable(typeof(ProxyType))]
    internal partial class DefaultTypeSerializerContext : JsonSerializerContext { }

    public class ProxyType : AbstractProxyType
    {
        protected override JsonTypeInfo ProxyTypeJsonTypeInfo => DefaultTypeSerializerContext.Default.ProxyType;
        public override Control GetUI() => null;
        public override object GetIcon() => Avalonia.Application.Current.FindResource("SemiIconGlobe");
        public override string GetName() => Locale<ProxyType>.GetString("Default");

        public override ProxyTypeButton[] GetButtons(ProxyRuleContext t) => [];
    }
}
