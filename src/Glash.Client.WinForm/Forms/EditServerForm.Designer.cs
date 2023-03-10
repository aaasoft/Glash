namespace Glash.Client.WinForm
{
    partial class EditServerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditServerForm));
            label1 = new Label();
            txtName = new TextBox();
            label2 = new Label();
            txtUrl = new TextBox();
            label3 = new Label();
            txtPassword = new TextBox();
            btnSave = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(89, 31);
            label1.TabIndex = 0;
            label1.Text = "Name:";
            // 
            // txtName
            // 
            txtName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtName.Location = new Point(12, 49);
            txtName.Name = "txtName";
            txtName.Size = new Size(557, 38);
            txtName.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 90);
            label2.Name = "label2";
            label2.Size = new Size(53, 31);
            label2.TabIndex = 0;
            label2.Text = "Url:";
            // 
            // txtUrl
            // 
            txtUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUrl.Location = new Point(12, 124);
            txtUrl.Name = "txtUrl";
            txtUrl.Size = new Size(557, 38);
            txtUrl.TabIndex = 2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 165);
            label3.Name = "label3";
            label3.Size = new Size(128, 31);
            label3.TabIndex = 0;
            label3.Text = "Password:";
            // 
            // txtPassword
            // 
            txtPassword.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPassword.Location = new Point(12, 199);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '●';
            txtPassword.Size = new Size(557, 38);
            txtPassword.TabIndex = 3;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSave.Location = new Point(12, 252);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(150, 46);
            btnSave.TabIndex = 100;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnCancel.Location = new Point(168, 252);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(150, 46);
            btnCancel.TabIndex = 101;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // EditServerForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(581, 310);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(txtPassword);
            Controls.Add(txtUrl);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(txtName);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "EditServerForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Add or edit server";
            Load += EditServerForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtName;
        private Label label2;
        private TextBox txtUrl;
        private Label label3;
        private TextBox txtPassword;
        private Button btnSave;
        private Button btnCancel;
    }
}