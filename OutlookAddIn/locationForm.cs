using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShuriOutlookAddIn
{
    public partial class locationForm : Form
    {
        Location _loc = null;
        string _mapUrl = "https://shuristoragecdn.blob.core.windows.net/apps/addin/olAddinMap.html";

        public locationForm(Location loc)
        {
            InitializeComponent();
            _loc = loc;
            txtAddress.Text = loc.Address;
      }
        public Location TheLocation
        {
            get { return _loc; } 
        }

        private bool ShowLoc()
        {
            if (TheLocation.Latitude == 0m && TheLocation.Longitude == 0m && !String.IsNullOrWhiteSpace(TheLocation.Address))
            {
                Location loc = GoogleAPI.GeoCode(TheLocation.Address);
                RefreshLoc(loc);
            }
            lblAddress.Text = TheLocation.Address;
            lblCity.Text = TheLocation.City;
            lblCountry.Text = TheLocation.Country;
            //lblError.Text = "Google place: " + TheLocation.Place_Id;
            lblError.Text = "";
            lblState.Text = TheLocation.State + ' ' + TheLocation.Postal;
            lblStreet.Text = TheLocation.Street;

            if (!(TheLocation.Latitude == 0m && TheLocation.Longitude == 0m))
            {
                //string url = Globals.ThisAddIn.APIEnv.BaseAppUrl + string.Format("/olAddinMap.html?lat={0}&lng={1}", TheLocation.Latitude, TheLocation.Longitude);
                string url = string.Format("{0}?lat={1}&lng={2}", _mapUrl, TheLocation.Latitude, TheLocation.Longitude);
                this.webBrowser1.Navigate(new Uri(url));
                this.Width = 680;
            }
            else
            {
                this.Width = 294;
                if (String.IsNullOrWhiteSpace(TheLocation.Address)) lblError.Text = "";
                else lblError.Text = "Google was unable to resolve the address";
            }
            gbAddress.Visible = (
                !string.IsNullOrEmpty(txtAddress.Text)
                || !string.IsNullOrEmpty(TheLocation.City)
                || !string.IsNullOrEmpty(TheLocation.Country)
                || !string.IsNullOrEmpty(TheLocation.Postal)
                || !string.IsNullOrEmpty(TheLocation.State)
                || !string.IsNullOrEmpty(TheLocation.Street)
                );

            return true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Location loc = GoogleAPI.GeoCode(txtAddress.Text);
            RefreshLoc(loc);
            ShowLoc();

        }

        private void RefreshLoc(Location locNew)
        {
            TheLocation.Address = locNew.Address;
            TheLocation.City = locNew.City;
            TheLocation.Country = locNew.Country;
            TheLocation.Latitude = locNew.Latitude;
            TheLocation.Longitude = locNew.Longitude;
            TheLocation.Place_Id = locNew.Place_Id;
            TheLocation.Postal = locNew.Postal;
            TheLocation.State = locNew.State;
            TheLocation.Street = locNew.Street;
            
        }

        private void locationForm_Load(object sender, EventArgs e)
        {
            ShowLoc();

        }

        private void pbClose_Click(object sender, EventArgs e)
        {
            Button butt = sender as Button;
            if (butt.Name == "bOK") this.DialogResult = DialogResult.OK;
            else this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void txtAddress_SelectAll(object sender, EventArgs e)
        {
            txtAddress.SelectAll();
        }
    }
}
