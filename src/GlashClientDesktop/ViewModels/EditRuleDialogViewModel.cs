using Glash.Client.Protocol.QpModel;
using Quick.Localize;

namespace GlashClientDesktop.ViewModels;

public class EditRuleDialogViewModel : ViewModelBase
{
    
    public string Text_Name { get; } = Locale.GetString("Name");
    public string Text_Listen { get; } = Locale.GetString("Listen");
    public string Text_Proxy { get; } = Locale.GetString("Proxy");

    public ProxyRuleInfo Model { get; set; }
}
