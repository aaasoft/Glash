using Microsoft.AspNetCore.Components;

namespace Glash.Blazor.Client.ProxyTypes
{
    public abstract class AbstractProxyType<TUI> : IProxyType
    {
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
