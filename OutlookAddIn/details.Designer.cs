namespace ShuriOutlookAddIn
{
    partial class details
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(details));
            this.cMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.bRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.bView = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lvUnknown = new System.Windows.Forms.ListView();
            this.txtTag = new System.Windows.Forms.TextBox();
            this.txtRecipient = new System.Windows.Forms.TextBox();
            this.ddDatabase = new System.Windows.Forms.ComboBox();
            this.ddType = new System.Windows.Forms.ComboBox();
            this.lblName = new System.Windows.Forms.Label();
            this.tvTags = new System.Windows.Forms.TreeView();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.ddOwnedByGroup = new System.Windows.Forms.ComboBox();
            this.lblSharing = new System.Windows.Forms.Label();
            this.lblUnknown = new System.Windows.Forms.Label();
            this.panUnknown = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panTags = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lbACompleteTag = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panRecip = new System.Windows.Forms.Panel();
            this.lblRec = new System.Windows.Forms.Label();
            this.lblAddRec = new System.Windows.Forms.Label();
            this.lbAComplete = new System.Windows.Forms.ListBox();
            this.panRecipients = new System.Windows.Forms.Panel();
            this.lblType = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblLoc = new System.Windows.Forms.Label();
            this.lblLocation = new System.Windows.Forms.Label();
            this.panLoc = new System.Windows.Forms.Panel();
            this.pbMap = new System.Windows.Forms.Button();
            this.bSave = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.bOK = new System.Windows.Forms.Button();
            this.tvRecips = new System.Windows.Forms.TreeView();
            this.cMenu.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panUnknown.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panRecip.SuspendLayout();
            this.panRecipients.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panLoc.SuspendLayout();
            this.SuspendLayout();
            // 
            // cMenu
            // 
            this.cMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bRemove,
            this.bView});
            this.cMenu.Name = "cMenuTags";
            this.cMenu.Size = new System.Drawing.Size(138, 48);
            this.cMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cMenuClick);
            // 
            // bRemove
            // 
            this.bRemove.Image = ((System.Drawing.Image)(resources.GetObject("bRemove.Image")));
            this.bRemove.Name = "bRemove";
            this.bRemove.Size = new System.Drawing.Size(137, 22);
            this.bRemove.Text = "Remove";
            // 
            // bView
            // 
            this.bView.Image = ((System.Drawing.Image)(resources.GetObject("bView.Image")));
            this.bView.Name = "bView";
            this.bView.Size = new System.Drawing.Size(137, 22);
            this.bView.Text = "View in App";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "search16.png");
            this.imageList1.Images.SetKeyName(1, "minus.png");
            // 
            // lvUnknown
            // 
            this.lvUnknown.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lvUnknown.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvUnknown.LargeImageList = this.imageList1;
            this.lvUnknown.Location = new System.Drawing.Point(25, 4);
            this.lvUnknown.Margin = new System.Windows.Forms.Padding(12);
            this.lvUnknown.MultiSelect = false;
            this.lvUnknown.Name = "lvUnknown";
            this.lvUnknown.Size = new System.Drawing.Size(201, 132);
            this.lvUnknown.SmallImageList = this.imageList1;
            this.lvUnknown.TabIndex = 5;
            this.lvUnknown.TileSize = new System.Drawing.Size(200, 24);
            this.toolTip1.SetToolTip(this.lvUnknown, "Click name to resolve");
            this.lvUnknown.UseCompatibleStateImageBehavior = false;
            this.lvUnknown.View = System.Windows.Forms.View.List;
            this.lvUnknown.Click += new System.EventHandler(this.lvUnknown_Click);
            // 
            // txtTag
            // 
            this.txtTag.BackColor = System.Drawing.SystemColors.Menu;
            this.txtTag.Location = new System.Drawing.Point(126, 14);
            this.txtTag.MaxLength = 50;
            this.txtTag.Name = "txtTag";
            this.txtTag.Size = new System.Drawing.Size(159, 20);
            this.txtTag.TabIndex = 2;
            this.toolTip1.SetToolTip(this.txtTag, "Begin typing the tag name to search for in Shuri app");
            this.txtTag.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtTag_KeyUp);
            this.txtTag.Leave += new System.EventHandler(this.txtTag_Leave);
            // 
            // txtRecipient
            // 
            this.txtRecipient.BackColor = System.Drawing.SystemColors.Menu;
            this.txtRecipient.Location = new System.Drawing.Point(36, 23);
            this.txtRecipient.MaxLength = 50;
            this.txtRecipient.Name = "txtRecipient";
            this.txtRecipient.Size = new System.Drawing.Size(147, 20);
            this.txtRecipient.TabIndex = 1;
            this.toolTip1.SetToolTip(this.txtRecipient, "Begin typing the name or email address you\'d like to search for in Shuri app");
            this.txtRecipient.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtRecipient_KeyUp);
            this.txtRecipient.Leave += new System.EventHandler(this.txtRecipient_Leave);
            // 
            // ddDatabase
            // 
            this.ddDatabase.DisplayMember = "Name";
            this.ddDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddDatabase.FormattingEnabled = true;
            this.ddDatabase.Location = new System.Drawing.Point(165, 9);
            this.ddDatabase.Name = "ddDatabase";
            this.ddDatabase.Size = new System.Drawing.Size(296, 21);
            this.ddDatabase.TabIndex = 4;
            this.toolTip1.SetToolTip(this.ddDatabase, "Everything in Shuri must reside in 1 database");
            this.ddDatabase.ValueMember = "Id";
            this.ddDatabase.SelectedIndexChanged += new System.EventHandler(this.ddDatabase_SelectedIndexChanged);
            // 
            // ddType
            // 
            this.ddType.DisplayMember = "Name";
            this.ddType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddType.FormattingEnabled = true;
            this.ddType.Location = new System.Drawing.Point(195, 13);
            this.ddType.Name = "ddType";
            this.ddType.Size = new System.Drawing.Size(266, 21);
            this.ddType.TabIndex = 0;
            this.toolTip1.SetToolTip(this.ddType, "Touch Type");
            this.ddType.ValueMember = "Id";
            this.ddType.SelectedIndexChanged += new System.EventHandler(this.ddType_SelectedIndexChanged);
            // 
            // lblName
            // 
            this.lblName.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblName.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblName.Location = new System.Drawing.Point(3, 1);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(467, 40);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "lblName";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.lblName, "Touch Name");
            // 
            // tvTags
            // 
            this.tvTags.BackColor = System.Drawing.Color.Snow;
            this.tvTags.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvTags.ContextMenuStrip = this.cMenu;
            this.tvTags.Location = new System.Drawing.Point(22, 51);
            this.tvTags.Margin = new System.Windows.Forms.Padding(0);
            this.tvTags.Name = "tvTags";
            this.tvTags.ShowLines = false;
            this.tvTags.ShowPlusMinus = false;
            this.tvTags.ShowRootLines = false;
            this.tvTags.Size = new System.Drawing.Size(444, 130);
            this.tvTags.TabIndex = 18;
            this.toolTip1.SetToolTip(this.tvTags, "Double-click to remove\nRight-click for more");
            this.tvTags.DoubleClick += new System.EventHandler(this.tv_DoubleClick);
            this.tvTags.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tv_MouseUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label2.Location = new System.Drawing.Point(88, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Editing Team";
            this.toolTip1.SetToolTip(this.label2, "Choose if you want help updating it");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(103, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Database";
            this.toolTip1.SetToolTip(this.label1, "Choose where to store this touch");
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.DarkGray;
            this.panel6.Controls.Add(this.panel3);
            this.panel6.Controls.Add(this.lblSharing);
            this.panel6.Location = new System.Drawing.Point(2, 546);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(470, 118);
            this.panel6.TabIndex = 7;
            this.toolTip1.SetToolTip(this.panel6, "Choose where to store this touch and if you want help updating it");
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel3.Controls.Add(this.ddOwnedByGroup);
            this.panel3.Controls.Add(this.ddDatabase);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Location = new System.Drawing.Point(0, 44);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(470, 78);
            this.panel3.TabIndex = 9;
            // 
            // ddOwnedByGroup
            // 
            this.ddOwnedByGroup.DisplayMember = "Name";
            this.ddOwnedByGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddOwnedByGroup.FormattingEnabled = true;
            this.ddOwnedByGroup.Location = new System.Drawing.Point(165, 39);
            this.ddOwnedByGroup.Name = "ddOwnedByGroup";
            this.ddOwnedByGroup.Size = new System.Drawing.Size(296, 21);
            this.ddOwnedByGroup.TabIndex = 5;
            this.ddOwnedByGroup.ValueMember = "Id";
            this.ddOwnedByGroup.SelectedIndexChanged += new System.EventHandler(this.ddOwnedByGroup_SelectedIndexChanged);
            // 
            // lblSharing
            // 
            this.lblSharing.AutoSize = true;
            this.lblSharing.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSharing.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblSharing.Location = new System.Drawing.Point(9, 13);
            this.lblSharing.Name = "lblSharing";
            this.lblSharing.Size = new System.Drawing.Size(58, 16);
            this.lblSharing.TabIndex = 0;
            this.lblSharing.Text = "Sharing";
            // 
            // lblUnknown
            // 
            this.lblUnknown.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUnknown.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblUnknown.Location = new System.Drawing.Point(58, 5);
            this.lblUnknown.Name = "lblUnknown";
            this.lblUnknown.Size = new System.Drawing.Size(118, 35);
            this.lblUnknown.TabIndex = 0;
            this.lblUnknown.Text = "Unknown Recipients";
            this.lblUnknown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panUnknown
            // 
            this.panUnknown.BackColor = System.Drawing.Color.LightSlateGray;
            this.panUnknown.Controls.Add(this.panel5);
            this.panUnknown.Controls.Add(this.lblUnknown);
            this.panUnknown.Location = new System.Drawing.Point(237, 132);
            this.panUnknown.Name = "panUnknown";
            this.panUnknown.Size = new System.Drawing.Size(235, 186);
            this.panUnknown.TabIndex = 10;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel5.Controls.Add(this.lvUnknown);
            this.panel5.Location = new System.Drawing.Point(0, 44);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(235, 141);
            this.panel5.TabIndex = 1;
            // 
            // panTags
            // 
            this.panTags.BackColor = System.Drawing.Color.Snow;
            this.panTags.Location = new System.Drawing.Point(0, 44);
            this.panTags.Name = "panTags";
            this.panTags.Size = new System.Drawing.Size(470, 141);
            this.panTags.TabIndex = 19;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(9, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "Tags";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Thistle;
            this.label5.Location = new System.Drawing.Point(98, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Add";
            // 
            // lbACompleteTag
            // 
            this.lbACompleteTag.FormattingEnabled = true;
            this.lbACompleteTag.Location = new System.Drawing.Point(126, 33);
            this.lbACompleteTag.Name = "lbACompleteTag";
            this.lbACompleteTag.Size = new System.Drawing.Size(213, 147);
            this.lbACompleteTag.TabIndex = 17;
            this.lbACompleteTag.Visible = false;
            this.lbACompleteTag.Click += new System.EventHandler(this.lbACompleteTag_Click);
            this.lbACompleteTag.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbACompleteTag_KeyDown);
            this.lbACompleteTag.Leave += new System.EventHandler(this.lbACompleteTag_Leave);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(55)))), ((int)(((byte)(100)))));
            this.panel1.Controls.Add(this.lbACompleteTag);
            this.panel1.Controls.Add(this.tvTags);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.txtTag);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.panTags);
            this.panel1.Location = new System.Drawing.Point(2, 317);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(470, 186);
            this.panel1.TabIndex = 2;
            // 
            // panRecip
            // 
            this.panRecip.AutoScroll = true;
            this.panRecip.BackColor = System.Drawing.Color.OldLace;
            this.panRecip.Controls.Add(this.tvRecips);
            this.panRecip.Location = new System.Drawing.Point(0, 44);
            this.panRecip.Name = "panRecip";
            this.panRecip.Padding = new System.Windows.Forms.Padding(24, 8, 0, 3);
            this.panRecip.Size = new System.Drawing.Size(235, 141);
            this.panRecip.TabIndex = 21;
            // 
            // lblRec
            // 
            this.lblRec.AutoSize = true;
            this.lblRec.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRec.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblRec.Location = new System.Drawing.Point(65, 6);
            this.lblRec.Name = "lblRec";
            this.lblRec.Size = new System.Drawing.Size(74, 16);
            this.lblRec.TabIndex = 0;
            this.lblRec.Text = "Recipients";
            // 
            // lblAddRec
            // 
            this.lblAddRec.AutoSize = true;
            this.lblAddRec.ForeColor = System.Drawing.Color.Khaki;
            this.lblAddRec.Location = new System.Drawing.Point(6, 26);
            this.lblAddRec.Name = "lblAddRec";
            this.lblAddRec.Size = new System.Drawing.Size(26, 13);
            this.lblAddRec.TabIndex = 4;
            this.lblAddRec.Text = "Add";
            // 
            // lbAComplete
            // 
            this.lbAComplete.FormattingEnabled = true;
            this.lbAComplete.Location = new System.Drawing.Point(37, 42);
            this.lbAComplete.Name = "lbAComplete";
            this.lbAComplete.Size = new System.Drawing.Size(203, 134);
            this.lbAComplete.TabIndex = 16;
            this.lbAComplete.Visible = false;
            this.lbAComplete.Click += new System.EventHandler(this.lbAComplete_Click);
            this.lbAComplete.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbAComplete_KeyDown);
            this.lbAComplete.Leave += new System.EventHandler(this.lbAComplete_Leave);
            // 
            // panRecipients
            // 
            this.panRecipients.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(163)))), ((int)(((byte)(119)))));
            this.panRecipients.Controls.Add(this.lbAComplete);
            this.panRecipients.Controls.Add(this.lblAddRec);
            this.panRecipients.Controls.Add(this.txtRecipient);
            this.panRecipients.Controls.Add(this.lblRec);
            this.panRecipients.Controls.Add(this.panRecip);
            this.panRecipients.Location = new System.Drawing.Point(2, 132);
            this.panRecipients.Name = "panRecipients";
            this.panRecipients.Size = new System.Drawing.Size(235, 186);
            this.panRecipients.TabIndex = 6;
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblType.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblType.Location = new System.Drawing.Point(10, 14);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(80, 16);
            this.lblType.TabIndex = 1;
            this.lblType.Text = "Touch Type";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(90)))), ((int)(((byte)(49)))));
            this.panel2.Controls.Add(this.ddType);
            this.panel2.Controls.Add(this.lblType);
            this.panel2.Location = new System.Drawing.Point(2, 88);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(470, 44);
            this.panel2.TabIndex = 8;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(141)))), ((int)(((byte)(90)))));
            this.panel4.Controls.Add(this.lblName);
            this.panel4.Location = new System.Drawing.Point(2, 44);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(470, 44);
            this.panel4.TabIndex = 11;
            // 
            // lblLoc
            // 
            this.lblLoc.AutoSize = true;
            this.lblLoc.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoc.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblLoc.Location = new System.Drawing.Point(9, 13);
            this.lblLoc.Name = "lblLoc";
            this.lblLoc.Size = new System.Drawing.Size(63, 16);
            this.lblLoc.TabIndex = 1;
            this.lblLoc.Text = "Location";
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblLocation.Location = new System.Drawing.Point(94, 15);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(58, 13);
            this.lblLocation.TabIndex = 7;
            this.lblLocation.Text = "lblLocation";
            // 
            // panLoc
            // 
            this.panLoc.BackColor = System.Drawing.Color.SteelBlue;
            this.panLoc.Controls.Add(this.pbMap);
            this.panLoc.Controls.Add(this.lblLocation);
            this.panLoc.Controls.Add(this.lblLoc);
            this.panLoc.Location = new System.Drawing.Point(2, 502);
            this.panLoc.Name = "panLoc";
            this.panLoc.Size = new System.Drawing.Size(470, 44);
            this.panLoc.TabIndex = 9;
            // 
            // pbMap
            // 
            this.pbMap.AutoSize = true;
            this.pbMap.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pbMap.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbMap.FlatAppearance.BorderSize = 0;
            this.pbMap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pbMap.Image = ((System.Drawing.Image)(resources.GetObject("pbMap.Image")));
            this.pbMap.Location = new System.Drawing.Point(431, 4);
            this.pbMap.Name = "pbMap";
            this.pbMap.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.pbMap.Size = new System.Drawing.Size(36, 36);
            this.pbMap.TabIndex = 3;
            this.pbMap.UseVisualStyleBackColor = true;
            this.pbMap.Click += new System.EventHandler(this.pbMap_Click);
            // 
            // bSave
            // 
            this.bSave.FlatAppearance.BorderSize = 0;
            this.bSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(204)))), ((int)(((byte)(51)))));
            this.bSave.Image = global::ShuriOutlookAddIn.Properties.Resources.ok_26;
            this.bSave.Location = new System.Drawing.Point(237, 0);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(235, 42);
            this.bSave.TabIndex = 12;
            this.bSave.Text = "Save";
            this.bSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // bCancel
            // 
            this.bCancel.FlatAppearance.BorderSize = 0;
            this.bCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.bCancel.Image = global::ShuriOutlookAddIn.Properties.Resources.cancel_26;
            this.bCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCancel.Location = new System.Drawing.Point(2, 0);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(235, 42);
            this.bCancel.TabIndex = 13;
            this.bCancel.Text = "Cancel";
            this.bCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // bOK
            // 
            this.bOK.FlatAppearance.BorderSize = 0;
            this.bOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.bOK.Image = global::ShuriOutlookAddIn.Properties.Resources.close_26;
            this.bOK.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bOK.Location = new System.Drawing.Point(3, -1);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(470, 42);
            this.bOK.TabIndex = 14;
            this.bOK.Text = "OK";
            this.bOK.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // tvRecips
            // 
            this.tvRecips.BackColor = System.Drawing.Color.OldLace;
            this.tvRecips.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvRecips.ContextMenuStrip = this.cMenu;
            this.tvRecips.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvRecips.Location = new System.Drawing.Point(24, 8);
            this.tvRecips.Margin = new System.Windows.Forms.Padding(0);
            this.tvRecips.Name = "tvRecips";
            this.tvRecips.ShowLines = false;
            this.tvRecips.ShowPlusMinus = false;
            this.tvRecips.ShowRootLines = false;
            this.tvRecips.Size = new System.Drawing.Size(211, 130);
            this.tvRecips.TabIndex = 21;
            this.tvRecips.DoubleClick += new System.EventHandler(this.tv_DoubleClick);
            // 
            // details
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(474, 667);
            this.ControlBox = false;
            this.Controls.Add(this.panLoc);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panRecipients);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panUnknown);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.bCancel);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "details";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Touch Details";
            this.Load += new System.EventHandler(this.details_Load);
            this.cMenu.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panUnknown.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panRecip.ResumeLayout(false);
            this.panRecipients.ResumeLayout(false);
            this.panRecipients.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panLoc.ResumeLayout(false);
            this.panLoc.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ContextMenuStrip cMenu;
        private System.Windows.Forms.ToolStripMenuItem bRemove;
        private System.Windows.Forms.ToolStripMenuItem bView;
        private System.Windows.Forms.ListView lvUnknown;
        private System.Windows.Forms.Label lblUnknown;
        private System.Windows.Forms.Panel panUnknown;
        private System.Windows.Forms.Panel panTags;
        private System.Windows.Forms.TreeView tvTags;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTag;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox lbACompleteTag;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panRecip;
        private System.Windows.Forms.Label lblRec;
        private System.Windows.Forms.TextBox txtRecipient;
        private System.Windows.Forms.Label lblAddRec;
        private System.Windows.Forms.ListBox lbAComplete;
        private System.Windows.Forms.Panel panRecipients;
        private System.Windows.Forms.Label lblSharing;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox ddOwnedByGroup;
        private System.Windows.Forms.ComboBox ddDatabase;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox ddType;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lblLoc;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.Panel panLoc;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button pbMap;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.TreeView tvRecips;
    }
}