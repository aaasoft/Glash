using Avalonia.Controls;
using Glash.Client;

namespace GlashClientDesktop.Core.ProxyTypes
{
    public interface IProxyType
    {
        string[] GetFormerIds();
        string GetName();
        string GetIcon();
        public Control GetUI();
        public ProxyTypeButton[] GetButtons(ProxyRuleContext t);
        string ToJson();
    }
}
