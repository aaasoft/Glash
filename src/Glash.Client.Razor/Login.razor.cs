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
                Global.Instance.Profile = null;
                if (!string.IsNullOrEmpty(value))
                    Global.Instance.Profile = ConfigDbContext.CacheContext.Find(new Model.Profile(value));
            }
        }

        private void ShowProfileManageWindow()
        {
            CurrentProfileId = null;
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
                var glashClient = new GlashClient(Global.Instance.Profile.ServerUrl);
                await glashClient.ConnectAsync(Global.Instance.Profile.Account, Global.Instance.Profile.Password);
                var agentList = await glashClient.GetAgentListAsync();
                agentList = agentList.OrderBy(t => t).ToArray();
                Global.Instance.GlashClient = glashClient;
                INavigator.Navigate<Main>(Main.PrepareParameter(Global.Instance.Profile, glashClient, agentList));
            }
            catch (Exception ex)
            {
                Global.Instance.GlashClient?.Dispose();
                Global.Instance.GlashClient = null;
                modalAlert.Show(Global.Instance.TextManager.GetText(ClientTexts.Error), ex.Message);
            }
            modalLoading.Close();
        }
    }
}
