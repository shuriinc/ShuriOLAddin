using Microsoft.Office.Tools.Ribbon;
using System;
using System.Collections.Generic;

namespace ShuriOutlookAddIn
{
    partial class ribbonInspector : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public ribbonInspector()
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
            this.tabAppointment = this.Factory.CreateRibbonTab();
            this.groupShuri = this.Factory.CreateRibbonGroup();
            this.bSync = this.Factory.CreateRibbonButton();
            this.bLoc = this.Factory.CreateRibbonButton();
            this.bOpenApp = this.Factory.CreateRibbonButton();
            this.bBreak = this.Factory.CreateRibbonButton();
            this.tabNewEmail = this.Factory.CreateRibbonTab();
            this.groupShuri1 = this.Factory.CreateRibbonGroup();
            this.bSync1 = this.Factory.CreateRibbonButton();
            this.bOpenApp1 = this.Factory.CreateRibbonButton();
            this.bBreak1 = this.Factory.CreateRibbonButton();
            this.tabReadEmail = this.Factory.CreateRibbonTab();
            this.groupShuri2 = this.Factory.CreateRibbonGroup();
            this.bSync2 = this.Factory.CreateRibbonButton();
            this.bOpenApp2 = this.Factory.CreateRibbonButton();
            this.bBreak2 = this.Factory.CreateRibbonButton();
            this.tabAppointment.SuspendLayout();
            this.groupShuri.SuspendLayout();
            this.tabNewEmail.SuspendLayout();
            this.groupShuri1.SuspendLayout();
            this.tabReadEmail.SuspendLayout();
            this.groupShuri2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabAppointment
            // 
            this.tabAppointment.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tabAppointment.ControlId.OfficeId = "TabAppointment";
            this.tabAppointment.Groups.Add(this.groupShuri);
            this.tabAppointment.Label = "TabAppointment";
            this.tabAppointment.Name = "tabAppointment";
            // 
            // groupShuri
            // 
            this.groupShuri.Items.Add(this.bSync);
            this.groupShuri.Items.Add(this.bLoc);
            this.groupShuri.Items.Add(this.bOpenApp);
            this.groupShuri.Items.Add(this.bBreak);
            this.groupShuri.Label = "Shuri";
            this.groupShuri.Name = "groupShuri";
            // 
            // bSync
            // 
            this.bSync.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bSync.Enabled = false;
            this.bSync.Image = global::ShuriOutlookAddIn.Properties.Resources.icon48alt;
            this.bSync.Label = "[working]";
            this.bSync.Name = "bSync";
            this.bSync.ShowImage = true;
            this.bSync.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bSync_Click);
            // 
            // bLoc
            // 
            this.bLoc.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bLoc.Image = global::ShuriOutlookAddIn.Properties.Resources.marker30;
            this.bLoc.Label = "Location";
            this.bLoc.Name = "bLoc";
            this.bLoc.ShowImage = true;
            this.bLoc.Visible = false;
            this.bLoc.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bLoc_Click);
            // 
            // bOpenApp
            // 
            this.bOpenApp.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bOpenApp.Image = global::ShuriOutlookAddIn.Properties.Resources.device_48;
            this.bOpenApp.Label = "Open Touch";
            this.bOpenApp.Name = "bOpenApp";
            this.bOpenApp.ShowImage = true;
            this.bOpenApp.Visible = false;
            this.bOpenApp.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bOpenApp1_Click);
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
            // tabNewEmail
            // 
            this.tabNewEmail.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tabNewEmail.ControlId.OfficeId = "TabNewMailMessage";
            this.tabNewEmail.Groups.Add(this.groupShuri1);
            this.tabNewEmail.Label = "TabNewMailMessage";
            this.tabNewEmail.Name = "tabNewEmail";
            // 
            // groupShuri1
            // 
            this.groupShuri1.Items.Add(this.bSync1);
            this.groupShuri1.Items.Add(this.bOpenApp1);
            this.groupShuri1.Items.Add(this.bBreak1);
            this.groupShuri1.Label = "Shuri";
            this.groupShuri1.Name = "groupShuri1";
            // 
            // bSync1
            // 
            this.bSync1.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bSync1.Enabled = false;
            this.bSync1.Image = global::ShuriOutlookAddIn.Properties.Resources.icon48alt;
            this.bSync1.Label = "[working]";
            this.bSync1.Name = "bSync1";
            this.bSync1.ShowImage = true;
            this.bSync1.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bSync_Click);
            // 
            // bOpenApp1
            // 
            this.bOpenApp1.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bOpenApp1.Image = global::ShuriOutlookAddIn.Properties.Resources.device_48;
            this.bOpenApp1.Label = "Open Touch";
            this.bOpenApp1.Name = "bOpenApp1";
            this.bOpenApp1.ShowImage = true;
            this.bOpenApp1.Visible = false;
            this.bOpenApp1.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bOpenApp1_Click);
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
            // tabReadEmail
            // 
            this.tabReadEmail.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tabReadEmail.ControlId.OfficeId = "TabReadMessage";
            this.tabReadEmail.Groups.Add(this.groupShuri2);
            this.tabReadEmail.Label = "TabReadMessage";
            this.tabReadEmail.Name = "tabReadEmail";
            // 
            // groupShuri2
            // 
            this.groupShuri2.Items.Add(this.bSync2);
            this.groupShuri2.Items.Add(this.bOpenApp2);
            this.groupShuri2.Items.Add(this.bBreak2);
            this.groupShuri2.Label = "Shuri";
            this.groupShuri2.Name = "groupShuri2";
            // 
            // bSync2
            // 
            this.bSync2.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bSync2.Enabled = false;
            this.bSync2.Image = global::ShuriOutlookAddIn.Properties.Resources.icon48alt;
            this.bSync2.Label = "[working]";
            this.bSync2.Name = "bSync2";
            this.bSync2.ShowImage = true;
            this.bSync2.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bSync_Click);
            // 
            // bOpenApp2
            // 
            this.bOpenApp2.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bOpenApp2.Image = global::ShuriOutlookAddIn.Properties.Resources.device_48;
            this.bOpenApp2.Label = "Open Touch";
            this.bOpenApp2.Name = "bOpenApp2";
            this.bOpenApp2.ShowImage = true;
            this.bOpenApp2.Visible = false;
            this.bOpenApp2.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bOpenApp1_Click);
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
            // ribbonInspector
            // 
            this.Name = "ribbonInspector";
            this.RibbonType = "Microsoft.Outlook.Appointment, Microsoft.Outlook.Mail.Compose, Microsoft.Outlook." +
    "Mail.Read";
            this.Tabs.Add(this.tabAppointment);
            this.Tabs.Add(this.tabNewEmail);
            this.Tabs.Add(this.tabReadEmail);
            this.Close += new System.EventHandler(this.Ribbon_Close);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon_Load);
            this.tabAppointment.ResumeLayout(false);
            this.tabAppointment.PerformLayout();
            this.groupShuri.ResumeLayout(false);
            this.groupShuri.PerformLayout();
            this.tabNewEmail.ResumeLayout(false);
            this.tabNewEmail.PerformLayout();
            this.groupShuri1.ResumeLayout(false);
            this.groupShuri1.PerformLayout();
            this.tabReadEmail.ResumeLayout(false);
            this.tabReadEmail.PerformLayout();
            this.groupShuri2.ResumeLayout(false);
            this.groupShuri2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupShuri;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bSync;
        internal RibbonButton bOpenApp;
        internal RibbonButton bBreak;
        internal RibbonButton bLoc;
        internal RibbonGroup groupShuri1;
        internal RibbonButton bSync1;
        internal RibbonButton bBreak1;
        internal RibbonButton bOpenApp1;
        private RibbonTab tabReadEmail;
        internal RibbonGroup groupShuri2;
        internal RibbonButton bSync2;
        internal RibbonButton bBreak2;
        internal RibbonButton bOpenApp2;
        internal RibbonTab tabAppointment;
        internal RibbonTab tabNewEmail;
    }

    partial class ThisRibbonCollection
    {
        internal ribbonInspector Ribbon
        {
            get { return this.GetRibbon<ribbonInspector>(); }
        }
    }
}
