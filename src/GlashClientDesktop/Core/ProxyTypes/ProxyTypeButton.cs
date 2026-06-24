using System.Windows.Input;
using Glash.Client;

namespace GlashClientDesktop.Core.ProxyTypes
{
    public class ProxyTypeButton
    {
        public string Name { get; private set; }
        public string Icon { get; private set; }
        public ICommand Command { get; private set; }

        public ProxyTypeButton(string name, string icon, ICommand command)
        {
            Name = name;
            Icon = icon;
            Command = command;
        }
    }
}
