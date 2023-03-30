using Microsoft.AspNetCore.Components;

namespace Glash.Server.BlazorApp.Controls
{
    public partial class EditClientInfo
    {
        [Parameter]
        public Model.ClientInfo Model { get; set; }

        [Parameter]
        public Action<Model.ClientInfo> OkAction { get; set; }

        private void Ok()
        {
            OkAction?.Invoke(Model);
        }

        public static Dictionary<string, object> PrepareParameter(Model.ClientInfo model, Action<Model.ClientInfo> okAction)
        {
            return new Dictionary<string, object>()
            {
                [nameof(Model)] = model,
                [nameof(OkAction)] = okAction,
            };
        }
    }
}
