using Glash.Client;
using System.Diagnostics;
using System.Net;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

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

        [SupportedOSPlatform("windows")]
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

        [SupportedOSPlatform("windows")]
        private void StartRDP(ProxyRuleContext proxyRuleContext)
        {
            var sb = new StringBuilder();
            var localIpAddress = proxyRuleContext.Config.LocalIPAddress;
            if (localIpAddress == IPAddress.Any.ToString())
                localIpAddress = IPAddress.Loopback.ToString();
            if (localIpAddress == IPAddress.IPv6Any.ToString())
                localIpAddress = IPAddress.IPv6Loopback.ToString();
            sb.AppendLine($"full address:s:{localIpAddress}:{proxyRuleContext.LocalPort}");
            sb.AppendLine($"username:s:{User}");
            sb.AppendLine($"password 51:b:{GetRdpPassWord(Password)}");
            var tmpFile = Path.GetTempFileName();
            var tmpFileInfo = new FileInfo(tmpFile);
            try
            {
                File.WriteAllText(tmpFile, sb.ToString());
                Process.Start("mstsc.exe", tmpFile);
                Task.Delay(1000).ContinueWith(t =>
                {
                    File.Delete(tmpFile);
                });                
            }
            catch
            {
                File.Delete(tmpFile);
                throw;
            }
        }
    }
}
