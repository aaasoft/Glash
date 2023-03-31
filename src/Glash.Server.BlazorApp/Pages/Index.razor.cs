﻿using Microsoft.AspNetCore.Components;

namespace Glash.Server.BlazorApp.Pages
{
    public partial class Index
    {
        public enum Texts
        {
            Title,
            Basic,
            AgentManage,
            ClientManage,
            TunnelManage
        }

        [Parameter]
        public RenderFragment Body { get; set; }

        private void Show<T>()
        {
            Body = Quick.Blazor.Bootstrap.Utils.BlazorUtils.GetRenderFragment<T>();
        }
    }
}
