using Glash.Client.WinForm.Core;
using Glash.Client.WinForm.Model;
using Glash.Client.WinForm.Utils;
using Glash.Core.Client;
using Newtonsoft.Json;
using Quick.Protocol.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Glash.Client.WinForm.Controls
{
    public partial class ProfileControl : UserControl
    {
        private int maxLogLines = 1000;
        private ServerInfo currentServerModel;
        private ServerContext currentServerContext;
        private ProxyInfo currentProxyModel;
        private Dictionary<string, ServerContext> serverDict = new Dictionary<string, ServerContext>();

        public ProfileInfo Profile { get; private set; }

        public ProfileControl()
        {
            InitializeComponent();
        }

        public void SetProfile(ProfileInfo profile)
        {
            Profile = profile;
        }

        public void Start()
        {
            foreach (var server in Profile.Model.ServerList)
                onServerAdded(server);
            refreshServerList();
        }

        public void Stop()
        {
            foreach (var server in serverDict.Values)
                server.Dispose();
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
            lbServers.DataSource = Profile.Model.ServerList;
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

        private string validateServerName(string serverName, string preServerName = null)
        {
            if (preServerName != null && preServerName == serverName)
                return serverName;
            var modelName = serverName;
            var currentIndex = 2;
            while (true)
            {
                if (Profile.Model.ServerList.Any(t => t.Name == modelName))
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
            Profile.Model.ServerList.Add(model);
            Profile.Save();
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
            Profile.Save();
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
            Profile.Model.ServerList.Remove(currentServerModel);
            Profile.Save();
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
            Profile.Save();
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
            Profile.Save();
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
            Profile.Save();
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
                Profile.Save();
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
                Profile.Save();
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
            Profile.Save();
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
            Profile.Save();
            refreshProxyList();
        }
    }
}
