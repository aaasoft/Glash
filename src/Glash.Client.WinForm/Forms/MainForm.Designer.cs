namespace Glash.Client.WinForm
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            niMain = new NotifyIcon(components);
            pnlProfiles = new Panel();
            btnImportProfile = new Button();
            btnDeleteProfile = new Button();
            btnRenameProfile = new Button();
            btnLoadProfile = new Button();
            btnNewProfile = new Button();
            lbProfiles = new ListBox();
            label1 = new Label();
            btnCloseProfile = new Button();
            pnlProfiles.SuspendLayout();
            SuspendLayout();
            // 
            // niMain
            // 
            niMain.Icon = (Icon)resources.GetObject("niMain.Icon");
            niMain.Text = "Glash Client";
            niMain.Visible = true;
            niMain.MouseClick += niMain_MouseClick;
            // 
            // pnlProfiles
            // 
            pnlProfiles.Controls.Add(btnImportProfile);
            pnlProfiles.Controls.Add(btnDeleteProfile);
            pnlProfiles.Controls.Add(btnRenameProfile);
            pnlProfiles.Controls.Add(btnLoadProfile);
            pnlProfiles.Controls.Add(btnNewProfile);
            pnlProfiles.Controls.Add(lbProfiles);
            pnlProfiles.Controls.Add(label1);
            pnlProfiles.Dock = DockStyle.Fill;
            pnlProfiles.Location = new Point(0, 0);
            pnlProfiles.Name = "pnlProfiles";
            pnlProfiles.Size = new Size(1871, 1095);
            pnlProfiles.TabIndex = 5;
            // 
            // btnImportProfile
            // 
            btnImportProfile.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnImportProfile.Location = new Point(168, 1019);
            btnImportProfile.Name = "btnImportProfile";
            btnImportProfile.Size = new Size(150, 64);
            btnImportProfile.TabIndex = 6;
            btnImportProfile.Text = "&Import";
            btnImportProfile.UseVisualStyleBackColor = true;
            btnImportProfile.Click += btnImportProfile_Click;
            // 
            // btnDeleteProfile
            // 
            btnDeleteProfile.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnDeleteProfile.Enabled = false;
            btnDeleteProfile.Location = new Point(636, 1019);
            btnDeleteProfile.Name = "btnDeleteProfile";
            btnDeleteProfile.Size = new Size(150, 64);
            btnDeleteProfile.TabIndex = 5;
            btnDeleteProfile.Text = "&Delete";
            btnDeleteProfile.UseVisualStyleBackColor = true;
            btnDeleteProfile.Click += btnDeleteProfile_Click;
            // 
            // btnRenameProfile
            // 
            btnRenameProfile.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRenameProfile.Enabled = false;
            btnRenameProfile.Location = new Point(480, 1019);
            btnRenameProfile.Name = "btnRenameProfile";
            btnRenameProfile.Size = new Size(150, 64);
            btnRenameProfile.TabIndex = 4;
            btnRenameProfile.Text = "&Rename";
            btnRenameProfile.UseVisualStyleBackColor = true;
            btnRenameProfile.Click += btnRenameProfile_Click;
            // 
            // btnLoadProfile
            // 
            btnLoadProfile.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnLoadProfile.Enabled = false;
            btnLoadProfile.Location = new Point(324, 1019);
            btnLoadProfile.Name = "btnLoadProfile";
            btnLoadProfile.Size = new Size(150, 64);
            btnLoadProfile.TabIndex = 3;
            btnLoadProfile.Text = "&Load";
            btnLoadProfile.UseVisualStyleBackColor = true;
            btnLoadProfile.Click += btnLoadProfile_Click;
            // 
            // btnNewProfile
            // 
            btnNewProfile.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnNewProfile.Location = new Point(12, 1019);
            btnNewProfile.Name = "btnNewProfile";
            btnNewProfile.Size = new Size(150, 64);
            btnNewProfile.TabIndex = 2;
            btnNewProfile.Text = "&New";
            btnNewProfile.UseVisualStyleBackColor = true;
            btnNewProfile.Click += btnNewProfile_Click;
            // 
            // lbProfiles
            // 
            lbProfiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lbProfiles.FormattingEnabled = true;
            lbProfiles.ItemHeight = 31;
            lbProfiles.Location = new Point(3, 37);
            lbProfiles.Name = "lbProfiles";
            lbProfiles.Size = new Size(1868, 965);
            lbProfiles.TabIndex = 1;
            lbProfiles.SelectedIndexChanged += lbProfiles_SelectedIndexChanged;
            lbProfiles.MouseDoubleClick += lbProfiles_MouseDoubleClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 3);
            label1.Name = "label1";
            label1.Size = new Size(104, 31);
            label1.TabIndex = 0;
            label1.Text = "Profiles:";
            // 
            // btnCloseProfile
            // 
            btnCloseProfile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCloseProfile.Location = new Point(1703, 0);
            btnCloseProfile.Name = "btnCloseProfile";
            btnCloseProfile.Size = new Size(168, 68);
            btnCloseProfile.TabIndex = 6;
            btnCloseProfile.Text = "Close Profile";
            btnCloseProfile.UseVisualStyleBackColor = true;
            btnCloseProfile.Click += btnCloseProfile_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1871, 1095);
            Controls.Add(btnCloseProfile);
            Controls.Add(pnlProfiles);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Glash Client";
            Load += MainForm_Load;
            SizeChanged += MainForm_SizeChanged;
            pnlProfiles.ResumeLayout(false);
            pnlProfiles.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private NotifyIcon niMain;
        private Panel pnlProfiles;
        private Button btnNewProfile;
        private ListBox lbProfiles;
        private Label label1;
        private Button btnRenameProfile;
        private Button btnLoadProfile;
        private Button btnImportProfile;
        private Button btnDeleteProfile;
        private Button btnCloseProfile;
    }
}