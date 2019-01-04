namespace ShuriOutlookAddIn
{
    partial class ribbonExplorer : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public ribbonExplorer()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TabAppointment1 = this.Factory.CreateRibbonTab();
            this.grpTouch1 = this.Factory.CreateRibbonGroup();
            this.tabMail = this.Factory.CreateRibbonTab();
            this.grpTouch2 = this.Factory.CreateRibbonGroup();
            this.tabShuri = this.Factory.CreateRibbonTab();
            this.grpSettings = this.Factory.CreateRibbonGroup();
            this.grpStatus = this.Factory.CreateRibbonGroup();
            this.lblStatus = this.Factory.CreateRibbonLabel();
            this.box1 = this.Factory.CreateRibbonBox();
            this.grpTouch = this.Factory.CreateRibbonGroup();
            this.grpLogin = this.Factory.CreateRibbonGroup();
            this.bOpenApp1 = this.Factory.CreateRibbonButton();
            this.bSync = this.Factory.CreateRibbonButton();
            this.bBreak = this.Factory.CreateRibbonButton();
            this.bOpenApp = this.Factory.CreateRibbonButton();
            this.bSync1 = this.Factory.CreateRibbonButton();
            this.bBreak1 = this.Factory.CreateRibbonButton();
            this.bSettings = this.Factory.CreateRibbonButton();
            this.bQuickSync = this.Factory.CreateRibbonButton();
            this.bSyncAll = this.Factory.CreateRibbonButton();
            this.bOpenApp2 = this.Factory.CreateRibbonButton();
            this.bSync2 = this.Factory.CreateRibbonButton();
            this.bBreak2 = this.Factory.CreateRibbonButton();
            this.bLogin = this.Factory.CreateRibbonButton();
            this.TabAppointment1.SuspendLayout();
            this.grpTouch1.SuspendLayout();
            this.tabMail.SuspendLayout();
            this.grpTouch2.SuspendLayout();
            this.tabShuri.SuspendLayout();
            this.grpSettings.SuspendLayout();
            this.grpStatus.SuspendLayout();
            this.box1.SuspendLayout();
            this.grpTouch.SuspendLayout();
            this.grpLogin.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabAppointment1
            // 
            this.TabAppointment1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.TabAppointment1.ControlId.OfficeId = "TabCalendar";
            this.TabAppointment1.Groups.Add(this.grpTouch1);
            this.TabAppointment1.Label = "TabCalendar";
            this.TabAppointment1.Name = "TabAppointment1";
            // 
            // grpTouch1
            // 
            this.grpTouch1.Items.Add(this.bOpenApp1);
            this.grpTouch1.Items.Add(this.bSync);
            this.grpTouch1.Items.Add(this.bBreak);
            this.grpTouch1.Label = "Shuri App";
            this.grpTouch1.Name = "grpTouch1";
            // 
            // tabMail
            // 
            this.tabMail.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tabMail.ControlId.OfficeId = "TabMail";
            this.tabMail.Groups.Add(this.grpTouch2);
            this.tabMail.Label = "TabMail";
            this.tabMail.Name = "tabMail";
            // 
            // grpTouch2
            // 
            this.grpTouch2.Items.Add(this.bOpenApp);
            this.grpTouch2.Items.Add(this.bSync1);
            this.grpTouch2.Items.Add(this.bBreak1);
            this.grpTouch2.Label = "Shuri App";
            this.grpTouch2.Name = "grpTouch2";
            // 
            // tabShuri
            // 
            this.tabShuri.Groups.Add(this.grpSettings);
            this.tabShuri.Groups.Add(this.grpStatus);
            this.tabShuri.Groups.Add(this.grpTouch);
            this.tabShuri.Groups.Add(this.grpLogin);
            this.tabShuri.Label = "Shuri";
            this.tabShuri.Name = "tabShuri";
            // 
            // grpSettings
            // 
            this.grpSettings.Items.Add(this.bSettings);
            this.grpSettings.Label = "Settings";
            this.grpSettings.Name = "grpSettings";
            // 
            // grpStatus
            // 
            this.grpStatus.Items.Add(this.lblStatus);
            this.grpStatus.Items.Add(this.box1);
            this.grpStatus.Label = "Status";
            this.grpStatus.Name = "grpStatus";
            // 
            // lblStatus
            // 
            this.lblStatus.Label = "[unknown]";
            this.lblStatus.Name = "lblStatus";
            // 
            // box1
            // 
            this.box1.BoxStyle = Microsoft.Office.Tools.Ribbon.RibbonBoxStyle.Vertical;
            this.box1.Items.Add(this.bQuickSync);
            this.box1.Items.Add(this.bSyncAll);
            this.box1.Name = "box1";
            // 
            // grpTouch
            // 
            this.grpTouch.Items.Add(this.bOpenApp2);
            this.grpTouch.Items.Add(this.bSync2);
            this.grpTouch.Items.Add(this.bBreak2);
            this.grpTouch.Label = "Shuri App";
            this.grpTouch.Name = "grpTouch";
            // 
            // grpLogin
            // 
            this.grpLogin.Items.Add(this.bLogin);
            this.grpLogin.Label = "Login";
            this.grpLogin.Name = "grpLogin";
            // 
            // bOpenApp1
            // 
            this.bOpenApp1.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bOpenApp1.Image = global::ShuriOutlookAddIn.Properties.Resources.device_48;
            this.bOpenApp1.Label = "Open App";
            this.bOpenApp1.Name = "bOpenApp1";
            this.bOpenApp1.ShowImage = true;
            this.bOpenApp1.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bOpenApp_Click);
            // 
            // bSync
            // 
            this.bSync.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bSync.Enabled = false;
            this.bSync.Image = global::ShuriOutlookAddIn.Properties.Resources.icon48alt;
            this.bSync.Label = "[waiting]";
            this.bSync.Name = "bSync";
            this.bSync.ShowImage = true;
            this.bSync.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bSync_Click);
            // 
            // bBreak
            // 
            this.bBreak.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bBreak.Label = "Break Sync";
            this.bBreak.Name = "bBreak";
            this.bBreak.OfficeImageId = "SharingRequestDeny";
            this.bBreak.ShowImage = true;
            this.bBreak.Visible = false;
            this.bBreak.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bBreakSync_Click);
            // 
            // bOpenApp
            // 
            this.bOpenApp.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bOpenApp.Image = global::ShuriOutlookAddIn.Properties.Resources.device_48;
            this.bOpenApp.Label = "Open App";
            this.bOpenApp.Name = "bOpenApp";
            this.bOpenApp.ShowImage = true;
            this.bOpenApp.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bOpenApp_Click);
            // 
            // bSync1
            // 
            this.bSync1.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bSync1.Enabled = false;
            this.bSync1.Image = global::ShuriOutlookAddIn.Properties.Resources.icon48alt;
            this.bSync1.Label = "[waiting]";
            this.bSync1.Name = "bSync1";
            this.bSync1.ShowImage = true;
            this.bSync1.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bSync_Click);
            // 
            // bBreak1
            // 
            this.bBreak1.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bBreak1.Label = "Break Sync";
            this.bBreak1.Name = "bBreak1";
            this.bBreak1.OfficeImageId = "SharingRequestDeny";
            this.bBreak1.ShowImage = true;
            this.bBreak1.Visible = false;
            this.bBreak1.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bBreakSync_Click);
            // 
            // bSettings
            // 
            this.bSettings.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bSettings.Label = "  ";
            this.bSettings.Name = "bSettings";
            this.bSettings.OfficeImageId = "AddInManager";
            this.bSettings.ScreenTip = "Manage  Addin Settings";
            this.bSettings.ShowImage = true;
            this.bSettings.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bSettings_Click);
            // 
            // bQuickSync
            // 
            this.bQuickSync.Label = "Refresh Status";
            this.bQuickSync.Name = "bQuickSync";
            this.bQuickSync.OfficeImageId = "Refresh";
            this.bQuickSync.ShowImage = true;
            this.bQuickSync.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bQuickSync_click);
            // 
            // bSyncAll
            // 
            this.bSyncAll.Label = "Full Sync";
            this.bSyncAll.Name = "bSyncAll";
            this.bSyncAll.OfficeImageId = "AccessRefreshAllLists";
            this.bSyncAll.ShowImage = true;
            this.bSyncAll.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bSyncAll_Click);
            // 
            // bOpenApp2
            // 
            this.bOpenApp2.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bOpenApp2.Image = global::ShuriOutlookAddIn.Properties.Resources.device_48;
            this.bOpenApp2.Label = "Open App";
            this.bOpenApp2.Name = "bOpenApp2";
            this.bOpenApp2.ShowImage = true;
            this.bOpenApp2.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bOpenApp_Click);
            // 
            // bSync2
            // 
            this.bSync2.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bSync2.Enabled = false;
            this.bSync2.Image = global::ShuriOutlookAddIn.Properties.Resources.icon48alt;
            this.bSync2.Label = "[waiting]";
            this.bSync2.Name = "bSync2";
            this.bSync2.ShowImage = true;
            this.bSync2.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bSync_Click);
            // 
            // bBreak2
            // 
            this.bBreak2.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bBreak2.Label = "Break Sync";
            this.bBreak2.Name = "bBreak2";
            this.bBreak2.OfficeImageId = "SharingRequestDeny";
            this.bBreak2.ShowImage = true;
            this.bBreak2.Visible = false;
            this.bBreak2.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bBreakSync_Click);
            // 
            // bLogin
            // 
            this.bLogin.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bLogin.Image = global::ShuriOutlookAddIn.Properties.Resources.icon48;
            this.bLogin.Label = "  ";
            this.bLogin.Name = "bLogin";
            this.bLogin.ShowImage = true;
            this.bLogin.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bLogin_Click);
            // 
            // ribbonExplorer
            // 
            this.Name = "ribbonExplorer";
            this.RibbonType = "Microsoft.Outlook.Explorer";
            this.Tabs.Add(this.TabAppointment1);
            this.Tabs.Add(this.tabMail);
            this.Tabs.Add(this.tabShuri);
            this.Close += new System.EventHandler(this.Ribbon_Close);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon_Load);
            this.TabAppointment1.ResumeLayout(false);
            this.TabAppointment1.PerformLayout();
            this.grpTouch1.ResumeLayout(false);
            this.grpTouch1.PerformLayout();
            this.tabMail.ResumeLayout(false);
            this.tabMail.PerformLayout();
            this.grpTouch2.ResumeLayout(false);
            this.grpTouch2.PerformLayout();
            this.tabShuri.ResumeLayout(false);
            this.tabShuri.PerformLayout();
            this.grpSettings.ResumeLayout(false);
            this.grpSettings.PerformLayout();
            this.grpStatus.ResumeLayout(false);
            this.grpStatus.PerformLayout();
            this.box1.ResumeLayout(false);
            this.box1.PerformLayout();
            this.grpTouch.ResumeLayout(false);
            this.grpTouch.PerformLayout();
            this.grpLogin.ResumeLayout(false);
            this.grpLogin.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab TabAppointment1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpTouch1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bOpenApp1;
        private Microsoft.Office.Tools.Ribbon.RibbonTab tabMail;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpTouch2;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bOpenApp;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bSync;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bSync1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bBreak;
        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabShuri;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpTouch;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bSync2;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bOpenApp2;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bBreak2;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bSettings;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bBreak1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpStatus;
        internal Microsoft.Office.Tools.Ribbon.RibbonLabel lblStatus;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bSyncAll;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bQuickSync;
        internal Microsoft.Office.Tools.Ribbon.RibbonBox box1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpLogin;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bLogin;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpSettings;
    }

    partial class ThisRibbonCollection
    {
        internal ribbonExplorer homeRibbon
        {
            get { return this.GetRibbon<ribbonExplorer>(); }
        }
    }
}
