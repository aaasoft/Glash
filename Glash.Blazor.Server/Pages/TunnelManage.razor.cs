using Glash.Server;
using Quick.Utils;
using Quick.Blazor.Bootstrap;
using Quick.Localize;

namespace Glash.Blazor.Server.Pages
{
    public partial class TunnelManage : ComponentBase_WithGettextSupport
    {
        private ModalAlert modalAlert;
        private UnitStringConverting storageUSC = UnitStringConverting.StorageUnitStringConverting;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private GlashServerTunnelContext[] GetTunnels() => Global.Instance.GlashServer.Tunnels;

        private static string TextId => Locale<TunnelManage>.GetString("Id");
        private static string TextAgent => Locale<TunnelManage>.GetString("Agent");
        private static string TextClient => Locale<TunnelManage>.GetString("Client");
        private static string TextTarget => Locale<TunnelManage>.GetString("Target");
        private static string TextConnectTime => Locale<TunnelManage>.GetString("Connect Time");
        private static string TextTotalBytes => Locale<TunnelManage>.GetString("Total Bytes");
        private static string TextCurrentSpeed => Locale<TunnelManage>.GetString("Current Speed");
        private static string TextOperate => Locale<TunnelManage>.GetString("Operate");
        private static string TextCloseTunnel => Locale<TunnelManage>.GetString("Close Tunnel");

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                beginRefresh(cts.Token);
            }
        }

        private void beginRefresh(CancellationToken token)
        {
            Task.Delay(1000, token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;
                InvokeAsync(StateHasChanged);
                beginRefresh(token);
            });
        }

        public override void Dispose()
        {
            base.Dispose();
            cts.Cancel();
        }

        private void CloseTunnel(GlashServerTunnelContext tunnel)
        {
            modalAlert.Show(
                TextCloseTunnel,
                Locale<TunnelManage>.GetString("Are you sure close tunnel[{0}]", tunnel.TunnelInfo.Id),
                new ()
                {
                    OkCallback = () =>
                    {
                        tunnel.OnError(new ApplicationException(Locale<TunnelManage>.GetString("Closed by server.")));
                    }
                });
        }
    }
}
