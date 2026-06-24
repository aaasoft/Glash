using Avalonia.Controls;
using Glash.Client.Protocol.QpModel;
using GlashClientDesktop.Core.ProxyTypes;
using Quick.Localize;
using ReactiveUI;

namespace GlashClientDesktop.ViewModels;

public class EditRuleDialogViewModel : ViewModelBase
{

    public string Text_Name { get; } = Locale.GetString("Name");
    public string Text_Listen { get; } = Locale.GetString("Listen");
    public string Text_Proxy { get; } = Locale.GetString("Proxy");
    public string Text_Basic { get; } = Locale.GetString("Basic");
    public string Text_Type { get; } = Locale.GetString("Type");

    private ProxyRuleInfo _Model;
    public ProxyRuleInfo Model
    {
        get => _Model;
        set
        {
            _Model = value;
            if (!string.IsNullOrEmpty(Model.ProxyType))
            {
                var proxyType = ProxyTypeManager.Instance.GetProxyTypeInfo(Model.ProxyType);
                if (proxyType != null)
                {
                    CurrentProxyTypeConfig = proxyType.CreateInstance(Model.ProxyTypeConfig);
                }
            }
        }
    }
    public ProxyTypeInfo[] ProxyTypes => ProxyTypeManager.Instance.GetProxyTypeInfos();

    private Control _CurrentProxyTypeControl;
    public Control CurrentProxyTypeControl
    {
        get => _CurrentProxyTypeControl;
        set => this.RaiseAndSetIfChanged(ref _CurrentProxyTypeControl, value);
    }

    private IProxyType _CurrentProxyTypeConfig;
    public IProxyType CurrentProxyTypeConfig
    {
        get => _CurrentProxyTypeConfig;
        set
        {
            this.RaiseAndSetIfChanged(ref _CurrentProxyTypeConfig, value);
            if (value == null)
            {
                CurrentProxyTypeControl = null;
            }
            else
            {
                CurrentProxyTypeControl = value.GetUI();
            }
        }
    }

    public ProxyTypeInfo CurrentProxyType
    {
        get => ProxyTypeManager.Instance.GetProxyTypeInfo(Model.ProxyType);
        set
        {
            var proxyType = ProxyTypeManager.Instance.GetProxyTypeInfo(Model.ProxyType);
            this.RaiseAndSetIfChanged(ref proxyType, value);

            if (proxyType == null)
                CurrentProxyTypeConfig = null;
            else
                CurrentProxyTypeConfig = proxyType.CreateInstance(Model.ProxyTypeConfig);
        }
    }
}
