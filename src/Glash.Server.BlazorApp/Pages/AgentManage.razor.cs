﻿using Microsoft.AspNetCore.Components;
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
    public partial class AgentManage : IDisposable
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

        [Parameter]
        public Action AgentChangedHandler { get; set; }

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
            modalWindow.Show<Controls.EditAgentInfo>(Global.Instance.TextManager.GetText(Texts.Edit), Controls.EditAgentInfo.PrepareParameter(
                JsonConvert.DeserializeObject<Model.AgentInfo>(JsonConvert.SerializeObject(model)),
                editModel =>
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

        protected override void OnInitialized()
        {
            base.OnInitialized();
            GlashServerMiddlewareExtensions.GlashServer.AgentConnected += GlashServer_AgentConnectedOrDisconnected;
            GlashServerMiddlewareExtensions.GlashServer.AgentDisconnected += GlashServer_AgentConnectedOrDisconnected;
        }

        private void GlashServer_AgentConnectedOrDisconnected(object sender, Core.Server.GlashAgentContext e)
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            GlashServerMiddlewareExtensions.GlashServer.AgentConnected -= GlashServer_AgentConnectedOrDisconnected;
            GlashServerMiddlewareExtensions.GlashServer.AgentDisconnected -= GlashServer_AgentConnectedOrDisconnected;
        }
    }
}
