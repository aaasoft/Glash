using Glash.Client.WinForm.Core;
using Glash.Core.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Glash.Client.WinForm
{
    public partial class EditProxyForm : Form
    {
        private bool isCreate = true;
        private ServerContext serverContext;
        public ProxyInfo Model { get; private set; }
        public EditProxyForm()
        {
            InitializeComponent();
        }

        public void SetServerContext(ServerContext serverContext)
        {
            this.serverContext = serverContext;
        }

        public void SetModel(ProxyInfo model)
        {
            isCreate = false;
            Model = new ProxyInfo()
            {
                Name = model.Name,
                Agent = model.Agent,
                LocalIPAddress = model.LocalIPAddress,
                LocalPort = model.LocalPort,
                RemoteHost = model.RemoteHost,
                RemotePort = model.RemotePort,
                Enable = model.Enable
            };
        }

        private void EditProxyForm_Load(object sender, EventArgs e)
        {
            if (isCreate)
            {
                Text = "Add Proxy";
                Model = new ProxyInfo();
            }
            else
            {
                Text = $"Edit Proxy[{Model.Name}]";
            }
            txtName.DataBindings.Add(nameof(txtName.Text), Model, nameof(Model.Name));
            cbAgent.DataBindings.Add(nameof(cbAgent.SelectedItem), Model, nameof(Model.Agent));
            txtLocalIPAddress.DataBindings.Add(nameof(txtName.Text), Model, nameof(Model.LocalIPAddress));
            nudLocalPort.DataBindings.Add(nameof(nudLocalPort.Value), Model, nameof(Model.LocalPort));
            txtRemoteHost.DataBindings.Add(nameof(txtRemoteHost.Text), Model, nameof(Model.RemoteHost));
            nudRemotePort.DataBindings.Add(nameof(nudRemotePort.Value), Model, nameof(Model.RemotePort));
            Task.Run(async () =>
            {
                var agents = await serverContext.GetAgentListAsync();
                Invoke(() =>
                {
                    cbAgent.Items.AddRange(agents);
                    cbAgent.DataBindings.Clear();
                    cbAgent.DataBindings.Add(nameof(cbAgent.SelectedItem), Model, nameof(Model.Agent));
                });
            });
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Model.Name))
            {
                txtName.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(Model.Agent))
            {
                cbAgent.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(Model.LocalIPAddress))
            {
                txtLocalIPAddress.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(Model.RemoteHost))
            {
                txtRemoteHost.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
