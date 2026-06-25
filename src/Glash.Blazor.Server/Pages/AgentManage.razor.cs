using System.Text.Json;
using Glash.Server;
using Microsoft.AspNetCore.Components;
using Quick.Blazor.Bootstrap;
using Quick.LiteDB.Plus;
using Quick.Localize;

namespace Glash.Blazor.Server.Pages
{
    public partial class AgentManage : ComponentBase_WithGettextSupport
    {
        private ModalWindow modalWindow;
        private ModalAlert modalAlert;

        [Parameter]
        public Action AgentChangedHandler { get; set; }

        private static string TextName => Locale<AgentManage>.GetString("Name");
        private static string TextChannelName => Locale<AgentManage>.GetString("Channel Name");
        private static string TextConnectTime => Locale<AgentManage>.GetString("Connect Time");

        private static string TextOperate => Locale<AgentManage>.GetString("Operate");
        private static string TextAdd => Locale<AgentManage>.GetString("Add");
        private static string TextLogs => Locale<AgentManage>.GetString("Logs");
        private static string TextEdit => Locale<AgentManage>.GetString("Edit");
        private static string TextDelete => Locale<AgentManage>.GetString("Delete");
        private static string TextError => Locale<AgentManage>.GetString("Error");

        private void Add()
        {
            modalWindow.Show(TextAdd,
                new DialogParameters<Controls.EditAgentInfo>()
                {
                    {x=>x.Model,new Model.AgentInfo()},
                    {x=>x.OkAction, model =>
                        {
                            try
                            {
                                ConfigDbContext.CacheContext.Add(model);
                                AgentChangedHandler?.Invoke();
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

        private void Edit(Model.AgentInfo model)
        {
            modalWindow.Show(TextEdit,
                new DialogParameters<Controls.EditAgentInfo>
                {
                    {x=>x.Model, JsonSerializer.Deserialize<Model.AgentInfo>(JsonSerializer.Serialize(model))},
                    {x=>x.OkAction,editModel =>
                        {
                            try
                            {
                                if (model.Context != null)
                                    model.Context.Dispose();
                                model.Password = editModel.Password;
                                ConfigDbContext.CacheContext.Update(model);
                                AgentChangedHandler?.Invoke();
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

        private void Delete(Model.AgentInfo model)
        {
            modalAlert.Show(TextDelete, Locale<AgentManage>.GetString("Are you sure to delete agent[{0}]?", model.Name), new ()
            {
                OkCallback = () =>
                {
                    try
                    {
                        if (model.Context != null)
                            model.Context.Dispose();
                        ConfigDbContext.CacheContext.Remove(model, true);
                        AgentChangedHandler?.Invoke();
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

        public static Dictionary<string, object> PrepareParameter(Action profileChangedHandler)
        {
            return new Dictionary<string, object>()
            {
                [nameof(AgentChangedHandler)] = profileChangedHandler
            };
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Global.Instance.GlashServer.AgentConnected += GlashServer_AgentConnectedOrDisconnected;
            Global.Instance.GlashServer.AgentDisconnected += GlashServer_AgentConnectedOrDisconnected;
        }

        private void GlashServer_AgentConnectedOrDisconnected(object sender, GlashAgentContext e)
        {
            InvokeAsync(StateHasChanged);
        }

        public override void Dispose()
        {
            Global.Instance.GlashServer.AgentConnected -= GlashServer_AgentConnectedOrDisconnected;
            Global.Instance.GlashServer.AgentDisconnected -= GlashServer_AgentConnectedOrDisconnected;
            base.Dispose();
        }
    }
}
