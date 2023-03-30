using Microsoft.AspNetCore.Components;

namespace Glash.Server.BlazorApp.Controls
{
    public partial class EditAgentInfo
    {
        [Parameter]
        public Model.AgentInfo Model { get; set; }

        [Parameter]
        public Action<Model.AgentInfo> OkAction { get; set; }

        private void Ok()
        {
            OkAction?.Invoke(Model);
        }

        public static Dictionary<string, object> PrepareParameter(Model.AgentInfo model, Action<Model.AgentInfo> okAction)
        {
            return new Dictionary<string, object>()
            {
                [nameof(Model)] = model,
                [nameof(OkAction)] = okAction,
            };
        }
    }
}
