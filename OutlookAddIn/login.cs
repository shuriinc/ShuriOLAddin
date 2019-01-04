#region Header

/*
 * Slovak Technical Services, Inc.
 * Ken Slovak
 * 5/18/17
 */

#endregion

using System;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ShuriOutlookAddIn
{
    public partial class LoginForm : Form
    {
        bool isInitialized = false;

        public LoginForm()
        {
            InitializeComponent();

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            string userId = "", userPwd = "";
            string encryptedID = Utilities.ReadRegStringValue(Properties.Resources.UserID);
            string encryptedPwd = Utilities.ReadRegStringValue(Properties.Resources.UserPassword);
            if (!String.IsNullOrEmpty(encryptedID))
            {
                userId = Utilities.Decrypt(encryptedID);
            }

            if (!String.IsNullOrEmpty(encryptedPwd))
            {
                userPwd = Utilities.Decrypt(encryptedPwd);
                checkboxSavePwd.Checked = true;
            }
            LoadInputs(userId, userPwd);

            //API
            ddEnviro.DataSource = Globals.ThisAddIn.Environments;
            ddEnviro.DisplayMember = "Name";
            foreach (ShuriEnvironment se in ddEnviro.Items)
            {
                if (se.Name == Globals.ThisAddIn.APIEnv.Name) ddEnviro.SelectedItem = se;
            }

            isInitialized = true;

        }

        public void LoadInputs(string username, string pw)
        {
            this.textboxUser.Text = username;
            this.textboxPassword.Text = pw;
            this.checkboxSavePwd.Checked = true;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Globals.ThisAddIn.Enabled = false;
            this.DialogResult = DialogResult.No;
            this.Close();

        }


        private void buttonLogin_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                Globals.ThisAddIn.APIEnv = (ShuriEnvironment)ddEnviro.SelectedItem;
                string token = DataAPIAnon.GetAuthToken(this.textboxUser.Text, this.textboxPassword.Text, 30, checkboxSavePwd.Checked);
                this.Cursor = Cursors.Default;
                string msg = "";


                if (token.Length > 5)
                {
                    switch (token.Substring(0, 5))
                    {
                        case "Error":
                            msg = token.Substring(5).Trim();
                            if (MessageBox.Show(msg + "\n\nTry again?", "Login Failed", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Cancel) this.DialogResult = DialogResult.No;
                            break;
                        case "offli":
                            msg = "Unable to contact Shuri\r\n\r\nRetry or press Cancel to work offline.";
                            DialogResult res = MessageBox.Show(msg, "Login timed out", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);
                            if (res == DialogResult.Cancel)
                            {
                             this.DialogResult = DialogResult.Cancel;
                               this.Close();
                            }
                            break;
                        default:
                            DataAPI.SetToken(token);
                            DataAPI.Login(true);
                            Globals.ThisAddIn.InitTheSync();

                            //reenable 
                            Globals.ThisAddIn.Enabled = true;

                            //save message - 1 time
                            if (Utilities.ReadRegStringValue("saveMessageShown") == null)
                            {
                                MessageBox.Show(String.Format("Login successful.\r\n\r\nSince you chose to save your password,\r\nyou will be automatically logged in to Shuri\r\nevery time you start Outlook.\r\n\r\nThis message will not display again.", Properties.Resources.AddinName), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Utilities.SetRegistryValue(Properties.Resources.RegistryPath, "saveMessageShown", "true", Microsoft.Win32.RegistryValueKind.String);
                            }


                            this.DialogResult = DialogResult.Yes;
                            this.Close();

                            break;
                    }

                }
            }
            catch
            {
                MessageBox.Show("Login Failed: ", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.No;
            }

        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            //if (textboxUser.Text.Trim().Length > 0) textboxPassword.Focus();

        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ddEnviro.Visible = !ddEnviro.Visible;
            lblEnviro.Visible = !lblEnviro.Visible;

        }

        private void bRegister_Click(object sender, EventArgs e)
        {
            registerForm regForm = new registerForm(this);
            regForm.StartPosition = FormStartPosition.CenterScreen;

            DialogResult res = regForm.ShowDialog();
            if (res == DialogResult.OK)
            {
                //user registered and is logged in
                this.DialogResult = DialogResult.OK;
                Globals.ThisAddIn.InitTheSync();

                this.Close();
            }
        }

        private void ddEnviro_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isInitialized)
            {
                Globals.ThisAddIn.APIEnv = (ShuriEnvironment)ddEnviro.SelectedItem;
            }
        }
    }
}