using Microsoft.AspNetCore.Components;

namespace Glash.Agent.BlazorApp.Pages
{
    public partial class Index
    {
        public enum Texts
        {
            Title,
            ProfileManage
        }

        [Parameter]
        public RenderFragment Body { get; set; }

        private void Show<T>()
        {
            Body = Quick.Blazor.Bootstrap.Utils.BlazorUtils.GetRenderFragment<T>();
        }
    }
}
