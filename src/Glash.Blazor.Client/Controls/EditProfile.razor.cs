using Microsoft.AspNetCore.Components;
using Quick.Blazor.Bootstrap;
using Quick.Localize;

namespace Glash.Blazor.Client.Controls
{
    public partial class EditProfile : ComponentBase_WithGettextSupport
    {
        [Parameter]
        public Model.Profile Model { get; set; }
        [Parameter]
        public Action<Model.Profile> OkAction { get; set; }

        private static string TextName=>Locale<EditProfile>.GetString("Name");
        private static string TextServerUrl=>Locale<EditProfile>.GetString("Server Url");
        private static string TextClientName=>Locale<EditProfile>.GetString("Client Name");
        private static string TextClientPassword=>Locale<EditProfile>.GetString("Client Password");
        private static string TextOk=>Locale<EditProfile>.GetString("OK");

        private void Ok()
        {
            OkAction?.Invoke(Model);
        }
    }
}
