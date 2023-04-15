using Glash.Client;

namespace Glash.Blazor.Client.ProxyTypes
{
    [ProxyType(typeof(Texts), nameof(Texts.ProxyTypeName))]
    public class RDP : AbstractProxyType<RDP_UI>
    {
        public enum Texts
        {
            ProxyTypeName,
            User,
            Password,
            ButtonStartRDP
        }

        public string User { get; set; }
        public string Password { get; set; }

        public override string Icon => "fa fa-desktop";

        public override ProxyTypeButton[] GetButtons()
        {
            return new ProxyTypeButton[]
            {
                new ProxyTypeButton(
                    Global.Instance.TextManager.GetText(Texts.ButtonStartRDP),
                    "fa fa-desktop",
                    StartRDP
                    )
            };
        }

        private void StartRDP(ProxyRuleContext proxyRuleContext)
        {

        }
    }
}
