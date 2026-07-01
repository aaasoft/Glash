using Microsoft.AspNetCore.Components;
using Quick.Blazor.Bootstrap;
using Quick.Localize;

namespace Glash.Blazor.Agent.Controls
{
    public partial class EditProfile : ComponentBase_WithGettextSupport
    {
        [Parameter]
        public Model.Profile Model { get; set; }
        [Parameter]
        public Action<Model.Profile> OkAction { get; set; }

        private static string TextName => Locale<EditProfile>.GetString("Name");
        private static string TextServerUrl => Locale<EditProfile>.GetString("Server Url");
        private static string TextAgentName => Locale<EditProfile>.GetString("Agent Name");
        private static string TextAgentPassword => Locale<EditProfile>.GetString("Agent Password");
        private static string TextOk => Locale<EditProfile>.GetString("OK");

        private void Ok()
        {
            OkAction?.Invoke(Model);
        }
    }
}
