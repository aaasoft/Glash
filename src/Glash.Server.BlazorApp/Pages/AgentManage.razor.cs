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
    public partial class AgentManage
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
        public Action AgentChangedHandler { get; set; }

        private void Add()
        {
            modalWindow.Show<Controls.EditAgentInfo>(Global.Instance.TextManager.GetText(Texts.Add), Controls.EditAgentInfo.PrepareParameter(
                new Model.AgentInfo(),
                model =>
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
                        modalAlert.Show(Global.Instance.TextManager.GetText(ServerTexts.Error), ex.Message);
                    }
                }
            ));
        }

        private void Edit(Model.AgentInfo model)
        {
            var editModel = JsonConvert.DeserializeObject<Model.AgentInfo>(JsonConvert.SerializeObject(model));
            modalWindow.Show<Controls.EditAgentInfo>(Global.Instance.TextManager.GetText(Texts.Edit), Controls.EditAgentInfo.PrepareParameter(
                editModel,
                model =>
                {
                    try
                    {
                        model.Password = editModel.Password;
                        ConfigDbContext.CacheContext.Update(model);
                        AgentChangedHandler?.Invoke();
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

        private void Delete(Model.AgentInfo model)
        {
            modalAlert.Show(Global.Instance.TextManager.GetText(Texts.Delete), Global.Instance.TextManager.GetText(Texts.DeleteConfirm, model.Name), () =>
            {
                try
                {
                    ConfigDbContext.CacheContext.Remove(model, true);
                    AgentChangedHandler?.Invoke();
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
                [nameof(AgentChangedHandler)] = profileChangedHandler
            };
        }
    }
}
