using Microsoft.AspNetCore.Components;
using Quick.Blazor.Bootstrap;
using Quick.Localize;

namespace Glash.Blazor.Client.Controls
{
    public partial class EditConnection : ComponentBase_WithGettextSupport
    {
        [Parameter]
        public Model.Connection Model { get; set; }
        [Parameter]
        public Action<Model.Connection> OkAction { get; set; }

        private static string TextName=>Locale<EditConnection>.GetString("Name");
        private static string TextServerUrl=>Locale<EditConnection>.GetString("Server Url");
        private static string TextUser=>Locale<EditConnection>.GetString("User");
        private static string TextPassword=>Locale<EditConnection>.GetString("Password");
        private static string TextOk=>Locale<EditConnection>.GetString("OK");

        private void Ok()
        {
            OkAction?.Invoke(Model);
        }
    }
}
