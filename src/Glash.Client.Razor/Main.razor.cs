using Glash.Core.Client;
using Microsoft.AspNetCore.Components;
using Quick.Blazor.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Client.Razor
{
    public partial class Main
    {
        public enum Texts
        {
            AddProxyRule,
            Logout,
            LogoutConfirm,
            EnableAll,
            DisableAll,
            System
        }


        [Parameter]
        public INavigator INavigator { get; set; }

        [Parameter]
        public GlashClient GlashClient { get; set; }
        [Parameter]
        public string[] Agents { get; set; }
        private bool isUserLogout = false;
        private ModalAlert modalAlert;

        public static Dictionary<string, object> PrepareParameter(GlashClient glashClient, string[] agents)
        {
            return new Dictionary<string, object>()
            {
                [nameof(GlashClient)] = glashClient,
                [nameof(Agents)] = agents
            };
        }

        protected override void OnParametersSet()
        {
            GlashClient.Disconnected += GlashClient_Disconnected;
        }

        private void GlashClient_Disconnected(object sender, EventArgs e)
        {
            if (isUserLogout)
                return;

            INavigator.Alert("GlashClient_Disconnected", "GlashClient_Disconnected");
            Logout();
        }

        private void logout()
        {
            isUserLogout = true;
            GlashClient.Dispose();
            GlashClient = null;
            INavigator.Navigate<Login>();
        }

        private void Logout()
        {
            modalAlert.Show(
                @Global.Instance.TextManager.GetText(Texts.Logout),
                @Global.Instance.TextManager.GetText(Texts.LogoutConfirm),
                () =>
                {
                    logout();
                }
            );
        }
    }
}
