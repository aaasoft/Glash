using Avalonia.Controls;
using Glash.Client;
using Microsoft.Win32;
using Quick.Localize;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace GlashClientDesktop.Core.ProxyTypes.WebType
{
    [JsonSerializable(typeof(ProxyType))]
    internal partial class WebSerializerContext : JsonSerializerContext { }

    public class ProxyType : AbstractProxyType
    {
        protected override JsonTypeInfo ProxyTypeJsonTypeInfo => WebSerializerContext.Default.ProxyType;
        public override Control GetUI() => new UI() { DataContext = this };
        public override string[] GetFormerIds() => ["Glash.Blazor.Client.ProxyTypes.Web", "GlashClientDesktop.Core.ProxyTypes.Web"];
        public override object GetIcon() => Avalonia.Application.Current.FindResource("SemiIconGlobe");
        public override string GetName() => Locale<ProxyType>.GetString("Web");

        [JsonIgnore]
        public string Text_Schema => Locale<ProxyType>.GetString("Schema");
        [JsonIgnore]
        public string Text_Path => Locale<ProxyType>.GetString("Path");
        [JsonIgnore]
        public string Text_DomainName => Locale<ProxyType>.GetString("Domain Name");

        public string Schema { get; set; }
        public string Path { get; set; }
        public string DomainName { get; set; }

        private string GetFullDomainName(ProxyRuleContext t)
        {
            if (!string.IsNullOrEmpty(DomainName))
                return $"{DomainName}.localhost";
            return $"{t.Config.Name}.{t.Config.Agent}.localhost";
        }

        private string GetUrl(ProxyRuleContext t) => $"{Schema}://{GetLocalIPAddress(t.Config.LocalIPAddress)}:{t.LocalPort}/{Path}";

        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        [SupportedOSPlatform("macos")]
        public override ProxyTypeButton[] GetButtons(ProxyRuleContext t)
        {
            return
            [
                new ProxyTypeButton(
                    Locale<ProxyType>.GetString("Visit"),
                    Avalonia.Application.Current.FindResource("SemiIconGlobe"),
                    ()=>
                    {
                        string url = null;
                        if(ReverseProxyManager.Instance.ListenPort < 0)
                        {
                            url = GetUrl(t);
                        }
                        else
                        {
                            url = $"http://{GetFullDomainName(t)}";
                            if(ReverseProxyManager.Instance.ListenPort != 80)
                                url+=$":{ReverseProxyManager.Instance.ListenPort}";
                        }
                        //使用系统默认浏览器打开
                        WaitForProcessMainWindow(OpenUrlByDefaultBrowser(url));
                    }
                )
            ];
        }

        public static Process OpenUrlByDefaultBrowser(string url)
        {
            if (OperatingSystem.IsWindows())
            {
                // 读取注册表默认浏览器
                string browserPath = null;
                using var reg = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command");
                if (reg?.GetValue("") is string cmd && !string.IsNullOrEmpty(cmd))
                {
                    // 去掉末尾的 "%1"
                    browserPath = cmd.Replace("\"%1\"", "").Trim();
                    if (browserPath.StartsWith("\""))
                    {
                        browserPath = browserPath.Substring(1, browserPath.LastIndexOf('"') - 1);
                    }
                }

                if (!string.IsNullOrEmpty(browserPath))
                {
                    return Process.Start(new ProcessStartInfo
                    {
                        FileName = browserPath,
                        Arguments = url,
                        UseShellExecute = false
                    });
                }
            }
            else if (OperatingSystem.IsLinux())
            {
                return Process.Start("xdg-open", url);
            }
            else if (OperatingSystem.IsMacOS())
            {
                return Process.Start("open", url);
            }
            return Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        [JsonIgnore]
        public string[] Schemas { get; set; } = ["http", "https"];

        public override void Start(ProxyRuleContext t)
        {
            base.Start(t);
            if (ReverseProxyManager.Instance.ListenPort < 0)
                return;
            var url = GetUrl(t);
            ReverseProxyManager.Instance.AddRule(GetFullDomainName(t), url);
        }

        public override void Stop(ProxyRuleContext t)
        {
            base.Stop(t);
            if (ReverseProxyManager.Instance.ListenPort < 0)
                return;
            ReverseProxyManager.Instance.RemoveRule(GetFullDomainName(t));
        }
    }
}
