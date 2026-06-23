using System.Reactive;
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

    private ProxyRuleInfo[] _Rules;
    public ProxyRuleInfo[] Rules
    {
        get => _Rules;
        set => this.RaiseAndSetIfChanged(ref _Rules, value);
    }

    private ProxyRuleInfo _CurrentRule;
    public ProxyRuleInfo CurrentRule
    {
        get => _CurrentRule;
        set => this.RaiseAndSetIfChanged(ref _CurrentRule, value);
    }

    public ReactiveCommand<Unit, Unit> StartCommand { get; }
    public ReactiveCommand<Unit, Unit> StopCommand { get; }

    public ConnectionAgentProxiesViewModel()
    {
        StartCommand = ReactiveCommand.CreateFromTask(ExecuteCommand_Start);
        StopCommand = ReactiveCommand.CreateFromTask(ExecuteCommand_Stop);
    }

    public async Task ExecuteCommand_Start()
    {
        await ConnectionContext.GlashClient.EnableProxyRule(CurrentRule.Id);
    }

    public async Task ExecuteCommand_Stop()
    {
        await ConnectionContext.GlashClient.DisableProxyRule(CurrentRule.Id);
    }
}
