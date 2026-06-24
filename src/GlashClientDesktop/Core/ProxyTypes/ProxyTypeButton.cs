using System.Windows.Input;

namespace GlashClientDesktop.Core.ProxyTypes
{
    public class ProxyTypeButton
    {
        public string Name { get; private set; }
        public object Icon { get; private set; }
        public ICommand Command { get; private set; }

        public ProxyTypeButton(string name, object icon, ICommand command)
        {
            Name = name;
            Icon = icon;
            Command = command;
        }
    }
}
