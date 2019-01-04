using System;
using Microsoft.Office.Tools.Ribbon;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Threading;
using Office = Microsoft.Office.Core;

namespace ShuriOutlookAddIn
{
    public partial class ribbonInspector
    {
        private TimerForm _timerForm = null;
        private bool _isSynced = false;
        private bool _isDirty = false;
        private Touch _touch = null;
        private Outlook.MailItem _mail = null;
        private Outlook.AppointmentItem _appt = null;
        private Outlook.OlItemType _itemType = Outlook.OlItemType.olJournalItem;   //olJournalItem == NONE


        #region Initialization & UI
        private void Ribbon_Load(object sender, RibbonUIEventArgs e)
        {
            Start_Timer();
        }

        private void Initialize(object sender, EventArgs e)
        {
            //Globals.ThisAddIn.IsInspectorOpen = true;

            //Debug.WriteLine("Initialize {0:T}", DateTime.Now);
            if (DataAPI.Ready())
            {
                _timerForm.timerApptRibbon.Stop();

                Outlook.Inspector insp = this.Context as Outlook.Inspector;
                if (insp.CurrentItem is Outlook.MailItem)
                {
                    _mail = insp.CurrentItem;
                    _mail.BeforeDelete += new Outlook.ItemEvents_10_BeforeDeleteEventHandler(Item_BeforeDelete);
                    _itemType = Outlook.OlItemType.olMailItem;
                }
                else if (insp.CurrentItem is Outlook.AppointmentItem)
                {
                    _appt = insp.CurrentItem;
                    _appt.BeforeDelete += new Outlook.ItemEvents_10_BeforeDeleteEventHandler(Item_BeforeDelete);
                    _appt.PropertyChange += _appt_PropertyChange;
                    _itemType = Outlook.OlItemType.olAppointmentItem;
                }
                insp = null;
                RefreshTouch();
                SetUI();
            }
            else _timerForm.timerApptRibbon.Interval = (_timerForm.timerApptRibbon.Interval * 2);

        }

        private void _appt_PropertyChange(string Name)
        {
            //Debug.WriteLine("prop Change {0}", Name);
            switch (Name)
            {
                case "RequiredAttendees":
                case "OptionalAttendees":
                case "Location":
                case "Subject":
                case "Start":
                case "End":
                    _isDirty = true;
                    break;
            }
        }

        public void SetUI()
        {
            if (!DataAPI.Online)
            {
                bSync.Image = bSync1.Image = bSync2.Image = Properties.Resources.icon48alt;
                bSync.Label = bSync1.Label = bSync2.Label = "Offline";
                bSync.ScreenTip = bSync1.ScreenTip = bSync2.ScreenTip = "Offline";
                bBreak.Visible = bLoc.Visible = bBreak1.Visible = bBreak2.Visible = false;
                bOpenApp.Visible = bOpenApp1.Visible = bOpenApp2.Visible = false;
                bLoc.Enabled = bSync.Enabled = bOpenApp.Enabled = bSync1.Enabled = bOpenApp1.Enabled = bSync2.Enabled = bOpenApp2.Enabled = false;
            }
            else if (_itemType == Outlook.OlItemType.olAppointmentItem && _appt.IsRecurring)
            {
                bSync.Image = bSync1.Image = bSync2.Image = Properties.Resources.icon48alt;
                bSync.Label = bSync1.Label = bSync2.Label = "[Recurring]";
                bSync.ScreenTip = bSync1.ScreenTip = bSync2.ScreenTip = "Recurring appointments may not be synced";
                bLoc.Enabled = bSync.Enabled = bSync1.Enabled = bBreak.Visible = bBreak1.Visible = bBreak2.Visible = bSync2.Enabled = false;
            }
            else
            {
                bSync.Image = bSync1.Image = bSync2.Image = (_isSynced ? Properties.Resources.chatTransGrn48 : Properties.Resources.icon48);
                bSync.Label = bSync1.Label = bSync2.Label = (_isSynced ? "Sync Details" : "Sync");
                bSync.ScreenTip = bSync1.ScreenTip = bSync2.ScreenTip = (_isSynced ? "Click to view touch details" : "Sync as new touch in Shuri");
                bBreak.Visible = bLoc.Visible = bBreak1.Visible = bBreak2.Visible = _isSynced;
                bOpenApp.Visible = bOpenApp1.Visible = bOpenApp2.Visible = (_isSynced && _touch.Id != Guid.Empty);
                bLoc.Enabled = bSync.Enabled = bOpenApp.Enabled = bSync1.Enabled = bOpenApp1.Enabled = bSync2.Enabled = bOpenApp2.Enabled = true;
            }
            Cursor.Current = Cursors.Default;

        }

        private void SetUIWorking()
        {
            bSync.Image = bSync1.Image = bSync2.Image = Properties.Resources.icon48alt;
            bSync.Label = bSync1.Label = bSync2.Label = "Working...";
            bLoc.Enabled = bSync.Enabled = bOpenApp.Enabled = bSync1.Enabled = bOpenApp1.Enabled = bSync2.Enabled = bOpenApp2.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
        }
        #endregion

        #region Public Properties & Methods

        public bool IsEmail
        {
            get { return _itemType == Outlook.OlItemType.olMailItem; }
        }

        #endregion
        private void RefreshTouch()
        {
            _isSynced = false;
            _touch = new Touch();
            if (_itemType != Outlook.OlItemType.olJournalItem)
            {
                try
                {
                    if (IsEmail) _touch = Globals.ThisAddIn.GetTouchForItem(_mail);
                    else _touch = Globals.ThisAddIn.GetTouchForItem(_appt);

                    if (_touch.Id != Guid.Empty)
                    {
                        //is still synced?
                        var doc = _touch.Documents.Find(d => d.UserType_Id == Guids.Doc_CalSync);
                        _isSynced = (doc != null);
                        if (_isSynced)
                        {
                            if (IsEmail)
                            {
                                if (!_mail.Sent) Globals.ThisAddIn.CompareAndSync(_touch, _mail);
                                Globals.ThisAddIn.RegisterEmailWrite(_mail);
                            }
                            else
                            {
                                Globals.ThisAddIn.CompareAndSync(_touch, _appt);
                                Globals.ThisAddIn.RegisterApptWrite(_appt);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Globals.ThisAddIn.HandleError(ex, "ribbonInspector:RefreshTouch");
                }

            }
        }

        #region Events 

        private void Ribbon_Close(object sender, EventArgs e)
        {
            _mail = null;
            _appt = null;
            _touch = null;
            _timerForm = null;
        }
        private void bOpenApp1_Click(object sender, RibbonControlEventArgs e)
        {
            string url = Globals.ThisAddIn.APIEnv.BaseAppUrl + "#/";
            if (_touch.Id != Guid.Empty) url += "home/touch/" + _touch.Id;
            System.Diagnostics.Process.Start(url);
        }

        private void bLoc_Click(object sender, RibbonControlEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Location loc = new Location();
            Guid origId = Guid.Empty;
            string origAddress = "";
            if (_appt != null)
            {
                if (_touch.Locations.Count == 0)
                {
                    loc = new Location()
                    {
                        UserType_Id = Guids.Loc_Business,
                        Address = _appt.Location,
                        changeType = ChangeType.Update,
                        Collection_Id = _touch.Collection_Id,
                        OwnedByGroup_Id = _touch.OwnedByGroup_Id,
                        OwnedBy_Id = _touch.OwnedBy_Id
                    };
                }
                else
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
                    if (origAddress == "" || origAddress != loc.Address)
                    {
                        //ReplaceLocation(origId, loc);
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

                        _appt.Location = loc.Address;
                    }
                    //SyncApptDetails();
                }
            }
        }



        private void bSync_Click(object sender, RibbonControlEventArgs e)
        {
            if (_itemType != Outlook.OlItemType.olJournalItem)
            {
                SetUIWorking();
                try
                {
                    if (!_isSynced)
                    {
                        _touch = Globals.ThisAddIn.NewTouch(_itemType);
                        //present details as dialog since this is new 
                        details detailsForm = null;
                        if (IsEmail)
                        {
                            Globals.ThisAddIn.CompareAndSync(_touch, _mail);
                            detailsForm = new details(_touch, _mail, true);
                        }
                        else
                        {
                            //check Recurrance
                            if (_appt.IsRecurring)
                            {
                                MessageBox.Show("Recurring appointments may not be synced.", "Shuri App", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                SetUI();
                                return;
                            }
                            Globals.ThisAddIn.CompareAndSync(_touch, _appt);
                            detailsForm = new details(_touch, _appt, true);
                        }


                        DialogResult result = detailsForm.ShowDialog();
                        detailsForm.Dispose();

                        if (result == DialogResult.Yes)
                        {
                            Globals.ThisAddIn.RegisterTouchUpdate(_touch, (IsEmail) ? Outlook.OlItemType.olMailItem : Outlook.OlItemType.olAppointmentItem);
                            if (IsEmail)
                            {
                                Globals.ThisAddIn.CompareAndSync(_touch, _mail);
                                //Globals.ThisAddIn.CaptureEml(_mail, _touch);
                                Globals.ThisAddIn.RegisterEmailWrite(_mail);
                                Globals.ThisAddIn.AddTouchId(_mail, _touch.Id);
                            }
                            else
                            {
                                Globals.ThisAddIn.CompareAndSync(_touch, _appt);
                                Globals.ThisAddIn.RegisterApptWrite(_appt);
                                Globals.ThisAddIn.AddTouchId(_appt, _touch.Id);
                            }
                            _isSynced = true;
                        }
                    }
                    else
                    {
                        if (_touch.Id != Guid.Empty)
                        {
                            RefreshTouch();
                            if (_isDirty) Globals.ThisAddIn.RegisterOLItemUpdate(_touch, (IsEmail) ? _mail.EntryID : _appt.EntryID);
                        }

                        if (IsEmail && _mail != null && (_touch.Id == Guid.Empty || _isDirty)) Globals.ThisAddIn.CompareAndSync(_touch, _mail);
                        else if (_appt != null && (_touch.Id == Guid.Empty || _isDirty)) Globals.ThisAddIn.CompareAndSync(_touch, _appt);
                        else if (_appt != null) Globals.ThisAddIn.ResolveRecipientsAppt(_touch, _appt);
                        _isDirty = false;

                        details detsForm = null;
                        if (IsEmail) detsForm = new details(_touch, _mail, false);
                        else detsForm = new details(_touch, _appt, false);

                        DialogResult res = detsForm.ShowDialog();
                        detsForm.Dispose();

                        if (res == DialogResult.Yes)
                        {
                            if (IsEmail) Globals.ThisAddIn.CompareAndSync(_touch, _mail);
                            else Globals.ThisAddIn.CompareAndSync(_touch, _appt);
                        }
                    }
                    SetUI();
                }
                catch (Exception ex)
                {
                    DataAPI.HandleError("RibbonInspector : bSync", ex);
                    SetUI();
                }
            }
        }


        private void bBreakSync_Click(object sender, RibbonControlEventArgs e)
        {
            if (_touch.Id != Guid.Empty)
            {
                //ask to delete app touch
                string msg = string.Format("Ready to break the sync.\n\nDelete the synced touch in the app, too?");
                DialogResult res = MessageBox.Show(msg, "Break Sync", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                _isSynced = false;
                if (res == DialogResult.Cancel)
                {
                    _isSynced = true;
                }
                else if (res == DialogResult.Yes)
                {
                    //delete touch
                    DataAPI.DeleteTouch(_touch.Id);
                }

                if (!_isSynced)
                {
                    if (IsEmail) BreakSync(_mail);
                    else BreakSync(_appt);
                }

                SetUI();
            }
        }

        private void Item_BeforeDelete(object Item, ref bool Cancel)
        {
            if (Item is Outlook.MailItem) BreakSync(Item as Outlook.MailItem);
            else if (Item is Outlook.AppointmentItem) BreakSync(Item as Outlook.AppointmentItem);
            else if (Item is Outlook.MeetingItem) BreakSync(Item as Outlook.AppointmentItem);
        }
        #endregion
        private void BreakSync(object item)
        {
            Globals.ThisAddIn.UnsyncItem(item);
            _touch = new Touch();

        }

        #region Helper methods
        private void Start_Timer()
        {
            try
            {
                _timerForm = new TimerForm();
                _timerForm.Visible = false;
                _timerForm.timerApptRibbon.Interval = 300;
                _timerForm.timerApptRibbon.Tick += Initialize;
                _timerForm.timerApptRibbon.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                Debug.WriteLine("Stacktrace: " + ex.StackTrace);
            }


        }


        #endregion

    }
}
