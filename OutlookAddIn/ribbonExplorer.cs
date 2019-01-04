using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace ShuriOutlookAddIn
{
    public partial class ribbonExplorer
    {

        private TimerForm _timerForm = null;
        private bool _isSynced = false;
        private Touch _touch = new Touch();
        private Outlook.MailItem _mail = null;
        private Outlook.AppointmentItem _appt = null;
        private Outlook.OlItemType _itemType = Outlook.OlItemType.olJournalItem;   //olJournalItem == NONE

        #region Initialization & UI
        private void Ribbon_Load(object sender, RibbonUIEventArgs e)
        {
            try
            {
                _timerForm = new TimerForm();
                _timerForm.Visible = false;
                _timerForm.timerCalRibbon.Interval = 1500;
                _timerForm.timerCalRibbon.Tick += Initialize;
                _timerForm.timerCalRibbon.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                Debug.WriteLine("Stacktrace: " + ex.StackTrace);
            }
        }

        private void Initialize(object sender, EventArgs e)
        {
            //Debug.WriteLine("Initialize {0:T}", DateTime.Now);
            SetUIWorking();
            if (DataAPI.Ready())
            {
                //Debug.WriteLine("Ready {0:T}", DateTime.Now);
                _timerForm.timerCalRibbon.Stop();
                //initial mail or appt item
                Outlook.Selection explSelection = Globals.ThisAddIn.Application.ActiveExplorer().Selection;
                if (explSelection != null && explSelection.Count > 0)
                {
                    if (explSelection[1] is Outlook.MailItem) SetMailItem(explSelection[1] as Outlook.MailItem);
                    else if (explSelection[1] is Outlook.AppointmentItem) SetApptItem(explSelection[1] as Outlook.AppointmentItem);
                }
                explSelection = null;

                if (DataAPI.Online) lblStatus.Label = Globals.ThisAddIn.SyncStatus;
                else lblStatus.Label = "";

            }
            SetUI();
        }


        public void SetUI()
        {
            if (!DataAPI.Online)
            {
                grpTouch.Visible = grpTouch1.Visible = grpTouch2.Visible = grpStatus.Visible = false;
                grpLogin.Visible = true;
            }
            else
            {
                grpTouch.Visible = grpTouch1.Visible = grpTouch2.Visible = grpStatus.Visible = true;
                grpLogin.Visible = false;

                if (_touch.Id != Guid.Empty)
                {
                    bOpenApp.Label = bOpenApp1.Label = bOpenApp2.Label = "Open Touch";
                    grpTouch1.Label = grpTouch2.Label = grpTouch.Label = "Synced";

                }
                else
                {
                    bOpenApp.Label = bOpenApp1.Label = bOpenApp2.Label = "Open App";
                    grpTouch.Label = grpTouch1.Label = grpTouch2.Label = "Shuri";
                }
                bOpenApp.Enabled = bOpenApp1.Enabled = bOpenApp2.Enabled = true;
                if (_itemType == Outlook.OlItemType.olJournalItem)
                {
                    bSync.Image = bSync1.Image = bSync2.Image = Properties.Resources.icon48alt;
                    bSync.Label = bSync1.Label = bSync2.ScreenTip = bSync1.ScreenTip = bSync2.Label = bSync.ScreenTip = "";
                    bSync.Enabled = bSync1.Enabled = bBreak.Visible = bBreak1.Visible = bSync2.Enabled = bBreak2.Visible = false;
                }
                else if (_itemType == Outlook.OlItemType.olAppointmentItem && _appt.IsRecurring)
                {
                    bSync.Image = bSync1.Image = bSync2.Image = Properties.Resources.icon48alt;
                    bSync.Label = bSync1.Label = bSync2.Label = "[Recurring]";
                    bSync.ScreenTip = bSync1.ScreenTip = bSync2.ScreenTip = "Recurring appointments may not be synced";
                    bSync.Enabled = bSync1.Enabled = bBreak.Visible = bBreak1.Visible = bSync2.Enabled = bBreak2.Visible = false;
                }
                else
                {
                    bSync.Image = bSync1.Image = bSync2.Image = (_isSynced ? Properties.Resources.chatTransGrn48 : Properties.Resources.icon48);
                    bSync.Label = bSync1.Label = bSync2.Label = (_isSynced ? "Sync Details" : "Sync");
                    bSync.ScreenTip = bSync1.ScreenTip = bSync2.ScreenTip = (_isSynced ? "Click to view Touch details" : "Sync as new Touch in Shuri");
                    bSync.Enabled = bSync1.Enabled = bSync2.Enabled = true;
                    bBreak.Visible = bBreak1.Visible = bBreak2.Visible = _isSynced;
                }

                //lblStatus.Visible = _showStatus;
                //bQuickSync.Label = (_showStatus) ? "Hide Status" : "Sync && Show";
                //bQuickSync.OfficeImageId = (_showStatus) ? "SkipOccurrence" : "RecurrenceEdit";
            }
            tabShuri.Label = (_isSynced) ? "Shuri*" : "Shuri";
        }

        private void SetUIWorking()
        {
            bSync.Image = bSync1.Image = Properties.Resources.icon48alt;
            bSync.Label = bSync1.Label = "Working...";
            bSync.Enabled = bOpenApp.Enabled = bSync1.Enabled = bOpenApp1.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
        }

        #endregion

        #region Public Properties & Methods  Explorer Reading Pane

        public bool IsEmail
        {
            get { return (_itemType == Outlook.OlItemType.olMailItem); }
        }
        public bool IsInitializing
        {
            get { return (_itemType == Outlook.OlItemType.olMailItem); }
        }

        public void ClearItem()
        {
            _mail = null;
            _appt = null;
            _isSynced = false;
            _itemType = Outlook.OlItemType.olJournalItem;
            _touch = new Touch();
            SetUI();
        }

        public void SetMailItem(Outlook.MailItem mail)
        {
            _mail = mail;
            _itemType = Outlook.OlItemType.olMailItem;
            RefreshTouch();
            SetUI();
        }


        public void SetApptItem(Outlook.AppointmentItem appt)
        {
            _appt = appt;
            _itemType = Outlook.OlItemType.olAppointmentItem;
            RefreshTouch();
            SetUI();
        }

        #endregion

        #region Events

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
                        if (IsEmail)
                        {
                            Globals.ThisAddIn.CompareAndSync(_touch, _mail);
                            details detailsForm = new details(_touch, _mail, true);
                            DialogResult result = detailsForm.ShowDialog();
                            detailsForm.Dispose();

                            if (result == DialogResult.Yes)
                            {
                                Globals.ThisAddIn.RegisterTouchUpdate(_touch, Outlook.OlItemType.olMailItem);
                                Globals.ThisAddIn.CompareAndSync(_touch, _mail);
                                //Globals.ThisAddIn.CaptureEml(_mail, _touch);
                                Globals.ThisAddIn.RegisterEmailWrite(_mail);
                                Globals.ThisAddIn.AddTouchId(_mail, _touch.Id);
                                //Globals.ThisAddIn.InitTheSync();
                                _isSynced = true;
                            }

                        }
                        else
                        {
                            Globals.ThisAddIn.CompareAndSync(_touch, _appt);
                            details detailsForm = new details(_touch, _appt, true);
                            DialogResult result = detailsForm.ShowDialog();
                            detailsForm.Dispose();

                            if (result == DialogResult.Yes)
                            {
                                Globals.ThisAddIn.RegisterTouchUpdate(_touch, Outlook.OlItemType.olAppointmentItem);
                                Globals.ThisAddIn.CompareAndSync(_touch, _appt);
                                Globals.ThisAddIn.RegisterApptWrite(_appt);
                                Globals.ThisAddIn.AddTouchId(_appt, _touch.Id);
                                //Globals.ThisAddIn.InitTheSync();
                                _isSynced = true;
                            }

                        }

                    }
                    else
                    {

                        if (_touch.Id != Guid.Empty) RefreshTouch();

                        if (IsEmail && _mail != null && _touch.Id == Guid.Empty) Globals.ThisAddIn.CompareAndSync(_touch, _mail);
                        else if (_appt != null && _touch.Id == Guid.Empty) Globals.ThisAddIn.CompareAndSync(_touch, _appt);
                        else if (_appt != null) Globals.ThisAddIn.ResolveRecipientsAppt(_touch, _appt);

                        details detsForm = null;
                        if (IsEmail) detsForm = new details(_touch, _mail, false);
                        else detsForm = new details(_touch, _appt, false);

                        DialogResult res = detsForm.ShowDialog();
                        detsForm.Dispose();

                        if (res == DialogResult.Yes)
                        {
                            if (IsEmail)
                            {
                                Globals.ThisAddIn.RegisterTouchUpdate(_touch, Outlook.OlItemType.olMailItem);
                                Globals.ThisAddIn.CompareAndSync(_touch, _mail);
                            }
                            else
                            {
                                Globals.ThisAddIn.RegisterTouchUpdate(_touch, Outlook.OlItemType.olAppointmentItem);
                                Globals.ThisAddIn.CompareAndSync(_touch, _appt);
                            }
                        }
                    }
                    SetUI();
                }
                catch (Exception ex)
                {
                    DataAPI.HandleError("RibbonExplorer : bSync", ex);
                    SetUI();
                }
            }
        }

        private void bBreakSync_Click(object sender, RibbonControlEventArgs e)
        {
            if (_touch.Id != Guid.Empty)
            {
                bool breakSync = true;

                //ask to delete app touch
                string msg = string.Format("Ready to break the sync.\n\nDelete the synced touch in the app, too?");
                DialogResult res = MessageBox.Show(msg, "Break Sync", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (res == DialogResult.Cancel)
                {
                    breakSync = false;
                    _isSynced = true;
                }
                else if (res == DialogResult.Yes)
                {
                    //delete touch
                    DataAPI.DeleteTouch(_touch.Id);
                }

                if (breakSync)
                {
                    if (IsEmail) Globals.ThisAddIn.UnsyncItem(_mail);
                    else Globals.ThisAddIn.UnsyncItem(_appt);
                    _touch = new Touch();
                    _isSynced = false;
                }
                SetUI();
            }
        }

        private void bOpenApp_Click(object sender, RibbonControlEventArgs e)
        {
            string url = Globals.ThisAddIn.APIEnv.BaseAppUrl;
            if (_touch.Id != Guid.Empty && _touch.Id != Guids.System) url += "#/home/touch/" + _touch.Id;
            System.Diagnostics.Process.Start(url);
        }

        private void bSettings_Click(object sender, RibbonControlEventArgs e)
        {
            // if (DataAPI.Online)
            // {
            using (SettingsForm settingsForm = new SettingsForm())
            {
                DialogResult res = settingsForm.ShowDialog();
                if (res == DialogResult.Cancel) ClearItem();  //user logged out
            }
            // }
        }

        private void Ribbon_Close(object sender, EventArgs e)
        {
            _mail = null;
            _appt = null;
            _touch = null;
            _timerForm = null;
        }

        //private void Item_Write(ref bool Cancel)
        //{
        //    if (Globals.ThisAddIn.Application.ActiveExplorer().Selection.Count > 0)
        //    {
        //        Guid tchId = Guid.Empty;
        //        var item = Globals.ThisAddIn.Application.ActiveExplorer().Selection[1];
        //        if (item != null)
        //        {
        //            Debug.WriteLine("Item_Write");
        //            if (item is Outlook.AppointmentItem || item is Outlook.MeetingItem)
        //            {
        //                Globals.ThisAddIn.ApptWrite(item as Outlook.AppointmentItem);
        //            }
        //            else if (item is Outlook.MailItem)
        //            {
        //                Globals.ThisAddIn.MailWrite(item as Outlook.MailItem);
        //            }
        //        }
        //    }
        //}

        #endregion

        #region Misc Methods

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
                    Globals.ThisAddIn.HandleError(ex, "Ribbon:RefreshTouch");
                }

            }
        }

        #endregion

        private void bSyncAll_Click(object sender, RibbonControlEventArgs e)
        {
            lblStatus.Label = "Syncing...";
            Globals.ThisAddIn.SyncDeep();


        }

        public void SyncAllComplete(string msg)
        {
            //incoming should have been created; all entryIDs should be valid now: check orphaned syncs
            List<Touch> syncTouches = DataAPI.SyncTouches();
            foreach (Touch tch in syncTouches)
            {
                bool deleteSyncItem = false;
                SyncItem si = Globals.ThisAddIn.GetSyncItem(tch);
                if (string.IsNullOrWhiteSpace(si.id)) deleteSyncItem = true;
                else
                {
                    try
                    {
                        var tester = Globals.ThisAddIn._nameSpace.GetItemFromID(si.id);//throws error on invalid entry id
                        Guid id = Globals.ThisAddIn.TouchId(tester);
                        if (id != tch.Id)
                        {
                            Globals.ThisAddIn.RemoveTouchId(tester);
                            Globals.ThisAddIn.AddTouchId(tester, tch.Id);
                        }
                    }
                    catch { deleteSyncItem = true; }
                }
                if (deleteSyncItem)
                {
                    Document doc = tch.Documents.Find(d => d.UserType_Id == Guids.Doc_CalSync);
                    if (doc != null) DataAPI.DeleteDocument(doc.Id);
                }
            }

            Globals.ThisAddIn.SyncStatus = null;
            lblStatus.Label = Globals.ThisAddIn.SyncStatus;
            MessageBox.Show(msg, "Full Sync Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void bQuickSync_click(object sender, RibbonControlEventArgs e)
        {
            QuickSync();
        }
        //private void bToggle_click(object sender, RibbonControlEventArgs e)
        //{
        //    _showStatus = !_showStatus;
        //    if (_showStatus)
        //    {
        //        QuickSync();
        //    }
        //    else
        //    {
        //        grpStatus.Label = "Syncing";
        //        SetUI();
        //   }

        //}
        private void QuickSync()
        {
            Cursor.Current = Cursors.WaitCursor;
            lblStatus.Label = "syncing...";
            Globals.ThisAddIn.SyncStatus = null;
            lblStatus.Label = Globals.ThisAddIn.SyncStatus;
            grpStatus.Label = "Status checked: " + DateTime.Now.ToString("dd-MMM hh:mm tt");
            SetUI();
            Cursor.Current = Cursors.Default;

        }

        private void bLogin_Click(object sender, RibbonControlEventArgs e)
        {
            if (!Globals.ThisAddIn.LoggingIn) DataAPI.Login(false);
        }
    }
}
