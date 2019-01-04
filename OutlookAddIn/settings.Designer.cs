namespace ShuriOutlookAddIn
{
    partial class SettingsForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.lblUser = new System.Windows.Forms.Label();
            this.panPrefs = new System.Windows.Forms.Panel();
            this.llProxy = new System.Windows.Forms.LinkLabel();
            this.cbUseProxy = new System.Windows.Forms.CheckBox();
            this.cbAddmetouch = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbCCConfirm = new System.Windows.Forms.CheckBox();
            this.lblEnviron = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.bLogout = new System.Windows.Forms.Button();
            this.panVersion = new System.Windows.Forms.Panel();
            this.lblCopy = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.panOK = new System.Windows.Forms.Panel();
            this.pbClose = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.panUser = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panWait = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panOffline = new System.Windows.Forms.Panel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.bLogin = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.panPrefs.SuspendLayout();
            this.panVersion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panOK.SuspendLayout();
            this.panUser.SuspendLayout();
            this.panWait.SuspendLayout();
            this.panOffline.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(20, 33);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(39, 13);
            this.lblUser.TabIndex = 5;
            this.lblUser.Text = "lblUser";
            // 
            // panPrefs
            // 
            this.panPrefs.BackColor = System.Drawing.Color.AliceBlue;
            this.panPrefs.Controls.Add(this.llProxy);
            this.panPrefs.Controls.Add(this.cbUseProxy);
            this.panPrefs.Controls.Add(this.cbAddmetouch);
            this.panPrefs.Controls.Add(this.label6);
            this.panPrefs.Controls.Add(this.cbCCConfirm);
            this.panPrefs.Location = new System.Drawing.Point(0, 141);
            this.panPrefs.Name = "panPrefs";
            this.panPrefs.Size = new System.Drawing.Size(370, 115);
            this.panPrefs.TabIndex = 6;
            this.panPrefs.Visible = false;
            // 
            // llProxy
            // 
            this.llProxy.AutoSize = true;
            this.llProxy.LinkColor = System.Drawing.Color.SteelBlue;
            this.llProxy.Location = new System.Drawing.Point(151, 82);
            this.llProxy.Name = "llProxy";
            this.llProxy.Size = new System.Drawing.Size(103, 13);
            this.llProxy.TabIndex = 9;
            this.llProxy.TabStop = true;
            this.llProxy.Text = "proxy server settings";
            this.llProxy.VisitedLinkColor = System.Drawing.Color.SteelBlue;
            this.llProxy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llProxy_LinkClicked);
            // 
            // cbUseProxy
            // 
            this.cbUseProxy.AutoSize = true;
            this.cbUseProxy.Location = new System.Drawing.Point(28, 81);
            this.cbUseProxy.Name = "cbUseProxy";
            this.cbUseProxy.Size = new System.Drawing.Size(114, 17);
            this.cbUseProxy.TabIndex = 8;
            this.cbUseProxy.Text = "Use a proxy server";
            this.cbUseProxy.UseVisualStyleBackColor = true;
            this.cbUseProxy.CheckedChanged += new System.EventHandler(this.cbUseProxy_CheckedChanged);
            // 
            // cbAddmetouch
            // 
            this.cbAddmetouch.AutoSize = true;
            this.cbAddmetouch.Location = new System.Drawing.Point(28, 32);
            this.cbAddmetouch.Name = "cbAddmetouch";
            this.cbAddmetouch.Size = new System.Drawing.Size(179, 17);
            this.cbAddmetouch.TabIndex = 5;
            this.cbAddmetouch.Text = "Add me to touches automatically";
            this.cbAddmetouch.UseVisualStyleBackColor = true;
            this.cbAddmetouch.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.DimGray;
            this.label6.Location = new System.Drawing.Point(13, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 14);
            this.label6.TabIndex = 7;
            this.label6.Text = "Preferences";
            // 
            // cbCCConfirm
            // 
            this.cbCCConfirm.AutoSize = true;
            this.cbCCConfirm.Location = new System.Drawing.Point(28, 56);
            this.cbCCConfirm.Name = "cbCCConfirm";
            this.cbCCConfirm.Size = new System.Drawing.Size(191, 17);
            this.cbCCConfirm.TabIndex = 6;
            this.cbCCConfirm.Text = "Send cc@shuri confirmation emails";
            this.cbCCConfirm.UseVisualStyleBackColor = true;
            this.cbCCConfirm.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
            // 
            // lblEnviron
            // 
            this.lblEnviron.BackColor = System.Drawing.Color.Transparent;
            this.lblEnviron.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEnviron.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblEnviron.Location = new System.Drawing.Point(62, 54);
            this.lblEnviron.Name = "lblEnviron";
            this.lblEnviron.Size = new System.Drawing.Size(187, 35);
            this.lblEnviron.TabIndex = 8;
            this.lblEnviron.Text = "label4";
            this.lblEnviron.Click += new System.EventHandler(this.lblEnviron_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DimGray;
            this.label1.Location = new System.Drawing.Point(12, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 14);
            this.label1.TabIndex = 7;
            this.label1.Text = "Logged in as";
            // 
            // bLogout
            // 
            this.bLogout.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.bLogout.Image = global::ShuriOutlookAddIn.Properties.Resources.icon24alt;
            this.bLogout.Location = new System.Drawing.Point(285, 23);
            this.bLogout.Name = "bLogout";
            this.bLogout.Size = new System.Drawing.Size(71, 51);
            this.bLogout.TabIndex = 4;
            this.bLogout.TabStop = false;
            this.bLogout.Text = "Logout";
            this.bLogout.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bLogout.UseVisualStyleBackColor = false;
            this.bLogout.Click += new System.EventHandler(this.bLogout_Click);
            // 
            // panVersion
            // 
            this.panVersion.BackColor = System.Drawing.Color.LightGray;
            this.panVersion.Controls.Add(this.lblCopy);
            this.panVersion.Controls.Add(this.pictureBox1);
            this.panVersion.Controls.Add(this.lblVersion);
            this.panVersion.Location = new System.Drawing.Point(0, 253);
            this.panVersion.Name = "panVersion";
            this.panVersion.Size = new System.Drawing.Size(370, 66);
            this.panVersion.TabIndex = 7;
            this.panVersion.Visible = false;
            // 
            // lblCopy
            // 
            this.lblCopy.AutoSize = true;
            this.lblCopy.BackColor = System.Drawing.Color.Transparent;
            this.lblCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCopy.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblCopy.Location = new System.Drawing.Point(79, 43);
            this.lblCopy.Name = "lblCopy";
            this.lblCopy.Size = new System.Drawing.Size(36, 12);
            this.lblCopy.TabIndex = 2;
            this.lblCopy.Text = "lblCopy";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ShuriOutlookAddIn.Properties.Resources.icon48;
            this.pictureBox1.Location = new System.Drawing.Point(9, 9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(57, 50);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.Location = new System.Drawing.Point(78, 17);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(63, 15);
            this.lblVersion.TabIndex = 1;
            this.lblVersion.Text = "lblVersion";
            // 
            // panOK
            // 
            this.panOK.BackColor = System.Drawing.Color.Black;
            this.panOK.Controls.Add(this.pbClose);
            this.panOK.Controls.Add(this.label4);
            this.panOK.Location = new System.Drawing.Point(0, 0);
            this.panOK.Name = "panOK";
            this.panOK.Size = new System.Drawing.Size(370, 48);
            this.panOK.TabIndex = 10;
            this.panOK.Click += new System.EventHandler(this.closeForm);
            // 
            // pbClose
            // 
            this.pbClose.AutoSize = true;
            this.pbClose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pbClose.BackColor = System.Drawing.Color.Transparent;
            this.pbClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbClose.FlatAppearance.BorderSize = 0;
            this.pbClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pbClose.Image = global::ShuriOutlookAddIn.Properties.Resources.close_26;
            this.pbClose.Location = new System.Drawing.Point(331, 8);
            this.pbClose.Name = "pbClose";
            this.pbClose.Size = new System.Drawing.Size(32, 32);
            this.pbClose.TabIndex = 1;
            this.toolTip1.SetToolTip(this.pbClose, "Close this form");
            this.pbClose.UseVisualStyleBackColor = false;
            this.pbClose.Click += new System.EventHandler(this.closeForm);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(8, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 19);
            this.label4.TabIndex = 2;
            this.label4.Text = "Settings";
            // 
            // panUser
            // 
            this.panUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(240)))), ((int)(((byte)(235)))));
            this.panUser.Controls.Add(this.button1);
            this.panUser.Controls.Add(this.lblEnviron);
            this.panUser.Controls.Add(this.bLogout);
            this.panUser.Controls.Add(this.label1);
            this.panUser.Controls.Add(this.lblUser);
            this.panUser.Location = new System.Drawing.Point(0, 48);
            this.panUser.Name = "panUser";
            this.panUser.Size = new System.Drawing.Size(370, 92);
            this.panUser.TabIndex = 9;
            this.panUser.Visible = false;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(240)))), ((int)(((byte)(235)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Image = global::ShuriOutlookAddIn.Properties.Resources.info30;
            this.button1.Location = new System.Drawing.Point(17, 46);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(39, 37);
            this.button1.TabIndex = 9;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.lblEnviron_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // panWait
            // 
            this.panWait.BackColor = System.Drawing.Color.Ivory;
            this.panWait.Controls.Add(this.label2);
            this.panWait.Location = new System.Drawing.Point(372, 48);
            this.panWait.Name = "panWait";
            this.panWait.Size = new System.Drawing.Size(370, 271);
            this.panWait.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(146, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 14);
            this.label2.TabIndex = 8;
            this.label2.Text = "Working...";
            // 
            // panOffline
            // 
            this.panOffline.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(240)))), ((int)(((byte)(235)))));
            this.panOffline.Controls.Add(this.linkLabel1);
            this.panOffline.Controls.Add(this.checkBox1);
            this.panOffline.Controls.Add(this.bLogin);
            this.panOffline.Controls.Add(this.label3);
            this.panOffline.Location = new System.Drawing.Point(797, 48);
            this.panOffline.Name = "panOffline";
            this.panOffline.Size = new System.Drawing.Size(370, 208);
            this.panOffline.TabIndex = 11;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.LinkColor = System.Drawing.Color.SteelBlue;
            this.linkLabel1.Location = new System.Drawing.Point(202, 178);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(103, 13);
            this.linkLabel1.TabIndex = 11;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "proxy server settings";
            this.linkLabel1.VisitedLinkColor = System.Drawing.Color.SteelBlue;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llProxy_LinkClicked);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(79, 177);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(114, 17);
            this.checkBox1.TabIndex = 10;
            this.checkBox1.Text = "Use a proxy server";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Click += new System.EventHandler(this.cbUseProxy_CheckedChanged);
            // 
            // bLogin
            // 
            this.bLogin.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.bLogin.Image = global::ShuriOutlookAddIn.Properties.Resources.icon24;
            this.bLogin.Location = new System.Drawing.Point(150, 79);
            this.bLogin.Name = "bLogin";
            this.bLogin.Size = new System.Drawing.Size(71, 51);
            this.bLogin.TabIndex = 9;
            this.bLogin.TabStop = false;
            this.bLogin.Text = "Login";
            this.bLogin.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bLogin.UseVisualStyleBackColor = false;
            this.bLogin.Click += new System.EventHandler(this.bLogin_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.DimGray;
            this.label3.Location = new System.Drawing.Point(31, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(183, 14);
            this.label3.TabIndex = 8;
            this.label3.Text = "You are off-line or not logged in.";
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.pbClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(369, 320);
            this.ControlBox = false;
            this.Controls.Add(this.panOffline);
            this.Controls.Add(this.panUser);
            this.Controls.Add(this.panOK);
            this.Controls.Add(this.panPrefs);
            this.Controls.Add(this.panVersion);
            this.Controls.Add(this.panWait);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Shuri Outlook Addin";
            this.Load += new System.EventHandler(this.settings_Load);
            this.panPrefs.ResumeLayout(false);
            this.panPrefs.PerformLayout();
            this.panVersion.ResumeLayout(false);
            this.panVersion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panOK.ResumeLayout(false);
            this.panOK.PerformLayout();
            this.panUser.ResumeLayout(false);
            this.panUser.PerformLayout();
            this.panWait.ResumeLayout(false);
            this.panWait.PerformLayout();
            this.panOffline.ResumeLayout(false);
            this.panOffline.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Panel panPrefs;
        private System.Windows.Forms.Button bLogout;
        private System.Windows.Forms.Panel panVersion;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblCopy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblEnviron;
        private System.Windows.Forms.Panel panOK;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button pbClose;
        private System.Windows.Forms.Panel panUser;
        private System.Windows.Forms.CheckBox cbCCConfirm;
        private System.Windows.Forms.CheckBox cbAddmetouch;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panWait;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel llProxy;
        private System.Windows.Forms.CheckBox cbUseProxy;
        private System.Windows.Forms.Panel panOffline;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button bLogin;
    }
}