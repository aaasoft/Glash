using System.Text.Json;
using Glash.Blazor.Client.Core;
using Glash.Client;
using Glash.Client.Protocol.QpModel;
using Quick.Blazor.Bootstrap;
using Quick.Localize;

namespace Glash.Blazor.Client
{
    public partial class Main : ComponentBase_WithGettextSupport
    {
        private static string TextProfile => Locale<Main>.GetString("Profile");
        private static string TextAdd => Locale<Main>.GetString("Add");
        private static string TextLogs => Locale<Main>.GetString("Logs");
        private static string TextEdit => Locale<Main>.GetString("Edit");
        private static string TextDuplicate => Locale<Main>.GetString("Duplicate");
        private static string TextDelete => Locale<Main>.GetString("Delete");
        private static string TextAgent => Locale<Main>.GetString("Agent");
        private static string TextError => Locale<Main>.GetString("Error");
        private static string TextAddProxyRule => Locale<Main>.GetString("Add Proxy Rule");
        private static string TextDuplicateProxyRule => Locale<Main>.GetString("Duplicate Proxy Rule");
        private static string TextEditProxyRule => Locale<Main>.GetString("Edit Proxy Rule");
        private static string TextDeleteProxyRule => Locale<Main>.GetString("Delete Proxy Rule");
        private static string TextListen => Locale<Main>.GetString("Listen");
        private static string TextProxy => Locale<Main>.GetString("Proxy");
        private static string TextAgentNotLogin => Locale<Main>.GetString("Agent not login");

        private ModalAlert modalAlert;
        private ModalWindow modalWindow;
        private ModalLoading modalLoading;
        private ModalPrompt modalPrompt;
        private ProfileContext[] ProfileContexts => ProfileContextManager.Instance.GetProfileContexts();
        //当前配置上下文
        private ProfileContext CurrentProfileContext;
        private AgentInfo CurrentAgent;

        private string _CurrentProfileId;
        public string CurrentProfileId
        {
            get { return _CurrentProfileId; }
            set
            {
                _CurrentProfileId = value;
                if (CurrentProfileContext != null)
                {
                    CurrentProfileContext.ConnectedChanged -= profileContext_ConnectedChanged;
                }
                CurrentProfileContext = null;
                if (!string.IsNullOrEmpty(value))
                {
                    CurrentProfileContext = ProfileContextManager.Instance.Get(value);
                    CurrentProfileContext.ConnectedChanged += profileContext_ConnectedChanged;
                }
                CurrentAgentName = CurrentProfileContext?.Agents?.FirstOrDefault()?.AgentName;
            }
        }

        private string _CurrentAgentName;
        private string CurrentAgentName
        {
            get { return _CurrentAgentName; }
            set
            {
                _CurrentAgentName = value;
                if (string.IsNullOrEmpty(value))
                    CurrentAgent = null;
                else
                    CurrentAgent = CurrentProfileContext?.Agents?.FirstOrDefault(t => t.AgentName == value);
            }
        }

        private Timer autoRefreshTimer;

        private void autoRefresh(object state)
        {
            InvokeAsync(StateHasChanged);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            CurrentProfileId = ProfileContextManager.Instance.GetProfileContexts()?.FirstOrDefault()?.Profile?.Id;
            autoRefreshTimer = new Timer(autoRefresh, null, 1000, 1000);
        }

        private void AddProfile()
        {
            modalWindow.Show(TextAdd,
                new DialogParameters<Controls.EditProfile>()
                {
                    {x=>x.Model,new Model.Profile(Guid.NewGuid().ToString("N"))},
                    {x=>x.OkAction, model =>
                        {
                            try
                            {
                                ProfileContextManager.Instance.Add(model);
                                CurrentProfileId = model.Id;
                                InvokeAsync(StateHasChanged);
                                modalWindow.Close();
                            }
                            catch (Exception ex)
                            {
                                modalAlert.Show(TextError, ex.Message);
                            }
                        }
                    }
                });
        }

        private void profileContext_ConnectedChanged(object sender, bool e)
        {
            if (e)
                CurrentAgentName = CurrentProfileContext?.Agents?.FirstOrDefault()?.AgentName;
            else
                CurrentAgentName = null;
            InvokeAsync(StateHasChanged);
        }

        private void ShowLogs()
        {
            var model = CurrentProfileContext.Profile;
            var context = ProfileContextManager.Instance.GetContext(model);
            if (context == null)
            {
                modalAlert.Show(TextError, $"未找到{model}的上下文！");
                return;
            }            
            modalAlert.Show(TextLogs, string.Join(Environment.NewLine, context.Logs), new() { UsePreTag = true });
        }

        private void ShowRuleContextLogs(ProxyRuleContext proxyRuleContext)
        {
            modalAlert.Show(TextLogs, string.Join(Environment.NewLine, proxyRuleContext.Logs), new() { UsePreTag = true });
        }

        private void EditProfile()
        {
            var model = CurrentProfileContext.Profile;
            modalWindow.Show(TextEdit,
                new DialogParameters<Controls.EditProfile>()
                {
                    { x=>x.Model,JsonSerializer.Deserialize<Model.Profile>(JsonSerializer.Serialize(model))},
                    {x=>x.OkAction,editModel =>
                        {
                            try
                            {
                                CurrentProfileId = null;
                                model.Name = editModel.Name;
                                model.ServerUrl = editModel.ServerUrl;
                                model.ClientName = editModel.ClientName;
                                model.ClientPassword = editModel.ClientPassword;
                                ProfileContextManager.Instance.Update(model);
                                CurrentProfileId=model.Id;
                                InvokeAsync(StateHasChanged);
                                modalWindow.Close();
                            }
                            catch (Exception ex)
                            {
                                modalAlert.Show(TextError, ex.Message);
                            }
                        }
                    }
                });
        }

        private void DeleteProfile()
        {
            var model = CurrentProfileContext.Profile;
            modalAlert.Show(TextDelete, Locale<Main>.GetString("Are you sure to delete Profile[{0}]?", model.Name), new()
            {
                OkCallback = () =>
                {
                    try
                    {
                        ProfileContextManager.Instance.Remove(model);
                        CurrentProfileId = ProfileContextManager.Instance.GetProfileContexts()?.FirstOrDefault()?.Profile.Id;
                        InvokeAsync(StateHasChanged);
                    }
                    catch (Exception ex)
                    {
                        Task.Delay(100).ContinueWith(t =>
                        {
                            modalAlert.Show(TextError, ex.Message);
                        });
                    }
                }
            });
        }

        private void AddProxyRule(string agent)
        {
            modalWindow.Show(TextAddProxyRule,
                new DialogParameters<Controls.EditProxyRule>()
                {
                    {x=>x.Model,new ProxyRuleInfo()
                        {
                            LocalIPAddress = "127.0.0.1",
                            LocalPort = 80,
                            RemoteHost = "127.0.0.1",
                            RemotePort = 80,
                            Agent = agent
                        }
                    },
                    {x=>x.OkAction, async model =>
                        {
                            modalLoading.Show(TextAddProxyRule, null, true);
                            try
                            {
                                await CurrentProfileContext.AddProxyRule(model);
                                _ = InvokeAsync(StateHasChanged);
                                modalWindow.Close();
                            }
                            catch (Exception ex)
                            {
                                modalAlert.Show(TextError, ex.Message);
                            }
                            modalLoading.Close();
                        }
                    }
                });
        }

        private void DuplicateProxyRule(ProxyRuleInfo model)
        {
            modalPrompt.Show(TextDuplicateProxyRule, model.Name, async newName =>
            {
                var newModel = new ProxyRuleInfo()
                {
                    Name = newName,
                    Agent = model.Agent,
                    LocalIPAddress = model.LocalIPAddress,
                    LocalPort = model.LocalPort,
                    RemoteHost = model.RemoteHost,
                    RemotePort = model.RemotePort,
                    ProxyType = model.ProxyType,
                    ProxyTypeConfig = model.ProxyTypeConfig
                };
                modalLoading.Show(TextDuplicateProxyRule, null, true);
                try
                {
                    await CurrentProfileContext.DuplicateProxyRule(newModel);
                    _ = InvokeAsync(StateHasChanged);
                    modalWindow.Close();
                }
                catch (Exception ex)
                {
                    modalAlert.Show(TextError, ex.Message);
                }
                modalLoading.Close();
            });
        }

        private void EditProxyRule(ProxyRuleInfo model)
        {
            modalWindow.Show(Locale<Main>.GetString(TextEditProxyRule),
                new DialogParameters<Controls.EditProxyRule>()
                {
                    {x=>x.Model, JsonSerializer.Deserialize<ProxyRuleInfo>(JsonSerializer.Serialize(model))},
                    {x=>x.OkAction, async editModel =>
                    {
                        modalLoading.Show(Locale<Main>.GetString(TextEditProxyRule), null, true);
                        try
                        {
                            model.Name = editModel.Name;
                            model.LocalIPAddress = editModel.LocalIPAddress;
                            model.LocalPort = editModel.LocalPort;
                            model.RemoteHost = editModel.RemoteHost;
                            model.RemotePort = editModel.RemotePort;
                            model.ProxyType = editModel.ProxyType;
                            model.ProxyTypeConfig = editModel.ProxyTypeConfig;

                            await CurrentProfileContext.EditProxyRule(model);
                            _ = InvokeAsync(StateHasChanged);
                            modalWindow.Close();
                        }
                        catch (Exception ex)
                        {
                            modalAlert.Show(TextError, ex.Message);
                        }
                        modalLoading.Close();
                    }
                    }
                });
        }

        private void DeleteProxyRule(ProxyRuleInfo model)
        {
            modalAlert.Show(
                TextDeleteProxyRule,
                Locale<Main>.GetString("Are you sure to delete ProxyRule[{0}]?", model.Name), new()
                {
                    OkCallback = async () =>
                    {
                        modalLoading.Show(TextDeleteProxyRule, null, true);
                        try
                        {
                            await CurrentProfileContext.DeleteProxyRule(model);
                            _ = InvokeAsync(StateHasChanged);
                        }
                        catch (Exception ex)
                        {
                            _ = Task.Delay(100).ContinueWith(t =>
                            {
                                modalAlert.Show(TextError, ex.Message);
                            });
                        }
                        modalLoading.Close();
                    }
                });
        }

        private async Task onProxyRuleEnableChanged(ProxyRuleContext proxyRuleContext)
        {
            if(proxyRuleContext.Config.Enable)
                await CurrentProfileContext.GlashClient.DisableProxyRule(proxyRuleContext.Config.Id);
            else
                await CurrentProfileContext.GlashClient.EnableProxyRule(proxyRuleContext.Config.Id);
            await InvokeAsync(StateHasChanged);
        }

        private ProxyRuleContext[] GetProxyRuleContexts(string agent)
        {
            var ret = CurrentProfileContext?.GlashClient?.ProxyRuleContexts?
                .Where(t => t.Config.Agent == agent)?
                .OrderBy(t => t.Config.Name)?
                .ToArray();
            if (ret == null)
                ret = new ProxyRuleContext[0];
            return ret;
        }

        public override void Dispose()
        {
            autoRefreshTimer.Dispose();
            foreach (var profileContext in ProfileContextManager.Instance.GetProfileContexts())
            {
                profileContext.ConnectedChanged -= profileContext_ConnectedChanged;
            }
            CurrentAgentName = null;
            CurrentProfileId = null;
            base.Dispose();
        }
    }
}
