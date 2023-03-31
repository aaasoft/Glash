using Glash.Core.Client;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Quick.Blazor.Bootstrap;
using Quick.Blazor.Bootstrap.Admin;
using Quick.EntityFrameworkCore.Plus;
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
            DuplicateProxyRule,
            EditProxyRule,
            DeleteProxyRule,
            DeleteProxyRuleConfirm,
            Logout,
            LogoutConfirm,
            EnableAll,
            DisableAll,
            System,
            Local,
            Remote,
            DisplayRows,
            DisconnectedFromServer
        }


        [Parameter]
        public INavigator INavigator { get; set; }
        [Parameter]
        public Model.Profile CurrentProfile { get; set; }
        [Parameter]
        public GlashClient GlashClient { get; set; }
        [Parameter]
        public string[] Agents { get; set; }
        private bool isUserLogout = false;
        private ModalAlert modalAlert;
        private ModalWindow modalWindow;
        private ModalPrompt modalPrompt;
        private ModalLoading modalLoading;
        private LogViewControl logViewControl;
                
        private Queue<string> logQueue = new Queue<string>();
        private string Logs;
        private int LogRows = 25;
        private int MAX_LOG_LINES = 1000;

        public static Dictionary<string, object> PrepareParameter(Model.Profile currentProfile, GlashClient glashClient, string[] agents)
        {
            return new Dictionary<string, object>()
            {
                [nameof(CurrentProfile)] = currentProfile,
                [nameof(GlashClient)] = glashClient,
                [nameof(Agents)] = agents
            };
        }

        protected override void OnParametersSet()
        {
            GlashClient.LogPushed += GlashClient_LogPushed;
            GlashClient.Disconnected += GlashClient_Disconnected;
            var agentHashSet = Agents.ToHashSet();
            var proxyRules = ConfigDbContext.CacheContext.Query<Model.ProxyRule>()
                .Where(t => t.ProfileId == CurrentProfile.Id && agentHashSet.Contains(t.Agent))
                .ToArray();
            GlashClient.AddProxyRules(proxyRules);
        }

        private void GlashClient_LogPushed(object sender, string e)
        {
            var line = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: {e}";
            lock (logQueue)
            {
                logQueue.Enqueue(line);
                while (logQueue.Count > MAX_LOG_LINES)
                    logQueue.Dequeue();
                Logs = string.Join(Environment.NewLine, logQueue);
            }
            logViewControl?.SetContent(Logs);
        }

        private void GlashClient_Disconnected(object sender, EventArgs e)
        {
            if (isUserLogout)
                return;

            INavigator.Alert(
                Global.Instance.TextManager.GetText(ClientTexts.Error),
                Global.Instance.TextManager.GetText(Texts.DisconnectedFromServer));
            logout();
        }

        private void logout()
        {
            isUserLogout = true;
            if (GlashClient != null)
            {
                GlashClient.LogPushed -= GlashClient_LogPushed;
                GlashClient.Disconnected -= GlashClient_Disconnected;
                GlashClient.Dispose();
                GlashClient = null;
            }
            Global.Instance.Profile = null;
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

        private void AddProxyRule(string agent)
        {
            modalWindow.Show<Controls.EditProxyRule>(Global.Instance.TextManager.GetText(Texts.AddProxyRule), Controls.EditProxyRule.PrepareParameter(
                new Model.ProxyRule()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    ProfileId = CurrentProfile.Id,
                    LocalIPAddress = "127.0.0.1",
                    LocalPort = 80,
                    RemotePort = 80,
                    Agent = agent
                },
                model =>
                {
                    try
                    {
                        ConfigDbContext.CacheContext.Add(model);
                        GlashClient.AddProxyRule(model);
                        InvokeAsync(StateHasChanged);
                        modalWindow.Close();
                    }
                    catch (Exception ex)
                    {
                        modalAlert.Show(Global.Instance.TextManager.GetText(ClientTexts.Error), ex.Message);
                    }
                }
            ));
        }

        private void DuplicateProxyRule(Model.ProxyRule model)
        {
            modalPrompt.Show(@Global.Instance.TextManager.GetText(Texts.DuplicateProxyRule), model.Name, newName =>
            {
                var newModel = new Model.ProxyRule()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = newName,
                    Agent = model.Agent,
                    ProfileId = model.ProfileId,
                    LocalIPAddress = model.LocalIPAddress,
                    LocalPort = model.LocalPort,
                    RemoteHost = model.RemoteHost,
                    RemotePort = model.RemotePort
                };
                try
                {
                    ConfigDbContext.CacheContext.Add(newModel);
                    GlashClient.AddProxyRule(newModel);
                    InvokeAsync(StateHasChanged);
                    modalWindow.Close();
                }
                catch (Exception ex)
                {
                    modalAlert.Show(Global.Instance.TextManager.GetText(ClientTexts.Error), ex.Message);
                }
            });
        }

        private void EditProxyRule(Model.ProxyRule model)
        {
            var editModel = JsonConvert.DeserializeObject<Model.ProxyRule>(JsonConvert.SerializeObject(model));
            modalWindow.Show<Controls.EditProxyRule>(Global.Instance.TextManager.GetText(Texts.EditProxyRule), Controls.EditProxyRule.PrepareParameter(
                editModel,
                model =>
                {
                    try
                    {
                        model.Name = editModel.Name;
                        model.LocalIPAddress = editModel.LocalIPAddress;
                        model.LocalPort = editModel.LocalPort;
                        model.RemoteHost = editModel.RemoteHost;
                        model.RemotePort = editModel.RemotePort;
                        GlashClient.RemoveProxyRule(model.Id);
                        ConfigDbContext.CacheContext.Update(model);
                        GlashClient.AddProxyRule(model);
                        InvokeAsync(StateHasChanged);
                        modalWindow.Close();
                    }
                    catch (Exception ex)
                    {
                        modalAlert.Show(Global.Instance.TextManager.GetText(ClientTexts.Error), ex.Message);
                    }
                }
            ));
        }

        private void DeleteProxyRule(Model.ProxyRule model)
        {
            modalAlert.Show(
                Global.Instance.TextManager.GetText(Texts.DeleteProxyRule),
                Global.Instance.TextManager.GetText(Texts.DeleteProxyRuleConfirm, model.Name),
                () =>
                {
                    try
                    {
                        GlashClient.RemoveProxyRule(model.Id);
                        ConfigDbContext.CacheContext.Remove(model);
                        InvokeAsync(StateHasChanged);
                    }
                    catch (Exception ex)
                    {
                        Task.Delay(100).ContinueWith(t =>
                        {
                            modalAlert.Show(Global.Instance.TextManager.GetText(ClientTexts.Error), ex.Message);
                        });
                    }
                });
        }

        private async Task onProxyRuleEnableChanged(ProxyRuleContext proxyRuleContext)
        {
            await Task.Delay(100);
            try
            {
                if (proxyRuleContext.Config.Enable)
                    GlashClient.EnableProxyRule(proxyRuleContext);
                else
                    GlashClient.DisableProxyRule(proxyRuleContext);
            }
            catch (Exception ex)
            {
                modalAlert.Show(Global.Instance.TextManager.GetText(ClientTexts.Error), ex.Message);
                await Task.Delay(100);
                await InvokeAsync(StateHasChanged);
            }
        }

        private ProxyRuleContext[] GetProxyRuleContexts(string agent)
        {
            return GlashClient.ProxyRuleContexts
                .Where(t => t.Config.Agent == agent)
                .OrderBy(t => t.Config.Name)
                .ToArray();
        }

        private void EnableAllProxyRules(string agent)
        {
            foreach (var item in GetProxyRuleContexts(agent))
                try { GlashClient.EnableProxyRule(item); }
                catch { }
            InvokeAsync(StateHasChanged);
        }

        private void DisableAllProxyRules(string agent)
        {
            foreach (var item in GetProxyRuleContexts(agent))
                try { GlashClient.DisableProxyRule(item); }
                catch { }
            InvokeAsync(StateHasChanged);
        }
    }
}
