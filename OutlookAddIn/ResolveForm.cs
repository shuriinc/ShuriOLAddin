using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShuriOutlookAddIn
{
    public partial class ResolveForm : Form
    {
        private string _resolve;
        private Touch _touch = new Touch();

        #region Constructor & Init
        public ResolveForm(Touch touch, string strToResolve)
        {
            InitializeComponent();

            _resolve = strToResolve;
            _touch = touch;

        }

        private void ResolveForm_Load(object sender, EventArgs e)
        {
            lblResolve.Text = _resolve;
            List<AutocompleteResult> theList = DataAPI.ResolveEmail(_resolve);
            lbGuesses.DataSource = theList;
            lbGuesses.DisplayMember = "Name";
            lbGuesses.ValueMember = "Id";
            lbGuesses.ClearSelected();
            //lblGuesses.Text = string.Format("{0} Suggestions", theList.Count);

            ddDatabase.Items.Clear();
            foreach (Subscription sub in DataAPI.TheUser.Subscriptions)
            {
                if (sub.Group_Id != Guid.Empty && sub.UpdatableGroup)
                {
                    // if (sub.Group_Id != Guid.Empty && sub.Updatable && DataAPI.TheUser.UpdatableSubscriptionIds.Contains(sub.Group_Id))

                    ddDatabase.Items.Add(sub);
                    if (sub.Group_Id == DataAPI.TheUser.DefaultCollection_Id)
                    {
                        ddDatabase.SelectedItem = sub;
                    }
                }

            }
            lbAComplete.BringToFront();

            SetUI();
            txtRecipient.Focus();
            Cursor = Cursors.Default;
        }
        #endregion

        #region UI
        private void SetUI_Event(object sender, EventArgs e)
        {
            SetUI();
        }

        private void SetUI()
        {
            bAdd.Enabled = (rbOrg.Checked && !String.IsNullOrWhiteSpace(txtOrgname.Text))
                || (rbPerson.Checked && !String.IsNullOrWhiteSpace(txtFirst.Text) && !String.IsNullOrWhiteSpace(txtLast.Text));
            bChoose.Enabled = lbGuesses.SelectedIndex >= 0;
            lblFirstname.Enabled = txtFirst.Enabled = lblLastname.Enabled = txtLast.Enabled = rbPerson.Checked;
            lblOrgname.Enabled = txtOrgname.Enabled = rbOrg.Checked;
            bAdd.Text = (rbOrg.Checked) ? "Add Organization" : "Add Person";
            bChoose.Text = "Choose ";
            if (lbGuesses.SelectedIndex >= 0) bChoose.Text += ((AutocompleteResult)lbGuesses.SelectedItem).Name;
            ddDatabase.Visible = lblDB.Visible = (rbOrg.Checked || rbPerson.Checked) ;

        }

        #endregion

        #region Other Events

        private void pbClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }


        private void bAdd_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            Subscription sub = (Subscription)ddDatabase.SelectedItem;

            Group db = DataAPI.GetGroup(sub.Group_Id, false);
            if (db.Id != Guid.Empty)
            {
                if (rbOrg.Checked)
                {
                    Group org = new Group() { Name = txtOrgname.Text, OwnedBy_Id = DataAPI.TheUser.Id, GrpType = GroupType.Organization, Collection_Id = db.Id, OwnedByGroup_Id = db.OwnedByGroup_Id, changeType = ChangeType.Update };
                    org.Id = DataAPI.PostGroup(org);
                    ContactPoint cp = new ContactPoint() { Name = _resolve, UserType_Id = Guids.CP_Email, OwnedBy_Id = DataAPI.TheUser.Id, Collection_Id = db.Id, OwnedByGroup_Id = db.OwnedByGroup_Id, Primitive = ContactPointPrimitive.Email };
                    cp.Id = DataAPI.PostCP(cp);
                    org.ContactPoints.Add(cp);
                    DataAPI.PostRelationship(new RelationshipPost() { entityId1 = org.Id, entityId2 = cp.Id, entityType1 = EntityTypes.Group, entityType2 = EntityTypes.ContactPoint });
                    ResolveOrgAndClose(org);
                }
                else
                {
                    Person per = new Person() { Firstname = txtFirst.Text, Lastname = txtLast.Text, Name = txtFirst.Text + " " + txtLast.Text, OwnedBy_Id = DataAPI.TheUser.Id, Collection_Id = db.Id, OwnedByGroup_Id = db.OwnedByGroup_Id, changeType = ChangeType.Update };
                    per.Id = DataAPI.PostPerson(per);
                    ContactPoint cp = new ContactPoint() { Name = _resolve, UserType_Id = Guids.CP_Email, OwnedBy_Id = DataAPI.TheUser.Id, Collection_Id = db.Id, OwnedByGroup_Id = db.OwnedByGroup_Id, Primitive = ContactPointPrimitive.Email };
                    cp.Id = DataAPI.PostCP(cp);
                    per.ContactPoints.Add(cp);
                    DataAPI.PostRelationship(new RelationshipPost() { entityId1 = per.Id, entityId2 = cp.Id, entityType1 = EntityTypes.Person, entityType2 = EntityTypes.ContactPoint });
                    ResolvePersonAndClose(per);
                }
            }
            else MessageBox.Show("Unable to get db info", "Error");
        }
        #endregion

        #region Autocomplete ------------------------------------------------

        int _pause = 400;
        int _minLength = 2;
        Timer _timer = null;
        bool _isSearching = false;

        private void txtRecipient_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) ACReset(true);
            else if (txtRecipient.Text.Length == 1 && e.KeyCode == Keys.Back) ACReset(false);
            else if (txtRecipient.Text.Length >= _minLength)
            {
                if (_isSearching)
                {
                    e.SuppressKeyPress = true;
                    return;
                }
                if (_timer == null)
                {
                    _timer = new Timer();
                    _timer.Interval = _pause;
                    _timer.Tick += _timer_Tick;
                }
                _timer.Stop();
                if ((e.KeyValue >= 64 && e.KeyValue <= 122)
                    || e.KeyCode == Keys.Back) _timer.Start();
                else if (e.KeyCode == Keys.Down && lbAComplete.Visible) lbAComplete.Focus();
            }
            else ACReset(false);
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            if (String.IsNullOrWhiteSpace(txtRecipient.Text)) ACReset(false);
            else
            {
                _isSearching = true;
                Cursor = Cursors.WaitCursor;
                UpdateData();
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up && lbAComplete.SelectedIndex == 0)
            {
                txtRecipient.Focus();
            }
            else if (e.KeyCode == Keys.Return) ResolveResult(sender, e);
        }
        private void UpdateData()
        {

            lbAComplete.DataSource = DataAPI.AutocompletePeopleOrgs(txtRecipient.Text);
            lbAComplete.DisplayMember = "Name";
            lbAComplete.ValueMember = "Id";
            lbAComplete.Visible = txtRecipient.Enabled = true;
            gbAddNew.Visible = !lbAComplete.Visible;

            _isSearching = false;
            Cursor = Cursors.Default;
        }

        private void chooseClick(object sender, EventArgs e)
        {
            ResolveResult(sender, e);
        }

        private bool ResolveResult(object sender, EventArgs e)
        {

            ListBox lb = (ListBox)sender;

            AutocompleteResult res = lb.SelectedItem as AutocompleteResult;

            Resolve(res);
            return true;
        }

        private void ACReset(bool cleartext)
        {
            lbAComplete.Visible = false;
            gbAddNew.Visible = !lbAComplete.Visible;
            if (cleartext) txtRecipient.Text = "";
            txtRecipient.Focus();
        }

        private void lbAComplete_Leave(object sender, EventArgs e)
        {
            ACReset(false);
        }
        #endregion

        #region Resolve
        private bool Resolve(AutocompleteResult res)
        {
            if (res != null)
            {
                //confirm
                string nm = res.Name;
                //if (nm.IndexOf("  ") > 0) nm = res.Name.Substring(0, res.Name.IndexOf("  "));
                string msg = string.Format("{0}\n\n to:\n\n {1}?", _resolve, nm);
                if (MessageBox.Show(msg, "Confirm Match", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ContactPoint cp = new ContactPoint()
                    {
                        Name = _resolve,
                        UserType_Id = Guids.CP_Email,
                        OwnedBy_Id = DataAPI.TheUser.Id,
                        Collection_Id = _touch.Collection_Id,
                        OwnedByGroup_Id = _touch.OwnedByGroup_Id,
                        Primitive = ContactPointPrimitive.Email
                    };

                    cp.Id = DataAPI.PostCP(cp);
                    cp.changeType = ChangeType.Update;
                    DataAPI.PostRelationship(new RelationshipPost() { entityType1 = res.EntityType, entityId1 = res.Id, entityType2 = EntityTypes.ContactPoint, entityId2 = cp.Id });
                    if (res.EntityType != EntityTypes.Person)
                    {
                        Group org = DataAPI.GetOrganization(res.Id);
                        org.ContactPoints.Add(cp);
                        ResolveOrgAndClose(org);
                    }
                    else
                    {
                        Person per = DataAPI.GetPerson(res.Id);
                        per.ContactPoints.Add(cp);
                        ResolvePersonAndClose(per);
                    }
                }
                else ACReset(false);

                SetUI();
            }
            return true;
        }

        private bool ResolvePersonAndClose(Person per)
        {
            //update touch in ribbon
            per.changeType = ChangeType.Update;
            _touch.People.Add(per);
            _touch.resolveStrings.RemoveAll(s => s == _resolve);
            _touch.changeType = ChangeType.Update;

            DataAPI.AssociateEmail(per.Id, EntityTypes.Person, _resolve);

            this.DialogResult = DialogResult.OK;
            return true;
        }
        private bool ResolveOrgAndClose(Group org)
        {
            //update touch in ribbon
            org.changeType = ChangeType.Update;
            _touch.Groups.Add(org);
            _touch.resolveStrings.RemoveAll(s => s == _resolve);
            _touch.changeType = ChangeType.Update;
            DataAPI.AssociateEmail(org.Id, EntityTypes.Group, _resolve);
            this.DialogResult = DialogResult.OK;
            return true;
        }

        private void txtRecipient_Click(object sender, EventArgs e)
        {
            txtRecipient.SelectAll();
        }



        private void bChoose_Click(object sender, EventArgs e)
        {
            AutocompleteResult res = lbGuesses.SelectedItem as AutocompleteResult;

            Resolve(res);
        }

        #endregion

        //ListBox examples
        //private void DrawItemHandler(object sender, DrawItemEventArgs e)
        //{
        //    AutocompleteResult[] data = _ACResults.ToArray();
        //    AutocompleteResult res = data[e.Index];
        //    Color color = (res.EntityType == EntityTypes.Person) ? AppColors.personBGLight : AppColors.orgBGLight;
        //    e.DrawBackground();
        //    e.Graphics.FillRectangle(new SolidBrush(color), e.Bounds);
        //    color = (res.EntityType == EntityTypes.Person) ? AppColors.personDark : AppColors.orgDark;
        //    e.Graphics.DrawString(res.Name, new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(color), e.Bounds);
        //}
        //private void MeasureItemHandler(object sender, MeasureItemEventArgs e)
        //{
        //    //e.ItemHeight = 22;
        //}

    }
}
