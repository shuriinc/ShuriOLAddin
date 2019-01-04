namespace ShuriOutlookAddIn
{
    partial class ResolveForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResolveForm));
            this.lblResolve = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.txtLast = new System.Windows.Forms.TextBox();
            this.txtFirst = new System.Windows.Forms.TextBox();
            this.lblLastname = new System.Windows.Forms.Label();
            this.lblFirstname = new System.Windows.Forms.Label();
            this.bAdd = new System.Windows.Forms.Button();
            this.lblDB = new System.Windows.Forms.Label();
            this.ddDatabase = new System.Windows.Forms.ComboBox();
            this.rbOrg = new System.Windows.Forms.RadioButton();
            this.rbPerson = new System.Windows.Forms.RadioButton();
            this.lbGuesses = new System.Windows.Forms.ListBox();
            this.lbAComplete = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRecipient = new System.Windows.Forms.TextBox();
            this.gbAddNew = new System.Windows.Forms.GroupBox();
            this.txtOrgname = new System.Windows.Forms.TextBox();
            this.lblOrgname = new System.Windows.Forms.Label();
            this.pbClose = new System.Windows.Forms.Button();
            this.gbChoose = new System.Windows.Forms.GroupBox();
            this.bChoose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.gbAddNew.SuspendLayout();
            this.gbChoose.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblResolve
            // 
            this.lblResolve.BackColor = System.Drawing.Color.Transparent;
            this.lblResolve.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResolve.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblResolve.Location = new System.Drawing.Point(12, 7);
            this.lblResolve.Name = "lblResolve";
            this.lblResolve.Size = new System.Drawing.Size(553, 30);
            this.lblResolve.TabIndex = 0;
            this.lblResolve.Text = "label1";
            this.lblResolve.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "imageNone");
            // 
            // txtLast
            // 
            this.txtLast.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLast.Location = new System.Drawing.Point(185, 59);
            this.txtLast.Name = "txtLast";
            this.txtLast.Size = new System.Drawing.Size(112, 20);
            this.txtLast.TabIndex = 13;
            this.txtLast.TextChanged += new System.EventHandler(this.SetUI_Event);
            // 
            // txtFirst
            // 
            this.txtFirst.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFirst.Location = new System.Drawing.Point(185, 36);
            this.txtFirst.Name = "txtFirst";
            this.txtFirst.Size = new System.Drawing.Size(112, 20);
            this.txtFirst.TabIndex = 12;
            this.txtFirst.TextChanged += new System.EventHandler(this.SetUI_Event);
            // 
            // lblLastname
            // 
            this.lblLastname.AutoSize = true;
            this.lblLastname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastname.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblLastname.Location = new System.Drawing.Point(122, 62);
            this.lblLastname.Name = "lblLastname";
            this.lblLastname.Size = new System.Drawing.Size(58, 13);
            this.lblLastname.TabIndex = 11;
            this.lblLastname.Text = "Last Name";
            // 
            // lblFirstname
            // 
            this.lblFirstname.AutoSize = true;
            this.lblFirstname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFirstname.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblFirstname.Location = new System.Drawing.Point(122, 41);
            this.lblFirstname.Name = "lblFirstname";
            this.lblFirstname.Size = new System.Drawing.Size(57, 13);
            this.lblFirstname.TabIndex = 10;
            this.lblFirstname.Text = "First Name";
            // 
            // bAdd
            // 
            this.bAdd.BackColor = System.Drawing.Color.Transparent;
            this.bAdd.Enabled = false;
            this.bAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bAdd.ForeColor = System.Drawing.SystemColors.ControlText;
            this.bAdd.Location = new System.Drawing.Point(66, 193);
            this.bAdd.Name = "bAdd";
            this.bAdd.Size = new System.Drawing.Size(176, 32);
            this.bAdd.TabIndex = 9;
            this.bAdd.Text = "Add";
            this.bAdd.UseVisualStyleBackColor = false;
            this.bAdd.Click += new System.EventHandler(this.bAdd_Click);
            // 
            // lblDB
            // 
            this.lblDB.AutoSize = true;
            this.lblDB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDB.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblDB.Location = new System.Drawing.Point(19, 153);
            this.lblDB.Name = "lblDB";
            this.lblDB.Size = new System.Drawing.Size(53, 13);
            this.lblDB.TabIndex = 8;
            this.lblDB.Text = "Database";
            this.lblDB.Visible = false;
            // 
            // ddDatabase
            // 
            this.ddDatabase.DisplayMember = "Name";
            this.ddDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddDatabase.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ddDatabase.FormattingEnabled = true;
            this.ddDatabase.Location = new System.Drawing.Point(75, 149);
            this.ddDatabase.Name = "ddDatabase";
            this.ddDatabase.Size = new System.Drawing.Size(222, 21);
            this.ddDatabase.TabIndex = 7;
            this.ddDatabase.ValueMember = "Id";
            this.ddDatabase.Visible = false;
            // 
            // rbOrg
            // 
            this.rbOrg.AutoSize = true;
            this.rbOrg.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbOrg.Location = new System.Drawing.Point(22, 100);
            this.rbOrg.Name = "rbOrg";
            this.rbOrg.Size = new System.Drawing.Size(84, 17);
            this.rbOrg.TabIndex = 1;
            this.rbOrg.Text = "Organization";
            this.rbOrg.UseVisualStyleBackColor = true;
            this.rbOrg.CheckedChanged += new System.EventHandler(this.SetUI_Event);
            // 
            // rbPerson
            // 
            this.rbPerson.AutoSize = true;
            this.rbPerson.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbPerson.Location = new System.Drawing.Point(22, 37);
            this.rbPerson.Name = "rbPerson";
            this.rbPerson.Size = new System.Drawing.Size(58, 17);
            this.rbPerson.TabIndex = 0;
            this.rbPerson.Text = "Person";
            this.rbPerson.UseVisualStyleBackColor = true;
            this.rbPerson.CheckedChanged += new System.EventHandler(this.SetUI_Event);
            // 
            // lbGuesses
            // 
            this.lbGuesses.BackColor = System.Drawing.Color.White;
            this.lbGuesses.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbGuesses.FormattingEnabled = true;
            this.lbGuesses.ItemHeight = 16;
            this.lbGuesses.Location = new System.Drawing.Point(21, 35);
            this.lbGuesses.Name = "lbGuesses";
            this.lbGuesses.Size = new System.Drawing.Size(218, 132);
            this.lbGuesses.TabIndex = 12;
            this.lbGuesses.Click += new System.EventHandler(this.SetUI_Event);
            this.lbGuesses.DoubleClick += new System.EventHandler(this.chooseClick);
            // 
            // lbAComplete
            // 
            this.lbAComplete.FormattingEnabled = true;
            this.lbAComplete.Location = new System.Drawing.Point(207, 74);
            this.lbAComplete.Name = "lbAComplete";
            this.lbAComplete.Size = new System.Drawing.Size(208, 147);
            this.lbAComplete.TabIndex = 15;
            this.lbAComplete.Visible = false;
            this.lbAComplete.Click += new System.EventHandler(this.chooseClick);
            this.lbAComplete.DoubleClick += new System.EventHandler(this.chooseClick);
            this.lbAComplete.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox1_KeyDown);
            this.lbAComplete.Leave += new System.EventHandler(this.lbAComplete_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(142, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 18);
            this.label1.TabIndex = 14;
            this.label1.Text = "Lookup";
            // 
            // txtRecipient
            // 
            this.txtRecipient.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtRecipient.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtRecipient.BackColor = System.Drawing.SystemColors.Window;
            this.txtRecipient.Location = new System.Drawing.Point(205, 12);
            this.txtRecipient.MaxLength = 50;
            this.txtRecipient.Name = "txtRecipient";
            this.txtRecipient.Size = new System.Drawing.Size(239, 20);
            this.txtRecipient.TabIndex = 1;
            this.txtRecipient.Click += new System.EventHandler(this.txtRecipient_Click);
            this.txtRecipient.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtRecipient_KeyUp);
            // 
            // gbAddNew
            // 
            this.gbAddNew.Controls.Add(this.txtOrgname);
            this.gbAddNew.Controls.Add(this.lblOrgname);
            this.gbAddNew.Controls.Add(this.bAdd);
            this.gbAddNew.Controls.Add(this.txtLast);
            this.gbAddNew.Controls.Add(this.lblDB);
            this.gbAddNew.Controls.Add(this.txtFirst);
            this.gbAddNew.Controls.Add(this.ddDatabase);
            this.gbAddNew.Controls.Add(this.rbPerson);
            this.gbAddNew.Controls.Add(this.lblFirstname);
            this.gbAddNew.Controls.Add(this.lblLastname);
            this.gbAddNew.Controls.Add(this.rbOrg);
            this.gbAddNew.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbAddNew.ForeColor = System.Drawing.Color.DimGray;
            this.gbAddNew.Location = new System.Drawing.Point(272, 102);
            this.gbAddNew.Name = "gbAddNew";
            this.gbAddNew.Size = new System.Drawing.Size(312, 241);
            this.gbAddNew.TabIndex = 17;
            this.gbAddNew.TabStop = false;
            this.gbAddNew.Text = "Add New ";
            // 
            // txtOrgname
            // 
            this.txtOrgname.Enabled = false;
            this.txtOrgname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOrgname.Location = new System.Drawing.Point(185, 99);
            this.txtOrgname.Name = "txtOrgname";
            this.txtOrgname.Size = new System.Drawing.Size(112, 20);
            this.txtOrgname.TabIndex = 15;
            this.txtOrgname.TextChanged += new System.EventHandler(this.SetUI_Event);
            // 
            // lblOrgname
            // 
            this.lblOrgname.AutoSize = true;
            this.lblOrgname.Enabled = false;
            this.lblOrgname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOrgname.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblOrgname.Location = new System.Drawing.Point(122, 102);
            this.lblOrgname.Name = "lblOrgname";
            this.lblOrgname.Size = new System.Drawing.Size(35, 13);
            this.lblOrgname.TabIndex = 14;
            this.lblOrgname.Text = "Name";
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
            this.pbClose.Location = new System.Drawing.Point(559, 7);
            this.pbClose.Name = "pbClose";
            this.pbClose.Size = new System.Drawing.Size(32, 32);
            this.pbClose.TabIndex = 18;
            this.toolTip1.SetToolTip(this.pbClose, "Close this form");
            this.pbClose.UseVisualStyleBackColor = false;
            this.pbClose.Click += new System.EventHandler(this.pbClose_Click);
            // 
            // gbChoose
            // 
            this.gbChoose.Controls.Add(this.bChoose);
            this.gbChoose.Controls.Add(this.lbGuesses);
            this.gbChoose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbChoose.ForeColor = System.Drawing.Color.DimGray;
            this.gbChoose.Location = new System.Drawing.Point(8, 102);
            this.gbChoose.Name = "gbChoose";
            this.gbChoose.Size = new System.Drawing.Size(257, 241);
            this.gbChoose.TabIndex = 19;
            this.gbChoose.TabStop = false;
            this.gbChoose.Text = "Suggestions";
            // 
            // bChoose
            // 
            this.bChoose.BackColor = System.Drawing.Color.Transparent;
            this.bChoose.Enabled = false;
            this.bChoose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bChoose.ForeColor = System.Drawing.SystemColors.ControlText;
            this.bChoose.Location = new System.Drawing.Point(21, 193);
            this.bChoose.Name = "bChoose";
            this.bChoose.Size = new System.Drawing.Size(218, 32);
            this.bChoose.TabIndex = 13;
            this.bChoose.Text = "Choose";
            this.bChoose.UseVisualStyleBackColor = false;
            this.bChoose.Click += new System.EventHandler(this.bChoose_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.SlateGray;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtRecipient);
            this.panel1.Location = new System.Drawing.Point(1, 42);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(594, 42);
            this.panel1.TabIndex = 20;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.Controls.Add(this.pbClose);
            this.panel2.Controls.Add(this.lblResolve);
            this.panel2.Location = new System.Drawing.Point(1, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(594, 42);
            this.panel2.TabIndex = 21;
            // 
            // ResolveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(593, 358);
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.gbChoose);
            this.Controls.Add(this.gbAddNew);
            this.Controls.Add(this.lbAComplete);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ResolveForm";
            this.RightToLeftLayout = true;
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Resolve Email Address ";
            this.Load += new System.EventHandler(this.ResolveForm_Load);
            this.gbAddNew.ResumeLayout(false);
            this.gbAddNew.PerformLayout();
            this.gbChoose.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblResolve;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.RadioButton rbOrg;
        private System.Windows.Forms.RadioButton rbPerson;
        private System.Windows.Forms.TextBox txtLast;
        private System.Windows.Forms.TextBox txtFirst;
        private System.Windows.Forms.Label lblLastname;
        private System.Windows.Forms.Label lblFirstname;
        private System.Windows.Forms.Button bAdd;
        private System.Windows.Forms.Label lblDB;
        private System.Windows.Forms.ComboBox ddDatabase;
        private System.Windows.Forms.ListBox lbGuesses;
        private System.Windows.Forms.ListBox lbAComplete;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRecipient;
        private System.Windows.Forms.GroupBox gbAddNew;
        private System.Windows.Forms.Button pbClose;
        private System.Windows.Forms.GroupBox gbChoose;
        private System.Windows.Forms.Button bChoose;
        private System.Windows.Forms.TextBox txtOrgname;
        private System.Windows.Forms.Label lblOrgname;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}