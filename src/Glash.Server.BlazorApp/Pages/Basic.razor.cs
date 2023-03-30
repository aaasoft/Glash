using Quick.Blazor.Bootstrap;
using System.ComponentModel.DataAnnotations;

namespace Glash.Server.BlazorApp.Pages
{
    public partial class Basic
    {
        [Required]
        private string ConnectionPassword { get; set; }
        private ModalAlert modalAlert;

        public enum Texts
        {
            ConnectionPassword,
            ApiAddress,
            ChangeConnectionPasswordSuccess
        }

        protected override void OnInitialized()
        {
            ConnectionPassword = Global.Instance.ConnectionPassword;
        }

        private string getGlashServerUrl()
        {
            var httpUrl = NavigationManager.Uri;
            var httpUri = new Uri(httpUrl);
            return $"qp.ws://{httpUri.Host}:{httpUri.Port}{httpUri.PathAndQuery}glash?Password={Global.Instance.ConnectionPassword}";
        }

        private void Ok()
        {
            try
            {
                Global.Instance.ConnectionPassword = ConnectionPassword;
                modalAlert.Show(
                    Global.Instance.TextManager.GetText(ServerTexts.Success),
                    Global.Instance.TextManager.GetText(Texts.ChangeConnectionPasswordSuccess));
            }
            catch (Exception ex)
            {
                modalAlert.Show(
                    Global.Instance.TextManager.GetText(ServerTexts.Error),
                    ex.Message);
            }
        }
    }
}
