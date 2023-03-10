namespace Glash.Client.WinForm.Controls
{
    partial class ProfileControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfileControl));
            scProfile = new SplitContainer();
            scServerListAndInfo = new SplitContainer();
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
            scProxyAndLog = new SplitContainer();
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
            gbLogs = new GroupBox();
            txtLogs = new TextBox();
            ((System.ComponentModel.ISupportInitialize)scProfile).BeginInit();
            scProfile.Panel1.SuspendLayout();
            scProfile.Panel2.SuspendLayout();
            scProfile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)scServerListAndInfo).BeginInit();
            scServerListAndInfo.Panel1.SuspendLayout();
            scServerListAndInfo.Panel2.SuspendLayout();
            scServerListAndInfo.SuspendLayout();
            gbServerList.SuspendLayout();
            toolStrip1.SuspendLayout();
            gbServerInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)scProxyAndLog).BeginInit();
            scProxyAndLog.Panel1.SuspendLayout();
            scProxyAndLog.Panel2.SuspendLayout();
            scProxyAndLog.SuspendLayout();
            gbProxyList.SuspendLayout();
            toolStrip2.SuspendLayout();
            gbLogs.SuspendLayout();
            SuspendLayout();
            // 
            // scProfile
            // 
            scProfile.Dock = DockStyle.Fill;
            scProfile.Location = new Point(0, 0);
            scProfile.Name = "scProfile";
            // 
            // scProfile.Panel1
            // 
            scProfile.Panel1.Controls.Add(scServerListAndInfo);
            // 
            // scProfile.Panel2
            // 
            scProfile.Panel2.Controls.Add(scProxyAndLog);
            scProfile.Size = new Size(1601, 994);
            scProfile.SplitterDistance = 533;
            scProfile.TabIndex = 4;
            // 
            // scServerListAndInfo
            // 
            scServerListAndInfo.Dock = DockStyle.Fill;
            scServerListAndInfo.Location = new Point(0, 0);
            scServerListAndInfo.Name = "scServerListAndInfo";
            scServerListAndInfo.Orientation = Orientation.Horizontal;
            // 
            // scServerListAndInfo.Panel1
            // 
            scServerListAndInfo.Panel1.Controls.Add(gbServerList);
            // 
            // scServerListAndInfo.Panel2
            // 
            scServerListAndInfo.Panel2.Controls.Add(gbServerInfo);
            scServerListAndInfo.Size = new Size(533, 994);
            scServerListAndInfo.SplitterDistance = 602;
            scServerListAndInfo.TabIndex = 1;
            // 
            // gbServerList
            // 
            gbServerList.Controls.Add(lbServers);
            gbServerList.Controls.Add(toolStrip1);
            gbServerList.Dock = DockStyle.Fill;
            gbServerList.Location = new Point(0, 0);
            gbServerList.Name = "gbServerList";
            gbServerList.Size = new Size(533, 602);
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
            lbServers.Size = new Size(527, 524);
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
            toolStrip1.Size = new Size(527, 41);
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
            gbServerInfo.Size = new Size(533, 388);
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
            txtServerState.Size = new Size(515, 164);
            txtServerState.TabIndex = 4;
            // 
            // txtServerUrl
            // 
            txtServerUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtServerUrl.Location = new Point(12, 143);
            txtServerUrl.Name = "txtServerUrl";
            txtServerUrl.ReadOnly = true;
            txtServerUrl.Size = new Size(515, 38);
            txtServerUrl.TabIndex = 3;
            // 
            // txtServerName
            // 
            txtServerName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtServerName.Location = new Point(12, 68);
            txtServerName.Name = "txtServerName";
            txtServerName.ReadOnly = true;
            txtServerName.Size = new Size(515, 38);
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
            // scProxyAndLog
            // 
            scProxyAndLog.Dock = DockStyle.Fill;
            scProxyAndLog.Location = new Point(0, 0);
            scProxyAndLog.Name = "scProxyAndLog";
            scProxyAndLog.Orientation = Orientation.Horizontal;
            // 
            // scProxyAndLog.Panel1
            // 
            scProxyAndLog.Panel1.Controls.Add(gbProxyList);
            // 
            // scProxyAndLog.Panel2
            // 
            scProxyAndLog.Panel2.Controls.Add(gbLogs);
            scProxyAndLog.Size = new Size(1064, 994);
            scProxyAndLog.SplitterDistance = 602;
            scProxyAndLog.TabIndex = 0;
            // 
            // gbProxyList
            // 
            gbProxyList.Controls.Add(lvProxyList);
            gbProxyList.Controls.Add(toolStrip2);
            gbProxyList.Dock = DockStyle.Fill;
            gbProxyList.Location = new Point(0, 0);
            gbProxyList.Name = "gbProxyList";
            gbProxyList.Size = new Size(1064, 602);
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
            lvProxyList.Size = new Size(1058, 524);
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
            toolStrip2.Size = new Size(1058, 41);
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
            // gbLogs
            // 
            gbLogs.Controls.Add(txtLogs);
            gbLogs.Dock = DockStyle.Fill;
            gbLogs.Location = new Point(0, 0);
            gbLogs.Name = "gbLogs";
            gbLogs.Size = new Size(1064, 388);
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
            txtLogs.Size = new Size(1058, 351);
            txtLogs.TabIndex = 5;
            // 
            // ProfileControl
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(scProfile);
            Name = "ProfileControl";
            Size = new Size(1601, 994);
            Load += ProfileControl_Load;
            scProfile.Panel1.ResumeLayout(false);
            scProfile.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)scProfile).EndInit();
            scProfile.ResumeLayout(false);
            scServerListAndInfo.Panel1.ResumeLayout(false);
            scServerListAndInfo.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)scServerListAndInfo).EndInit();
            scServerListAndInfo.ResumeLayout(false);
            gbServerList.ResumeLayout(false);
            gbServerList.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            gbServerInfo.ResumeLayout(false);
            gbServerInfo.PerformLayout();
            scProxyAndLog.Panel1.ResumeLayout(false);
            scProxyAndLog.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)scProxyAndLog).EndInit();
            scProxyAndLog.ResumeLayout(false);
            gbProxyList.ResumeLayout(false);
            gbProxyList.PerformLayout();
            toolStrip2.ResumeLayout(false);
            toolStrip2.PerformLayout();
            gbLogs.ResumeLayout(false);
            gbLogs.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer scProfile;
        private SplitContainer scServerListAndInfo;
        private GroupBox gbServerList;
        private ListBox lbServers;
        private ToolStrip toolStrip1;
        private ToolStripButton btnAddServer;
        private ToolStripButton btnEditServer;
        private ToolStripButton btnDuplicateServer;
        private ToolStripButton btnDeleteServer;
        private GroupBox gbServerInfo;
        private TextBox txtServerState;
        private TextBox txtServerUrl;
        private TextBox txtServerName;
        private Label label4;
        private Label label2;
        private Label label1;
        private SplitContainer scProxyAndLog;
        private GroupBox gbProxyList;
        private ListView lvProxyList;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private ToolStrip toolStrip2;
        private ToolStripButton btnAddProxy;
        private ToolStripButton btnEditProxy;
        private ToolStripButton btnDeleteProxy;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton btnDuplicateProxy;
        private ToolStripButton btnEnableProxy;
        private ToolStripButton btnDisableProxy;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton btnEnableAllProxy;
        private ToolStripButton btnDisableAllProxy;
        private GroupBox gbLogs;
        private TextBox txtLogs;
    }
}
