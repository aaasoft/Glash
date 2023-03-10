namespace Glash.Client.WinForm
{
    partial class EditProxyForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditProxyForm));
            btnCancel = new Button();
            btnSave = new Button();
            label2 = new Label();
            txtName = new TextBox();
            label1 = new Label();
            cbType = new ComboBox();
            label3 = new Label();
            cbAgent = new ComboBox();
            label4 = new Label();
            txtLocalIPAddress = new TextBox();
            label5 = new Label();
            nudLocalPort = new NumericUpDown();
            label6 = new Label();
            label7 = new Label();
            txtRemoteHost = new TextBox();
            nudRemotePort = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)nudLocalPort).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudRemotePort).BeginInit();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnCancel.Location = new Point(168, 570);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(150, 46);
            btnCancel.TabIndex = 101;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSave.Location = new Point(12, 570);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(150, 46);
            btnSave.TabIndex = 100;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 89);
            label2.Name = "label2";
            label2.Size = new Size(76, 31);
            label2.TabIndex = 103;
            label2.Text = "Type:";
            // 
            // txtName
            // 
            txtName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtName.Location = new Point(12, 48);
            txtName.Name = "txtName";
            txtName.Size = new Size(557, 38);
            txtName.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 14);
            label1.Name = "label1";
            label1.Size = new Size(89, 31);
            label1.TabIndex = 104;
            label1.Text = "Name:";
            // 
            // cbType
            // 
            cbType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbType.FormattingEnabled = true;
            cbType.Location = new Point(12, 123);
            cbType.Name = "cbType";
            cbType.Size = new Size(557, 39);
            cbType.TabIndex = 2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 165);
            label3.Name = "label3";
            label3.Size = new Size(90, 31);
            label3.TabIndex = 103;
            label3.Text = "Agent:";
            // 
            // cbAgent
            // 
            cbAgent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbAgent.FormattingEnabled = true;
            cbAgent.Location = new Point(12, 199);
            cbAgent.Name = "cbAgent";
            cbAgent.Size = new Size(557, 39);
            cbAgent.TabIndex = 3;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 241);
            label4.Name = "label4";
            label4.Size = new Size(199, 31);
            label4.TabIndex = 104;
            label4.Text = "Local IPAddress:";
            // 
            // txtLocalIPAddress
            // 
            txtLocalIPAddress.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtLocalIPAddress.Location = new Point(12, 275);
            txtLocalIPAddress.Name = "txtLocalIPAddress";
            txtLocalIPAddress.Size = new Size(557, 38);
            txtLocalIPAddress.TabIndex = 4;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 316);
            label5.Name = "label5";
            label5.Size = new Size(133, 31);
            label5.TabIndex = 104;
            label5.Text = "Local Port:";
            // 
            // nudLocalPort
            // 
            nudLocalPort.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            nudLocalPort.Location = new Point(12, 350);
            nudLocalPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            nudLocalPort.Name = "nudLocalPort";
            nudLocalPort.Size = new Size(557, 38);
            nudLocalPort.TabIndex = 5;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 391);
            label6.Name = "label6";
            label6.Size = new Size(171, 31);
            label6.TabIndex = 104;
            label6.Text = "Remote Host:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 466);
            label7.Name = "label7";
            label7.Size = new Size(165, 31);
            label7.TabIndex = 104;
            label7.Text = "Remote Port:";
            // 
            // txtRemoteHost
            // 
            txtRemoteHost.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtRemoteHost.Location = new Point(12, 425);
            txtRemoteHost.Name = "txtRemoteHost";
            txtRemoteHost.Size = new Size(557, 38);
            txtRemoteHost.TabIndex = 6;
            // 
            // nudRemotePort
            // 
            nudRemotePort.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            nudRemotePort.Location = new Point(12, 500);
            nudRemotePort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            nudRemotePort.Name = "nudRemotePort";
            nudRemotePort.Size = new Size(557, 38);
            nudRemotePort.TabIndex = 7;
            // 
            // EditProxyForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(581, 629);
            Controls.Add(nudRemotePort);
            Controls.Add(nudLocalPort);
            Controls.Add(cbAgent);
            Controls.Add(cbType);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(txtRemoteHost);
            Controls.Add(txtLocalIPAddress);
            Controls.Add(txtName);
            Controls.Add(label7);
            Controls.Add(label5);
            Controls.Add(label6);
            Controls.Add(label4);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "EditProxyForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "EditProxyForm";
            Load += EditProxyForm_Load;
            ((System.ComponentModel.ISupportInitialize)nudLocalPort).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudRemotePort).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnCancel;
        private Button btnSave;
        private Label label2;
        private TextBox txtName;
        private Label label1;
        private ComboBox cbType;
        private Label label3;
        private ComboBox cbAgent;
        private Label label4;
        private TextBox txtLocalIPAddress;
        private Label label5;
        private NumericUpDown nudLocalPort;
        private Label label6;
        private Label label7;
        private TextBox txtRemoteHost;
        private NumericUpDown nudRemotePort;
    }
}