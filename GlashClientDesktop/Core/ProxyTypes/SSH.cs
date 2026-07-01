using Avalonia.Controls;
using Glash.Client;
using Microsoft.Win32;
using Quick.Localize;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.InteropServices;
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
        public override string GetName() => Locale<SSH>.GetString("SSH");

        [JsonIgnore]
        public string Text_User => Locale<SSH>.GetString("User");
        [JsonIgnore]
        public string Text_Password => Locale<SSH>.GetString("Password");
        [JsonIgnore]
        public string Text_Terminal => Locale<SSH>.GetString("Terminal");
        [JsonIgnore]
        public string[] Terminals { get; set; } = ["putty", "plink"];

        [Required]
        public string User { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Terminal { get; set; }

        public override ProxyTypeButton[] GetButtons(ProxyRuleContext t)
        {
            return
            [
                new ProxyTypeButton(
                    Locale<SSH>.GetString("Terminal"),
                    Avalonia.Application.Current.FindResource("SemiIconTerminal"),
                    ()=>
                    {
                        if(string.IsNullOrEmpty(Terminal))
                            Terminal="putty";
                        if(OperatingSystem.IsWindows())
                        {
                            try
                            {
                                var process = Process.Start(Terminal,$"-ssh -l {User} -pw {Password} -P {t.LocalPort} {GetLocalIPAddress(t.Config.LocalIPAddress)}");
                                WaitForProcessMainWindow(process);
                            }
                            catch (System.ComponentModel.Win32Exception)
                            {
                                throw new IOException(Locale<SSH>.GetString("Can't found {0},please install {0} first.","PuTTY"));
                            }
                        }
                    }),
                new ProxyTypeButton(
                    Locale<SSH>.GetString("Start File Transfer"),
                    Avalonia.Application.Current.FindResource("SemiIconFolder"),
                    ()=>
                    {
                        if(OperatingSystem.IsWindows())
                        {
                            //从注册表中读取NSIS的安装目录
                            RegistryView view;
                            // 如果当前是x86进程，强制打开64位注册表视图
                            if (RuntimeInformation.ProcessArchitecture == Architecture.X86
                                && Environment.Is64BitOperatingSystem)
                                view = RegistryView.Registry64;
                            else
                                view = RegistryView.Default;
                            var localMachineKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view);
                            //从注册表中读取NSIS的安装目录
                            var regKey = localMachineKey.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\winscp3_is1", false);
                            if (regKey == null)
                            {
                                throw new IOException(Locale<SSH>.GetString("Can't found {0},please install {0} first.","WinSCP"));
                            }
                            var installLocation = regKey.GetValue("InstallLocation").ToString();
                            var exeFile = Path.Combine(installLocation, "WinSCP.exe");
                            var process = Process.Start(exeFile, $"/ini=nul sftp://{GetLocalIPAddress(t.Config.LocalIPAddress)}:{t.LocalPort}/ -username={User} -password={Password}");
                            WaitForProcessMainWindow(process);
                        }
                    })
            ];
        }
    }
}
