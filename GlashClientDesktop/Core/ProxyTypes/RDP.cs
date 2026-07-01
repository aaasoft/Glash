using Glash.Client;
using Quick.Localize;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Avalonia.Controls;

namespace GlashClientDesktop.Core.ProxyTypes
{
    [JsonSerializable(typeof(RDP))]
    internal partial class RDPSerializerContext : JsonSerializerContext { }

    public class RDP : AbstractProxyType
    {
        protected override JsonTypeInfo ProxyTypeJsonTypeInfo => RDPSerializerContext.Default.RDP;
        public override Control GetUI() => new RDP_UI() { DataContext = this };
        public override string[] GetFormerIds() => ["Glash.Blazor.Client.ProxyTypes.RDP"];
        public override string GetName() => Locale<RDP>.GetString("RDP");
        public override object GetIcon() => Avalonia.Application.Current.FindResource("SemiIconDesktop");
        [JsonIgnore]
        public string Text_User => Locale<RDP>.GetString("User");
        [JsonIgnore]
        public string Text_Password => Locale<RDP>.GetString("Password");

        [Required]
        public string User { get; set; }
        [Required]
        public string Password { get; set; }

        public override ProxyTypeButton[] GetButtons(ProxyRuleContext t)
        {
            return
            [
                new ProxyTypeButton(
                    Locale<RDP>.GetString("Start RDP"),
                    Avalonia.Application.Current.FindResource("SemiIconDesktop"),
                    ()=>StartRDP(t)
                    )
            ];
        }

        /// <summary>
        /// 获取RDP密码
        /// </summary>
        [SupportedOSPlatform("windows")]
        private static string GetRdpPassWord(string pw)
        {
            byte[] secret = Encoding.Unicode.GetBytes(pw);
            byte[] encryptedSecret = ProtectedData.Protect(secret, null, DataProtectionScope.LocalMachine);
            return BitConverter.ToString(encryptedSecret).Replace("-", string.Empty);
        }

        private string getRdpFileName(ProxyRuleContext t)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(t.Config.Name);
            sb.Append("-");
            sb.Append(t.Config.Agent);
            sb.Replace('.', '_');
            sb.Replace(' ', '_');
            foreach (var c in Path.GetInvalidFileNameChars())
                sb.Replace(c, '_');
            return sb.ToString();
        }

        private void StartRDP(ProxyRuleContext t)
        {
            if (OperatingSystem.IsWindows())
            {
                var sb = new StringBuilder();
                sb.AppendLine($"full address:s:{GetLocalIPAddress(t.Config.LocalIPAddress)}:{t.LocalPort}");
                sb.AppendLine($"username:s:{User}");
                sb.AppendLine($"password 51:b:{GetRdpPassWord(Password)}");
                var tmpFile = Path.Combine(Path.GetTempPath(), getRdpFileName(t));
                try
                {
                    File.WriteAllText(tmpFile, sb.ToString());
                    var process = Process.Start("mstsc.exe", tmpFile);
                    WaitForProcessMainWindow(process);
                    if (File.Exists(tmpFile))
                        File.Delete(tmpFile);
                }
                catch
                {
                    File.Delete(tmpFile);
                    throw;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
