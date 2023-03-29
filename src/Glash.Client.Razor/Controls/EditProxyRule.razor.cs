using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Client.Razor.Controls
{
    public partial class EditProxyRule
    {
        [Parameter]
        public Model.ProxyRule Model { get; set; }
        [Parameter]
        public Action<Model.ProxyRule> OkAction { get; set; }

        private void Ok()
        {
            OkAction?.Invoke(Model);
        }

        public static Dictionary<string, object> PrepareParameter(Model.ProxyRule model, Action<Model.ProxyRule> okAction)
        {
            return new Dictionary<string, object>()
            {
                [nameof(Model)] = model,
                [nameof(OkAction)] = okAction,
            };
        }
    }
}
