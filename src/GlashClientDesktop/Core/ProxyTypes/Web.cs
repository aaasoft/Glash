using Avalonia.Controls;
using Glash.Client;
using Quick.Localize;
using ReactiveUI;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace GlashClientDesktop.Core.ProxyTypes
{
    [JsonSerializable(typeof(Web))]
    internal partial class WebSerializerContext : JsonSerializerContext { }

    public class Web : AbstractProxyType
    {
        protected override JsonTypeInfo ProxyTypeJsonTypeInfo => WebSerializerContext.Default.Web;
        public override Control GetUI() => new Web_UI() { DataContext = this };
        public override string[] GetFormerIds() => ["Glash.Blazor.Client.ProxyTypes.Web"];
        public override object GetIcon() => Avalonia.Application.Current.FindResource("SemiIconGlobe");
        public override string GetName() => Locale.GetString("Web");

        [JsonIgnore]
        public string Text_Schema => Locale.GetString("Schema");
        [JsonIgnore]
        public string Text_Path => Locale.GetString("Path");

        public string Schema { get; set; }
        public string Path { get; set; }

        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        [SupportedOSPlatform("macos")]
        public override ProxyTypeButton[] GetButtons(ProxyRuleContext t)
        {
            return
            [
                new ProxyTypeButton(
                    Locale.GetString("Visit"),
                    Avalonia.Application.Current.FindResource("SemiIconGlobe"),
                    ReactiveCommand.Create(()=>
                    {
                        if(string.IsNullOrEmpty(Schema))
                            Schema="http";
                        var url = $"{Schema}://{GetLocalIPAddress(t.Config.LocalIPAddress)}:{t.LocalPort}/{Path}";
                        try
                        {
                            //先尝试使用Chrome浏览器打开
                            var psi = new ProcessStartInfo("chrome");
                            psi.ArgumentList.Add("-incognito");
                            psi.ArgumentList.Add(url);
                            psi.UseShellExecute = true;
                            var process = Process.Start(psi);
                            WaitForProcessMainWindow(process);
                        }
                        catch
                        {
                            //使用系统默认浏览器打开
                            var psi = new ProcessStartInfo(url);
                            psi.UseShellExecute = true;
                            var process = Process.Start(psi);;
                            WaitForProcessMainWindow(process);
                        }
                    })
                )
            ];
        }

        [JsonIgnore]
        public string[] Schemas { get; set; } = ["http", "https"];
    }
}
