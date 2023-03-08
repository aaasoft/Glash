using Glash.Core.Client;
using Quick.Protocol.Utils;

namespace Glash.Client.WinForm
{
    public class ServerContext : IDisposable
    {
        private CancellationTokenSource cts;
        private GlashClient glashClient;
        public ServerInfo Model { get; private set; }
        public string State { get; private set; }
        public bool IsConnected { get; private set; } = false;
        public Action stateChanged;
        private Action isConnectedChangedAction;
        public ServerContext(ServerInfo model, Action stateChanged = null, Action isConnectedChangedAction = null)
        {
            Model = model;
            this.stateChanged = stateChanged;
            this.isConnectedChangedAction = isConnectedChangedAction;

            cts?.Cancel();
            cts = new CancellationTokenSource();
            try
            {
                glashClient = new GlashClient(model.Url, model.Password);
                glashClient.AddProxyPortInfos(model.ProxyList.ToArray());
                glashClient.Disconnected += GlashClient_Disconnected;
                _ = beginConnect(cts.Token);
            }
            catch (Exception ex)
            {
                changeState("Error." + ExceptionUtils.GetExceptionString(ex));
            }
        }

        private void changeState(string state)
        {
            State = state;
            stateChanged?.Invoke();
        }

        private void changeIsConnected(bool isConnected)
        {
            IsConnected = isConnected;
            isConnectedChangedAction?.Invoke();
        }

        private void GlashClient_Disconnected(object sender, EventArgs e)
        {
            changeIsConnected(false);
            changeState("Disonnected");
            var currentCts = cts;
            if (currentCts == null)
                return;
            _ = delayToConnect(currentCts.Token);
        }

        private async Task delayToConnect(CancellationToken token)
        {
            try
            {
                await Task.Delay(5000, token);
                _ = beginConnect(token);
            }
            catch { }
        }

        private async Task beginConnect(CancellationToken token)
        {
            try
            {
                changeState("Connecting...");
                await glashClient.ConnectAsync();
                changeIsConnected(true);
                changeState("Connected");
            }
            catch (Exception ex)
            {
                changeIsConnected(false);
                changeState("Connect error:" + ExceptionUtils.GetExceptionMessage(ex));
                _ = delayToConnect(token);
                return;
            }
        }

        public async Task<string[]> GetAgentListAsync()
        {
            return await glashClient.GetAgentListAsync();
        }

        public void OnProxyAdded(ProxyInfo model)
        {
            glashClient.AddProxyPortInfo(model);
        }

        public void OnProxyRemoved(ProxyInfo model)
        {
            glashClient.RemoveProxyPortInfo(model.Name);
        }

        public void Dispose()
        {
            foreach (var proxy in Model.ProxyList)
                OnProxyRemoved(proxy);
            cts?.Cancel();
            cts = null;
            glashClient?.Dispose();
            glashClient = null;
        }

        public void EnableProxy(ProxyInfo currentProxyModel)
        {
            glashClient.EnableProxyInfo(currentProxyModel.Name);
        }

        public void DisableProxy(ProxyInfo currentProxyModel)
        {
            glashClient.DisableProxyPortInfo(currentProxyModel.Name);
        }
    }
}
