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
    public partial class EditServerForm : Form
    {
        private bool isCreate = true;
        public ServerInfo Model { get; private set; }
        public EditServerForm()
        {
            InitializeComponent();
        }

        public void SetModel(ServerInfo model)
        {
            isCreate = false;
            Model = new ServerInfo()
            {
                Name = model.Name,
                Password = model.Password,
                Url = model.Url,
                ProxyList = model.ProxyList
            };
        }

        private void EditServerForm_Load(object sender, EventArgs e)
        {
            if (isCreate)
            {
                Text = "Add Server";
                Model = new ServerInfo();
            }
            else
            {
                Text = $"Edit Server[{Model.Name}]";
                txtName.Enabled = false;
            }
            txtName.DataBindings.Add(nameof(txtName.Text), Model, nameof(Model.Name));
            txtUrl.DataBindings.Add(nameof(txtUrl.Text), Model, nameof(Model.Url));
            txtPassword.DataBindings.Add(nameof(txtPassword.Text), Model, nameof(Model.Password));
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Model.Name))
            {
                txtName.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(Model.Url))
            {
                txtUrl.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(Model.Password))
            {
                txtPassword.Focus();
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
