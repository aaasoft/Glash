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
    public partial class ClientManage
    {
        private ModalWindow modalWindow;
        private ModalLoading modalLoading;
        private ModalAlert modalAlert;

        public enum Texts
        {
            Operate,
            Add,
            Edit,
            Delete,
            DeleteConfirm
        }

        [Parameter]
        public Action ClientChangedHandler { get; set; }

        private void setClientRelateAgents(string clientId, string[] agents)
        {
            var oldModels = ConfigDbContext.CacheContext
                .Query<Model.ClientAgentRelation>(t => t.ClientId == clientId)
                .ToHashSet();
            var newModels = agents.Select(t => new Model.ClientAgentRelation()
            {
                AgentId = t,
                ClientId = clientId
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
                new Model.ClientInfo(Guid.NewGuid().ToString("N")),
                (model, agents) =>
                {
                    try
                    {
                        ConfigDbContext.CacheContext.Add(model);
                        setClientRelateAgents(model.Id, agents);
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
            var editModel = JsonConvert.DeserializeObject<Model.ClientInfo>(JsonConvert.SerializeObject(model));
            modalWindow.Show<Controls.EditClientInfo>(Global.Instance.TextManager.GetText(Texts.Edit), Controls.EditClientInfo.PrepareParameter(
                editModel,
                (model,agents) =>
                {
                    try
                    {
                        model.Name = editModel.Name;
                        model.Password = editModel.Password;
                        ConfigDbContext.CacheContext.Update(model);
                        setClientRelateAgents(model.Id, agents);
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
    }
}
