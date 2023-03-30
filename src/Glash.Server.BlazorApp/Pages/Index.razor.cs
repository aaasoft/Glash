using Microsoft.AspNetCore.Components;

namespace Glash.Server.BlazorApp.Pages
{
    public partial class Index
    {
        public enum Texts
        {
            Title,
            Dashboard,
            AgentManage,
            ClientManage
        }

        [Parameter]
        public RenderFragment Body { get; set; }

        private void Show<T>()
        {
            Body = Quick.Blazor.Bootstrap.Utils.BlazorUtils.GetRenderFragment<T>();
        }
    }
}
