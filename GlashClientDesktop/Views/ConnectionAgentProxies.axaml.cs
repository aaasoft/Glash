using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Glash.Client;

namespace GlashClientDesktop.Views;

public partial class ConnectionAgentProxies : UserControl
{
    public ConnectionAgentProxies()
    {
        InitializeComponent();
    }

    private async void CopyLocalEndPoint_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Control control && control.DataContext is ProxyRuleContext rule)
            await CopyToClipboardAsync(rule.LocalEndPoint);
    }

    private async void CopyRemoteEndPoint_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Control control && control.DataContext is ProxyRuleContext rule)
            await CopyToClipboardAsync(rule.RemoteEndPoint);
    }

    private async Task CopyToClipboardAsync(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.Clipboard != null)
            await topLevel.Clipboard.SetTextAsync(text);
    }
}