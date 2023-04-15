using Microsoft.AspNetCore.Components;

namespace Glash.Blazor.Client.ProxyTypes
{
    public abstract class AbstractProxyType<TUI> : IProxyType
    {
        public abstract string Icon { get; }
        public string Name
        {
            get
            {
                return ProxyTypeManager.Instance
                     .GetProxyTypeInfo(this.GetType().FullName)?
                     .Name;
            }
        }

        public abstract ProxyTypeButton[] GetButtons();

        public RenderFragment GetUI()
        {
            return Quick.Blazor.Bootstrap.Utils.BlazorUtils.GetRenderFragment<TUI>(
                new Dictionary<string, object>()
                {
                    ["Model"] = this
                });
        }
    }
}
