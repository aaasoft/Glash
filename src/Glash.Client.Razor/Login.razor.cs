using Quick.Blazor.Bootstrap;
using Quick.EntityFrameworkCore.Plus;
using Quick.Localize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Client.Razor
{
    public partial class Login
    {
        [TextResource]
        public enum Texts
        {
            Title,
            ChooseProfile,
            Login,
            ProfileManage
        }

        private string Message;
        private string ServerUrl;
        private string User;
        private string Password;
        private ModalWindow modalWindow;
        private string _CurrentProfileId;
        public string CurrentProfileId
        {
            get { return _CurrentProfileId; }
            set
            {
                _CurrentProfileId = value;
                Model.Profile CurrentProfile = null;
                if (!string.IsNullOrEmpty(value))
                    CurrentProfile = ConfigDbContext.CacheContext.Find(new Model.Profile(value));
                ServerUrl = CurrentProfile?.ServerUrl;
                User = CurrentProfile?.User;
                Password = CurrentProfile?.Password;
            }
        }

        private void ShowProfileManageWindow()
        {
            modalWindow.Show<Profiles.ProfileManage>(
                Global.Instance.TextManager.GetText(Texts.ProfileManage),
                Profiles.ProfileManage.PrepareParameter(
                    () => InvokeAsync(StateHasChanged)
                )
            );
        }

        private void OnPost()
        {
            
        }
    }
}
