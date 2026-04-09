using Glash.Client.Protocol.QpModel;
using Microsoft.AspNetCore.Components;
using Quick.Blazor.Bootstrap;
using Quick.Localize;

namespace Glash.Blazor.Client.Controls
{
    public partial class EditProxyRule : ComponentBase_WithGettextSupport
    {
        [Parameter]
        public ProxyRuleInfo Model { get; set; }

        [Parameter]
        public Action<ProxyRuleInfo> OkAction { get; set; }

        private static string TextName=>Locale.GetString("Name");
        private static string TextAgent=>Locale.GetString("Agent");        
        private static string TextListen=>Locale.GetString("Listen");
        private static string TextProxy=>Locale.GetString("Proxy");
        private static string TextOk=>Locale.GetString("OK");

        private void Ok()
        {
            OkAction?.Invoke(Model);
        }
    }
}
