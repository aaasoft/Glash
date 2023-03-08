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
            btnDeleteServer = new ToolStripButton();
            gbServerInfo = new GroupBox();
            lblServerState = new Label();
            label4 = new Label();
            lblServerUrl = new Label();
            label2 = new Label();
            lblServerName = new Label();
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
            btnEnableProxy = new ToolStripButton();
            btnDisableProxy = new ToolStripButton();
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            btnEnableAllProxy = new ToolStripButton();
            btnDisableAllProxy = new ToolStripButton();
            gbServerList.SuspendLayout();
            toolStrip1.SuspendLayout();
            gbServerInfo.SuspendLayout();
            gbProxyList.SuspendLayout();
            toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
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
            gbServerList.Size = new Size(569, 945);
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
            lbServers.Size = new Size(563, 867);
            lbServers.TabIndex = 1;
            lbServers.SelectedIndexChanged += lbServers_SelectedIndexChanged;
            // 
            // toolStrip1
            // 
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.ImageScalingSize = new Size(32, 32);
            toolStrip1.Items.AddRange(new ToolStripItem[] { btnAddServer, btnEditServer, btnDeleteServer });
            toolStrip1.Location = new Point(3, 34);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(563, 41);
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
            gbServerInfo.Controls.Add(lblServerState);
            gbServerInfo.Controls.Add(label4);
            gbServerInfo.Controls.Add(lblServerUrl);
            gbServerInfo.Controls.Add(label2);
            gbServerInfo.Controls.Add(lblServerName);
            gbServerInfo.Controls.Add(label1);
            gbServerInfo.Dock = DockStyle.Fill;
            gbServerInfo.Location = new Point(0, 0);
            gbServerInfo.Name = "gbServerInfo";
            gbServerInfo.Size = new Size(1134, 217);
            gbServerInfo.TabIndex = 1;
            gbServerInfo.TabStop = false;
            gbServerInfo.Text = "Server Info";
            gbServerInfo.Visible = false;
            // 
            // lblServerState
            // 
            lblServerState.AutoSize = true;
            lblServerState.Location = new Point(167, 130);
            lblServerState.Name = "lblServerState";
            lblServerState.Size = new Size(73, 31);
            lblServerState.TabIndex = 1;
            lblServerState.Text = "State";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(39, 130);
            label4.Name = "label4";
            label4.Size = new Size(79, 31);
            label4.TabIndex = 0;
            label4.Text = "State:";
            // 
            // lblServerUrl
            // 
            lblServerUrl.AutoSize = true;
            lblServerUrl.Location = new Point(167, 85);
            lblServerUrl.Name = "lblServerUrl";
            lblServerUrl.Size = new Size(47, 31);
            lblServerUrl.TabIndex = 1;
            lblServerUrl.Text = "Url";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(65, 85);
            label2.Name = "label2";
            label2.Size = new Size(53, 31);
            label2.TabIndex = 0;
            label2.Text = "Url:";
            // 
            // lblServerName
            // 
            lblServerName.AutoSize = true;
            lblServerName.Location = new Point(167, 44);
            lblServerName.Name = "lblServerName";
            lblServerName.Size = new Size(163, 31);
            lblServerName.TabIndex = 1;
            lblServerName.Text = "Server Name";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(29, 44);
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
            gbProxyList.Size = new Size(1134, 724);
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
            lvProxyList.Size = new Size(1128, 646);
            lvProxyList.TabIndex = 2;
            lvProxyList.UseCompatibleStateImageBehavior = false;
            lvProxyList.View = View.Details;
            lvProxyList.SelectedIndexChanged += lvProxyList_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            columnHeader1.Width = 240;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Type";
            columnHeader2.Width = 80;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Agent";
            columnHeader3.Width = 180;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Local";
            columnHeader4.Width = 240;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "Remote";
            columnHeader5.Width = 240;
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
            toolStrip2.Items.AddRange(new ToolStripItem[] { btnAddProxy, btnEditProxy, btnDeleteProxy, toolStripSeparator1, btnEnableProxy, btnDisableProxy, btnEnableAllProxy, btnDisableAllProxy });
            toolStrip2.Location = new Point(3, 34);
            toolStrip2.Name = "toolStrip2";
            toolStrip2.Size = new Size(1128, 41);
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
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(gbServerList);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(1707, 945);
            splitContainer1.SplitterDistance = 569;
            splitContainer1.TabIndex = 3;
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
            splitContainer2.Panel1.Controls.Add(gbServerInfo);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(gbProxyList);
            splitContainer2.Size = new Size(1134, 945);
            splitContainer2.SplitterDistance = 217;
            splitContainer2.TabIndex = 0;
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
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1707, 945);
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
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
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
        private Label lblServerState;
        private Label label4;
        private Label lblServerUrl;
        private Label label2;
        private Label lblServerName;
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
    }
}