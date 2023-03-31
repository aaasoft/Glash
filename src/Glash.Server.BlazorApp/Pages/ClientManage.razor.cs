using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Quick.Blazor.Bootstrap;
using Quick.Blazor.Bootstrap.Admin.Utils;
using Quick.EntityFrameworkCore.Plus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Server.BlazorApp.Pages
{
    public partial class ClientManage : IDisposable
    {
        public enum Texts
        {
            Operate,
            Add,
            Edit,
            Delete,
            DeleteConfirm
        }

        private ModalWindow modalWindow;
        private ModalLoading modalLoading;
        private ModalAlert modalAlert;
        private CancellationTokenSource cts = new CancellationTokenSource();

        [Parameter]
        public Action ClientChangedHandler { get; set; }
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                beginRefresh(cts.Token);
            }
        }

        private void beginRefresh(CancellationToken token)
        {
            Task.Delay(1000, token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;
                InvokeAsync(StateHasChanged);
                beginRefresh(token);
            });
        }

        private void setClientRelateAgents(string clientId, string[] agents)
        {
            var oldModels = ConfigDbContext.CacheContext
                .Query<Model.ClientAgentRelation>(t => t.ClientName == clientId)
                .ToHashSet();
            var newModels = agents.Select(t => new Model.ClientAgentRelation()
            {
                AgentName = t,
                ClientName = clientId
            }).ToHashSet();
            var toAddList = new List<Model.ClientAgentRelation>();
            var toDelList = new List<Model.ClientAgentRelation>();

            foreach (var model in newModels)
            {
                if (oldModels.Contains(model))
                    continue;
                toAddList.Add(model);
            }
            foreach (var model in oldModels)
            {
                if (newModels.Contains(model))
                    continue;
                toDelList.Add(model);
            }
            ConfigDbContext.CacheContext.AddRange(toAddList);
            ConfigDbContext.CacheContext.RemoveRange(toDelList.ToArray());
        }

        private void Add()
        {
            modalWindow.Show<Controls.EditClientInfo>(Global.Instance.TextManager.GetText(Texts.Add), Controls.EditClientInfo.PrepareParameter(
                new Model.ClientInfo(),
                (model, agents) =>
                {
                    try
                    {
                        ConfigDbContext.CacheContext.Add(model);
                        setClientRelateAgents(model.Name, agents);
                        ClientChangedHandler?.Invoke();
                        InvokeAsync(StateHasChanged);
                        modalWindow.Close();
                    }
                    catch (Exception ex)
                    {
                        modalAlert.Show(Global.Instance.TextManager.GetText(ServerTexts.Error), ex.Message);
                    }
                }
            ));
        }

        private void Edit(Model.ClientInfo model)
        {
            modalWindow.Show<Controls.EditClientInfo>(Global.Instance.TextManager.GetText(Texts.Edit), Controls.EditClientInfo.PrepareParameter(
                JsonConvert.DeserializeObject<Model.ClientInfo>(JsonConvert.SerializeObject(model)),
                (editModel, agents) =>
                {
                    try
                    {
                        if (model.Context != null)
                            model.Context.Dispose();
                        model.Password = editModel.Password;
                        ConfigDbContext.CacheContext.Update(model);
                        setClientRelateAgents(model.Name, agents);
                        ClientChangedHandler?.Invoke();
                        InvokeAsync(StateHasChanged);
                        modalWindow.Close();
                    }
                    catch (Exception ex)
                    {
                        modalAlert.Show(Global.Instance.TextManager.GetText(ServerTexts.Error), ex.Message);
                    }
                }
            ));
        }

        private void Delete(Model.ClientInfo model)
        {
            modalAlert.Show(Global.Instance.TextManager.GetText(Texts.Delete), Global.Instance.TextManager.GetText(Texts.DeleteConfirm, model.Name), () =>
            {
                try
                {
                    if (model.Context != null)
                        model.Context.Dispose();
                    ConfigDbContext.CacheContext.Remove(model, true);
                    ClientChangedHandler?.Invoke();
                    InvokeAsync(StateHasChanged);
                }
                catch (Exception ex)
                {
                    Task.Delay(100).ContinueWith(t =>
                    {
                        modalAlert.Show(Global.Instance.TextManager.GetText(ServerTexts.Error), ex.Message);
                    });
                }
            });
        }

        public static Dictionary<string, object> PrepareParameter(Action profileChangedHandler)
        {
            return new Dictionary<string, object>()
            {
                [nameof(ClientChangedHandler)] = profileChangedHandler
            };
        }

        public void Dispose()
        {
            cts.Cancel();
        }
    }
}
