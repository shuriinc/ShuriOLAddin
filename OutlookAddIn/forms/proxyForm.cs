using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShuriOutlookAddIn
{
    public partial class proxyForm : Form
    {
        private bool _isDirty = false; 
        private bool _initializing = false; 

        public proxyForm()
        {
            InitializeComponent();

        }
        private void proxyForm_Load(object sender, EventArgs e)
        {
            _initializing = true;
            string passEncrypt = "";

            txtHost.Text = Utilities.ReadRegStringValue(RegKeys.ProxyHost);
            txtPort.Text = Utilities.ReadRegStringValue(RegKeys.ProxyPort);
            txtUser.Text = Utilities.ReadRegStringValue(RegKeys.ProxyUser);
            passEncrypt = Utilities.ReadRegStringValue(RegKeys.ProxyPass);
 
            if (!String.IsNullOrEmpty(passEncrypt)) txtPass.Text = Utilities.Decrypt(passEncrypt);

            SetUI();

            _initializing = false;
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            Crypto crypt = new Crypto();
            string iv = "";
            string secret = "";

            Utilities.SetRegistryValue(Properties.Resources.RegistryPath, RegKeys.ProxyHost, txtHost.Text, Microsoft.Win32.RegistryValueKind.String);
            Utilities.SetRegistryValue(Properties.Resources.RegistryPath, RegKeys.ProxyPort, txtPort.Text, Microsoft.Win32.RegistryValueKind.String);
            Utilities.SetRegistryValue(Properties.Resources.RegistryPath, RegKeys.ProxyUser, txtUser.Text, Microsoft.Win32.RegistryValueKind.String);

            string passEncrypt = crypt.AESEncrypt(txtPass.Text, out secret, out iv);
            passEncrypt += ("~" + secret + "~" + iv);
            Utilities.SetRegistryValue(Properties.Resources.RegistryPath, RegKeys.ProxyPass, passEncrypt, Microsoft.Win32.RegistryValueKind.String);
            this.DialogResult = DialogResult.Yes;
        }

        private void SetUI()
        {
            if (_isDirty)
            {
                bOK.Visible = false;
                bCancel.Visible = bSave.Visible = true;

            }
            else
            {
                bOK.Visible = true;
                bCancel.Visible = bSave.Visible = false;
            }
        }

        private void txt_Enter(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            txt.SelectAll();
        }

        private void txt_TextChanged(object sender, EventArgs e)
        {
            if (!_initializing)
            {
                _isDirty = (
                            !(string.IsNullOrWhiteSpace(txtHost.Text))
                            && !(string.IsNullOrWhiteSpace(txtPort.Text))
                            && !(string.IsNullOrWhiteSpace(txtUser.Text))
                            && !(string.IsNullOrWhiteSpace(txtPass.Text))
                            );
                SetUI();
            }
        }

        private void txt_KeyUp(object sender, KeyEventArgs e)
        {
            _isDirty = true;
            SetUI();
        }
    }
}
