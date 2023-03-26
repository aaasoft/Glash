using Glash.Client.WinForm.Core;
using Glash.Client.WinForm.Utils;
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

namespace Glash.Client.WinForm.Forms
{
    public partial class ProfileNameHandleForm : Form
    {
        public string ProfileName { get; set; }
        private Action<string> OkAction;

        public ProfileNameHandleForm()
        {
            InitializeComponent();
        }

        public void Init(string profileName, Action<string> okAction)
        {
            ProfileName = profileName;
            OkAction = okAction;
            txtName.Text = ProfileName;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                ProfileName = txtName.Text.Trim();
                if (!ProfileName.EndsWith(ProfileUtils.PROFILE_FILE_EXTENSION))
                {
                    ProfileName += ProfileUtils.PROFILE_FILE_EXTENSION;
                    txtName.Text = ProfileName;
                }
                OkAction?.Invoke(ProfileName);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (MessageTitleAndTextException ex)
            {
                MessageBox.Show(ex.Text, ex.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ExceptionUtils.GetExceptionString(ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
