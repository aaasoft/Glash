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
        private static string TextConnection => Locale<Main>.GetString("Connection");
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
        private ConnectionContext[] ConnectionContexts => ConnectionContextManager.Instance.GetConnectionContexts();
        //当前配置上下文
        private ConnectionContext CurrentConnectionContext;
        private AgentInfo CurrentAgent;

        private string _CurrentConnectionId;
        public string CurrentConnectionId
        {
            get { return _CurrentConnectionId; }
            set
            {
                _CurrentConnectionId = value;
                if (CurrentConnectionContext != null)
                {
                    CurrentConnectionContext.ConnectedChanged -= connectionContext_ConnectedChanged;
                }
                CurrentConnectionContext = null;
                if (!string.IsNullOrEmpty(value))
                {
                    CurrentConnectionContext = ConnectionContextManager.Instance.Get(value);
                    CurrentConnectionContext.ConnectedChanged += connectionContext_ConnectedChanged;
                }
                CurrentAgentName = CurrentConnectionContext?.Agents?.FirstOrDefault()?.AgentName;
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
                    CurrentAgent = CurrentConnectionContext?.Agents?.FirstOrDefault(t => t.AgentName == value);
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
            CurrentConnectionId = ConnectionContextManager.Instance.GetConnectionContexts()?.FirstOrDefault()?.Connection?.Id;
            autoRefreshTimer = new Timer(autoRefresh, null, 1000, 1000);
        }

        private void AddConnection()
        {
            modalWindow.Show(TextAdd,
                new DialogParameters<Controls.EditConnection>()
                {
                    {x=>x.Model,new Model.Connection(Guid.NewGuid().ToString("N"))},
                    {x=>x.OkAction, model =>
                        {
                            try
                            {
                                ConnectionContextManager.Instance.Add(model);
                                CurrentConnectionId = model.Id;
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

        private void connectionContext_ConnectedChanged(object sender, bool e)
        {
            if (e)
                CurrentAgentName = CurrentConnectionContext?.Agents?.FirstOrDefault()?.AgentName;
            else
                CurrentAgentName = null;
            InvokeAsync(StateHasChanged);
        }

        private void ShowLogs()
        {
            var model = CurrentConnectionContext.Connection;
            var context = ConnectionContextManager.Instance.GetContext(model);
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

        private void EditConnection()
        {
            var model = CurrentConnectionContext.Connection;
            modalWindow.Show(TextEdit,
                new DialogParameters<Controls.EditConnection>()
                {
                    { x=>x.Model,JsonSerializer.Deserialize<Model.Connection>(JsonSerializer.Serialize(model))},
                    {x=>x.OkAction,editModel =>
                        {
                            try
                            {
                                CurrentConnectionId = null;
                                model.Name = editModel.Name;
                                model.ServerUrl = editModel.ServerUrl;
                                model.User = editModel.User;
                                model.Password = editModel.Password;
                                ConnectionContextManager.Instance.Update(model);
                                CurrentConnectionId=model.Id;
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

        private void DeleteConnection()
        {
            var model = CurrentConnectionContext.Connection;
            modalAlert.Show(TextDelete, Locale<Main>.GetString("Are you sure to delete Connection[{0}]?", model.Name), new()
            {
                OkCallback = () =>
                {
                    try
                    {
                        ConnectionContextManager.Instance.Remove(model);
                        CurrentConnectionId = ConnectionContextManager.Instance.GetConnectionContexts()?.FirstOrDefault()?.Connection.Id;
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
                                await CurrentConnectionContext.AddProxyRule(model);
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
                    await CurrentConnectionContext.DuplicateProxyRule(newModel);
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

                            await CurrentConnectionContext.EditProxyRule(model);
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
                            await CurrentConnectionContext.DeleteProxyRule(model);
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
                await CurrentConnectionContext.GlashClient.DisableProxyRule(proxyRuleContext.Config.Id);
            else
                await CurrentConnectionContext.GlashClient.EnableProxyRule(proxyRuleContext.Config.Id);
            await InvokeAsync(StateHasChanged);
        }

        private ProxyRuleContext[] GetProxyRuleContexts(string agent)
        {
            var ret = CurrentConnectionContext?.GlashClient?.ProxyRuleContexts?
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
            foreach (var connectionContext in ConnectionContextManager.Instance.GetConnectionContexts())
            {
                connectionContext.ConnectedChanged -= connectionContext_ConnectedChanged;
            }
            CurrentAgentName = null;
            CurrentConnectionId = null;
            base.Dispose();
        }
    }
}
