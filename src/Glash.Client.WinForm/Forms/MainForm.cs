using Glash.Client.WinForm.Core;
using Glash.Client.WinForm.Forms;
using Glash.Client.WinForm.Model;
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
        private string currentProfileName = null;
        private Controls.ProfileControl currentProfileControl = null;

        public MainForm()
        {
            InitializeComponent();
            ensureOnlyOne();
        }

        private void refreshMainForm()
        {
            if (currentProfileControl == null)
            {
                Text = $"{Application.ProductName} v{Application.ProductVersion}";
                pnlProfiles.Visible = true;
                btnCloseProfile.Visible = false;
            }
            else
            {
                Text = $"{currentProfileName} - {Application.ProductName} v{Application.ProductVersion}";
                pnlProfiles.Visible = false;
                btnCloseProfile.Visible = true;
            }
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
            lbProfiles.DataSource = ProfileUtils.GetProfileNames();
            refreshMainForm();
        }

        private void refreshProfiles()
        {
            Invoke(() =>
            {
                var preSelectedProfileName = currentProfileName;
                currentProfileName = null;
                lbProfiles.SelectedItem = null;

                lbProfiles.DataSource = ProfileUtils.GetProfileNames();
                lbProfiles.SelectedItem = preSelectedProfileName;
            });
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

        private void btnNewProfile_Click(object sender, EventArgs e)
        {
            var form = new ProfileNameHandleForm();
            form.Init("NewProfile" + ProfileUtils.PROFILE_FILE_EXTENSION, t =>
            {
                var file = ProfileUtils.GetProfileFullPathFromProfileName(t);
                if (File.Exists(file))
                    throw new MessageTitleAndTextException("Create profile error", $"Profile[{t}] already exist.");
                using (var writer = File.CreateText(file))
                    writer.Write(ProfileUtils.GetDefaultProfileContent());
            });
            var dr = form.ShowDialog();
            if (dr == DialogResult.Cancel)
                return;
            lbProfiles.SelectedItem = null;
            refreshProfiles();
        }

        private void btnImportProfile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = $"Glash Client Profile file(*{ProfileUtils.PROFILE_FILE_EXTENSION})|*{ProfileUtils.PROFILE_FILE_EXTENSION}";
            var dr = ofd.ShowDialog();
            if (dr == DialogResult.Cancel)
                return;

            try
            {
                foreach (var file in ofd.FileNames)
                {
                    var profileName = Path.GetFileNameWithoutExtension(file);
                    var currentIndex = 2;
                    var currentProfileName = profileName;
                    while (true)
                    {
                        var dstFileName = ProfileUtils.GetProfileFullPathFromProfileName(currentProfileName + ProfileUtils.PROFILE_FILE_EXTENSION);
                        if (!File.Exists(dstFileName))
                        {
                            File.Copy(file, dstFileName);
                            break;
                        }
                        currentProfileName = $"{profileName}({currentIndex})";
                        currentIndex++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Import profile failed.Reason:{ExceptionUtils.GetExceptionMessage(ex)}", "Import Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                refreshProfiles();
            }
        }


        private void btnLoadProfile_Click(object sender, EventArgs e)
        {
            try
            {
                pnlProfiles.Visible = false;
                Application.DoEvents();

                var profile = ProfileInfo.Load(currentProfileName);
                currentProfileControl = new Controls.ProfileControl();
                currentProfileControl.SetProfile(profile);
                currentProfileControl.Dock = DockStyle.Fill;
                Controls.Add(currentProfileControl);
                refreshMainForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Load profile {currentProfileName} failed.Reason:{ExceptionUtils.GetExceptionMessage(ex)}", "Load Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                refreshMainForm();
            }
        }

        private void btnRenameProfile_Click(object sender, EventArgs e)
        {
            var srcProfileName = currentProfileName;
            var form = new ProfileNameHandleForm();
            form.Init(srcProfileName, t =>
            {
                if (srcProfileName == t)
                    return;

                var file = ProfileUtils.GetProfileFullPathFromProfileName(t);
                if (File.Exists(file))
                    throw new MessageTitleAndTextException("Rename profile error", $"Profile[{t}] already exist.");
                File.Move(ProfileUtils.GetProfileFullPathFromProfileName(srcProfileName), file);
            });
            var dr = form.ShowDialog();
            if (dr == DialogResult.Cancel)
                return;
            refreshProfiles();
        }

        private void btnDeleteProfile_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show($"Do you want to delete profile [{currentProfileName}]?", "Delete Profile", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.Cancel)
                return;
            try
            {
                File.Delete(ProfileUtils.GetProfileFullPathFromProfileName(currentProfileName));
                refreshProfiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Delete profile {currentProfileName} failed.Reason:{ExceptionUtils.GetExceptionMessage(ex)}", "Delete Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void lbProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentProfileName = lbProfiles.SelectedItem?.ToString();
            if (currentProfileName == null)
            {
                btnLoadProfile.Enabled = false;
                btnRenameProfile.Enabled = false;
                btnDeleteProfile.Enabled = false;
            }
            else
            {
                btnLoadProfile.Enabled = true;
                btnRenameProfile.Enabled = true;
                btnDeleteProfile.Enabled = true;
            }
        }

        private void lbProfiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (currentProfileName != null)
                btnLoadProfile_Click(sender, e);
        }

        private void btnCloseProfile_Click(object sender, EventArgs e)
        {
            if (currentProfileControl != null)
            {
                Controls.Remove(currentProfileControl);
                currentProfileControl.OnClose();
                currentProfileControl = null;
            }
            refreshProfiles();
            refreshMainForm();
        }
    }
}