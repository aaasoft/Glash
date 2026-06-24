using Avalonia.Controls;

namespace GlashClientDesktop.Core.ProxyTypes
{
    public interface IProxyType
    {
        string[] GetFormerIds();
        string GetName();
        string GetIcon();
        public Control GetUI();
        public ProxyTypeButton[] GetButtons();
        string ToJson();
    }
}
