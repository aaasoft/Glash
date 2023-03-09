using Glash.Client.WinForm.Utils;
using Glash.Core.Client;
using Newtonsoft.Json;
using Quick.Protocol.Utils;
using System.Collections;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;

namespace Glash.Client.WinForm
{
    public partial class MainForm : Form
    {
        private Config config;
        private int maxLogLines = 1000;
        private ServerInfo currentServerModel;
        private ServerContext currentServerContext;
        private ProxyInfo currentProxyModel;
        private Dictionary<string, ServerContext> serverDict = new Dictionary<string, ServerContext>();

        public MainForm()
        {
            InitializeComponent();
            Text += $" v{Application.ProductVersion}";
            ensureOnlyOne();
        }

        private NamedPipeServerStream createNewNamedPipedServerStream(String pipeName)
        {
            return new NamedPipeServerStream(
                    pipeName,
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);
        }

        private void ensureOnlyOne()
        {
            var pipeName = this.GetType().FullName;
            try
            {
                var serverStream = createNewNamedPipedServerStream(pipeName);
                AsyncCallback ac = null;
                ac = ar =>
                {
                    Invoke(() => showForm());
                    serverStream.Close();
                    serverStream = createNewNamedPipedServerStream(pipeName);
                    serverStream.BeginWaitForConnection(ac, null);
                };
                serverStream.BeginWaitForConnection(ac, null);
            }
            catch
            {
                try
                {
                    var clientStream = new NamedPipeClientStream(pipeName);
                    clientStream.Connect();
                    clientStream.Close();
                }
                finally
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    Environment.Exit(0);
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            config = ConfigFileUtils.Load<Config>();
            if (config == null)
                config = new Config();
            foreach (var server in config.ServerList)
                onServerAdded(server);
            refreshServerList();
        }

        private void pushLog(string log)
        {
            txtLogs.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: {log}{Environment.NewLine}");
            var lines = txtLogs.Lines;
            if (lines.Length > maxLogLines)
            {
                lines = lines.Skip(lines.Length - maxLogLines).ToArray();
                txtLogs.Lines = lines;
            }
            txtLogs.Select(txtLogs.TextLength, 0);
            txtLogs.ScrollToCaret();
        }

        private void onServerAdded(ServerInfo serverInfo)
        {
            var serverName = serverInfo.Name;
            serverDict[serverInfo.Name] = new ServerContext(
                serverInfo,
                () =>
                {
                    if (currentServerModel != serverInfo)
                        return;
                    Invoke(() => txtServerState.Text = currentServerContext.State);
                },
                () =>
                {
                    if (currentServerModel != serverInfo)
                        return;
                    if (currentServerContext.IsConnected)
                        pushLog($"[{serverName}]Connected.");
                    else
                        pushLog($"[{serverName}]Disonnected.");
                },
                e => pushLog($"[{serverName}]{e}")
            );
        }

        private void onServerRemoved(ServerInfo serverInfo)
        {
            var serverContext = serverDict[serverInfo.Name];
            serverDict.Remove(serverInfo.Name);
            serverContext.Dispose();
        }

        private void refreshServerList()
        {
            lbServers.DataSource = null;
            lbServers.DataSource = config.ServerList;
            lbServers.DisplayMember = "Name";
        }

        private void refreshProxyList()
        {
            lvProxyList.Items.Clear();
            foreach (var proxy in currentServerModel.ProxyList)
            {
                var lvi = lvProxyList.Items.Add(proxy.Name);
                lvi.SubItems.Add(proxy.Type.ToString());
                lvi.SubItems.Add(proxy.Agent);
                lvi.SubItems.Add($"{proxy.LocalIPAddress}:{proxy.LocalPort}");
                lvi.SubItems.Add($"{proxy.RemoteHost}:{proxy.RemotePort}");
                lvi.SubItems.Add(proxy.Enable.ToString());
                lvi.Tag = proxy;
            }
        }
        private void showForm()
        {
            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void niMain_MouseClick(object sender, MouseEventArgs e)
        {
            showForm();
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
        }

        private string validateServerName(string serverName, string preServerName = null)
        {
            if (preServerName != null && preServerName == serverName)
                return serverName;
            var modelName = serverName;
            var currentIndex = 2;
            while (true)
            {
                if (config.ServerList.Any(t => t.Name == modelName))
                {
                    modelName = $"{serverName}({currentIndex})";
                    currentIndex++;
                    continue;
                }
                break;
            }
            return modelName;
        }

        private void addServer(ServerInfo model)
        {
            config.ServerList.Add(model);
            ConfigFileUtils.Save(config);
            onServerAdded(model);
            refreshServerList();
        }

        private void btnAddServer_Click(object sender, EventArgs e)
        {
            var form = new EditServerForm();
            var dr = form.ShowDialog();
            if (dr == DialogResult.Cancel)
                return;
            var model = form.Model;
            model.Name = validateServerName(model.Name);
            addServer(model);
        }

        private void btnEditServer_Click(object sender, EventArgs e)
        {
            var form = new EditServerForm();
            form.SetModel(currentServerModel);
            var dr = form.ShowDialog();
            if (dr == DialogResult.Cancel)
                return;
            onServerRemoved(currentServerModel);
            var model = form.Model;
            model.Name = validateServerName(model.Name, currentServerModel.Name);
            currentServerModel.Name = model.Name;
            currentServerModel.Url = model.Url;
            currentServerModel.Password = model.Password;
            ConfigFileUtils.Save(config);
            onServerAdded(currentServerModel);
            refreshServerList();
        }

        private void btnDuplicateServer_Click(object sender, EventArgs e)
        {
            var json = JsonConvert.SerializeObject(currentServerModel);
            var newModel = JsonConvert.DeserializeObject<ServerInfo>(json);
            newModel.Name = validateServerName(newModel.Name);
            foreach (var proxy in newModel.ProxyList)
                proxy.Enable = false;
            addServer(newModel);
        }

        private void btnDeleteServer_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show($"Do you want to delete {currentServerModel}", "Delete Server", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.Cancel)
                return;
            config.ServerList.Remove(currentServerModel);
            ConfigFileUtils.Save(config);
            onServerRemoved(currentServerModel);
            refreshServerList();
        }

        private void lbServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentServerModel = lbServers.SelectedItem as ServerInfo;
            if (currentServerModel == null)
            {
                currentServerContext = null;
                btnEditServer.Enabled = false;
                btnDuplicateServer.Enabled = false;
                btnDeleteServer.Enabled = false;
                gbServerInfo.Visible = false;
                gbProxyList.Visible = false;
                lvProxyList.Items.Clear();

                txtServerName.Clear();
                txtServerUrl.Clear();
                txtServerState.Clear();
            }
            else
            {
                currentServerContext = serverDict[currentServerModel.Name];
                btnEditServer.Enabled = true;
                btnDuplicateServer.Enabled = true;
                btnDeleteServer.Enabled = true;
                gbServerInfo.Visible = true;
                gbProxyList.Visible = true;

                txtServerName.Text = currentServerModel.Name;
                txtServerUrl.Text = currentServerModel.Url;
                txtServerState.Text = currentServerContext.State;

                refreshProxyList();
            }
        }

        private string validateProxyName(ServerInfo serverModel, string proxyName, string preProxyName = null)
        {
            if (preProxyName != null && preProxyName == proxyName)
                return proxyName;
            var modelName = proxyName;
            var currentIndex = 2;
            while (true)
            {
                if (serverModel.ProxyList.Any(t => t.Name == modelName))
                {
                    modelName = $"{proxyName}({currentIndex})";
                    currentIndex++;
                    continue;
                }
                break;
            }
            return modelName;
        }

        private void addProxy(ServerContext serverContext, ProxyInfo model)
        {
            serverContext.Model.ProxyList.Add(model);
            ConfigFileUtils.Save(config);
            serverContext.OnProxyAdded(model);
            refreshProxyList();
        }

        private void btnAddProxy_Click(object sender, EventArgs e)
        {
            var form = new EditProxyForm();
            form.SetServerContext(currentServerContext);
            var dr = form.ShowDialog();
            if (dr == DialogResult.Cancel)
                return;
            var model = form.Model;
            model.Name = validateProxyName(currentServerModel, model.Name);
            addProxy(currentServerContext, model);
        }

        private void btnEditProxy_Click(object sender, EventArgs e)
        {
            var form = new EditProxyForm();
            form.SetServerContext(currentServerContext);
            form.SetModel(currentProxyModel);
            var dr = form.ShowDialog();
            if (dr == DialogResult.Cancel)
                return;
            currentServerContext.OnProxyRemoved(currentProxyModel);
            var model = form.Model;
            model.Name = validateProxyName(currentServerModel, model.Name, currentProxyModel.Name);
            currentProxyModel.Name = model.Name;
            currentProxyModel.Type = model.Type;
            currentProxyModel.Agent = model.Agent;
            currentProxyModel.LocalIPAddress = model.LocalIPAddress;
            currentProxyModel.LocalPort = model.LocalPort;
            currentProxyModel.RemoteHost = model.RemoteHost;
            currentProxyModel.RemotePort = model.RemotePort;
            ConfigFileUtils.Save(config);
            currentServerContext.OnProxyAdded(currentProxyModel);
            refreshProxyList();
        }

        private void btnDuplicateProxy_Click(object sender, EventArgs e)
        {
            var json = JsonConvert.SerializeObject(currentProxyModel);
            var newModel = JsonConvert.DeserializeObject<ProxyInfo>(json);
            newModel.Name = validateProxyName(currentServerModel, newModel.Name);
            newModel.Enable = false;
            addProxy(currentServerContext, newModel);
        }

        private void btnDeleteProxy_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show($"Do you want to delete {currentProxyModel}", "Delete Proxy", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.Cancel)
                return;
            currentServerContext.OnProxyRemoved(currentProxyModel);
            currentServerModel.ProxyList.Remove(currentProxyModel);
            ConfigFileUtils.Save(config);
            refreshProxyList();
        }

        private void lvProxyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvProxyList.SelectedItems.Count == 0)
            {
                currentProxyModel = null;
                btnEditProxy.Enabled = false;
                btnDuplicateProxy.Enabled = false;
                btnDeleteProxy.Enabled = false;
                btnEnableProxy.Enabled = false;
                btnDisableProxy.Enabled = false;
            }
            else
            {
                currentProxyModel = (ProxyInfo)lvProxyList.SelectedItems[0].Tag;
                btnEditProxy.Enabled = true;
                btnDuplicateProxy.Enabled = true;
                btnDeleteProxy.Enabled = true;
                btnEnableProxy.Enabled = true;
                btnDisableProxy.Enabled = true;
            }
        }

        private void btnEnableProxy_Click(object sender, EventArgs e)
        {
            try
            {
                currentServerContext.EnableProxy(currentProxyModel);
                ConfigFileUtils.Save(config);
                refreshProxyList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Enable {currentProxyModel} failed,Reason:{ExceptionUtils.GetExceptionMessage(ex)}", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDisableProxy_Click(object sender, EventArgs e)
        {
            try
            {
                currentServerContext.DisableProxy(currentProxyModel);
                ConfigFileUtils.Save(config);
                refreshProxyList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Disable {currentProxyModel} failed,Reason:{ExceptionUtils.GetExceptionMessage(ex)}", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEnableAllProxy_Click(object sender, EventArgs e)
        {
            foreach (var proxy in currentServerModel.ProxyList)
            {
                if (proxy.Enable)
                    continue;
                try
                {
                    currentServerContext.EnableProxy(proxy);
                }
                catch { }
            }
            ConfigFileUtils.Save(config);
            refreshProxyList();
        }

        private void btnDisableAllProxy_Click(object sender, EventArgs e)
        {
            foreach (var proxy in currentServerModel.ProxyList)
            {
                if (!proxy.Enable)
                    continue;
                try
                {
                    currentServerContext.DisableProxy(proxy);
                }
                catch { }
            }
            ConfigFileUtils.Save(config);
            refreshProxyList();
        }
    }
}