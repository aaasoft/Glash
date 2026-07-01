using Avalonia.Controls;
using Glash.Client.Protocol.QpModel;
using GlashClientDesktop.Core.ProxyTypes;
using Quick.Localize;
using ReactiveUI;

namespace GlashClientDesktop.ViewModels;

public class EditRuleDialogViewModel : ViewModelBase
{

    public string Text_Name { get; } = Locale<EditRuleDialogViewModel>.GetString("Name");
    public string Text_Listen { get; } = Locale<EditRuleDialogViewModel>.GetString("Listen");
    public string Text_Proxy { get; } = Locale<EditRuleDialogViewModel>.GetString("Proxy");
    public string Text_Basic { get; } = Locale<EditRuleDialogViewModel>.GetString("Basic");
    public string Text_Type { get; } = Locale<EditRuleDialogViewModel>.GetString("Type");

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
                    CurrentProxyType = ProxyTypeManager.Instance.GetProxyTypeInfo(Model.ProxyType);
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

    private ProxyTypeInfo _CurrentProxyType;
    public ProxyTypeInfo CurrentProxyType
    {
        get => _CurrentProxyType;
        set
        {
            this.RaiseAndSetIfChanged(ref _CurrentProxyType, value);
            if (value == null)
                CurrentProxyTypeConfig = null;
            else
                CurrentProxyTypeConfig = value.CreateInstance(Model.ProxyTypeConfig);
        }
    }
}
