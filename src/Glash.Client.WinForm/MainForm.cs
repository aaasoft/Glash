using Glash.Client.WinForm.Utils;
using Glash.Core.Client;

namespace Glash.Client.WinForm
{
    public partial class MainForm : Form
    {
        private Config config;
        private ServerInfo currentServerModel;
        private ServerContext currentServerContext;
        private ProxyInfo currentProxyModel;
        private Dictionary<string, ServerContext> serverDict = new Dictionary<string, ServerContext>();

        public MainForm()
        {
            InitializeComponent();
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

        private void onServerAdded(ServerInfo serverInfo)
        {
            serverDict[serverInfo.Name] = new ServerContext(
                serverInfo,
                () =>
                {
                    if (currentServerModel != serverInfo)
                        return;
                    Invoke(() => lblServerState.Text = currentServerContext.State);
                },
                () =>
                {
                    if (currentServerModel != serverInfo)
                        return;
                    //Invoke(() => lblServerState.Text = currentServerContext.State);
                });
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

        private void niMain_MouseClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
        }

        private void btnAddServer_Click(object sender, EventArgs e)
        {
            var form = new EditServerForm();
            var dr = form.ShowDialog();
            if (dr == DialogResult.Cancel)
                return;
            var model = form.Model;
            var modelName = model.Name;
            var currentIndex = 2;
            while (true)
            {
                if (config.ServerList.Any(t => t.Name == model.Name))
                {
                    model.Name = $"{modelName}({currentIndex})";
                    currentIndex++;
                    continue;
                }
                break;
            }
            config.ServerList.Add(model);
            ConfigFileUtils.Save(config);
            onServerAdded(model);
            refreshServerList();
        }

        private void btnEditServer_Click(object sender, EventArgs e)
        {
            var form = new EditServerForm();
            form.SetModel(currentServerModel);
            var dr = form.ShowDialog();
            if (dr == DialogResult.Cancel)
                return;
            var model = form.Model;
            currentServerModel.Url = model.Url;
            currentServerModel.Password = model.Password;
            ConfigFileUtils.Save(config);
            onServerRemoved(model);
            onServerAdded(model);
            refreshServerList();
        }

        private void btnDeleteServer_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show($"Do you want to delete {currentServerModel}", Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
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
                btnDeleteServer.Enabled = false;
                gbServerInfo.Visible = false;
                gbProxyList.Visible = false;
                lvProxyList.Items.Clear();
            }
            else
            {
                currentServerContext = serverDict[currentServerModel.Name];
                btnEditServer.Enabled = true;
                btnDeleteServer.Enabled = true;
                gbServerInfo.Visible = true;
                gbProxyList.Visible = true;

                lblServerName.Text = currentServerModel.Name;
                lblServerUrl.Text = currentServerModel.Url;
                lblServerState.Text = currentServerContext.State;

                refreshProxyList();
            }
        }

        private void btnAddProxy_Click(object sender, EventArgs e)
        {
            var form = new EditProxyForm();
            form.SetServerContext(currentServerContext);
            var dr = form.ShowDialog();
            if (dr == DialogResult.Cancel)
                return;
            var model = form.Model;
            var modelName = model.Name;
            var currentIndex = 2;
            while (true)
            {
                if (currentServerModel.ProxyList.Any(t => t.Name == model.Name))
                {
                    model.Name = $"{modelName}({currentIndex})";
                    currentIndex++;
                    continue;
                }
                break;
            }
            currentServerModel.ProxyList.Add(model);
            ConfigFileUtils.Save(config);
            currentServerContext.OnProxyAdded(model);
            refreshProxyList();
        }

        private void btnEditProxy_Click(object sender, EventArgs e)
        {
            var form = new EditProxyForm();
            form.SetServerContext(currentServerContext);
            form.SetModel(currentProxyModel);
            var dr = form.ShowDialog();
            if (dr == DialogResult.Cancel)
                return;
            var model = form.Model;
            currentServerContext.OnProxyRemoved(currentProxyModel);
            currentProxyModel.Type = model.Type;
            currentProxyModel.Agent = model.Agent;
            currentProxyModel.LocalIPAddress = model.LocalIPAddress;
            currentProxyModel.LocalPort = model.LocalPort;
            currentProxyModel.RemoteHost = model.RemoteHost;
            currentProxyModel.RemotePort = model.RemotePort;
            ConfigFileUtils.Save(config);
            currentServerContext.OnProxyAdded(model);
            refreshProxyList();
        }

        private void btnDeleteProxy_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show($"Do you want to delete {currentProxyModel}", Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
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
                btnDeleteProxy.Enabled = false;
            }
            else
            {
                currentProxyModel = (ProxyInfo)lvProxyList.SelectedItems[0].Tag;
                btnEditProxy.Enabled = true;
                btnDeleteProxy.Enabled = true;
            }
        }

        private void btnEnableProxy_Click(object sender, EventArgs e)
        {
            currentServerContext.EnableProxy(currentProxyModel);
            ConfigFileUtils.Save(config);
            refreshProxyList();
        }

        private void btnDisableProxy_Click(object sender, EventArgs e)
        {
            currentServerContext.DisableProxy(currentProxyModel);
            ConfigFileUtils.Save(config);
            refreshProxyList();
        }

        private void btnEnableAllProxy_Click(object sender, EventArgs e)
        {
            foreach (var proxy in currentServerModel.ProxyList)
            {
                if (proxy.Enable)
                    continue;
                currentServerContext.EnableProxy(proxy);
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
                currentServerContext.DisableProxy(proxy);
            }
            ConfigFileUtils.Save(config);
            refreshProxyList();
        }
    }
}