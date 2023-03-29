using Glash.Core.Client;
using Microsoft.AspNetCore.Components;
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

        private string ServerUrl;
        private string User;
        private string Password;
        private ModalWindow modalWindow;
        private ModalAlert modalAlert;
        private ModalLoading modalLoading;

        [Parameter]
        public INavigator INavigator { get; set; }

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
            modalWindow.Show<ProfileManage>(
                Global.Instance.TextManager.GetText(Texts.ProfileManage),
                ProfileManage.PrepareParameter(
                    () => InvokeAsync(StateHasChanged)
                )
            );
        }

        private async Task OnPost()
        {
            modalLoading.Show(null, null, true);
            try
            {
                var glashClient = new GlashClient(ServerUrl);
                await glashClient.ConnectAsync(User, Password);
                var agentList = await glashClient.GetAgentListAsync();

                Global.Instance.GlashClient = glashClient;
                INavigator.Navigate<Main>(Main.PrepareParameter(glashClient, agentList));
            }
            catch (Exception ex)
            {
                Global.Instance.GlashClient?.Dispose();
                Global.Instance.GlashClient = null;
                modalAlert.Show("Error", ex.Message);
            }
            modalLoading.Close();
        }
    }
}
