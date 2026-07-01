using Microsoft.AspNetCore.Components;
using Quick.Blazor.Bootstrap;
using Quick.Localize;
using System.ComponentModel.DataAnnotations;

namespace Glash.Blazor.Server.Pages
{
    public partial class Basic : ComponentBase_WithGettextSupport
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Required]
        private string ConnectionPassword { get; set; }
        private ModalAlert modalAlert;

        private static string TextConnectionPassword => Locale<Basic>.GetString("Connection Password");
        private static string TextApiAddress => Locale<Basic>.GetString("Api Address");
        private static string TextOk => Locale<Basic>.GetString("OK");

        protected override void OnInitialized()
        {
            ConnectionPassword = Global.Instance.ConnectionPassword;
        }

        private string getGlashServerUrl()
        {
            var httpUrl = NavigationManager.Uri;
            var httpUri = new Uri(httpUrl);
            string scheme;
            string hostAndPort = null;
            switch (httpUri.Scheme)
            {
                case "http":
                    scheme = "qp.ws";
                    if (httpUri.Port == 80)
                        hostAndPort = httpUri.Host;
                    break;
                case "https":
                    scheme = "qp.wss";
                    if (httpUri.Port == 443)
                        hostAndPort = httpUri.Host;
                    break;
                default:
                    throw new ArgumentException($"Unknown schema: {httpUri.Scheme}");
            }
            if (string.IsNullOrEmpty(hostAndPort))
                hostAndPort = $"{httpUri.Host}:{httpUri.Port}";
            var url = $"{scheme}://{hostAndPort}{httpUri.PathAndQuery}";
            url = url.Substring(0, url.LastIndexOf("/"));
            url += $"{Global.Instance.GlashServerPath}?Password={Global.Instance.ConnectionPassword}";
            return url;
        }

        private void Ok()
        {
            try
            {
                Global.Instance.ConnectionPassword = ConnectionPassword;
                modalAlert.Show(
                    Locale<Basic>.GetString("Success"),
                    Locale<Basic>.GetString("Change connection password success."));
            }
            catch (Exception ex)
            {
                modalAlert.Show(
                    Locale<Basic>.GetString("Error"),
                    ex.Message);
            }
        }
    }
}
