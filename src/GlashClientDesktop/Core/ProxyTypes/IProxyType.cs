using Avalonia.Controls;

namespace GlashClientDesktop.Core.ProxyTypes
{
    public interface IProxyType
    {
        string[] FormerIds{get;}
        string Name { get; }
        string Icon { get; }
        public Control GetUI();
        public ProxyTypeButton[] GetButtons();
        string ToJson();
    }
}
