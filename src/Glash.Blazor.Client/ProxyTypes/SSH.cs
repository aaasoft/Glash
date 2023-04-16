using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Blazor.Client.ProxyTypes
{
    [ProxyType(typeof(Texts), nameof(Texts.ProxyTypeName))]
    public class SSH : AbstractProxyType<SSH_UI>
    {
        public enum Texts
        {
            ProxyTypeName,
            User,
            Password,
            ButtonStartTerminal,
            ButtonStartFileTransfer
        }

        public string User { get; set; }
        public string Password { get; set; }
        private const string EXE_FOLDER = "runtimes/win-x64/native";
        public override string Icon => "fa fa-linux";

        [SupportedOSPlatform("windows")]
        public override ProxyTypeButton[] GetButtons()
        {
            return new ProxyTypeButton[]
            {
                new ProxyTypeButton(
                    Global.Instance.TextManager.GetText(Texts.ButtonStartTerminal),
                    "fa fa-terminal",
                    t=>
                    {
                        Process.Start($"{EXE_FOLDER}/plink",$"-ssh -l {User} -pw {Password} -P {t.LocalPort} {GetLocalIPAddress(t.Config.LocalIPAddress)}");
                    }),
                new ProxyTypeButton(
                    Global.Instance.TextManager.GetText(Texts.ButtonStartFileTransfer),
                    "fa fa-folder",
                    t=>
                    {
                        Process.Start($"{EXE_FOLDER}/WinSCP",$"/ini=nul sftp://{GetLocalIPAddress(t.Config.LocalIPAddress)}:{t.LocalPort}/ -username={User} -password={Password}");
                    })
            };
        }
    }
}
