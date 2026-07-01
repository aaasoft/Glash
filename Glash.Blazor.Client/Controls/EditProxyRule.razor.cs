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

        private static string TextName=>Locale<EditProxyRule>.GetString("Name");
        private static string TextAgent=>Locale<EditProxyRule>.GetString("Agent");        
        private static string TextListen=>Locale<EditProxyRule>.GetString("Listen");
        private static string TextProxy=>Locale<EditProxyRule>.GetString("Proxy");
        private static string TextOk=>Locale<EditProxyRule>.GetString("OK");

        private void Ok()
        {
            OkAction?.Invoke(Model);
        }
    }
}
