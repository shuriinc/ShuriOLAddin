using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace ShuriOutlookAddIn
{
    public partial class SettingsForm : Form
    {
        private bool _initialized = false;
        private TimerForm _timerForm = null;

        public SettingsForm()
        {
            InitializeComponent();
            lblUser.Text = "";
        }

        private void settings_Load(object sender, EventArgs e)
        {
            this.Width = 385;
            panWait.Left = 0;
            SetUIWorking();
            lblVersion.Text = "Shuri Outlook Addin Version " + GetVersion();// Properties.Resources.Version;
            lblCopy.Text = string.Format("©Copyright 2017-{0} Shuri, Inc. All rights reserved.", DateTime.Now.Year);
            _timerForm = new TimerForm();
            _timerForm.Visible = false;
            _timerForm.timerSettings.Interval = 400;
            _timerForm.timerSettings.Tick += Initialize;
            _timerForm.timerSettings.Start();

        }

        private string GetVersion()
        {
            string s = "[not deployed]";
            try
            {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment applicationDeployment = ApplicationDeployment.CurrentDeployment;

                Version version = applicationDeployment.CurrentVersion;

                s = String.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            }

            }
            catch { }
            return s;
        }

        private void Initialize(object sender, EventArgs e)
        {
            //Debug.WriteLine("Initialize Settings {0:T}", DateTime.Now);
            if (DataAPI.Ready())
            {
                //Debug.WriteLine("Ready settings {0:T}", DateTime.Now); 
                _timerForm.timerSettings.Stop();
                if (DataAPI.Online)DataAPI.RefreshUser();
                SetUI();
            }

        }

        private void SetUIWorking()
        {
            panPrefs.Visible = panVersion.Visible = panUser.Visible = false;
            panWait.Visible = true;
        }
        private bool SetUI()
        {
            lblUser.Text = "";
            lblEnviron.Text = Globals.ThisAddIn.APIEnv.Name;

            cbUseProxy.Checked = false;
            try
            {
                string uprx = Utilities.ReadRegStringValue(RegKeys.UseProxy);
                if (!string.IsNullOrWhiteSpace(uprx)) cbUseProxy.Checked = Convert.ToBoolean(uprx);
            }
            catch { }

            if (DataAPI.Online)
            {
                lblUser.Text = string.Format("{0} - {1}", DataAPI.TheUser.Name, DataAPI.TheUser.Username);

                if (!DataAPI.DBsFiltered)
                {
                    lblEnviron.Text += "\nViewing all databases.";
                    string viewing = "Viewing:\n\n";
                    foreach (Subscription sub in DataAPI.TheUser.Subscriptions)
                    {
                        if (sub.Group_Id != Guid.Empty) viewing += sub.Name + "\n";
                    }
                    lblEnviron.Tag = viewing;
                }
                else
                {
                    string dbs = "";
                    int cntId = 0, cntSub = 0;
                    foreach (Guid id in DataAPI.TheUser.SubscriptionIds) if (id != Guid.Empty && id != Guids.System) cntId++;
                    foreach (Subscription sub in DataAPI.TheUser.Subscriptions) if (sub.Group_Id != Guid.Empty && sub.Group_Id != Guids.System) cntSub++;

                    dbs = string.Format("DBs filtered.  Viewing: {0} of {1} ", cntId, cntSub);

                    string viewing = "Viewing:\n\n";
                    string ignoring = "\n\nIgnoring:\n\n";
                    foreach (Subscription sub in DataAPI.TheUser.Subscriptions)
                    {
                        if (sub.Group_Id != Guid.Empty)
                        {
                            Guid res = DataAPI.TheUser.SubscriptionIds.Find(s => s == sub.Group_Id && s != Guid.Empty);
                            if (res != Guid.Empty) viewing += sub.Name + "\n";
                            else ignoring += sub.Name + "\n";
                        }
                    }
                    //if (title.IndexOf("\n") > -1) title = title.Substring(0, title.LastIndexOf("\n"));
                    lblEnviron.Tag = viewing + ignoring;
                    if (dbs != "")
                    {
                        lblEnviron.Text += "\n\n" + dbs;
                    }
                }


                cbCCConfirm.Checked = (DataAPI.UserPreferences.ContainsKey("ccconfirm") && Convert.ToBoolean(DataAPI.UserPreferences["ccconfirm"]));
                cbAddmetouch.Checked = (DataAPI.UserPreferences.ContainsKey("addmetouch") && Convert.ToBoolean(DataAPI.UserPreferences["addmetouch"]));
                panPrefs.Visible = panVersion.Visible = panUser.Visible = true;
                panOffline.Visible = false;
            }
            else
            {
                panPrefs.Visible = panUser.Visible = false;
                panOffline.Visible = panVersion.Visible = true;
                panOffline.Left = 0;
            }

            panWait.Visible = false;
            _initialized = true;
            return true;
        }

        private void bLogout_Click(object sender, EventArgs e)
        {
            Utilities.DeleteRegKey(Properties.Resources.OAuthToken);
            Globals.ThisAddIn.Enabled = false;
            bool res = DataAPI.Logout();
            this.DialogResult = DialogResult.Cancel;
        }



        private void closeForm(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

  
        private void cb_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                CheckBox cb = (CheckBox)sender;

                string prefname = cb.Name.Substring(2).ToLower();
                string prefvalue = cb.Checked.ToString().ToLower();

                DataAPI.PostUserPreference(prefname, prefvalue);
            }

        }

        private void resetPrefs_Click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegKeys.Clear();
            cbAddmetouch.Checked = true;
            cbCCConfirm.Checked = true;
            cbUseProxy.Checked = false;
            MessageBox.Show("Your preferences have been reset.", "Reset Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        //private void lblEnviron_MouseHover(object sender, EventArgs e)
        //{
        //    if (lblEnviron.Tag != null) toolTip1.Show(lblEnviron.Tag.ToString(), this, this.PointToClient(Cursor.Position), 12000);
        //}

        private void lblEnviron_Click(object sender, EventArgs e)
        {
            if (lblEnviron.Tag != null) MessageBox.Show(lblEnviron.Tag.ToString(), "Database Summary - Shuri App Filters", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cbUseProxy_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                if (cbUseProxy.Checked)
                {
                    Utilities.SetRegistryValue(Properties.Resources.RegistryPath, RegKeys.UseProxy, "true", Microsoft.Win32.RegistryValueKind.String);
                    //show form if settings are blank
                    if (!HasProxy()) ShowProxyForm();

                }
                else Utilities.DeleteRegKey(RegKeys.UseProxy);
            }
        }

        private void llProxy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowProxyForm();
        }

        private void ShowProxyForm()
        {
            proxyForm prox = new proxyForm();
            DialogResult res = prox.ShowDialog();
            if (!HasProxy()) cbUseProxy.Checked = false;
        }

        private bool HasProxy()
        {
            var host = Utilities.ReadRegStringValue(RegKeys.ProxyHost);
            //var port = Utilities.ReadRegStringValue(RegKeys.ProxyPort);
            if (string.IsNullOrWhiteSpace(host)) return false;// || string.IsNullOrWhiteSpace(port)
            else return true;

        }
        private void bLogin_Click(object sender, EventArgs e)
        {
            SetUIWorking();
            DataAPI.Login(false);
            Globals.ThisAddIn.InitTheSync();
            Initialize(sender, e);
        }
    }
}
