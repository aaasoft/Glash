using System.Reactive;
using Glash.Client;
using Glash.Client.Protocol.QpModel;
using GlashClientDesktop.Core;
using Quick.Localize;
using ReactiveUI;

namespace GlashClientDesktop.ViewModels;

public class ConnectionAgentProxiesViewModel : ViewModelBase
{
    public string Text_Listen => Locale.GetString("Listen");
    public string Text_Proxy => Locale.GetString("Proxy");

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
            CurrentRuleEnable = value.Config.Enable;
        }
    }

    private bool _CurrentRuleEnable = false;
    public bool CurrentRuleEnable
    {
        get => _CurrentRuleEnable;
        set => this.RaiseAndSetIfChanged(ref _CurrentRuleEnable, value);
    }

    public ReactiveCommand<Unit, Unit> AddCommand { get; }
    public ReactiveCommand<Unit, Unit> EditCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    public ReactiveCommand<Unit, Unit> StartCommand { get; }
    public ReactiveCommand<Unit, Unit> StopCommand { get; }

    public ConnectionAgentProxiesViewModel()
    {
        AddCommand = ReactiveCommand.CreateFromTask(ExecuteCommand_Add);
        EditCommand = ReactiveCommand.CreateFromTask(ExecuteCommand_Edit, this.WhenAnyValue(
                x => x.CurrentRule,
                x => x.CurrentRuleEnable,
                (currentRule, currentRuleEnable) => currentRule!=null && !currentRuleEnable));
        DeleteCommand = ReactiveCommand.CreateFromTask(ExecuteCommand_Delete, this.WhenAnyValue(
                x => x.CurrentRule,
                x => x.CurrentRuleEnable,
                (currentRule, currentRuleEnable) => currentRule!=null && !currentRuleEnable));

        StartCommand = ReactiveCommand.CreateFromTask(ExecuteCommand_Start);
        StopCommand = ReactiveCommand.CreateFromTask(ExecuteCommand_Stop);
    }

    public async Task ExecuteCommand_Add()
    {

    }

    public async Task ExecuteCommand_Edit()
    {

    }

    public async Task ExecuteCommand_Delete()
    {

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
}
