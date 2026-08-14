using Avalonia.Controls;
using Glash.Client;
using Quick.Localize;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace GlashClientDesktop.Core.ProxyTypes
{
    [JsonSerializable(typeof(DefaultType))]
    internal partial class DefaultTypeSerializerContext : JsonSerializerContext { }

    public class DefaultType : AbstractProxyType
    {
        protected override JsonTypeInfo ProxyTypeJsonTypeInfo => DefaultTypeSerializerContext.Default.DefaultType;
        public override Control GetUI() => null;
        public override object GetIcon() => Avalonia.Application.Current.FindResource("SemiIconGlobe");
        public override string GetName() => Locale<Web>.GetString("Default");

        public override ProxyTypeButton[] GetButtons(ProxyRuleContext t) => [];
    }
}
