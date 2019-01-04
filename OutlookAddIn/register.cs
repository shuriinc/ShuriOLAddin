using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShuriOutlookAddIn
{
    public partial class registerForm : Form
    {
        int formClosed = 370, formOpen = 726, formHeight = 496;
        int flyoutX = 356;
        int flyoutY = 58;
        bool isOpen = false;
        string lastOpened = "";
        bool isInitialized = false;
        Image imageOK = Properties.Resources.ok_26;
        Image imageNO = Properties.Resources.cancel_26;
        LoginForm loginForm = null;

        internal RegisterModel rModel = new RegisterModel();

        public registerForm(LoginForm parent)
        {
            InitializeComponent();
            Size size = this.Size;
            size.Width = formClosed;
            size.Height = formHeight;
            this.Size = size;
            loginForm = parent;
        }

        #region event handlers

        private void registerForm_Load(object sender, EventArgs e)
        {
            SetUI("");

            //API
            ddEnviro.DataSource = Globals.ThisAddIn.Environments;
            ddEnviro.DisplayMember = "Name";
            string envName = Globals.ThisAddIn.APIEnv.Name;
            foreach (ShuriEnvironment se in ddEnviro.Items)
            {
                if (se.Name == envName) ddEnviro.SelectedItem = se;
            }

            isInitialized = true;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            //this.Close();
        }

        private void bRegister_Click(object sender, EventArgs e)
        {
            string regResult = "";

            //Register
            if (string.IsNullOrWhiteSpace(textboxLastname.Text) || string.IsNullOrWhiteSpace(textboxUser.Text) || string.IsNullOrWhiteSpace(textboxPassword.Text) || !cbIAgree.Checked) regResult = "Missing registration information.";
            else {
                rModel = new RegisterModel()
                {
                    Firstname = textBoxFirstname.Text,
                    Lastname = textboxLastname.Text,
                    Sitename = textBoxSitename.Text,
                    UserName = textboxUser.Text,
                    Password = textboxPassword.Text,
                    userAgreed = cbIAgree.Checked,
                    freeTrial = cbFreeTrial.Checked
                };
                regResult = DataAPIAnon.Register(rModel);
            }
            if (regResult == "")
            {
                string token = DataAPIAnon.GetAuthToken(rModel.UserName, rModel.Password, 30, cbRemember.Checked);
                DataAPI.SetToken(token);
                loginForm.LoadInputs(rModel.UserName, rModel.Password);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                DialogResult res = MessageBox.Show(regResult, "Unable to Register", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bFAQ_Click(object sender, EventArgs e)
        {
            if (lastOpened == "faqs" && isOpen) isOpen = false;
            else isOpen = true;

            SetUI("faqs");

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel link = (LinkLabel)sender;
            string mode = link.Tag.ToString();

            if (lastOpened == mode && isOpen) isOpen = false;
            else isOpen = true;
            
            SetUI(mode);
        }

        private void cbIAgree_CheckedChanged(object sender, EventArgs e)
        {
            SetEnableReg();
        }

        private void textboxUser_TextChanged(object sender, EventArgs e)
        {
            SetEnableReg();
            lblInUse.Visible = false;
            bool isValid = false;
            bool showIcon = false;
            if (!string.IsNullOrWhiteSpace(textboxUser.Text))
            {
                showIcon = true;
                if (DataAPI.IsValidEmail(textboxUser.Text))
                {
                    //bool inU
                    isValid = DataAPIAnon.IsUsernameOK(textboxUser.Text);
                    lblInUse.Visible = !isValid;
                }
            }
            textboxUser.Tag = isValid;
            pbUsername.Visible = showIcon;
            if (isValid) pbUsername.Image = imageOK;
            else pbUsername.Image = imageNO;
        }

        #endregion

        private void SetEnableReg()
        {
            bRegister.Enabled = (cbIAgree.Checked
                && textboxLastname.Text.Length > 1
                && Convert.ToBoolean(textboxUser.Tag)
                && Convert.ToBoolean(textboxPassword.Tag));

        }

        private void textboxPassword_TextChanged(object sender, EventArgs e)
        {
            SetEnableReg();
            bool isValid = false;
            bool showIcon = false;
            if (!string.IsNullOrWhiteSpace(textboxPassword.Text))
            {
                showIcon = true;
                isValid = DataAPIAnon.IsGoodPassword(textboxPassword.Text);
            }
            textboxPassword.Tag = isValid;
            pbPassword.Visible = showIcon;
            if (isValid) pbPassword.Image = imageOK;
            else pbPassword.Image = imageNO;
            lblPwdRules.Visible = !isValid;

        }


        private void cbShowPwd_CheckedChanged(object sender, EventArgs e)
        {
            //textboxPassword.char
            CheckBox cb = (CheckBox)sender;
            if (cb.Checked) textboxPassword.PasswordChar = Char.MinValue;
             else textboxPassword.PasswordChar = Convert.ToChar("*");
        }

         private void cbSite_CheckedChanged(object sender, EventArgs e)
        {
            textBoxSitename.Visible = lblSitename.Visible = cbSitename.Checked; 
        }

        private void pbSitehelp_Click(object sender, EventArgs e)
        {
            if (lastOpened == "site" && isOpen) isOpen = false;
            else isOpen = true;

            SetUI("site");

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            ddEnviro.Visible = lblEnviro.Visible = !ddEnviro.Visible;
        }

        private void ddEnviro_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isInitialized)
            {
                Globals.ThisAddIn.APIEnv = (ShuriEnvironment)ddEnviro.SelectedItem;
            }
        }

        private void pb_MouseHover(object sender, EventArgs e)
        {
            Control con = (Control)sender;
            if (con.Tag != null) toolTip1.Show(con.Tag.ToString(), con);
        }

        private void textBoxSitename_TextChanged(object sender, EventArgs e)
        {
            bool isValid = false;
            bool showIcon = false;
            if (textBoxSitename.Text.Length > 2)
            {
                    showIcon = true;
                    isValid = DataAPIAnon.IsTeamnameOK(textBoxSitename.Text);
            }
            pbSitename.Visible = showIcon;
            if (isValid) pbSitename.Image = imageOK;
            else pbSitename.Image = imageNO;

        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = Globals.ThisAddIn.APIEnv.BaseWWWUrl + "work/#/master/main";
            System.Diagnostics.Process.Start(url);

        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "http://shuri.com/ar";
            System.Diagnostics.Process.Start(url);

        }


        private void pbFree_Click(object sender, EventArgs e)
        {
            if (lastOpened == "free" && isOpen) isOpen = false;
            else isOpen = true;
            SetUI("free");

        }

        private void textboxLastname_TextChanged(object sender, EventArgs e)
        {
            SetEnableReg();
        }

        private void textboxPassword_Leave(object sender, EventArgs e)
        {
            lblPwdRules.Visible = false;
        }

        private void SetUI(string mode)
        {

            Point loc = new Point(900, flyoutY);
            panTerms.Location = panSecurity.Location = panFAQ.Location = panSite.Location = panFreeMo.Location = loc;
            loc = new Point(flyoutX, flyoutY);
            //lblNoIT.Visible = false;

            switch (mode)
            {
                case "":
                    isOpen = false;
                    break;
                case "faqs":
                    lblTitle.Text = "Shuri FAQ's";
                    panFAQ.Location = loc;
                    break;
                case "security":
                    lblTitle.Text = "Security && Privacy";
                    panSecurity.Location = loc;
                    break;
                case "site":
                    lblTitle.Text = "Provision Your Site";
                    panSite.Location = loc;
                    //lblNoIT.Visible = true;
                    break;
                case "terms":
                    lblTitle.Text = "Terms && Conditions";
                    panTerms.Location = loc;
                    break;
                case "free":
                    lblTitle.Text = "Free Analyst Relations";
                    panFreeMo.Location = loc;
                    break;
            }


            Size size = this.Size;
            if (!isOpen) size.Width = formClosed;
            else size.Width = formOpen;
            this.Size = size;

            lastOpened = mode;
        }
    }
}
