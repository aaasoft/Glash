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
            gbServerList = new GroupBox();
            lbServers = new ListBox();
            toolStrip1 = new ToolStrip();
            btnAddServer = new ToolStripButton();
            btnEditServer = new ToolStripButton();
            btnDuplicateServer = new ToolStripButton();
            btnDeleteServer = new ToolStripButton();
            gbServerInfo = new GroupBox();
            txtServerState = new TextBox();
            txtServerUrl = new TextBox();
            txtServerName = new TextBox();
            label4 = new Label();
            label2 = new Label();
            label1 = new Label();
            gbProxyList = new GroupBox();
            lvProxyList = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            columnHeader5 = new ColumnHeader();
            columnHeader6 = new ColumnHeader();
            toolStrip2 = new ToolStrip();
            btnAddProxy = new ToolStripButton();
            btnEditProxy = new ToolStripButton();
            btnDeleteProxy = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            btnDuplicateProxy = new ToolStripButton();
            btnEnableProxy = new ToolStripButton();
            btnDisableProxy = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            btnEnableAllProxy = new ToolStripButton();
            btnDisableAllProxy = new ToolStripButton();
            splitContainer1 = new SplitContainer();
            splitContainer3 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            gbLogs = new GroupBox();
            txtLogs = new TextBox();
            gbServerList.SuspendLayout();
            toolStrip1.SuspendLayout();
            gbServerInfo.SuspendLayout();
            gbProxyList.SuspendLayout();
            toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            gbLogs.SuspendLayout();
            SuspendLayout();
            // 
            // niMain
            // 
            niMain.Icon = (Icon)resources.GetObject("niMain.Icon");
            niMain.Text = "Glash Client";
            niMain.Visible = true;
            niMain.MouseClick += niMain_MouseClick;
            // 
            // gbServerList
            // 
            gbServerList.Controls.Add(lbServers);
            gbServerList.Controls.Add(toolStrip1);
            gbServerList.Dock = DockStyle.Fill;
            gbServerList.Location = new Point(0, 0);
            gbServerList.Name = "gbServerList";
            gbServerList.Size = new Size(623, 665);
            gbServerList.TabIndex = 0;
            gbServerList.TabStop = false;
            gbServerList.Text = "Server List";
            // 
            // lbServers
            // 
            lbServers.DisplayMember = "Name";
            lbServers.Dock = DockStyle.Fill;
            lbServers.FormattingEnabled = true;
            lbServers.ItemHeight = 31;
            lbServers.Location = new Point(3, 75);
            lbServers.Name = "lbServers";
            lbServers.Size = new Size(617, 587);
            lbServers.TabIndex = 1;
            lbServers.SelectedIndexChanged += lbServers_SelectedIndexChanged;
            // 
            // toolStrip1
            // 
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.ImageScalingSize = new Size(32, 32);
            toolStrip1.Items.AddRange(new ToolStripItem[] { btnAddServer, btnEditServer, btnDuplicateServer, btnDeleteServer });
            toolStrip1.Location = new Point(3, 34);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(617, 41);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // btnAddServer
            // 
            btnAddServer.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnAddServer.Image = (Image)resources.GetObject("btnAddServer.Image");
            btnAddServer.ImageTransparentColor = Color.Magenta;
            btnAddServer.Name = "btnAddServer";
            btnAddServer.Size = new Size(65, 35);
            btnAddServer.Text = "Add";
            btnAddServer.Click += btnAddServer_Click;
            // 
            // btnEditServer
            // 
            btnEditServer.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnEditServer.Enabled = false;
            btnEditServer.Image = (Image)resources.GetObject("btnEditServer.Image");
            btnEditServer.ImageTransparentColor = Color.Magenta;
            btnEditServer.Name = "btnEditServer";
            btnEditServer.Size = new Size(61, 35);
            btnEditServer.Text = "Edit";
            btnEditServer.Click += btnEditServer_Click;
            // 
            // btnDuplicateServer
            // 
            btnDuplicateServer.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnDuplicateServer.Enabled = false;
            btnDuplicateServer.Image = (Image)resources.GetObject("btnDuplicateServer.Image");
            btnDuplicateServer.ImageTransparentColor = Color.Magenta;
            btnDuplicateServer.Name = "btnDuplicateServer";
            btnDuplicateServer.Size = new Size(126, 35);
            btnDuplicateServer.Text = "Duplicate";
            btnDuplicateServer.Click += btnDuplicateServer_Click;
            // 
            // btnDeleteServer
            // 
            btnDeleteServer.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnDeleteServer.Enabled = false;
            btnDeleteServer.Image = (Image)resources.GetObject("btnDeleteServer.Image");
            btnDeleteServer.ImageTransparentColor = Color.Magenta;
            btnDeleteServer.Name = "btnDeleteServer";
            btnDeleteServer.Size = new Size(93, 35);
            btnDeleteServer.Text = "Delete";
            btnDeleteServer.Click += btnDeleteServer_Click;
            // 
            // gbServerInfo
            // 
            gbServerInfo.Controls.Add(txtServerState);
            gbServerInfo.Controls.Add(txtServerUrl);
            gbServerInfo.Controls.Add(txtServerName);
            gbServerInfo.Controls.Add(label4);
            gbServerInfo.Controls.Add(label2);
            gbServerInfo.Controls.Add(label1);
            gbServerInfo.Dock = DockStyle.Fill;
            gbServerInfo.Location = new Point(0, 0);
            gbServerInfo.Name = "gbServerInfo";
            gbServerInfo.Size = new Size(623, 426);
            gbServerInfo.TabIndex = 1;
            gbServerInfo.TabStop = false;
            gbServerInfo.Text = "Server Info";
            gbServerInfo.Visible = false;
            // 
            // txtServerState
            // 
            txtServerState.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtServerState.Location = new Point(12, 218);
            txtServerState.Multiline = true;
            txtServerState.Name = "txtServerState";
            txtServerState.ReadOnly = true;
            txtServerState.Size = new Size(599, 196);
            txtServerState.TabIndex = 4;
            // 
            // txtServerUrl
            // 
            txtServerUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtServerUrl.Location = new Point(12, 143);
            txtServerUrl.Name = "txtServerUrl";
            txtServerUrl.ReadOnly = true;
            txtServerUrl.Size = new Size(605, 38);
            txtServerUrl.TabIndex = 3;
            // 
            // txtServerName
            // 
            txtServerName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtServerName.Location = new Point(12, 68);
            txtServerName.Name = "txtServerName";
            txtServerName.ReadOnly = true;
            txtServerName.Size = new Size(605, 38);
            txtServerName.TabIndex = 2;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(16, 184);
            label4.Name = "label4";
            label4.Size = new Size(79, 31);
            label4.TabIndex = 0;
            label4.Text = "State:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 109);
            label2.Name = "label2";
            label2.Size = new Size(53, 31);
            label2.TabIndex = 0;
            label2.Text = "Url:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 34);
            label1.Name = "label1";
            label1.Size = new Size(89, 31);
            label1.TabIndex = 0;
            label1.Text = "Name:";
            // 
            // gbProxyList
            // 
            gbProxyList.Controls.Add(lvProxyList);
            gbProxyList.Controls.Add(toolStrip2);
            gbProxyList.Dock = DockStyle.Fill;
            gbProxyList.Location = new Point(0, 0);
            gbProxyList.Name = "gbProxyList";
            gbProxyList.Size = new Size(1244, 665);
            gbProxyList.TabIndex = 2;
            gbProxyList.TabStop = false;
            gbProxyList.Text = "Proxy List";
            gbProxyList.Visible = false;
            // 
            // lvProxyList
            // 
            lvProxyList.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4, columnHeader5, columnHeader6 });
            lvProxyList.Dock = DockStyle.Fill;
            lvProxyList.FullRowSelect = true;
            lvProxyList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvProxyList.Location = new Point(3, 75);
            lvProxyList.MultiSelect = false;
            lvProxyList.Name = "lvProxyList";
            lvProxyList.Size = new Size(1238, 587);
            lvProxyList.TabIndex = 2;
            lvProxyList.UseCompatibleStateImageBehavior = false;
            lvProxyList.View = View.Details;
            lvProxyList.SelectedIndexChanged += lvProxyList_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            columnHeader1.Width = 280;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Type";
            columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Agent";
            columnHeader3.Width = 200;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Local";
            columnHeader4.Width = 260;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "Remote";
            columnHeader5.Width = 260;
            // 
            // columnHeader6
            // 
            columnHeader6.Text = "Enable";
            columnHeader6.Width = 100;
            // 
            // toolStrip2
            // 
            toolStrip2.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip2.ImageScalingSize = new Size(32, 32);
            toolStrip2.Items.AddRange(new ToolStripItem[] { btnAddProxy, btnEditProxy, btnDeleteProxy, toolStripSeparator1, btnDuplicateProxy, btnEnableProxy, btnDisableProxy, toolStripSeparator2, btnEnableAllProxy, btnDisableAllProxy });
            toolStrip2.Location = new Point(3, 34);
            toolStrip2.Name = "toolStrip2";
            toolStrip2.Size = new Size(1238, 41);
            toolStrip2.TabIndex = 1;
            toolStrip2.Text = "toolStrip2";
            // 
            // btnAddProxy
            // 
            btnAddProxy.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnAddProxy.Image = (Image)resources.GetObject("btnAddProxy.Image");
            btnAddProxy.ImageTransparentColor = Color.Magenta;
            btnAddProxy.Name = "btnAddProxy";
            btnAddProxy.Size = new Size(65, 35);
            btnAddProxy.Text = "Add";
            btnAddProxy.Click += btnAddProxy_Click;
            // 
            // btnEditProxy
            // 
            btnEditProxy.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnEditProxy.Enabled = false;
            btnEditProxy.Image = (Image)resources.GetObject("btnEditProxy.Image");
            btnEditProxy.ImageTransparentColor = Color.Magenta;
            btnEditProxy.Name = "btnEditProxy";
            btnEditProxy.Size = new Size(61, 35);
            btnEditProxy.Text = "Edit";
            btnEditProxy.Click += btnEditProxy_Click;
            // 
            // btnDeleteProxy
            // 
            btnDeleteProxy.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnDeleteProxy.Enabled = false;
            btnDeleteProxy.Image = (Image)resources.GetObject("btnDeleteProxy.Image");
            btnDeleteProxy.ImageTransparentColor = Color.Magenta;
            btnDeleteProxy.Name = "btnDeleteProxy";
            btnDeleteProxy.Size = new Size(93, 35);
            btnDeleteProxy.Text = "Delete";
            btnDeleteProxy.Click += btnDeleteProxy_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 41);
            // 
            // btnDuplicateProxy
            // 
            btnDuplicateProxy.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnDuplicateProxy.Enabled = false;
            btnDuplicateProxy.Image = (Image)resources.GetObject("btnDuplicateProxy.Image");
            btnDuplicateProxy.ImageTransparentColor = Color.Magenta;
            btnDuplicateProxy.Name = "btnDuplicateProxy";
            btnDuplicateProxy.Size = new Size(126, 35);
            btnDuplicateProxy.Text = "Duplicate";
            btnDuplicateProxy.Click += btnDuplicateProxy_Click;
            // 
            // btnEnableProxy
            // 
            btnEnableProxy.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnEnableProxy.Enabled = false;
            btnEnableProxy.Image = (Image)resources.GetObject("btnEnableProxy.Image");
            btnEnableProxy.ImageTransparentColor = Color.Magenta;
            btnEnableProxy.Name = "btnEnableProxy";
            btnEnableProxy.Size = new Size(94, 35);
            btnEnableProxy.Text = "Enable";
            btnEnableProxy.Click += btnEnableProxy_Click;
            // 
            // btnDisableProxy
            // 
            btnDisableProxy.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnDisableProxy.Enabled = false;
            btnDisableProxy.Image = (Image)resources.GetObject("btnDisableProxy.Image");
            btnDisableProxy.ImageTransparentColor = Color.Magenta;
            btnDisableProxy.Name = "btnDisableProxy";
            btnDisableProxy.Size = new Size(101, 35);
            btnDisableProxy.Text = "Disable";
            btnDisableProxy.Click += btnDisableProxy_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 41);
            // 
            // btnEnableAllProxy
            // 
            btnEnableAllProxy.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnEnableAllProxy.Image = (Image)resources.GetObject("btnEnableAllProxy.Image");
            btnEnableAllProxy.ImageTransparentColor = Color.Magenta;
            btnEnableAllProxy.Name = "btnEnableAllProxy";
            btnEnableAllProxy.Size = new Size(130, 35);
            btnEnableAllProxy.Text = "Enable All";
            btnEnableAllProxy.Click += btnEnableAllProxy_Click;
            // 
            // btnDisableAllProxy
            // 
            btnDisableAllProxy.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnDisableAllProxy.Image = (Image)resources.GetObject("btnDisableAllProxy.Image");
            btnDisableAllProxy.ImageTransparentColor = Color.Magenta;
            btnDisableAllProxy.Name = "btnDisableAllProxy";
            btnDisableAllProxy.Size = new Size(137, 35);
            btnDisableAllProxy.Text = "Disable All";
            btnDisableAllProxy.Click += btnDisableAllProxy_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer3);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(1871, 1095);
            splitContainer1.SplitterDistance = 623;
            splitContainer1.TabIndex = 3;
            // 
            // splitContainer3
            // 
            splitContainer3.Dock = DockStyle.Fill;
            splitContainer3.Location = new Point(0, 0);
            splitContainer3.Name = "splitContainer3";
            splitContainer3.Orientation = Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(gbServerList);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(gbServerInfo);
            splitContainer3.Size = new Size(623, 1095);
            splitContainer3.SplitterDistance = 665;
            splitContainer3.TabIndex = 1;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(gbProxyList);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(gbLogs);
            splitContainer2.Size = new Size(1244, 1095);
            splitContainer2.SplitterDistance = 665;
            splitContainer2.TabIndex = 0;
            // 
            // gbLogs
            // 
            gbLogs.Controls.Add(txtLogs);
            gbLogs.Dock = DockStyle.Fill;
            gbLogs.Location = new Point(0, 0);
            gbLogs.Name = "gbLogs";
            gbLogs.Size = new Size(1244, 426);
            gbLogs.TabIndex = 0;
            gbLogs.TabStop = false;
            gbLogs.Text = "Logs";
            // 
            // txtLogs
            // 
            txtLogs.Dock = DockStyle.Fill;
            txtLogs.Location = new Point(3, 34);
            txtLogs.Multiline = true;
            txtLogs.Name = "txtLogs";
            txtLogs.ReadOnly = true;
            txtLogs.ScrollBars = ScrollBars.Vertical;
            txtLogs.Size = new Size(1238, 389);
            txtLogs.TabIndex = 5;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1871, 1095);
            Controls.Add(splitContainer1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Glash Client";
            Load += MainForm_Load;
            SizeChanged += MainForm_SizeChanged;
            gbServerList.ResumeLayout(false);
            gbServerList.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            gbServerInfo.ResumeLayout(false);
            gbServerInfo.PerformLayout();
            gbProxyList.ResumeLayout(false);
            gbProxyList.PerformLayout();
            toolStrip2.ResumeLayout(false);
            toolStrip2.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            gbLogs.ResumeLayout(false);
            gbLogs.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private NotifyIcon niMain;
        private GroupBox gbServerList;
        private ToolStrip toolStrip1;
        private ToolStripButton btnAddServer;
        private ToolStripButton btnEditServer;
        private ToolStripButton btnDeleteServer;
        private ListBox lbServers;
        private GroupBox gbServerInfo;
        private GroupBox gbProxyList;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private Label label4;
        private Label label2;
        private Label label1;
        private ToolStrip toolStrip2;
        private ToolStripButton btnAddProxy;
        private ToolStripButton btnEditProxy;
        private ToolStripButton btnDeleteProxy;
        private ListView lvProxyList;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton btnEnableProxy;
        private ToolStripButton btnDisableProxy;
        private ToolStripButton btnEnableAllProxy;
        private ToolStripButton btnDisableAllProxy;
        private ToolStripButton btnDuplicateProxy;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton btnDuplicateServer;
        private SplitContainer splitContainer3;
        private TextBox txtServerState;
        private TextBox txtServerUrl;
        private TextBox txtServerName;
        private GroupBox gbLogs;
        private TextBox txtLogs;
    }
}