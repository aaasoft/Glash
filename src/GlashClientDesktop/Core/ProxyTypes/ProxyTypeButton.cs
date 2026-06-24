using Glash.Client;

namespace GlashClientDesktop.Core.ProxyTypes
{
    public class ProxyTypeButton
    {
        public string Name { get; private set; }
        public string Icon { get; private set; }
        public Action<ProxyRuleContext> Handler { get; private set; }

        public ProxyTypeButton(string name, string icon, Action<ProxyRuleContext> handler)
        {
            Name = name;
            Icon = icon;
            Handler = handler;
        }

        public void Invoke(ProxyRuleContext proxyRuleContext)
        {
            Handler.Invoke(proxyRuleContext);
        }
    }
}
