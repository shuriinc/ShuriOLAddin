using System;
using System.Collections.Generic;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShuriOutlookAddIn
{
    public partial class details : Form
    {
        #region Globals & Init
        private Touch _touch = new Touch();
        private bool _isDirty = false;
        private bool _isEmail = false;
        private bool _isInitialized = false;
        private Font _tvheaderFont = new Font(new FontFamily("Arial"), 7, FontStyle.Bold, GraphicsUnit.Point);
        private List<UserType> _touchTypes = new List<UserType>();
        private Outlook.MailItem _mail = null;
        private Outlook.AppointmentItem _appt = null;

        private bool _clearWarningShown;
        public details(Touch touch, Outlook.MailItem outlookItem, bool isNew)
        {
            InitializeComponent();
            _touch = touch;
            _isDirty = isNew;

            _isEmail = true;
            _mail = outlookItem as Outlook.MailItem;

        }
        public details(Touch touch, Outlook.AppointmentItem outlookItem, bool isNew)
        {
            InitializeComponent();
            _touch = touch;
            _isDirty = isNew;
            _isEmail = false;
            _appt = outlookItem as Outlook.AppointmentItem;
        }
        ~details()
        {
            _mail = null;
            _appt = null;
            _touch = null;
        }

        private void details_Load(object sender, EventArgs e)
        {
            this.Enabled = false;

            _touchTypes = DataAPI.GetToucheTypes();
            if (_touch.UserType_Id == Guid.Empty)
            {
                //throw new Exception("");
                Debug.WriteLine("no touch user type assigned.");
            }

            if (_touch.Collection_Id == Guid.Empty) _touch.Collection_Id = DataAPI.TheUser.DefaultCollection_Id;

            //get editing team for db
            Guid ogId = _touch.OwnedByGroup_Id;
            if (ogId == Guid.Empty && _touch.Id == Guid.Empty)
            {
                Group db = DataAPI.GetGroup(_touch.Collection_Id, false);
                ogId = db.OwnedByGroup_Id;
            }
            List<Group> editTeams = DataAPI.EditTeamsDB(_touch.Collection_Id);
            SetEditTeams(editTeams, ogId);
            //ddType.Visible = ddOwnedByGroup.Visible = ddDatabase.Visible = false;
            InitDropDowns();

            //Debug.WriteLine("Shown");
            PaintTouch();
            //ddType.Visible = ddOwnedByGroup.Visible = ddDatabase.Visible = true;
            _isInitialized = true;

            if (_isEmail) txtTag.Focus();
            else txtRecipient.Focus();
            this.Enabled = true;
        }

        private void InitDropDowns()
        {
            _touchTypes.RemoveAll(t => (TouchPrimitive)t.Primitive == TouchPrimitive.TrackedEmail);
            //if (_isEmail) _touchTypes.RemoveAll(t => (TouchPrimitive)t.Primitive != TouchPrimitive.Email);
            ddType.DataSource = _touchTypes;
            ddType.DisplayMember = "Name";
            ddType.ValueMember = "Id";

            ddDatabase.Items.Clear();
            foreach (Subscription sub in DataAPI.TheUser.Subscriptions)
            {
                if (sub.Group_Id != Guid.Empty && sub.UpdatableGroup) ddDatabase.Items.Add(sub);
            }

        }
        #endregion

        private bool PaintTouch()
        {

            lblName.Text = _touch.Name;
            //lblRec.Text = "Recipients";
            //lblUnknown.Text = "Unknown Recipients";
            lblRec.Text = "Participants";
            lblUnknown.Text = "Unknown Participants";

            this.Text = "Touch Details";
            if (_touch.Id == Guid.Empty)
            {
                this.Text = "New Sync Details";
            }

            bSave.Visible = bCancel.Visible = (_isDirty);
            bOK.Visible = !_isDirty;

            if (_isEmail)
            {
                lblLoc.Visible = lblLocation.Visible = pbMap.Visible = lblAddRec.Visible = txtRecipient.Visible =  false;
                panLoc.BackColor = AppColors.tagBGLight;
                if (_touch.Id != Guid.Empty)
                {
                    ddDatabase.Enabled = ddOwnedByGroup.Enabled = ddType.Enabled =  false;
                    lblRec.Enabled = lblSharing.Enabled = lblType.Enabled =  false;
                    tvRecips.MouseUp -= tv_MouseUp;
                    tvRecips.DoubleClick -= tv_DoubleClick;
                }
            }

            #region Recipients
            lvUnknown.Items.Clear();
            List<string> unknowns = _touch.resolveStrings.OrderBy(t => t).ToList();
            foreach (string str in unknowns)
            {
                lvUnknown.Items.Add(str, 0);
            }
            //if (isNew || !_isEmail)
            //{
            //}
            List<Person> people = _touch.People.FindAll(t => t.changeType != ChangeType.Remove).OrderBy(t => t.Lastname).ToList();
            List<Group> orgs = _touch.Groups.FindAll(t => t.changeType != ChangeType.Remove && t.GrpType == GroupType.Organization).OrderBy(t => t.Name).ToList();

            List<TreeNode> recips = new List<TreeNode>();

            if (people.Count > 0 && orgs.Count > 0)
            {
                //separate them out
                TreeNode nodeP = new TreeNode() { Name = "People", Text = "People", NodeFont = _tvheaderFont, ForeColor = AppColors.personDark };
                foreach (Person per in people)
                {
                    TreeNode perNode = new TreeNode() { Name = per.Name, Tag = per, Text = per.Name };
                    nodeP.Nodes.Add(perNode);
                }
                recips.Add(nodeP);
                TreeNode nodeO = new TreeNode() { Name = "Organizations", Text = "Organizations", NodeFont = _tvheaderFont, ForeColor = AppColors.orgDark };
                foreach (Group org in orgs)
                {
                    TreeNode orgNode = new TreeNode() { Name = org.Name, Tag = org, Text = org.Name };
                    nodeO.Nodes.Add(orgNode);
                }
                recips.Add(nodeO);

            }
            else if (people.Count > 0 || orgs.Count > 0)
            {
                foreach (Person per in people)
                {
                    TreeNode perNode = new TreeNode() { Name = per.Name, Tag = per, Text = per.Name };
                    recips.Add(perNode);
                    //Debug.WriteLine(per.Name);
                }
                foreach (Group org in orgs)
                {
                    TreeNode orgNode = new TreeNode() { Name = org.Name, Tag = org, Text = org.Name };
                    recips.Add(orgNode);
                }
            }

            tvRecips.Nodes.Clear();
            if (recips.Count > 0)
            {
                tvRecips.Nodes.AddRange(recips.ToArray());
                tvRecips.ExpandAll();
                tvRecips.Nodes[0].EnsureVisible();
                foreach (TreeNode node in tvRecips.Nodes)
                {
                    string t = node.Text;
                    node.Text = "";
                    node.Text = t;
                }

            }


            //set Recips width
            int wWide = 470, wNarrow = 242;
            if (lvUnknown.Items.Count == 0)
            {
                //full width
                tvRecips.Size = new Size(wWide, tvRecips.Size.Height);
                panRecipients.Size = new Size(wWide, panRecipients.Size.Height);
                panRecip.Size = new Size(wWide, panRecip.Size.Height);
                tvRecips.Location = new Point(22, 51);
                lblRec.Location = new Point(10, 16);
                lblAddRec.Location = new Point(98, 17);
                txtRecipient.Location = new Point(126, 14);
                lbAComplete.Location = new Point(126, 34);
                panUnknown.Visible = false;

            }
            else
            {
                tvRecips.Size = new Size(wNarrow, tvRecips.Size.Height);
                panRecipients.Size = new Size(wNarrow, panRecipients.Size.Height);
                panRecip.Size = new Size(wNarrow, panRecip.Size.Height);
                tvRecips.Location = new Point(0, 51);
                lblRec.Location = new Point(53, 4);
                lblAddRec.Location = new Point(6, 24);
                txtRecipient.Location = new Point(36, 22);
                lbAComplete.Location = new Point(36, 42);
                panUnknown.Visible = true;
            }
            #endregion

            #region Tags
            List<Tag> tags = _touch.Tags.FindAll(t => t.changeType != ChangeType.Remove).OrderBy(t => t.Name).ToList();
            List<TreeNode> uts = new List<TreeNode>();

            foreach (Tag tg in tags)
            {
                TreeNode ut = uts.Find(u => (u.Tag.ToString() == tg.UserType_Id.ToString()) || (Guid.Parse(u.Tag.ToString()) == Guid.Empty && u.Name == tg.Typename));
                if (ut == null)
                {
                    ut = new TreeNode() { Tag = tg.UserType_Id, NodeFont = _tvheaderFont, ForeColor = AppColors.tagDark };
                    ut.Text = ut.Name = tg.Typename;
                    uts.Add(ut);
                }
                TreeNode tag = new TreeNode() { Name = tg.Name, Tag = tg, Text = tg.Name };
                ut.Nodes.Add(tag);
            }

            List<TreeNode> utsSort = uts.OrderBy(u => u.Name).ToList();

            //fix loose tags
            TreeNode loose = utsSort.Find(u => Guid.Parse(u.Tag.ToString()) == Guids.Tag_Loose);
            if (loose != null) loose.Text = loose.Name = "Loose Tags";

            tvTags.Nodes.Clear();
            if (utsSort.Count > 0)
            {
                tvTags.Nodes.AddRange(utsSort.ToArray());
                tvTags.ExpandAll();
                tvTags.Nodes[0].EnsureVisible();
                foreach (TreeNode node in tvTags.Nodes)
                {
                    string t = node.Text;
                    node.Text = "";
                    node.Text = t;
                }
            }
            tvTags.BackColor = panTags.BackColor = AppColors.tagBGLight;

            #endregion

            #region Locations 

            if (_touch.Locations.Count > 0)
            {
                lblLocation.Text = _touch.Locations[0].Address;
            }
            else
            {
                lblLocation.Text = "";
            }
            #endregion

            #region Type/DB Dropdowns
            _isInitialized = false;

            if (ddType.Items.Count > 0)
            {
                int idx = 0;
                for (var j = 0; j < ddType.Items.Count; j++)
                {
                    UserType ut = ddType.Items[j] as UserType;
                    if (_touch.UserType_Id == ut.Id)
                    {
                        idx = j;
                        break;
                    }
                }
                ddType.SelectedIndex = idx;
            }

            if (ddDatabase.Items.Count > 0)
            {
                int idx = 0;
                for (var j = 0; j < ddDatabase.Items.Count; j++)
                {
                    Subscription sub = ddDatabase.Items[j] as Subscription;
                    if (_touch.Collection_Id == sub.Group_Id)
                    {
                        idx = j;
                        break;
                    }
                }
                ddDatabase.SelectedIndex = idx;
            }
            _isInitialized = true;
            #endregion

            return _isInitialized;

        }


        #region recipients
        private bool AddRecipient(AutocompleteResult res, string email)
        {
            //add a dummy cp to enable sync back to appt
            ContactPoint cp = new ContactPoint() { Name = email, Primitive = ContactPointPrimitive.Email };

            if (res.EntityType == EntityTypes.Person)
            {
                //Person per = new Person() { Id = res.Id, changeType = ChangeType.Update, Name = res.Name, ImageUrl = res.ImageUrlThumb };
                Person per = new Person();// { Id = res.Id, changeType = ChangeType.Update, Name = res.Name, ImageUrl = res.ImageUrlThumb };
                per = DataAPI.GetPerson(res.Id);
                per.ContactPoints.Add(cp);
                per.changeType = ChangeType.Update;
                _touch.People.Add(per);
                MakeDirty();
            }
            else if (res.EntityType == EntityTypes.Organization)
            {
                //Group org = new Group() { Id = res.Id, changeType = ChangeType.Update, Name = res.Name, ImageUrl = res.ImageUrlThumb, GrpType = GroupType.Organization };
                Group org = new Group();// { Id = res.Id, changeType = ChangeType.Update, Name = res.Name, ImageUrl = res.ImageUrlThumb };
                org = DataAPI.GetOrganization(res.Id);
                org.ContactPoints.Add(cp);
                org.changeType = ChangeType.Update;
                _touch.Groups.Add(org);
                MakeDirty();
            }
            _touch.changeType = ChangeType.Update;
            return true;
        }

        private bool RemoveRecipient(object entity)
        {
            if (_isInitialized)
            {
                Guid entityId = Guid.Empty;
                EntityTypes entityType = EntityTypes.All;
                string name = "";
                if (entity is Person)
                {
                    Person perNew = (Person)entity;
                    perNew.changeType = ChangeType.Remove;
                    name = perNew.Name;
                    entityId = perNew.Id;
                    entityType = EntityTypes.Person;
                    Person per = _touch.People.Find(p => p.Id == perNew.Id);
                    if (per != null) per.changeType = ChangeType.Remove;
                    _touch.changeType = ChangeType.Update;
                }
                else if (entity is Group)
                {
                    Group orgNew = (Group)entity;
                    orgNew.changeType = ChangeType.Remove;
                    name = orgNew.Name;
                    entityId = orgNew.Id;
                    entityType = EntityTypes.Group;
                    Group org = _touch.Groups.Find(p => p.Id == orgNew.Id);
                    if (org != null) org.changeType = ChangeType.Remove;
                    _touch.changeType = ChangeType.Update;
                }

                if (entityId != Guid.Empty && entityType != EntityTypes.All)
                {
                    string email = DataAPI.EmailForEntity(entityId, entityType);
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        MessageBox.Show("No email address on record for " + name, "Get Email Address Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (email.Length >= 5 && email.Substring(0, 5).ToLower() == "error")
                    {
                        MessageBox.Show(email, "Get Email Address Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (email.IndexOf("@") > 0)
                    {

                        //todo fix
                        //_ribbon.RemoveRecipient(name, email);
                        _touch.changeType = ChangeType.Update;
                    }

                }
                MakeDirty();
            }
            return true;
        }

        private void lvUnknown_Click(object sender, EventArgs e)
        {
            //check if filters and warn of clearing
            //Debug.WriteLine("click");
            var ok2Continue = false;
            if (DataAPI.DBsFiltered && !_clearWarningShown)
            {
                var ok2Clear = MessageBox.Show("This will clear your database filters. All databases will be now be visible.  \n\nOK to continue?", "OK to See All Databases?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (ok2Clear == DialogResult.OK)
                {
                    _clearWarningShown = true;
                    DataAPI.ClearDBFilters();
                    ok2Continue = true;
                }
            }
            else ok2Continue = true;

            if (ok2Continue) ResolveUnknown(lvUnknown.SelectedItems[0].Text);

        }

        private void ResolveUnknown(string str)
        {
            //todo fix

            ResolveForm resForm = new ResolveForm(_touch, str);
            DialogResult res = resForm.ShowDialog();
            lvUnknown.Items[lvUnknown.SelectedIndices[0]].Selected = false;
            if (res == DialogResult.OK)
            {
                MakeDirty();
                PaintTouch();
            }

        }

        #endregion

        #region Tags

        private bool AddTag(AutocompleteResult res)
        {
            Tag theTag = _touch.Tags.Find(tch => tch.Id == res.Id);
            if (theTag == null)
            {
                theTag = new Tag() { Id = res.Id, Name = res.Name, changeType = ChangeType.Update, Typename = res.ImageUrlThumb };
                //get the UT?
                Tag tagForUT = _touch.Tags.Find(t => t.Typename == theTag.Typename);
                if (tagForUT != null) theTag.UserType_Id = tagForUT.UserType_Id;
                _touch.Tags.Add(theTag);
            }
            else
            {
                theTag.changeType = ChangeType.Update;
            }
            _touch.changeType = ChangeType.Update;

            AddTagEmailCheck(theTag.Id);

            ResetACTag(true);
            MakeDirty();
            txtTag.Focus();

            return true;
        }

        private bool AddNewTag()
        {
            string name = txtTag.Text;

            if (!string.IsNullOrWhiteSpace(name))
            {
                Tag tg = _touch.Tags.Find(t => t.Name == name);
                if (tg == null)
                {
                    if (MessageBox.Show("Add new tag: " + name + "?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        tg = new Tag()
                        {
                            Name = name,
                            changeType = ChangeType.Update,
                            Collection_Id = DataAPI.TheUser.DefaultCollection_Id,
                            OwnedByGroup_Id = DataAPI.TheUser.DefaultOwnedByGroup_Id,
                            Typename = "POST", //trigger a Tag POST
                            UserType_Id = Guids.Tag_Loose
                        };
                        _touch.Tags.Add(tg);
                        _touch.changeType = ChangeType.Update;
                        AddTagEmailCheck(tg.Id);
                    }
                }
                ResetACTag(true);
                MakeDirty();
                txtTag.Focus();
            }
            return true;
        }

        private bool RemoveTag(Tag tag)
        {
            if (_isInitialized)
            {
                Tag tg = _touch.Tags.Find(t => t.Id == tag.Id);
                if (tg != null)
                {
                    tg.changeType = ChangeType.Remove;
                    _touch.changeType = ChangeType.Update;
                    //existing email touch: save the relationship
                    if (_isEmail && _touch.Id != Guid.Empty)
                    {
                        DataAPI.DeleteRelationship(new RelationshipPost() { entityType1 = EntityTypes.Touch, entityId1 = _touch.Id, entityType2 = EntityTypes.Tag, entityId2 = tag.Id });
                    }
                    MakeDirty();
                }
            }
            return true;
        }

        private bool AddTagEmailCheck(Guid tagId)
        {
            //existing email touch: save the relationship
            if (_isEmail && _touch.Id != Guid.Empty)
            {
                DataAPI.PostRelationship(new RelationshipPost() { entityType1 = EntityTypes.Touch, entityId1 = _touch.Id, entityType2 = EntityTypes.Tag, entityId2 = tagId });
            }
            return true;

        }

        #endregion

        #region Type & Sharing
        private void ddType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isInitialized)
            {
                ComboBox cb = (ComboBox)sender;
                UserType ut = (UserType)cb.SelectedItem;
                _touch.UserType_Id = ut.Id;
                _touch.changeType = ChangeType.Update;
                MakeDirty();
                Utilities.SetRegKey((_isEmail) ? RegKeys.TouchTypeMail : RegKeys.TouchTypeAppt, ut.Id.ToString());
            }
        }
        private void ddDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isInitialized)
            {
                ComboBox cb = (ComboBox)sender;
                Subscription sub = (Subscription)cb.SelectedItem;
                _touch.Collection_Id = sub.Group_Id;
                _touch.changeType = ChangeType.Update;
                MakeDirty();
                if (sub.Group_Id != Guid.Empty)
                {
                    Utilities.SetRegKey(RegKeys.DefaultDB, sub.Group_Id.ToString());
                    Group db = DataAPI.GetGroup(sub.Group_Id, false);
                    List<Group> editTeams = DataAPI.EditTeamsDB(_touch.Collection_Id);
                    //avoid firing off a lot of change events
                    _isInitialized = false;
                    SetEditTeams(editTeams, db.OwnedByGroup_Id);
                    Utilities.SetRegKey(RegKeys.DefaultOG, db.OwnedByGroup_Id.ToString());
                    _isInitialized = true;
                }

            }
        }

        private void ddOwnedByGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isInitialized)
            {
                ComboBox cb = (ComboBox)sender;
                Group tm = (Group)cb.SelectedItem;
                _touch.OwnedByGroup_Id = tm.Id;
                _touch.changeType = ChangeType.Update;
                Utilities.SetRegKey(RegKeys.DefaultOG, tm.Id.ToString());
                MakeDirty();
            }
        }

        private void SetEditTeams(List<Group> editTeams, Guid ogId)
        {
            var hasNone = editTeams.Find(t => t.Id == Guid.Empty);
            if (hasNone == null)
            {
                Group noTeam = new Group() { Name = "(none)", Id = Guid.Empty };
                editTeams.Add(noTeam);
            }

            ddOwnedByGroup.DataSource = editTeams;
            Group hit = editTeams.Find(t => t.Id == ogId);
            if (hit != null) ddOwnedByGroup.SelectedItem = hit;
            else if (editTeams.Count > 0)
            {
                hit = ddOwnedByGroup.Items[0] as Group;
                ddOwnedByGroup.SelectedItem = hit;
                _touch.OwnedByGroup_Id = hit.Id;
            }

        }

        #endregion

        #region Misc Events

        private void pbMap_Click(object sender, EventArgs e)
        {
            Location loc = new Location();
            Guid origId = Guid.Empty;
            string origAddress = "";
            if (_touch.Locations.Count > 0)
            {
                loc = _touch.Locations[0];
                origId = loc.Id;
                origAddress = loc.Address;
            }
            locationForm frm = new locationForm(loc);
            DialogResult result = frm.ShowDialog();
            if (result == DialogResult.OK)
            {
                loc = frm.TheLocation;
                lblLocation.Text = loc.Address;
                if (origAddress == "" || origAddress != loc.Address)
                {
                    loc.changeType = ChangeType.Update;
                    foreach (Location l in _touch.Locations)
                    {
                        l.changeType = ChangeType.Remove;
                        DataAPI.DeleteLocation(l.Id);
                    }

                    loc.changeType = ChangeType.Update;
                    loc.Id = DataAPI.PostLocation(loc);
                    DataAPI.PostRelationship(new RelationshipPost() { entityType1 = EntityTypes.Touch, entityId1 = _touch.Id, entityType2 = EntityTypes.Location, entityId2 = loc.Id });
                    _touch.Locations.Add(loc);

                    _touch.changeType = ChangeType.Update;
                }
                MakeDirty();
            }
        }

        private void cMenuClick(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip cMenu = sender as ContextMenuStrip;
            TreeView tv = (TreeView)cMenu.SourceControl;
            object obj = tv.SelectedNode.Tag;
            switch (e.ClickedItem.Name)
            {
                case "bRemove":
                    if (obj is Tag) RemoveTag(obj as Tag);
                    else RemoveRecipient(obj);
                    break;
                case "bView":
                    Process.Start(Utilities.AppLink(obj));
                    break;
            }
            tv.SelectedNode = null;
        }

        private void tv_MouseUp(object sender, MouseEventArgs e)
        {
            TreeView tv = (TreeView)sender;

            if (e.Button == MouseButtons.Right)
            {
                // Select the clicked node
                tv.SelectedNode = tv.GetNodeAt(e.X, e.Y);

                if (tv.SelectedNode != null)
                {
                    cMenu.Show(tv, e.Location);
                }
            }
        }
        private void tv_DoubleClick(object sender, EventArgs e)
        {
            TreeView tv = (TreeView)sender;
            if (tv.SelectedNode != null && tv.SelectedNode.Tag != null)
            {
                object obj = tv.SelectedNode.Tag;
                if (obj is Tag) RemoveTag(obj as Tag);
                else RemoveRecipient(obj);
                //tv.SelectedNode = null;
            }

        }
        #endregion

        #region Autocompletes
        int _pause = 400;
        int _noRecs = 20;
        int _minLength = 2;

        #region Recipients AC
        Timer _timer = null;
        bool _isSearching = false;
        private void _timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            if (String.IsNullOrWhiteSpace(txtRecipient.Text)) ResetACRecip(false);
            else
            {
                _isSearching = true;
                Cursor = Cursors.WaitCursor;
                GetACResultsRecip();
            }
        }



        private void txtRecipient_KeyUp(object sender, KeyEventArgs e)
        {
            //Debug.WriteLine(txtRecipient.Text);
            if (e.KeyCode == Keys.Escape) ResetACRecip(true);
            else if (txtRecipient.Text.Length == 1 && e.KeyCode == Keys.Back) ResetACRecip(false);
            else if (txtRecipient.Text.Length >= _minLength)
            {
                if (_isSearching)
                {
                    e.SuppressKeyPress = true;
                    return;
                }
                if (_timer == null)
                {
                    _timer = new Timer
                    {
                        Interval = _pause
                    };
                    _timer.Tick += _timer_Tick;
                }
                _timer.Stop();
                if ((e.KeyValue >= 64 && e.KeyValue <= 122)
                    || e.KeyCode == Keys.Back) _timer.Start();
                else if (e.KeyCode == Keys.Down && lbAComplete.Visible) lbAComplete.Focus();
            }
            else ResetACRecip(false);
        }

        private void txtRecipient_Leave(object sender, EventArgs e)
        {
            if (!lbAComplete.Focused) ResetACRecip(true);
        }

        private void lbAComplete_Click(object sender, EventArgs e)
        {
            if (lbAComplete.SelectedItem != null)
            {
                AutocompleteResult res = lbAComplete.SelectedItem as AutocompleteResult;
                RecipientSelected(res);
            }
        }

        private void lbAComplete_Leave(object sender, EventArgs e)
        {
            ResetACRecip(false);
        }

        private void lbAComplete_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up && lbAComplete.SelectedIndex == 0)
            {
                txtRecipient.Focus();
            }
            else if (e.KeyCode == Keys.Return) lbAComplete_Click(sender, e);

        }


        private bool RecipientSelected(AutocompleteResult res)
        {
            if (res != null)
            {
                if (res.EntityType == EntityTypes.Private)
                {
                    Group grp = DataAPI.GetGroup(res.Id, true);
                    foreach (Person p in grp.People)
                    {
                        AutocompleteResult per = new AutocompleteResult()
                        {
                            EntityType = EntityTypes.Person,
                            Id = p.Id,
                            Name = p.Name
                        };
                        if (p.ContactPoints.Count > 0 && Utilities.IsValidEmail(p.ContactPoints[0].Name))
                        {
                            AddRecipient(per, p.ContactPoints[0].Name);
                        }
                    }
                    foreach (Group o in grp.Groups)
                    {
                        AutocompleteResult org = new AutocompleteResult()
                        {
                            EntityType = EntityTypes.Organization,
                            Id = o.Id,
                            Name = o.Name
                        };
                        if (o.ContactPoints.Count > 0 && Utilities.IsValidEmail(o.ContactPoints[0].Name))
                        {
                            AddRecipient(org, o.ContactPoints[0].Name);
                        }
                    }
                }
                else
                {
                    string email = DataAPI.EmailForEntity(res.Id, res.EntityType);
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        MessageBox.Show("No email address on record for " + res.Name, "Get Email Address Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (email.Length >= 5 && email.Substring(0, 5).ToLower() == "error")
                    {
                        MessageBox.Show(email, "Get Email Address Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (email.IndexOf("@") > 0)
                    {
                        //add to appt
                        AddRecipient(res, email);
                    }
                }
            }
            ResetACRecip(true);
            PaintTouch();
            txtRecipient.Focus();

            return true;
        }

        private bool GetACResultsRecip()
        {
            List<AutocompleteResult> results = DataAPI.AutocompleteEmailRecipients(txtRecipient.Text, _noRecs);
            LoadLbAC(results);
            _isSearching = false;
            Cursor = Cursors.Default;
            return true;
        }

        private void LoadLbAC(List<AutocompleteResult> results)
        {
            lbAComplete.DataSource = results;
            lbAComplete.DisplayMember = "Name";
            lbAComplete.ValueMember = "Id";
            lbAComplete.Visible = txtRecipient.Enabled = true;

        }
        private void ResetACRecip(bool cleartext)
        {
            lbAComplete.Visible = false;
            if (cleartext) txtRecipient.Text = "";
            //txtRecipient.Focus();
        }

        #endregion

        #region Tags AC
        List<AutocompleteResult> _acResultsTag = new List<AutocompleteResult>();
        Timer _timerTag = null;
        bool _isSearchingTag = false;

        private void _timerTag_Tick(object sender, EventArgs e)
        {
            _timerTag.Stop();

            if (String.IsNullOrWhiteSpace(txtTag.Text)) ResetACTag(false);
            else
            {
                _isSearchingTag = true;
                Cursor = Cursors.WaitCursor;
                GetACResultsTag();
            }
        }

        private void txtTag_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) ResetACTag(true);
            else if (txtTag.Text.Length == 1 && e.KeyCode == Keys.Back) ResetACTag(false);
            else if (txtTag.Text.Length >= _minLength)
            {
                if (_isSearchingTag)
                {
                    e.SuppressKeyPress = true;
                    return;
                }
                if (_timerTag == null)
                {
                    _timerTag = new Timer
                    {
                        Interval = _pause
                    };
                    _timerTag.Tick += _timerTag_Tick;
                }
                _timerTag.Stop();
                if ((e.KeyValue >= 64 && e.KeyValue <= 122)
                    || e.KeyCode == Keys.Back) _timerTag.Start();
                else if (e.KeyCode == Keys.Down && lbACompleteTag.Visible) lbACompleteTag.Focus();
                else if (e.KeyCode == Keys.Return)
                {
                    //Is a result spelled out?  if not, add new Tag?
                    AutocompleteResult res = _acResultsTag.Find(r => r.Name.ToLower() == txtTag.Text.ToLower());
                    if (res != null) AddTag(res);
                    else AddNewTag();
                }
            }
            else ResetACTag(false);
        }

        private void txtTag_Leave(object sender, EventArgs e)
        {
            if (!lbACompleteTag.Focused) ResetACTag(true);
        }

        private void lbACompleteTag_Click(object sender, EventArgs e)
        {
            if (lbACompleteTag.SelectedItem != null)
            {
                AutocompleteResult res = lbACompleteTag.SelectedItem as AutocompleteResult;
                AddTag(res);
            }

        }

        private void lbACompleteTag_Leave(object sender, EventArgs e)
        {
            ResetACTag(false);
        }

        private void lbACompleteTag_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up && lbACompleteTag.SelectedIndex == 0)
            {
                txtTag.Focus();
            }
            else if (e.KeyCode == Keys.Return) lbACompleteTag_Click(sender, e);

        }

        private bool IsTagInTouch(Guid tagId)
        {
            return (_touch.Tags.Find(t => (t.Id == tagId && t.changeType != ChangeType.Remove)) != null);
        }

        private bool GetACResultsTag()
        {
            _acResultsTag = DataAPI.GetTagsAC(txtTag.Text);
            List<AutocompleteResult> results = _acResultsTag.FindAll(r => !(IsTagInTouch(r.Id)));
            LoadLbACTags(results);
            _isSearchingTag = false;
            Cursor = Cursors.Default;
            return true;
        }

        private void LoadLbACTags(List<AutocompleteResult> results)
        {
            lbACompleteTag.DataSource = results;
            lbACompleteTag.DisplayMember = "Name";
            lbACompleteTag.ValueMember = "Id";
            lbACompleteTag.Visible = txtTag.Enabled = true;
            lbACompleteTag.SelectedIndex = -1;

        }

        private void ResetACTag(bool cleartext)
        {
            lbACompleteTag.Visible = false;
            _acResultsTag.Clear();
            if (cleartext) txtTag.Text = "";
            //txtTag.Focus();
        }

        #endregion
        #endregion


        private void bOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            _touch.Id = DataAPI.PostTouch(_touch, true);
            var ty = (_isEmail) ? Outlook.OlItemType.olMailItem : Outlook.OlItemType.olAppointmentItem;
            Globals.ThisAddIn.RegisterTouchUpdate(_touch, ty);
            // Debug.WriteLine("Touch updated by details form.");
            this.DialogResult = DialogResult.Yes;
        }


        private void bCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

  
        private void MakeDirty()
        {
            _isDirty = true;
            PaintTouch();
        }

    }
}
