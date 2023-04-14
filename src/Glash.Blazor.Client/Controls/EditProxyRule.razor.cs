using Glash.Client.Protocol.QpModel;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Blazor.Client.Controls
{
    public partial class EditProxyRule
    {
        [Parameter]
        public ProxyRuleInfo Model { get; set; }
        private RenderFragment ProxyTypeUI;

        [Parameter]
        public Action<ProxyRuleInfo> OkAction { get; set; }

        private void Ok()
        {
            OkAction?.Invoke(Model);
        }

        public static Dictionary<string, object> PrepareParameter(ProxyRuleInfo model, Action<ProxyRuleInfo> okAction)
        {
            return new Dictionary<string, object>()
            {
                [nameof(Model)] = model,
                [nameof(OkAction)] = okAction,
            };
        }

        private void onProxyTypeChanged(string value)
        {
            Model.ProxyType = value;
            if (string.IsNullOrEmpty(Model.ProxyType))
                ProxyTypeUI = null;
            else
                ProxyTypeUI = ProxyTypes.ProxyTypeManager.Instance
                    .GetProxyTypeInfo(Model.ProxyType).CreateInstance(Model.ProxyTypeConfig)
                    .GetUI();
            InvokeAsync(StateHasChanged);
        }
    }
}
