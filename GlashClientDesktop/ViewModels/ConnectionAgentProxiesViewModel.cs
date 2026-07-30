using System.Reactive;
using Avalonia.Controls;
using Avalonia.Threading;
using Glash.Client;
using Glash.Client.Protocol.QpModel;
using GlashClientDesktop.Core;
using GlashClientDesktop.Core.ProxyTypes;
using GlashClientDesktop.Views;
using Quick.Localize;
using Quick.Utils;
using ReactiveUI;
using Ursa.Controls;

namespace GlashClientDesktop.ViewModels;

public class ConnectionAgentProxiesViewModel : ViewModelBase
{
    public string Text_Listen => Locale<ConnectionAgentProxiesViewModel>.GetString("Listen");
    public string Text_Proxy => Locale<ConnectionAgentProxiesViewModel>.GetString("Proxy");
    public string Text_DeleteConfirm => Locale<ConnectionAgentProxiesViewModel>.GetString("Delete Confirm");
    public string Text_DeleteRuleConfirm => Locale<ConnectionAgentProxiesViewModel>.GetString("Are you sure to delete selected rule?");

    public ConnectionContext ConnectionContext { get; set; }

    private string _Name;
    public string Name
    {
        get => _Name;
        set => this.RaiseAndSetIfChanged(ref _Name, value);
    }

    private bool _Connected;
    public bool Connected
    {
        get => _Connected;
        set => this.RaiseAndSetIfChanged(ref _Connected, value);
    }

    private ProxyRuleContext[] _Rules;
    public ProxyRuleContext[] Rules
    {
        get => _Rules;
        set => this.RaiseAndSetIfChanged(ref _Rules, value);
    }

    private ProxyRuleContext _CurrentRule;
    public ProxyRuleContext CurrentRule
    {
        get => _CurrentRule;
        set
        {
            this.RaiseAndSetIfChanged(ref _CurrentRule, value);
            if (value == null)
                CurrentRuleEnable = false;
            else
                CurrentRuleEnable = value.Config.Enable;
        }
    }

    private ProxyTypeButton[] _CurrentRuleProxyTypeButtons;
    public ProxyTypeButton[] CurrentRuleProxyTypeButtons
    {
        get => _CurrentRuleProxyTypeButtons;
        set => this.RaiseAndSetIfChanged(ref _CurrentRuleProxyTypeButtons, value);
    }

    private bool _CurrentRuleEnable = false;
    public bool CurrentRuleEnable
    {
        get => _CurrentRuleEnable;
        set
        {
            this.RaiseAndSetIfChanged(ref _CurrentRuleEnable, value);
            if (value && !string.IsNullOrEmpty(CurrentRule.Config.ProxyType))
            {
                var proxyTypeInfo = ProxyTypeManager.Instance.GetProxyTypeInfo(CurrentRule.Config.ProxyType);
                var proxyType = proxyTypeInfo.CreateInstance(CurrentRule.Config.ProxyTypeConfig);
                CurrentRuleProxyTypeButtons = proxyType.GetButtons(CurrentRule);
            }
            else
            {
                CurrentRuleProxyTypeButtons = null;
            }
        }
    }

    public ReactiveCommand<Unit, Unit> AddCommand { get; }
    public ReactiveCommand<Unit, Unit> EditCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
    public ReactiveCommand<Unit, Unit> FakeDeleteCommand { get; }

    public ReactiveCommand<Unit, Unit> CopyCommand { get; }
    public ReactiveCommand<Unit, Unit> StartCommand { get; }
    public ReactiveCommand<Unit, Unit> StopCommand { get; }
    public ReactiveCommand<Unit, Unit> LogCommand { get; }
    public ReactiveCommand<ProxyTypeButton, Unit> RuleButtonCommand { get; }



    public ConnectionAgentProxiesViewModel()
    {
        AddCommand = ReactiveCommand.CreateFromTask(ExecuteCommand_Add);
        CopyCommand = ReactiveCommand.CreateFromTask(ExecuteCommand_Copy, this.WhenAnyValue(
                x => x.CurrentRule,
                new Func<ProxyRuleContext, bool>(x => x != null)));
        EditCommand = ReactiveCommand.CreateFromTask(ExecuteCommand_Edit, this.WhenAnyValue(
                x => x.CurrentRule,
                x => x.CurrentRuleEnable,
                (currentRule, currentRuleEnable) => currentRule != null && !currentRuleEnable));
        DeleteCommand = ReactiveCommand.Create(ExecuteCommand_Delete, this.WhenAnyValue(
                x => x.CurrentRule,
                x => x.CurrentRuleEnable,
                (currentRule, currentRuleEnable) => currentRule != null && !currentRuleEnable));
        FakeDeleteCommand = ReactiveCommand.Create(() => { }, this.WhenAnyValue(
                x => x.CurrentRule,
                x => x.CurrentRuleEnable,
                (currentRule, currentRuleEnable) => currentRule != null && !currentRuleEnable));

        StartCommand = ReactiveCommand.CreateFromTask(ExecuteCommand_Start);
        StopCommand = ReactiveCommand.CreateFromTask(ExecuteCommand_Stop);
        LogCommand = ReactiveCommand.CreateFromTask(ExecuteCommand_Log);
        RuleButtonCommand = ReactiveCommand.CreateFromTask<ProxyTypeButton>(ExecuteCommand_RuleButton);
    }

    private void refreshRules()
    {
        Rules = ConnectionContext.ProxyRules
            .Where(t => t.Config.Agent == Name)
            .ToArray();
    }

    private async Task innerAdd(ProxyRuleInfo model)
    {
        var options = new OverlayDialogOptions()
        {
            Buttons = DialogButton.OKCancel,
            IsCloseButtonVisible = true,
            Title = Locale<ConnectionAgentProxiesViewModel>.GetString("Add Rule"),
            CanResize = true
        };
        var vm = new EditRuleDialogViewModel() { Model = model };
        var ret = await OverlayDialog.ShowStandardAsync<EditRuleDialog, EditRuleDialogViewModel>(vm, null, options);
        if (ret == DialogResult.OK)
        {
            model.ProxyType = vm.CurrentProxyType.Id;
            model.ProxyTypeConfig = vm.CurrentProxyTypeConfig?.ToJson();
            await ConnectionContext.AddProxyRule(model);
            refreshRules();
        }
    }

    public async Task ExecuteCommand_Add()
    {
        var model = new ProxyRuleInfo()
        {
            Id = Guid.NewGuid().ToString("N"),
            Agent = Name,
            Enable = false
        };
        await innerAdd(model);
    }

    public async Task ExecuteCommand_Copy()
    {
        var copyModel = CurrentRule.Config;
        var model = new ProxyRuleInfo()
        {
            Id = Guid.NewGuid().ToString("N"),
            Agent = Name,
            Enable = false,
            Name = copyModel.Name,
            LocalIPAddress = copyModel.LocalIPAddress,
            LocalPort = copyModel.LocalPort,
            RemoteHost = copyModel.RemoteHost,
            RemotePort = copyModel.RemotePort,
            ProxyType = copyModel.ProxyType,
            ProxyTypeConfig = copyModel.ProxyTypeConfig
        };
        await innerAdd(model);
    }

    public async Task ExecuteCommand_Edit()
    {
        var options = new OverlayDialogOptions()
        {
            Buttons = DialogButton.OKCancel,
            IsCloseButtonVisible = true,
            Title = Locale<ConnectionAgentProxiesViewModel>.GetString("Edit Rule"),
            CanResize = true
        };
        var model = CurrentRule.Config;
        var editModel = new ProxyRuleInfo()
        {
            Id = model.Id,
            Name = model.Name,
            Agent = model.Agent,
            Enable = model.Enable,
            LocalIPAddress = model.LocalIPAddress,
            LocalPort = model.LocalPort,
            ProxyType = model.ProxyType,
            ProxyTypeConfig = model.ProxyTypeConfig,
            RemoteHost = model.RemoteHost,
            RemotePort = model.RemotePort
        };
        var vm = new EditRuleDialogViewModel() { Model = editModel };
        var ret = await OverlayDialog.ShowStandardAsync<EditRuleDialog, EditRuleDialogViewModel>(vm, null, options);
        if (ret == DialogResult.OK)
        {
            editModel.ProxyType = vm.CurrentProxyType.Id;
            editModel.ProxyTypeConfig = vm.CurrentProxyTypeConfig?.ToJson();
            await ConnectionContext.EditProxyRule(editModel);
            refreshRules();
        }
    }

    public void ExecuteCommand_Delete()
    {
        var uiDispatcher = Dispatcher.CurrentDispatcher;
        Task.Delay(100).ContinueWith(async t =>
        {
            await ConnectionContext.DeleteProxyRule(CurrentRule.Config);
            uiDispatcher.Invoke(() =>
            {
                CurrentRule = null;
                refreshRules();
            });
        });
    }

    public async Task ExecuteCommand_Start()
    {
        await ConnectionContext.GlashClient.EnableProxyRule(CurrentRule.Config.Id);
        CurrentRuleEnable = true;
    }

    public async Task ExecuteCommand_Stop()
    {
        await ConnectionContext.GlashClient.DisableProxyRule(CurrentRule.Config.Id);
        CurrentRuleEnable = false;
    }

    public async Task ExecuteCommand_Log()
    {
        await OverlayMessageBox.ShowAsync(string.Join(Environment.NewLine, CurrentRule.Logs), Locale<ConnectionAgentProxiesViewModel>.GetString("Logs"), null, MessageBoxIcon.Information);
    }

    public async Task ExecuteCommand_RuleButton(ProxyTypeButton button)
    {
        try
        {
            await Task.Run(button.Command);
        }
        catch (Exception ex)
        {
            await OverlayMessageBox.ShowAsync(Locale<ConnectionAgentProxiesViewModel>.GetString("Execute {0} error,reason:{1}", button.Name, ExceptionUtils.GetExceptionMessage(ex)), Locale<ConnectionAgentProxiesViewModel>.GetString("Error"));
        }
    }
}
