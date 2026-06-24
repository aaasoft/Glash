using System.Diagnostics;
using System.Net;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Avalonia.Controls;
using GlashClientDesktop.ViewModels;

namespace GlashClientDesktop.Core.ProxyTypes
{
    public abstract class AbstractProxyType : ViewModelBase, IProxyType
    {
        protected abstract JsonTypeInfo ProxyTypeJsonTypeInfo { get; }
        public abstract string GetIcon();
        public abstract string GetName();

        public virtual string[] GetFormerIds() => null;

        public abstract ProxyTypeButton[] GetButtons();
        public abstract Control GetUI();

        public string ToJson() => JsonSerializer.Serialize(this, ProxyTypeJsonTypeInfo);

        protected string GetLocalIPAddress(string localIPAddress)
        {
            if (localIPAddress == IPAddress.Any.ToString())
                localIPAddress = IPAddress.Loopback.ToString();
            if (localIPAddress == IPAddress.IPv6Any.ToString())
                localIPAddress = IPAddress.IPv6Loopback.ToString();
            return localIPAddress;
        }

        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        [SupportedOSPlatform("macos")]
        protected IntPtr WaitForProcessMainWindow(Process process)
        {
            while (true)
            {
                Thread.Sleep(1000);
                if (process.HasExited)
                    return IntPtr.Zero;
                var hWnd = process.MainWindowHandle;
                if (hWnd != IntPtr.Zero)
                    return hWnd;
            }
        }
    }
}
