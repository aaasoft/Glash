using Avalonia.Controls;
using Glash.Client;
using Microsoft.Win32;
using Quick.Localize;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace GlashClientDesktop.Core.ProxyTypes
{
    [JsonSerializable(typeof(SSH))]
    internal partial class SSHSerializerContext : JsonSerializerContext { }

    public class SSH : AbstractProxyType
    {
        protected override JsonTypeInfo ProxyTypeJsonTypeInfo => SSHSerializerContext.Default.SSH;
        public override Control GetUI() => new SSH_UI() { DataContext = this };
        public override string[] GetFormerIds() => ["Glash.Blazor.Client.ProxyTypes.SSH"];
        public override object GetIcon() => Avalonia.Application.Current.FindResource("SemiIconTerminal");
        public override string GetName() => Locale.GetString("SSH");

        [JsonIgnore]
        public string Text_User => Locale.GetString("User");
        [JsonIgnore]
        public string Text_Password => Locale.GetString("Password");
        [JsonIgnore]
        public string Text_Terminal => Locale.GetString("Terminal");
        [JsonIgnore]
        public string[] Terminals { get; set; } = ["putty", "plink"];

        [Required]
        public string User { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Terminal { get; set; }

        [SupportedOSPlatform("windows")]
        public override ProxyTypeButton[] GetButtons(ProxyRuleContext t)
        {
            return
            [
                new ProxyTypeButton(
                    Locale.GetString("Terminal"),
                    Avalonia.Application.Current.FindResource("SemiIconTerminal"),
                    ()=>
                    {
                        if(string.IsNullOrEmpty(Terminal))
                            Terminal="putty";
                        try
                        {
                            var process = Process.Start(Terminal,$"-ssh -l {User} -pw {Password} -P {t.LocalPort} {GetLocalIPAddress(t.Config.LocalIPAddress)}");
                            WaitForProcessMainWindow(process);
                        }
                        catch (System.ComponentModel.Win32Exception)
                        {
                            throw new IOException(Locale.GetString("Can't found {0},please install {0} first.","PuTTY"));
                        }
                    }),
                new ProxyTypeButton(
                    Locale.GetString("Start File Transfer"),
                    Avalonia.Application.Current.FindResource("SemiIconFolder"),
                    ()=>
                    {
#pragma warning disable CA1416 // 验证平台兼容性
                        //从注册表中读取NSIS的安装目录
                        var regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\winscp3_is1", false);
                        if (regKey == null)
                        {
                            throw new IOException(Locale.GetString("Can't found {0},please install {0} first.","WinSCP"));
                        }
                        var installLocation = regKey.GetValue("InstallLocation").ToString();
                        var exeFile = Path.Combine(installLocation, "WinSCP.exe");
#pragma warning restore CA1416 // 验证平台兼容性
                        var process = Process.Start(exeFile, $"/ini=nul sftp://{GetLocalIPAddress(t.Config.LocalIPAddress)}:{t.LocalPort}/ -username={User} -password={Password}");
                        WaitForProcessMainWindow(process);
                    })
            ];
        }
    }
}
