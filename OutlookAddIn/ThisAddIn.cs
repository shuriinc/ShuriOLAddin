#region Header

/*
 * Slovak Technical Services, Inc.
 * Ken Slovak
 * 5/18/17
 */

#endregion

using System;
using System.Collections.Generic;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace ShuriOutlookAddIn
{
    public partial class ThisAddIn
    {
        #region globals
        internal Outlook.NameSpace _nameSpace = null;
        internal Outlook.Inspectors _inspectors = null;
        internal Outlook.Explorers _explorers = null;
        internal Outlook.Explorer _explorer = null;
        internal Outlook.Folder _folder = null;
        internal Outlook.MAPIFolder _deletedFolder = null;
        internal Outlook.MAPIFolder _draftsFolder = null;
        private List<Outlook.Items> _outlookItems = null;
        private List<Outlook.AppointmentItem> _syncedAppts = new List<Outlook.AppointmentItem>();
        private List<Outlook.MailItem> _syncedMails = new List<Outlook.MailItem>();
        private List<Document> _pendingRemoves = null;


        private Boolean? _enabled = null;
        private bool _loggingIn = false;
        private bool _initializing = false;
        private bool _syncing = false;
        private string _syncStatus = null;
        private TimerForm _timerForm = null;
        private string _lastSelectedEntryID = "";
        internal string _tracker = "[ShuriTracker:{0}]";
        internal int _trackPrefixLen = 14;
        private string _trackerHtmlWrapper = "<br /><br /><br /><div style='color:#aaaaaa'>{0}</div>";

        //private DelayWriteObj _delayWriteObj = new DelayWriteObj();

        private ShuriEnvironment _environment = null;
        public List<ShuriEnvironment> Environments = new List<ShuriEnvironment>()
        {
            new ShuriEnvironment("Production", "https://api.shuri.com/api/", "7C8CCCB1-6AC7-4F77-B599-ADA8E1558C6F", "https://app.shuri.com/", "http://www.shuri.com/"),
            new ShuriEnvironment("Staging", "https://apistage.shuri.com/api/","DDA2F592-1C6C-43DD-8FE3-EC4F22AD7C16", "https://appstage.shuri.com/", "http://wwwstage.shuri.com/"),
            new ShuriEnvironment("Development", "http://localhost:64000/api/","262835E3-D190-476E-861C-AF192269ECEF", "http://localhost:8080/", "http://localhost:8181/"),
        };

        #endregion

        #region Addin Properties & Public methods

        public ShuriEnvironment APIEnv
        {
            get
            {
                if (_environment == null)
                {
                    _environment = Environments.First();
                    string envName = Utilities.ReadRegStringValue("Environment");
                    if (!String.IsNullOrEmpty(envName))
                    {
                        foreach (ShuriEnvironment se in Environments)
                        {
                            if (se.Name == envName) _environment = se;
                        }
                    }
                }
                return _environment;
            }
            set
            {
                _environment = value;
                if (_environment != null) Utilities.SetRegistryValue(Properties.Resources.RegistryPath, "Environment", _environment.Name, Microsoft.Win32.RegistryValueKind.String);

            }
        }

        public bool Enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = true;
                    string strEnabled = Utilities.ReadRegStringValue("Enabled");
                    if (!String.IsNullOrEmpty(strEnabled))
                    {
                        _enabled = Convert.ToBoolean(strEnabled);
                    }
                }
                return (bool)_enabled;

            }
            set
            {
                _enabled = value;
                Utilities.SetRegistryValue(Properties.Resources.RegistryPath, "Enabled", _enabled.ToString(), Microsoft.Win32.RegistryValueKind.String);
                RefreshUI();

            }
        }
        //public bool InitializingItem
        //{
        //    get
        //    {
        //        return _initializingItem;

        //    }
        //    set
        //    {
        //        _initializingItem = value;
        //    }
        //}

        //public bool IsInspectorOpen
        //{
        //    get
        //    {
        //        return _isInspectorOpen;
        //    }
        //    set
        //    {
        //        _isInspectorOpen = value;
        //        //Debug.WriteLine("Set isInspectorOpen to: " + _isInspectorOpen.ToString());
        //    }
        //}

        public bool LoggingIn
        {
            get
            {
                return _loggingIn;

            }
            set
            {
                _loggingIn = value;
            }
        }

        public void RefreshUI()
        {
            foreach (var rib in Globals.Ribbons)
            {
                if (rib.GetType() == typeof(ribbonExplorer))
                {
                    //string s = "";
                    ((ribbonExplorer)rib).SetUI();
                }
                if (rib.GetType() == typeof(ribbonInspector))
                {
                    //string s = "";
                    ((ribbonInspector)rib).SetUI();
                }
            }

        }

        //public List<Outlook.AppointmentItem> SyncedAppts
        //{
        //    get {
        //        if (_syncedAppts != null) return _syncedAppts;
        //        else
        //        {

        //        }
        //    }
        //    set { _syncedAppts = value; }
        //}
        //public List<Outlook.MailItem> SyncedMails
        //{
        //    get { return _syncedMails; }
        //    set { _syncedMails = value; }
        //}

        #endregion

        #region Add-in Events
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            try
            {

                //InitializeAddin();
                // Delay the initialization with a timer -------------------------------------------------
                // Cannot use background threading with Outlook object model.
                _timerForm = new TimerForm();
                _timerForm.Visible = false;
                _timerForm.timer1.Interval = 200;
                _timerForm.timer1.Tick += InitializeAddin;
                _timerForm.timer1.Start();
            }
            catch (Exception ex)
            {
                HandleError(ex, "ThisAddIn_Startup");

            }
        }

        private void InitializeAddin(object sender, EventArgs e)
        {
            try
            {
                _timerForm.timer1.Stop();

                _nameSpace = Application.GetNamespace("MAPI");
                // add shutdown handler
                ((Outlook.ApplicationEvents_11_Event)Application).Quit += new Outlook.ApplicationEvents_11_QuitEventHandler(AppQuit);

                Utilities.WriteDoNotDisableKeyToRegistry();

                // we'll need this for each open Inspector to add to List of Inspectors. Add Item.BeforeDelete() handler for open items to handle deletions in non-default Store.
                // Will also need to add Item.Delete handler for items that are selected in non-default Stores. Add when Explorer.SelectionChange() fires for selected appointments.
                _inspectors = Application.Inspectors;
                _inspectors.NewInspector += _inspectors_NewInspector;

                _explorers = Application.Explorers;

                if (_explorers.Count > 0)
                {
                    _explorer = _explorers[1];
                    _explorer.BeforeFolderSwitch += new Outlook.ExplorerEvents_10_BeforeFolderSwitchEventHandler(_explorer_BeforeFolderSwitch);
                    _explorer.SelectionChange += new Outlook.ExplorerEvents_10_SelectionChangeEventHandler(_explorer_SelectionChange);
                    _folder = _explorer.CurrentFolder as Outlook.Folder;

                    if (_folder.DefaultItemType == Outlook.OlItemType.olAppointmentItem || _folder.DefaultItemType == Outlook.OlItemType.olMailItem)
                    {
                        _folder.BeforeItemMove += _folder_BeforeItemMove;
                    }
                }
                _deletedFolder = _nameSpace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderDeletedItems);

                // add unhandled exception handlers
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                System.Windows.Forms.Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

                // global advanced search
                Application.AdvancedSearchComplete += Application_AdvancedSearchComplete;


                bool result = InitializeAddinData(sender, e);
            }
            catch (Exception ex)
            {
                HandleError(ex, "InitializeAddin");
            }
        }

        private bool InitializeAddinData(object sender, EventArgs e)
        {
            bool result = false;
            if (!_initializing)
            {
                try
                {
                    _initializing = true;
                    if (Enabled)
                    {
                        DataAPI.Login(true);
                        if (DataAPI.Ready())
                        {
                            DataAPI.SetUser();

                            InitTheSync();

                            //todo add preference check - default = NO
                            // SyncAll(false);

                            //bool isCurrent = CurrentVersion();                          
                        }
                    }
                    result = true;
                    _initializing = false;
                }
                catch (Exception ex)
                {
                    HandleError(ex, "InitializeAddinData");
                }
            }
            return result;
        }

        public bool InitTheSync()
        {
            try
            {
                //Debug.WriteLine("Start: " + DateTime.Now.ToString("yyyyMMddHHmmss"));
                _outlookItems = new List<Outlook.Items>();
                //post calendars user pref
                List<SyncCalendar> allCalendars = new List<SyncCalendar>();

                List<Outlook.MAPIFolder> allFolders = new List<Outlook.MAPIFolder>();
                foreach (Outlook.MAPIFolder folder in _nameSpace.Folders)
                {
                    allFolders.AddRange(GetSubFolders(folder));
                }

                foreach (Outlook.MAPIFolder mapiFolder in allFolders)
                {
                    switch (mapiFolder.DefaultItemType)
                    {
                        case Outlook.OlItemType.olAppointmentItem:
                            allCalendars.Add(new SyncCalendar() { id = mapiFolder.EntryID, name = mapiFolder.Name, storeId = mapiFolder.StoreID });
                            break;
                        case Outlook.OlItemType.olMailItem:
                            if (mapiFolder.Name.ToLower().IndexOf("inbox") > -1)
                            {
                                Outlook.Items items = mapiFolder.Items;
                                items.ItemAdd -= new Outlook.ItemsEvents_ItemAddEventHandler(NewItemCreated);
                                items.ItemAdd += new Outlook.ItemsEvents_ItemAddEventHandler(NewItemCreated);
                                _outlookItems.Add(items);
                            }
                            break;
                    }
                }

                string json = JsonConvert.SerializeObject(allCalendars);
                DataAPI.PostUserPreference("calendars", json);
                DataAPI.PostUserPreference("calsync", "outlook");   //turns it on
                //Debug.WriteLine(string.Format("InitTheSync complete.  {0:G}", DateTime.Now));
                //DataAPI.PostAudit("Addin user: " + DataAPI.TheUser.Name, string.Format("Initialize complete.  {0:U}", DateTime.UtcNow));
                allFolders = null;

                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex, "InitTheSync");
                return false;
            }

        }

        private object CurrentItem
        {
            get
            {
                if (Application.ActiveWindow() is Outlook.Explorer && Application.ActiveExplorer().Selection.Count > 0) return Application.ActiveExplorer().Selection[1];
                else if ((Application.ActiveWindow() is Outlook.Inspector)) return Application.ActiveInspector().CurrentItem;
                else return null;
            }
        }

        private void Item_Write(ref bool Cancel)
        {
            if (!_syncing)
            {
                var item = CurrentItem;
                if (item != null)
                {
                    if (item is Outlook.AppointmentItem || item is Outlook.MeetingItem)
                    {
                        ApptWrite(item as Outlook.AppointmentItem);
                    }
                    else if (item is Outlook.MailItem)
                    {
                        MailWrite(item as Outlook.MailItem);
                    }
                }
            }
        }

        public void ApptWrite(Outlook.AppointmentItem appt)
        {
            if (!_syncing)
            {
                Touch tch = GetTouchForItem(appt);
                if (tch.Id != Guid.Empty)
                {
                    RegisterOLItemUpdate(tch, appt.EntryID);
                    CompareAndSync(tch, appt);
                    Debug.WriteLine("AppointmentItem write");
                }
            }
        }
        public void MailWrite(Outlook.MailItem mail)
        {
            if (!_syncing)
            {
                Touch tch = GetTouchForItem(mail);
                if (tch.Id != Guid.Empty)
                {
                    RegisterOLItemUpdate(tch, mail.EntryID);
                    CompareAndSync(tch, mail);
                    Debug.WriteLine("MailItem write");
                }
            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see https://go.microsoft.com/fwlink/?LinkId=506785
        }

        private void AppQuit()
        {
            try
            {
                if (_timerForm != null)
                {
                    _timerForm.timer1.Stop();
                }

                _enabled = null;
                _loggingIn = false;
                _initializing = false;
                _environment = null;
                _outlookItems = null;
                _deletedFolder = null;
                _inspectors = null;
                _folder = null;
                _explorer = null;
                _explorers = null;
                _nameSpace = null;
            }
            catch (Exception ex)
            {
                HandleError(ex, "ThisAddIn_Shutdown");
            }
        }
        #endregion
        private void _explorer_SelectionChange()
        {
            if (Application.ActiveExplorer().Selection != null && Application.ActiveExplorer().Selection.Count > 0)
            {
                Outlook.MailItem mail = null;
                Outlook.AppointmentItem appt = null;
                string entryID = "";

                if (Application.ActiveExplorer().Selection[1] is Outlook.MailItem)
                {
                    mail = Application.ActiveExplorer().Selection[1] as Outlook.MailItem;
                    entryID = mail.EntryID;
                }
                else if (Application.ActiveExplorer().Selection[1] is Outlook.AppointmentItem)
                {
                    appt = Application.ActiveExplorer().Selection[1] as Outlook.AppointmentItem;
                    entryID = appt.EntryID;
                }

                //Debug.WriteLine(string.Format("_explorer_SelectionChange {0} : {1}\n\n", entryID, _lastSelectedEntryID));

                if (entryID != _lastSelectedEntryID)
                {
                    _lastSelectedEntryID = entryID;

                    if (mail != null)
                    {
                        //Debug.WriteLine("Mail");
                        foreach (var rib in Globals.Ribbons)
                        {
                            if (rib.GetType() == typeof(ribbonExplorer))
                            {
                                ((ribbonExplorer)rib).SetMailItem(mail);
                            }
                        }
                    }
                    else if (appt != null)
                    {
                        //Debug.WriteLine("Appt");
                        foreach (var rib in Globals.Ribbons)
                        {
                            if (rib.GetType() == typeof(ribbonExplorer))
                            {
                                ((ribbonExplorer)rib).SetApptItem(appt);
                            }
                        }
                    }
                    else
                    {
                        //Debug.WriteLine("Else");
                        foreach (var rib in Globals.Ribbons)
                        {
                            if (rib.GetType() == typeof(ribbonExplorer))
                            {
                                ((ribbonExplorer)rib).ClearItem();
                            }
                        }
                    }
                }

                mail = null;
                appt = null;
            }
            else
            {
                //Debug.WriteLine("No Selection");
                foreach (var rib in Globals.Ribbons)
                {
                    if (rib.GetType() == typeof(ribbonExplorer))
                    {
                        ((ribbonExplorer)rib).ClearItem();
                    }
                }
                _lastSelectedEntryID = "";
            }

        }

        private void NewItemCreated(object oItem)
        {
            if (oItem == null) return;
            Touch tch = null;

            if (oItem is Outlook.MailItem)
            {
                Outlook.MailItem mail = (Outlook.MailItem)oItem;

                Outlook.Conversation conv = null;
                try { conv = mail.GetConversation(); }
                catch { Debug.WriteLine("Email with no conversation"); }
                if (conv != null)
                {
                    var items = conv.GetRootItems();
                    foreach (var item in items)
                    {
                        if (item is Outlook.MailItem)
                        {
                            tch = GetTouchForItem(((Outlook.MailItem)item));
                            if (tch.Id != Guid.Empty) break;
                        }
                    }
                }

                if (tch != null && tch.Id != Guid.Empty)
                {
                    //Debug.WriteLine("Conversation posted: " + touchId.ToString());
                    DateTime dt = DateTime.UtcNow;
                    if (mail.ReceivedTime != null) dt = mail.ReceivedTime;
                    string sender = string.IsNullOrWhiteSpace(mail.SenderName) ? mail.SenderEmailAddress : mail.SenderName;
                    string friendlyName = string.Format("Reply-{0:yy-mm-dd}-{1}.eml", dt, sender);
                    string filename = mail.EntryID + ".eml";
                    tch.changeType = ChangeType.Update;
                    tch.Description = string.Format("---- Reply from {0} {1:d} ----\n{2}\n--------\n\n{3}", mail.SenderName, dt, mail.Body, tch.Description);

                    //CaptureEml(mail, tch);

                    #region Add sender?
                    List<AutocompleteResult> resolves = DataAPI.ResolveEmail(mail.SenderEmailAddress);
                    if (resolves.Count == 1)
                    {
                        AutocompleteResult entity = resolves[0];
                        if (entity.EntityType == EntityTypes.Person)
                        {
                            Person per = new Person()
                            {
                                Id = entity.Id,
                                changeType = ChangeType.Update,
                                Name = mail.SenderName
                            };
                            tch.People.Add(per);
                        }
                        else if (entity.EntityType == EntityTypes.Organization || entity.EntityType == EntityTypes.Group)
                        {
                            Group org = new Group()
                            {
                                Id = entity.Id,
                                changeType = ChangeType.Update,
                                GrpType = GroupType.Organization,
                                Name = mail.SenderName
                            };
                            tch.Groups.Add(org);
                        }
                    }
                    #endregion

                    RefreshAttachmentsTouchFromMail(tch, mail);
                    DataAPI.PostTouch(tch, true);
                    RegisterSync(tch, mail);

                }
                else
                {
                    // Debug.WriteLine("Not ours");

                }
                mail = null;
                conv = null;

            }
        }

        public void RegisterApptWrite(Outlook.AppointmentItem appt)
        {
            if (appt != null && !(string.IsNullOrWhiteSpace(appt.EntryID)))
            {
                appt.Write -= Item_Write;
                appt.Write += Item_Write;
                _syncedAppts.Add(appt);
                //var exist = _syncedAppts.Find(a => a.EntryID == appt.EntryID);
                //if (exist == null)
                //{
                //}
            }
        }
        public void RegisterEmailWrite(Outlook.MailItem mail)
        {
            if (mail != null && !(string.IsNullOrWhiteSpace(mail.EntryID)))
            {
                var exist = _syncedMails.Find(a => a.EntryID == mail.EntryID);
                if (exist == null)
                {
                    mail.Write -= Item_Write;
                    mail.Write += Item_Write;
                    _syncedMails.Add(mail);
                }
            }
        }

        internal Outlook.AppointmentItem ApptForTouchId(Guid touchId)
        {
            Outlook.AppointmentItem appt = null;
            string finderFilter = string.Format("[{0}] = '{1}'", Properties.Resources.ShuriTouchID, touchId);
            Debug.WriteLine("finderFilter: " + finderFilter);
            Debug.WriteLine("_outlookItems: " + _outlookItems.Count);
            foreach (Outlook.Items items in _outlookItems)
            {
                Debug.WriteLine("items: " + items.Count);
                appt = items.Find(finderFilter);
                if (appt != null)
                {
                    break;
                }
            }
            return appt;
        }
        internal Outlook.MailItem MailForTouchId(Guid touchId)
        {
            Outlook.MailItem mail = null;
            string finderFilter = string.Format("[{0}] = '{1}'", Properties.Resources.ShuriTouchID, touchId);

            foreach (Outlook.Items items in _outlookItems)
            {
                mail = items.Find(finderFilter);
                if (mail != null) break;
            }
            return mail;
        }
        #region Sync
        //internal string SyncAll(bool statusOnly)
        //{
        //    string status = "";
        //    string tchSynced = "TouchSynced";
        //    int cntMailOrphan = 0, cntMailNew = 0, cntApptNew = 0, cntApptOrphan = 0, cntSyncAppt = 0, cntOutAppt = 0, cntSyncMail = 0, cntOutMail = 0;
        //    //System.Threading.Thread.Sleep(1000);
        //    try
        //    {
        //        List<Touch> syncedTouches = DataAPI.SyncTouches();

        //        foreach (Outlook.AppointmentItem appt in _syncedAppts)
        //        {
        //            Guid touchId = TouchId(appt);
        //            if (touchId == Guid.Empty) status += "\nError:  A synced appt has no touchId: " + appt.Subject;
        //            else
        //            {
        //                Touch tch = syncedTouches.Find(t => t.Id == touchId);
        //                if (tch == null)
        //                {
        //                    cntApptOrphan++;
        //                    if (!statusOnly) RemoveTouchId(appt);
        //                }
        //                else
        //                {
        //                    tch.Typename = tchSynced;
        //                    bool needsSync = CompareAndSync(tch, appt, statusOnly);
        //                    if (needsSync)
        //                    {
        //                        cntOutAppt++;
        //                    }
        //                    else cntSyncAppt++;
        //                }
        //            }
        //        }

        //        foreach (Outlook.MailItem mail in _syncedMails)
        //        {
        //            Guid touchId = TouchId(mail);
        //            if (touchId == Guid.Empty)
        //            {
        //                Debug.WriteLine("\nError:  A synced mail has no touchId: " + mail.Subject + "\n\n" + mail.Body);
        //                cntMailOrphan++;
        //                if (!statusOnly) RemoveTouchId(mail);
        //            }
        //            else
        //            {
        //                Touch tch = syncedTouches.Find(t => t.Id == touchId);
        //                if (tch == null)
        //                {
        //                    cntMailOrphan++;
        //                    if (!statusOnly) RemoveTouchId(mail);
        //                }
        //                else
        //                {
        //                    bool needsSync = CompareAndSync(tch, mail, statusOnly);
        //                    if (needsSync)
        //                    {
        //                        cntOutMail++;
        //                    }
        //                    else cntSyncMail++;

        //                    tch.Typename = tchSynced;
        //                }
        //            }
        //        }



        //        foreach (Touch tch in syncedTouches.FindAll(t => t.Typename != tchSynced))
        //        {
        //            SyncItem syncItem = GetSyncItem(tch);
        //            if (syncItem.platform == "outlook")
        //            {
        //                if (syncItem.itemType == Outlook.OlItemType.olMailItem)
        //                {
        //                    if (!statusOnly)
        //                    {
        //                        Outlook.MailItem newmail = Application.CreateItem(Outlook.OlItemType.olMailItem) as Outlook.MailItem;
        //                        //_draftsFolder.Items.Add(newmail);
        //                        RegisterTouchUpdate(tch);
        //                        CompareAndSync(tch, newmail);
        //                        RemoveTouchId(newmail);  //just in case lingering tracker
        //                        AddTouchId(newmail, tch.Id);
        //                        newmail.Save();
        //                    }
        //                    cntMailNew++;

        //                }
        //                else if (syncItem.itemType == Outlook.OlItemType.olAppointmentItem)
        //                {
        //                    if (!statusOnly)
        //                    {
        //                        try
        //                        {
        //                            //create item
        //                            Outlook.AppointmentItem newappt = Application.CreateItem(Outlook.OlItemType.olAppointmentItem) as Outlook.AppointmentItem;
        //                            RegisterTouchUpdate(tch);
        //                            CompareAndSync(tch, newappt);
        //                            RemoveTouchId(newappt);  //just in case lingering tracker
        //                            AddTouchId(newappt, tch.Id);
        //                            newappt.Save();
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            HandleError(ex, "CreateApptFromTouch");
        //                        }

        //                    }
        //                    //else cntOutAppt++;
        //                    cntApptNew++;
        //                }
        //            }
        //            else
        //            {
        //                if (!statusOnly) DeleteSyncDoc(tch);
        //                Debug.WriteLine("Got a bad DocSync document: " + tch.Name + " " + tch.Id.ToString());
        //            }
        //        }


        //        status += string.Format("{0} total appointments synced\n", _syncedAppts.Count);
        //        //if (cntAppt > 0) status += string.Format("  - {0} in sync\n", cntSyncAppt);
        //        if (cntOutAppt > 0) status += string.Format("  - {0} out of sync\n", cntOutAppt);
        //        if (cntApptNew > 0) status += string.Format("  - {0} incoming\n", cntApptNew);
        //        if (cntApptOrphan > 0) status += string.Format("  - {0} orphaned - pending unsync\n", cntApptOrphan);

        //        //pending deletes from app
        //        int cntDele = 0;
        //        List<Document> docsToDele = DataAPI.GetDeletedSyncs(); //need new class to handle: itemEntryID , item olType
        //        if (!statusOnly)
        //        {
        //            foreach (Document doc in docsToDele)
        //            {
        //                Guid touchId = Guid.Empty;
        //                Guid.TryParse(doc.Value, out touchId);
        //                if (touchId != Guid.Empty)
        //                {
        //                    Outlook.AppointmentItem appt2Dele = null;
        //                    //appt = _nameSpace.GetItemFromID(syncItem.id) as Outlook.AppointmentItem;
        //                    //if (appt != null)

        //                    //    _syncAppts.TryGetValue(touchId, out appt2Dele);
        //                    if (appt2Dele != null)
        //                    {
        //                        appt2Dele.Delete();
        //                        cntDele++;
        //                    }
        //                    DataAPI.DeleteDocument(doc.Id);
        //                }
        //            }

        //        }
        //        if (docsToDele.Count > 0) status += string.Format("  - {0} appointments pending deletion.\n", docsToDele.Count);

        //        status += string.Format("\n{0} total emails synced.\n", _syncedMails.Count);
        //        if (cntMailNew > 0) status += string.Format("  - {0} incoming\n", cntMailNew);
        //        if (cntMailOrphan > 0) status += string.Format("  - {0} orphaned - pending unsync\n", cntMailOrphan);

        //        if (!statusOnly)
        //        {
        //            DataAPI.UpdateLastSync();
        //            InitTheSync();
        //        }
        //        else
        //        {
        //            DateTime last = DataAPI.LastSyncDt();
        //            status += string.Format("\nLast full sync: {0}", (last == DateTime.MinValue) ? "never" : string.Format("{0:d} {0:t}", last));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        HandleError(ex, "SyncAll");
        //    }

        //    return status;
        //}

        private string RefreshSyncStatus()
        {
            StringBuilder status = new StringBuilder();
            DateTime started = DateTime.Now;
            int cntMailNew = 0, cntApptNew = 0, cntSyncAppt = 0, cntOutAppt = 0, cntSyncMail = 0, cntOutMail = 0;

            try
            {
                List<Touch> syncedTouches = DataAPI.SyncTouches();

                foreach (Touch tch in syncedTouches)
                {
                    SyncItem si = GetSyncItem(tch);
                    switch (si.itemType)
                    {
                        case Outlook.OlItemType.olAppointmentItem:
                            if (si.lastSyncAppt == si.lastSyncTouch) cntSyncAppt++;
                            else if (si.lastSyncAppt == DateTime.MinValue)
                            {
                                cntApptNew++;
                                //incoming 
                                try
                                {
                                    //create item
                                    Outlook.AppointmentItem newappt = Application.CreateItem(Outlook.OlItemType.olAppointmentItem) as Outlook.AppointmentItem;
                                    //get fully hydrated touch
                                    Touch fullTch = DataAPI.GetTouch(tch.Id);
                                    RegisterTouchUpdate(fullTch, Outlook.OlItemType.olAppointmentItem);
                                    CompareAndSync(fullTch, newappt);
                                    RemoveTouchId(newappt);  //just in case lingering tracker
                                    AddTouchId(newappt, fullTch.Id);
                                    newappt.Save();
                                }
                                catch (Exception ex) { HandleError(ex, "CreateApptFromTouch"); }
                            }
                            else
                            {
                                //needs sync
                                cntOutAppt++;
                            }

                            break;
                        case Outlook.OlItemType.olMailItem:
                            if (si.lastSyncAppt == si.lastSyncTouch) cntSyncMail++;
                            else if (si.lastSyncAppt == DateTime.MinValue)
                            {
                                cntMailNew++;
                                //incoming 
                                try
                                {
                                    //create item
                                    Outlook.AppointmentItem newappt = Application.CreateItem(Outlook.OlItemType.olAppointmentItem) as Outlook.AppointmentItem;
                                    RegisterTouchUpdate(tch, Outlook.OlItemType.olMailItem);
                                    CompareAndSync(tch, newappt);
                                    RemoveTouchId(newappt);  //just in case lingering tracker
                                    AddTouchId(newappt, tch.Id);
                                    newappt.Save();
                                }
                                catch (Exception ex) { HandleError(ex, "CreateApptFromTouch"); }
                            }
                            else
                            {
                                //needs sync
                                cntOutMail++;
                            }
                            break;
                    }
                }


                status.AppendLine(string.Format("{0} appointments synced", (cntSyncAppt + cntOutAppt + cntApptNew)));
                if (cntOutAppt > 0) status.AppendLine(string.Format("  - {0} out of sync", cntOutAppt));
                if (cntApptNew > 0) status.AppendLine(string.Format("  - {0} just arrived", cntApptNew));

                //pending deletes from app
                //List<Document> docsToDele = DataAPI.GetDeletedSyncs();
                //if (docsToDele.Count > 0) status.AppendLine(string.Format("  - {0} appointments pending deletion.", docsToDele.Count));

                //pending entity removes
                //_pendingRemoves = DataAPI.GetSyncRemoves();
                //if (_pendingRemoves.Count > 0) status.AppendLine(string.Format("  - {0} pending recipient removals.", _pendingRemoves.Count));

                status.AppendLine(string.Format("{0} emails synced.", (cntSyncMail + cntOutMail + cntMailNew)));
                if (cntMailNew > 0) status.AppendLine(string.Format("  - {0} just arrived", cntMailNew));
            }
            catch (Exception ex)
            {
                DataAPI.HandleError("RefreshSyncStatus", ex);
            }
            return status.ToString();
        }

        public string SyncStatus
        {
            get
            {
                if (!(string.IsNullOrWhiteSpace(_syncStatus))) return _syncStatus;
                else
                {
                    _syncStatus = RefreshSyncStatus();
                    return _syncStatus;
                }
            }

            set
            {
                _syncStatus = value;
            }
        }
        int _syncDeepSearchesStarted = 0;
        int _syncDeepSearchesCompleted = 0;

        public void SyncDeep()
        {
            _syncDeepSearchesStarted = 0;
            _syncDeepSearchesCompleted = 0;

            DateTime syncBackTo = DateTime.Now.AddMonths(-2);
            string sinceDate = syncBackTo.ToString("MM.dd.yyyy HH:mm");

            Globals.ThisAddIn.SyncStatus = null;
            List<string> foldersToIgnore = new List<string>() { "trash", "outbox", "spam", "junk", "deleted", "junk e-mail" };
            List<Outlook.MAPIFolder> allFolders = new List<Outlook.MAPIFolder>();
            foreach (Outlook.MAPIFolder folderTopLevel in Globals.ThisAddIn._nameSpace.Folders)
            {
                foreach (Outlook.MAPIFolder folder2Level in folderTopLevel.Folders)
                {
                    allFolders.Add(folder2Level);
                }

            }
            List<Outlook.MAPIFolder> folders = allFolders.FindAll(f => null == foldersToIgnore.Find(i => i == f.Name.ToLower()));

            string syncFilterAppt = string.Format("\"urn:schemas:calendar:dtstart\" >  '{0}'", sinceDate);
            string syncFilterMail = string.Format("\"urn:schemas:httpmail:textdescription\" like '%{0}%' AND \"urn:schemas:httpmail:datereceived\" > '{1}'", Globals.ThisAddIn._tracker.Substring(0, Globals.ThisAddIn._trackPrefixLen), sinceDate);
            foreach (Outlook.MAPIFolder fld in folders)
            {
                switch (fld.DefaultItemType)
                {
                    case Outlook.OlItemType.olMailItem:
                        Globals.ThisAddIn.Application.AdvancedSearch("'" + fld.FolderPath + "'", syncFilterMail, true, "MailSync");
                        _syncDeepSearchesStarted++;
                        break;
                    case Outlook.OlItemType.olAppointmentItem:
                        Globals.ThisAddIn.Application.AdvancedSearch("'" + fld.FolderPath + "'", syncFilterAppt, true, "ApptSync");
                        _syncDeepSearchesStarted++;
                        break;
                }
            }

        }
         public bool CompareAndSync(Touch tch, Outlook.AppointmentItem appt)
        {
            return CompareAndSync(tch, appt, false);
        }

        public bool CompareAndSync(Touch tch, Outlook.AppointmentItem appt, bool statusOnly)
        {
            int secondsTolerance = 0;
            string log = "";
            bool needsSync = false;
            _syncing = true;
            try
            {
                SyncItem syncItem = GetSyncItem(tch);
                if (syncItem.itemType == Outlook.OlItemType.olJournalItem)
                {
                    //new sync for existing item just needs refresh
                    RefreshTouchFromAppt(tch, appt);
                }
                else
                {
                    DateTime lsa = (syncItem.lastSyncAppt.HasValue) ? Convert.ToDateTime(syncItem.lastSyncAppt) : DateTime.MinValue;
                    log += string.Format("\r\n\r\nappt: {0:u}  tch {1:u} | {2} | {3}", lsa, syncItem.lastSyncTouch, appt.Subject, tch.Name);
                    if (Math.Abs(lsa.Subtract(syncItem.lastSyncTouch).TotalSeconds) > secondsTolerance)
                    {
                        needsSync = true;
                        if (!statusOnly)
                        {
                            if (lsa.CompareTo(syncItem.lastSyncTouch) == 1)
                            {
                                RefreshTouchFromAppt(tch, appt);
                                //log += "RefreshTouchFromAppt required.";
                            }
                            else
                            {
                                RefreshApptFromTouch(appt, tch);
                                //log += "RefreshApptFromTouch required.";
                            }
                            tch.Id = DataAPI.PostTouch(tch, false);
                            RefreshAttachmentsTouchFromAppt(tch, appt);
                            //re-fetch the touch to get doc id's etc.
                            tch = DataAPI.GetTouch(tch.Id);

                            RegisterSync(tch, appt);
                            appt.Save();
                        }
                    }
                    //Debug.WriteLine("No appt sync required.");
                }
                DataAPI.PostAudit("Compare and Sync", log);

                //check for entity removals
                if (_pendingRemoves != null && _pendingRemoves.Count > 0)
                {
                    var removes = _pendingRemoves.FindAll(d => d.UserType_Id == Guids.Doc_CalSyncRemoval && d.Value.ToLower().IndexOf(tch.Id.ToString().ToLower()) > -1);
                    if (removes != null && removes.Count > 0) RemovePending(removes, tch, appt);
                }

                //update entryId in sync doc
                if (!statusOnly) UpdateSyncDocEntryId(tch, appt.EntryID);
            }
            catch (Exception ex)
            {
                HandleError(ex, "CompareAndSync Appt");
            }
            _syncing = false;

            return needsSync;
        }

        private void UpdateSyncDocEntryId(Touch tch, string entryId)
        {
            Document doc = tch.Documents.Find(d => d.UserType_Id == Guids.Doc_CalSync);
            if (doc != null)
            {
                SyncItem si = null;
                try { JsonConvert.DeserializeObject<SyncItem>(doc.Value); }
                catch {
                    string x = "";
                }
                if (si != null)
                {
                    if (si.id != entryId)
                    {
                        si.id = entryId;
                        doc.Value = JsonConvert.SerializeObject(si);
                        DataAPI.PostDocument(doc);
                    }
                }
            }
        }

        public class PendingObject
        {
            public Guid touchId { get; set; }
            public Guid entityId { get; set; }
            public EntityTypes entityType { get; set; }
            public string entryId { get; set; }
            public int itemType { get; set; }
        }

        private void RemovePending(List<Document> removes, Touch tch, Outlook.AppointmentItem appt)
        {
            foreach (Document doc in removes)
            {
                try
                {
                    PendingObject pend = JsonConvert.DeserializeObject<PendingObject>(doc.Value);

                    List<string> addresses = new List<string>();
                    if (pend.entityType == EntityTypes.Person)
                    {
                        var per = DataAPI.GetPerson(pend.entityId);
                        foreach (ContactPoint cp in per.ContactPoints.FindAll(c => c.Primitive == ContactPointPrimitive.Email && !string.IsNullOrWhiteSpace(c.Name))) addresses.Add(cp.Name.ToLower());
                    }
                    else if (pend.entityType == EntityTypes.Organization || pend.entityType == EntityTypes.Group)
                    {
                        var org = DataAPI.GetOrganization(pend.entityId);
                        foreach (ContactPoint cp in org.ContactPoints.FindAll(c => c.Primitive == ContactPointPrimitive.Email && !string.IsNullOrWhiteSpace(c.Name))) addresses.Add(cp.Name.ToLower());
                    }

                    if (addresses.Count > 0)
                    {
                        for (int i = appt.Recipients.Count; i > 0; i--)
                        {
                            Outlook.Recipient rec = appt.Recipients[i];
                            string addr = SMTPAddress(rec.AddressEntry).ToLower();
                            if (!string.IsNullOrWhiteSpace(addr) && addresses.Contains(addr))
                            {
                                appt.Recipients.Remove(i);
                            }
                        }
                    }

                    //completed - get rid of doc
                    DataAPI.DeleteDocument(doc.Id);
                    _pendingRemoves.RemoveAll(r => r.Id == doc.Id);
                }
                catch (Exception ex)
                {
                    DataAPI.HandleError("RemovePending", ex);
                }
            }
        }


        public bool CompareAndSync(Touch tch, Outlook.MailItem mail)
        {
            return CompareAndSync(tch, mail, false);
        }

        public bool CompareAndSync(Touch tch, Outlook.MailItem mail, bool statusOnly)
        {
            int secondsTolerance = 0;
            bool needsSync = false;
            try
            {
                _syncing = true;
                SyncItem syncItem = GetSyncItem(tch);
                if (syncItem.itemType == Outlook.OlItemType.olJournalItem)
                {
                    //new, not synced, just needs refresh
                    RefreshTouchFromMail(tch, mail);
                }
                else
                {
                    DateTime lsa = (syncItem.lastSyncAppt.HasValue) ? Convert.ToDateTime(syncItem.lastSyncAppt) : DateTime.MinValue;
                    if (Math.Abs(lsa.Subtract(syncItem.lastSyncTouch).TotalSeconds) > secondsTolerance)
                    {
                        needsSync = true;
                        if (!statusOnly)
                        {
                            if (lsa.CompareTo(syncItem.lastSyncTouch) == 1)
                            {
                                RefreshTouchFromMail(tch, mail);
                                tch.Id = DataAPI.PostTouch(tch, false);
                                RefreshAttachmentsTouchFromMail(tch, mail);
                            }
                            else
                            {
                                RefreshMailFromTouch(tch, mail);
                            }
                            RegisterSync(tch, mail);
                        }
                    }
                }
                //update entryId in sync doc
                if (!statusOnly) UpdateSyncDocEntryId(tch, mail.EntryID);

            }
            catch (Exception ex)
            {
                HandleError(ex, "CompareAndSync Mail");
            }
            finally { _syncing = false; }

            return needsSync;
        }

        public Touch NewTouch(Outlook.OlItemType itemType)
        {
            string regkeyValue = "";
            bool isEmail = (itemType == Outlook.OlItemType.olMailItem);
            Touch theTouch = new Touch();

            Guid utId = Guids.Tch_Appointment;
            regkeyValue = Utilities.ReadRegStringValue((isEmail) ? RegKeys.TouchTypeMail : RegKeys.TouchTypeAppt);
            if (!String.IsNullOrWhiteSpace(regkeyValue)) Guid.TryParse(regkeyValue, out utId);

            Guid dbId = DataAPI.TheUser.DefaultCollection_Id;
            Guid ogId = DataAPI.TheUser.DefaultOwnedByGroup_Id;
            regkeyValue = Utilities.ReadRegStringValue(RegKeys.DefaultDB);
            if (!String.IsNullOrWhiteSpace(regkeyValue)) Guid.TryParse(regkeyValue, out dbId);

            regkeyValue = Utilities.ReadRegStringValue(RegKeys.DefaultOG);
            if (!String.IsNullOrWhiteSpace(regkeyValue))
            {
                if (regkeyValue == Guid.Empty.ToString()) ogId = Guid.Empty;  //always valid
                else
                {
                    //check valid for the db
                    List<Group> tms = DataAPI.EditTeamsDB(dbId);
                    if (tms.Find(t => t.Id == Guid.Parse(regkeyValue)) != null) ogId = Guid.Parse(regkeyValue);
                    else Utilities.DeleteRegKey(RegKeys.DefaultOG);
                }
            }

            theTouch = new Touch() { Collection_Id = dbId, OwnedBy_Id = DataAPI.TheUser.Id, OwnedByGroup_Id = ogId, UserType_Id = utId, ModifiedDt = DateTime.MinValue };
            if (DataAPI.UserPreferences.ContainsKey("addmetouch") && Convert.ToBoolean(DataAPI.UserPreferences["addmetouch"]))
            {
                Person me = new Person() { Id = DataAPI.TheUser.Id, Name = DataAPI.TheUser.Name, ImageUrl = DataAPI.TheUser.ImageUrl, changeType = ChangeType.Update };
                theTouch.People.Add(me);
            }

            return theTouch;
        }

        private string Foldername(object outlookItem)
        {
            string result = "";
            Outlook.MAPIFolder par = null;
            try
            {
                if (outlookItem is Outlook.MailItem)
                {
                    par = ((outlookItem as Outlook.MailItem).Parent) as Outlook.MAPIFolder;
                    result = par.Name;
                }
                else if (outlookItem is Outlook.AppointmentItem)
                {
                    par = ((outlookItem as Outlook.AppointmentItem).Parent) as Outlook.MAPIFolder;
                    result = par.Name;
                }
            }
            catch { }
            finally { par = null; }

            return result;
        }

        internal void RegisterSync(Touch tch, object outlookItem)
        {
            bool addRelation = false;
            bool addDocToTouch = false;

            Document doc = new Document() { UserType_Id = Guids.Doc_CalSync, OwnedBy_Id = DataAPI.TheUser.Id, changeType = ChangeType.Update, Collection_Id = tch.Collection_Id, OwnedByGroup_Id = tch.OwnedByGroup_Id };
            SyncItem syncItem = new SyncItem();

            List<Document> existDocs = tch.Documents.FindAll(d => d.UserType_Id == Guids.Doc_CalSync);
            if (existDocs != null && existDocs.Count > 0)
            {
                doc = existDocs[0];
                try { syncItem = JsonConvert.DeserializeObject<SyncItem>(doc.Value); }
                catch { }
                addRelation = (doc.Id == Guid.Empty);
            }
            else addRelation = addDocToTouch = true;

            if (outlookItem is Outlook.MailItem)
            {
                syncItem.id = (outlookItem as Outlook.MailItem).EntryID;
                syncItem.itemType = Outlook.OlItemType.olMailItem;

            }
            else if (outlookItem is Outlook.AppointmentItem || outlookItem is Outlook.MeetingItem)
            {
                syncItem.id = (outlookItem as Outlook.AppointmentItem).EntryID;
                syncItem.itemType = Outlook.OlItemType.olAppointmentItem;
            }
            syncItem.platform = "outlook";
            syncItem.folderName = Foldername(outlookItem);
            syncItem.lastSyncAppt = syncItem.lastSyncTouch = DateTime.UtcNow;
            doc.Value = JsonConvert.SerializeObject(syncItem);
            doc.changeType = tch.changeType = ChangeType.Update;

            doc.Id = DataAPI.PostDocument(doc);

            if (addRelation) DataAPI.PostRelationship(new RelationshipPost() { entityId1 = tch.Id, entityType1 = EntityTypes.Touch, entityId2 = doc.Id, entityType2 = EntityTypes.Document });
            if (addDocToTouch) tch.Documents.Add(doc);
        }

        public Guid TouchId(object outlookItem)
        {
            Guid touchId = Guid.Empty;
            Outlook.MailItem mail = null;
            Outlook.AppointmentItem appt = null;
            try
            {
                if (outlookItem is Outlook.MailItem)
                {
                    mail = outlookItem as Outlook.MailItem;
                    if (mail != null && mail.Body.IndexOf(_tracker.Substring(0, _trackPrefixLen)) != -1)
                    {
                        int idx = mail.Body.IndexOf(_tracker.Substring(0, _trackPrefixLen));
                        string sId = mail.Body.Substring(idx + _trackPrefixLen, 36);
                        Guid.TryParse(sId, out touchId);
                        //Outlook.UserProperties props = mail.UserProperties;
                        //Outlook.UserProperty prop = props.Find(Properties.Resources.ShuriTouchID, true);
                        //if (prop != null) Guid.TryParse(prop.Value, out touchId);
                        //prop = null;
                        //props = null;
                    }
                }
                else if (outlookItem is Outlook.AppointmentItem)
                {
                    appt = outlookItem as Outlook.AppointmentItem;
                    if (appt != null && !(string.IsNullOrWhiteSpace(appt.EntryID)))
                    {
                        Outlook.UserProperties props = appt.UserProperties;
                        Outlook.UserProperty prop = props.Find(Properties.Resources.ShuriTouchID, true);
                        if (prop != null) Guid.TryParse(prop.Value, out touchId);
                        else
                        {
                            //look by entryId
                            touchId = DataAPI.TouchIdByEntryId(appt.EntryID);
                        }
                        prop = null;
                        props = null;
                    }
                }
            }
            catch { }
            finally
            {
                mail = null;
                appt = null;
            }

            return touchId;
        }
        public void AddTouchId(object item, Guid touchId)
        {
            if (item is Outlook.MailItem) AddTouchId((item as Outlook.MailItem), touchId);
            else if (item is Outlook.AppointmentItem) AddTouchId((item as Outlook.AppointmentItem), touchId);
        }
        public void AddTouchId(Outlook.MailItem mail, Guid touchId)
        {
            if (mail.Body.IndexOf(_tracker.Substring(0, _trackPrefixLen)) == -1)
            {
                string trk = string.Format(_tracker, touchId);

                if (mail.HTMLBody.IndexOf("</body>") > -1)
                {
                    int idx = mail.HTMLBody.IndexOf("</body>");
                    mail.HTMLBody = mail.HTMLBody.Insert(idx, string.Format(_trackerHtmlWrapper, trk));
                }
                else mail.Body += "\n\n\n" + trk;
                _syncing = true;
                mail.Save();
                _syncing = false;
            }
            //Outlook.UserProperties props = mail.UserProperties;
            //Outlook.UserProperty prop = props.Find(Properties.Resources.ShuriTouchID, true);
            //if (prop == null)
            //{
            //    prop = props.Add(Properties.Resources.ShuriTouchID, Outlook.OlUserPropertyType.olText, false);
            //}
            //if (prop.Value == null || prop.Value != touchId.ToString())
            //{
            //    prop.Value = touchId.ToString();
            //}
            //prop = null;
            //props = null;
        }
        public void AddTouchId(Outlook.AppointmentItem appt, Guid touchId)
        {
            Outlook.UserProperties props = appt.UserProperties;
            Outlook.UserProperty prop = props.Find(Properties.Resources.ShuriTouchID, true);
            if (prop == null)
            {
                prop = props.Add(Properties.Resources.ShuriTouchID, Outlook.OlUserPropertyType.olText, false);
            }
            if (prop.Value == null || prop.Value != touchId.ToString())
            {
                prop.Value = touchId.ToString();
                appt.Save();
            }
            prop = null;
            props = null;
        }


        public void RemoveTouchId(object outlookItem)
        {
            Outlook.MailItem mail = null;
            Outlook.AppointmentItem appt = null;
            if (outlookItem is Outlook.MailItem)
            {
                mail = outlookItem as Outlook.MailItem;
                if (mail != null && mail.Body.IndexOf(_tracker.Substring(0, _trackPrefixLen)) != -1)
                {
                    int idx = mail.Body.IndexOf(_tracker.Substring(0, _trackPrefixLen));
                    mail.Body = mail.Body.Remove(idx, 51);
                    mail.Save();
                    //string trk = mail.Body.Substring(idx, 50);
                    //mail.HTMLBody = mail.HTMLBody.Replace(string.Format(_trackerHtmlWrapper, trk), "");

                }
                //    Outlook.UserProperties props = mail.UserProperties;
                //Outlook.UserProperty prop = props.Find(Properties.Resources.ShuriTouchID, true);
                //if (prop != null)
                //{
                //    prop.Delete();
                //    mail.Save();
                //}
                //prop = null;
                //props = null;
            }
            else if (outlookItem is Outlook.AppointmentItem)
            {
                appt = outlookItem as Outlook.AppointmentItem;
                Outlook.UserProperties props = appt.UserProperties;
                Outlook.UserProperty prop = props.Find(Properties.Resources.ShuriTouchID, true);
                if (prop != null)
                {
                    prop.Delete();
                    appt.Save();
                }
                prop = null;
                props = null;
            }
            mail = null;
            appt = null;
        }

        internal void RegisterTouchUpdate(Touch tch, Outlook.OlItemType itemtype)
        {
            SyncItem syncItem = new SyncItem();
            Document doc = tch.Documents.Find(d => d.UserType_Id == Guids.Doc_CalSync);
            if (doc != null)
            {
                try { syncItem = JsonConvert.DeserializeObject<SyncItem>(doc.Value); }
                catch { }
            }
            else
            {
                doc = new Document() { UserType_Id = Guids.Doc_CalSync, OwnedBy_Id = DataAPI.TheUser.Id, changeType = ChangeType.Update, Collection_Id = tch.Collection_Id, OwnedByGroup_Id = tch.OwnedByGroup_Id };
                syncItem.platform = "outlook";
                syncItem.folderName = "";
                syncItem.itemType = itemtype;
                tch.Documents.Add(doc);
            }
            syncItem.lastSyncTouch = DateTime.UtcNow;
            doc.Value = JsonConvert.SerializeObject(syncItem);
            doc.changeType = tch.changeType = ChangeType.Update;


        }

        internal void RegisterOLItemUpdate(Touch tch, string entryId)
        {
            SyncItem syncItem = null;
            Document doc = tch.Documents.Find(d => d.UserType_Id == Guids.Doc_CalSync);
            if (doc != null)
            {
                try { syncItem = JsonConvert.DeserializeObject<SyncItem>(doc.Value); }
                catch { }
                syncItem.id = entryId;
                syncItem.lastSyncAppt = DateTime.UtcNow;
                doc.Value = JsonConvert.SerializeObject(syncItem);
                doc.changeType = tch.changeType = ChangeType.Update;
            }
            else Debug.WriteLine("RegisterOLItemUpdate needs an existing syncDoc");
        }
        #endregion
        public class RecipientPerson
        {
            ~RecipientPerson()
            {
                recipient = null;
                person = null;
            }
            public RecipientPerson(Outlook.Recipient recip)
            {
                recipient = recip;
                person = null;
            }
            public Outlook.Recipient recipient { get; set; }
            public Person person { get; set; }
        }
        public class RecipientOrg
        {
            ~RecipientOrg()
            {
                recipient = null;
                org = null;
            }
            public RecipientOrg(Outlook.Recipient recip)
            {
                recipient = recip;
                org = null;
            }
            public Outlook.Recipient recipient { get; set; }
            public Group org { get; set; }
        }
        #region Appt Sync Helpers

        // const string PR_SMTP_ADDRESS = "http://schemas.microsoft.com/mapi/proptag/0x39FE001E";
        private bool RefreshApptFromTouch(Outlook.AppointmentItem appt, Touch tch)
        {
            bool result = true;
            try
            {
                DateTime dtNow = DateTime.UtcNow;
                if (tch != null)
                {
                    Debug.WriteLine("Appt:  " + appt.Subject);

                    appt.StartUTC = tch.DateStart;
                    if (tch.DateEnd != null) appt.EndUTC = (DateTime)tch.DateEnd;
                    else appt.EndUTC = appt.StartUTC.AddHours(1);

                    if (tch.Name != "Outlook Appointment") appt.Subject = tch.Name;

                    appt.Body = tch.Description;

                    if (tch.Locations.Count > 0) appt.Location = tch.Locations[0].Address;
                    else appt.Location = "";

                    #region recipients
                    if (appt.Recipients.Count > 0 || tch.People.Count > 0)
                    {
                        appt.Recipients.ResolveAll();
                        List<RecipientPerson> recipPeople = new List<RecipientPerson>();
                        List<RecipientOrg> recipOrgs = new List<RecipientOrg>();

                        //first, match to touch
                        foreach (Outlook.Recipient rec in appt.Recipients)
                        {
                            string smtp = SMTPAddress(rec.AddressEntry);
                            Debug.WriteLine("Appt.recipient  " + rec.Name + " SMTP=" + smtp);

                            RecipientPerson rp = new RecipientPerson(rec);
                            rp.person = tch.People.Find(p => (p.changeType != ChangeType.Remove) && null != p.ContactPoints.Find(c => c.Primitive == ContactPointPrimitive.Email && c.Name.ToLower() == smtp.ToLower()));
                            if (rp.person != null) recipPeople.Add(rp);
                            else
                            {
                                RecipientOrg ro = new RecipientOrg(rec);
                                ro.org = tch.Groups.Find(o => (o.changeType != ChangeType.Remove) && null != o.ContactPoints.Find(c => c.Primitive == ContactPointPrimitive.Email && c.Name.ToLower() == smtp.ToLower()));
                                if (ro.org != null) recipOrgs.Add(ro);
                                else
                                {
                                    //smtp address NOT found, assume person
                                    recipPeople.Add(rp);
                                }
                            }
                        }

                        //remove the recognized recips with no  match
                        foreach (RecipientPerson rp in recipPeople)
                        {
                            if (rp.person == null)
                            {
                                string smtp = SMTPAddress(rp.recipient.AddressEntry);
                                List<AutocompleteResult> recognized = DataAPI.ResolveEmail(smtp);
                                if (recognized != null && recognized.Count > 0)
                                {
                                    //recognized, but not in list
                                    appt.Recipients.Remove(rp.recipient.Index);
                                    Debug.WriteLine("unrecognized person in Shuri " + smtp);
                                }
                            }
                        }

                        foreach (RecipientOrg ro in recipOrgs)
                        {
                            if (ro.org == null)
                            {
                                string smtp = SMTPAddress(ro.recipient.AddressEntry);
                                List<AutocompleteResult> recognized = DataAPI.ResolveEmail(smtp);
                                if (recognized != null && recognized.Count > 0)
                                {
                                    //recognized, but not in list
                                    appt.Recipients.Remove(ro.recipient.Index);
                                    Debug.WriteLine("unrecognized ORG in Shuri " + smtp);
                                }
                            }
                        }





                        //add the missing people
                        foreach (Person per in tch.People.FindAll(p => p.changeType != ChangeType.Remove))
                        {
                            Debug.WriteLine("Touch person " + per.Name);
                            RecipientPerson rp = recipPeople.Find(r => r.person != null && r.person.Id == per.Id);
                            if (rp == null)
                            {
                                ContactPoint cp = per.ContactPoints.Find(c => c.Primitive == ContactPointPrimitive.Email && !string.IsNullOrWhiteSpace(c.Name));
                                if (cp != null)
                                {
                                    string eml = string.Format("{0} <{1}>", per.Name.Replace("(", "").Replace(")", ""), cp.Name);
                                    Outlook.Recipient recip = appt.Recipients.Add(eml);
                                }

                                Debug.WriteLine("added");
                            }
                            else Debug.WriteLine("existed");
                        }
                        //add the missing orgs
                        foreach (Group org in tch.Groups.FindAll(p => p.changeType != ChangeType.Remove))
                        {
                            Debug.WriteLine("Touch org " + org.Name);
                            RecipientOrg ro = recipOrgs.Find(r => r.org != null && r.org.Id == org.Id);
                            if (ro == null)
                            {
                                ContactPoint cp = org.ContactPoints.Find(c => c.Primitive == ContactPointPrimitive.Email && !string.IsNullOrWhiteSpace(c.Name));
                                if (cp != null)
                                {
                                    string eml = string.Format("{0} <{1}>", org.Name.Replace("(", "").Replace(")", ""), cp.Name);
                                    Outlook.Recipient recip = appt.Recipients.Add(eml);
                                }

                                Debug.WriteLine("added");
                            }
                            else Debug.WriteLine("existed");
                        }
                        appt.MeetingStatus = Outlook.OlMeetingStatus.olMeeting;
                        appt.Recipients.ResolveAll();
                    }
                    #endregion

                    RefreshAttachmentsApptFromTouch(tch, appt);

                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "RefreshApptFromTouch");
            }
            return result;
        }

        public void RefreshTouchFromAppt(Touch theTouch, Outlook.AppointmentItem appt)
        {
            try
            {
                appt.Save();

                theTouch.DateStart = appt.StartUTC;
                theTouch.DateEnd = appt.EndUTC;
                if (appt.Body != null) theTouch.Description = appt.Body;
                if (appt.Subject != null) theTouch.Name = appt.Subject;
                if (theTouch.Name.Length > 120) theTouch.Name = theTouch.Name.Substring(0, 120);
                theTouch.changeType = ChangeType.Update;
                if (String.IsNullOrEmpty(theTouch.Name)) theTouch.Name = "Outlook Appointment";
                if (theTouch.UserType_Id == Guid.Empty) theTouch.UserType_Id = Guids.Tch_Appointment;

                if (!string.IsNullOrEmpty(appt.Location))
                {
                    if (theTouch.Locations.Count == 0 || (theTouch.Locations.Count > 0 && appt.Location != theTouch.Locations[0].Address))
                    {
                        if (theTouch.Locations.Count > 0)
                        {
                            foreach (Location locat in theTouch.Locations)
                            {
                                locat.changeType = ChangeType.Remove;
                            }
                        }

                        //add the initial and only location.  after this, must be managed via app or details form
                        Location loc = new Location()
                        {
                            UserType_Id = Guids.Loc_Business,
                            Address = appt.Location,
                            changeType = ChangeType.Update,
                            Collection_Id = theTouch.Collection_Id,
                            OwnedByGroup_Id = theTouch.OwnedByGroup_Id,
                            OwnedBy_Id = theTouch.OwnedBy_Id
                        };
                        theTouch.Locations.Add(loc);
                    }
                }
                ResolveRecipientsAppt(theTouch, appt);
            }
            catch (Exception ex)
            {
                HandleError(ex, "RefreshTouchFromAppt");
            }
        }

        public void ResolveRecipientsAppt(Touch theTouch, Outlook.AppointmentItem appt)
        {
            if (appt.Recipients.Count > 0 || !string.IsNullOrWhiteSpace(appt.RequiredAttendees)) appt.Recipients.ResolveAll();

            if (appt.Recipients.Count > 0)
            {
                List<string> recipsList = new List<string>();

                Outlook.Recipients recips = appt.Recipients;

                Outlook.Recipient recip = null;
                for (int i = 1; i <= recips.Count; i++)
                {
                    recip = recips[i];
                    string smtp = SMTPAddress(recip.AddressEntry);
                    if (!string.IsNullOrWhiteSpace(smtp)) recipsList.Add(smtp);
                    recip = null;
                }
                recips = null;

                List<string> unknownList = new List<string>();

                foreach (string sRecip in recipsList)
                {
                    bool foundIt = false;
                    foreach (Person p in theTouch.People)
                    {
                        if ((p.Id == DataAPI.TheUser.Id && sRecip == DataAPI.TheUser.EmailAddress) || p.ContactPoints.Find(cp => cp.Name.ToLower() == sRecip.ToLower()) != null)
                        {
                            foundIt = true;
                            p.changeType = ChangeType.None;
                            break;
                        }
                    }
                    foreach (Group g in theTouch.Groups)
                    {
                        if (g.ContactPoints.Find(cp => cp.Name.ToLower() == sRecip.ToLower()) != null)
                        {
                            foundIt = true;
                            g.changeType = ChangeType.None;
                            break;
                        }
                    }
                    if (!foundIt) unknownList.Add(sRecip);
                }



                if (unknownList.Count > 0)
                {
                    Touch resolverTch = DataAPI.Resolver(unknownList);
                    foreach (Person per in resolverTch.People)
                    {
                        if (theTouch.People.Find(person => person.Id == per.Id) == null)
                        {
                            per.changeType = ChangeType.Update;
                            theTouch.People.Add(per);
                        }
                    }
                    foreach (Group grp in resolverTch.Groups)
                    {
                        if (theTouch.Groups.Find(group => group.Id == grp.Id) == null)
                        {
                            grp.changeType = ChangeType.Update;
                            theTouch.Groups.Add(grp);
                        }
                    }
                    foreach (string str in resolverTch.resolveStrings)
                    {
                        if (theTouch.resolveStrings.Find(rStr => rStr == str) == null) theTouch.resolveStrings.Add(str);
                    }
                }

            }
        }

        private bool RefreshAttachmentsTouchFromAppt(Touch theTouch, Outlook.AppointmentItem appt)
        {
            bool result = false;

            //sync attachments
            try
            {
                if (appt.Attachments.Count > 0)
                {
                    foreach (Outlook.Attachment att in appt.Attachments)
                    {
                        bool addAttach = true;
                        foreach (Document docAtt in theTouch.Documents)
                        {
                            if (docAtt.Primitive == DocumentPrimitive.File && (docAtt.Name == att.DisplayName) || att.FileName.ToLower().IndexOf(docAtt.Name.ToLower()) > -1)
                            {
                                addAttach = false;
                                docAtt.Typename = "SYNCED";
                                break;
                            }
                        }

                        if (addAttach)
                        {
                            //write locally
                            string localpath = Path.GetTempFileName();
                            //Debug.WriteLine(localpath);
                            att.SaveAsFile(localpath);
                            byte[] bFile = File.ReadAllBytes(localpath);
                            string strFile64 = Convert.ToBase64String(bFile);

                            Document doc = DataAPI.PostAttachment(new DataAPI.AttachmentPost() { attachmentBase64String = strFile64, contentType = "", filename = att.FileName, touchId = theTouch.Id });
                            doc.changeType = ChangeType.Update;
                            doc.Typename = "SYNCED";
                            theTouch.Documents.Add(doc);

                            try { File.Delete(localpath); }
                            catch { }
                        }
                    }

                }

                //any touches to delete?
                for (int i = theTouch.Documents.Count - 1; i >= 0; i--)
                {
                    if (theTouch.Documents[i].Primitive == DocumentPrimitive.File && theTouch.Documents[i].Typename != "SYNCED")
                    {
                        DataAPI.DeleteDocument(theTouch.Documents[i].Id);
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                HandleError(ex, "RefreshAttachmentsTouchFromAppt");
            }
            return result;
        }

        private bool RefreshAttachmentsApptFromTouch(Touch theTouch, Outlook.AppointmentItem appt)
        {
            bool result = false;
            try
            {
                List<Outlook.Attachment> syncedAtts = new List<Outlook.Attachment>();
                Outlook.Attachments attachments = appt.Attachments;

                foreach (Document docAtt in theTouch.Documents)
                {
                    if (docAtt.Primitive == DocumentPrimitive.File)
                    {
                        bool addAttach = true;
                        foreach (Outlook.Attachment att in attachments)
                        {
                            if (docAtt.Name == att.DisplayName)
                            {
                                addAttach = false;
                                syncedAtts.Add(att);
                                break;
                            }
                        }
                        if (addAttach)
                        {
                            //save file from storage here.
                            string localpath = DataAPI.GetFile(docAtt.Value);
                            if (localpath != "error")
                            {
                                Outlook.Attachment attNew = attachments.Add(localpath, Outlook.OlAttachmentType.olByValue, 1, docAtt.Name);
                                var blockLev = attNew.BlockLevel;
                                syncedAtts.Add(attNew);
                            }
                        }
                    }
                }

                //any appts to delete?
                for (int i = attachments.Count; i > 0; i--)
                {
                    //Debug.WriteLine(attachments.Count);
                    //Debug.WriteLine(i);
                    //Debug.WriteLine(attachments[1]);
                    //Debug.WriteLine(attachments[0]);
                    //Debug.WriteLine(attachments[i - 1]);
                    Outlook.Attachment att = attachments[i];
                    bool isSynced = false;
                    foreach (Outlook.Attachment attSynced in syncedAtts)
                    {
                        if (attSynced.FileName == att.FileName)
                        {
                            isSynced = true;
                            break;
                        }
                    }
                    if (!isSynced) appt.Attachments.Remove(i);
                }

                result = true;
            }
            catch (Exception ex)
            {
                HandleError(ex, "RefreshAttachmentsApptFromTouch");
            }
            return result;
        }

        internal bool DeleteAppointment(Outlook.AppointmentItem appt)
        {
            try
            {
                Touch tch = GetTouchForItem(appt);
                if (tch.Id != Guid.Empty)
                {
                    string msg = string.Format("Delete the synced touch: \n\n{0} \n\n in the app?", tch.Name);
                    DialogResult res = MessageBox.Show(msg, "Synced To Shuri", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (res == DialogResult.Cancel)
                    {
                        return true;
                    }
                    else if (res == DialogResult.Yes)
                    {
                        //delete touch
                        DataAPI.DeleteTouch(tch.Id);
                        UnsyncItem(appt);
                    }
                    else
                    {
                        //break sync
                        UnsyncItem(appt);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "DeleteAppointment");
            }

            return false;  //Cancel
        }

        #endregion

        #region Mail Sync Helpers
        private void RefreshMailFromTouch(Touch tch, Outlook.MailItem mail)
        {
            try
            {
                if (tch != null && tch.Id != Guid.Empty && !mail.Sent)
                {
                    if (tch.Name != "Outlook Email") mail.Subject = tch.Name;
                    mail.Body = tch.Description;

                    //recipients - update only if blank
                    if (tch.People.Count > 0 && (mail.Recipients == null || mail.Recipients.Count == 0))
                    {
                        bool anyRecips = false;
                        foreach (Person per in tch.People)
                        {
                            ContactPoint cpoint = per.ContactPoints.Find(cp => cp.Primitive == ContactPointPrimitive.Email && !String.IsNullOrWhiteSpace(cp.Name));
                            if (cpoint != null && !String.IsNullOrWhiteSpace(cpoint.Name) && DataAPI.IsValidEmail(cpoint.Name))
                            {
                                string addr = cpoint.Name;
                                string eml = string.Format("{0} <{1}>", per.Name.Replace("(", "").Replace(")", ""), addr);
                                Outlook.Recipient recip = mail.Recipients.Add(eml);
                                recip.Resolve();
                                anyRecips = true;
                                break;
                            }
                        }
                        bool autoaddorg = (DataAPI.UserPreferences.ContainsKey("autoaddorg") && Convert.ToBoolean(DataAPI.UserPreferences["autoaddorg"]));
                        if (!autoaddorg)
                        {
                            foreach (Group org in tch.Groups.FindAll(g => g.GrpType == GroupType.Organization))
                            {
                                string addr = org.ContactPoints.Find(cp => cp.Primitive == ContactPointPrimitive.Email && !String.IsNullOrWhiteSpace(cp.Name)).Name;
                                if (!String.IsNullOrWhiteSpace(addr) && DataAPI.IsValidEmail(addr))
                                {
                                    string eml = string.Format("{0} <{1}>", org.Name.Replace("(", "").Replace(")", ""), addr);
                                    Outlook.Recipient recip = mail.Recipients.Add(eml);
                                    anyRecips = true;
                                    break;
                                }
                            }
                        }

                        if (anyRecips) mail.Recipients.ResolveAll();
                    }

                    mail.Save();
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "RefreshMailFromTouch");
            }
        }

        public void RefreshTouchFromMail(Touch theTouch, Outlook.MailItem mail)
        {
            try
            {
                if (theTouch.Id == Guid.Empty || !mail.Sent)
                {

                    theTouch.changeType = ChangeType.Update;
                    if (mail.SentOn != null && mail.SentOn.Subtract(DateTime.UtcNow).TotalDays < 1) theTouch.DateStart = mail.SentOn;
                    else theTouch.DateStart = DateTime.UtcNow;
                    theTouch.Description = Utilities.CleanWhitespace(mail.Body);

                    if (mail.Subject != null) theTouch.Name = mail.Subject;
                    if (theTouch.Name.Length > 120) theTouch.Name = theTouch.Name.Substring(0, 120);
                    if (String.IsNullOrEmpty(theTouch.Name)) theTouch.Name = "Outlook Email";

                    if (theTouch.UserType_Id == Guid.Empty) theTouch.UserType_Id = Guids.Tch_Email;

                    #region recips
                    foreach (Person p in theTouch.People) p.changeType = ChangeType.Remove;
                    foreach (Group grp in theTouch.Groups) grp.changeType = ChangeType.Remove;

                    Debug.WriteLine("mail recip cnt: " + mail.Recipients.Count);
                    //Debug.WriteLine("is saved " + mail.Saved);

                    if (mail.Recipients.Count > 0 || !string.IsNullOrWhiteSpace(mail.To)) mail.Recipients.ResolveAll();

                    List<string> recipsList = new List<string>();

                    if (mail.Recipients.Count > 0)
                    {

                        Outlook.Recipients recips = mail.Recipients;

                        Outlook.Recipient recip = null;
                        for (int i = 1; i <= recips.Count; i++)
                        {
                            recip = recips[i];
                            string smtp = SMTPAddress(recip.AddressEntry);
                            if (!string.IsNullOrWhiteSpace(smtp)) recipsList.Add(smtp);
                            recip = null;
                        }
                        recips = null;
                    }

                    //participants embedded in the email?
                    var extracted = Utilities.EmailAddressesInString(mail.Body);
                    Debug.WriteLine("extracted recip cnt: " + extracted.Count);
                    foreach (string emailAddress in extracted)
                    {
                        if (!recipsList.Contains(emailAddress)) recipsList.Add(emailAddress);
                    }

                    //from??
                    string fromAddr = SMTPAddress(mail.Sender);
                    if (!recipsList.Contains(fromAddr)) recipsList.Add(fromAddr);

                    List<string> unknownList = new List<string>();

                    foreach (string sRecip in recipsList)
                    {
                        bool foundIt = false;
                        foreach (Person p in theTouch.People)
                        {
                            if ((p.Id == DataAPI.TheUser.Id && sRecip == DataAPI.TheUser.EmailAddress) || p.ContactPoints.Find(cp => cp.Name.ToLower() == sRecip.ToLower()) != null)
                            {
                                foundIt = true;
                                p.changeType = ChangeType.None;
                                break;
                            }
                        }
                        foreach (Group g in theTouch.Groups)
                        {
                            if (g.ContactPoints.Find(cp => cp.Name.ToLower() == sRecip.ToLower()) != null)
                            {
                                foundIt = true;
                                g.changeType = ChangeType.None;
                                break;
                            }
                        }
                        if (!foundIt) unknownList.Add(sRecip);
                    }



                    if (unknownList.Count > 0)
                    {
                        Touch resolverTch = DataAPI.Resolver(unknownList);
                        foreach (Person per in resolverTch.People)
                        {
                            if (theTouch.People.Find(person => person.Id == per.Id) == null)
                            {
                                per.changeType = ChangeType.Update;
                                theTouch.People.Add(per);
                            }
                        }
                        foreach (Group grp in resolverTch.Groups)
                        {
                            if (theTouch.Groups.Find(group => group.Id == grp.Id) == null)
                            {
                                grp.changeType = ChangeType.Update;
                                theTouch.Groups.Add(grp);
                            }
                        }
                        foreach (string str in resolverTch.resolveStrings)
                        {
                            if (theTouch.resolveStrings.Find(rStr => rStr == str) == null) theTouch.resolveStrings.Add(str);
                        }
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "RefreshTouchFromEmail");
            }
        }

        private bool RefreshAttachmentsTouchFromMail(Touch theTouch, Outlook.MailItem email)
        {
            bool result = false;

            //sync attachments
            try
            {
                if (email.Attachments.Count > 0)
                {
                    foreach (Outlook.Attachment att in email.Attachments)
                    {
                        if (!IsNoisyAttachment(att))
                        {
                            bool addAttach = true;
                            foreach (Document docAtt in theTouch.Documents)
                            {
                                if (docAtt.Primitive == DocumentPrimitive.File && ((docAtt.Name == att.DisplayName) || att.FileName.ToLower().IndexOf(docAtt.Name.ToLower()) > -1))
                                {
                                    addAttach = false;
                                    docAtt.Typename = "SYNCED";
                                    break;
                                }
                            }

                            if (addAttach)
                            {
                                //write locally
                                string localpath = Path.GetTempFileName();
                                //Debug.WriteLine(localpath);
                                att.SaveAsFile(localpath);
                                byte[] bFile = File.ReadAllBytes(localpath);
                                string strFile64 = Convert.ToBase64String(bFile);

                                Document doc = DataAPI.PostAttachment(new DataAPI.AttachmentPost() { attachmentBase64String = strFile64, contentType = "", filename = att.FileName, touchId = theTouch.Id });
                                doc.changeType = ChangeType.Update;
                                doc.Typename = "SYNCED";
                                theTouch.Documents.Add(doc);

                                try { File.Delete(localpath); }
                                catch { }
                            }
                        }
                    }

                }

                //any touches to delete?
                for (int i = theTouch.Documents.Count - 1; i >= 0; i--)
                {
                    if (theTouch.Documents[i].Primitive == DocumentPrimitive.File && theTouch.Documents[i].Typename != "SYNCED")
                    {
                        DataAPI.DeleteDocument(theTouch.Documents[i].Id);
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                HandleError(ex, "RefreshAttachmentsTouchFromEmail");
            }
            return result;
        }

        internal bool DeleteEmail(Outlook.MailItem email)
        {
            try
            {
                Touch tch = GetTouchForItem(email);
                if (tch.Id != Guid.Empty)
                {
                    string msg = string.Format("Delete the synced touch named \"{0}\"  in the app?", tch.Name);
                    DialogResult res = MessageBox.Show(msg, "Synced To Shuri", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (res == DialogResult.Cancel)
                    {
                        return true;
                    }
                    else if (res == DialogResult.Yes)
                    {
                        //delete touch
                        DataAPI.DeleteTouch(tch.Id);
                        UnsyncItem(email);
                    }
                    else
                    {
                        //break sync
                        UnsyncItem(email);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "DeleteEmail");
            }
            return false;  //Cancel
        }

        #endregion

        #region Sync Helpers
        private bool IsNoisyAttachment(Outlook.Attachment att)
        {
            bool result = false;

            if (att.FileName.Length > 5 && att.FileName.ToLower().Substring(0, 5) == "image") result = true;

            return result;
        }

        private string SMTPAddress(Outlook.AddressEntry ae)
        {
            string smtp = "";
            try
            {
                string x = (ae == null) ? "null" : ae.AddressEntryUserType.ToString();
                if ((ae.AddressEntryUserType == Outlook.OlAddressEntryUserType.olExchangeUserAddressEntry) ||
                    (ae.AddressEntryUserType == Outlook.OlAddressEntryUserType.olExchangeRemoteUserAddressEntry))
                {
                    Outlook.ExchangeUser exUser = null;
                    exUser = ae.GetExchangeUser();
                    if (exUser != null)
                    {
                        smtp = exUser.PrimarySmtpAddress;
                    }
                    exUser = null;
                }
                else
                {
                    smtp = ae.Address;
                }
            }
            catch { }
            return smtp;

        }

        public List<Outlook.MAPIFolder> GetSubFolders(Outlook.MAPIFolder mapiFolder)
        {
            List<Outlook.MAPIFolder> list = new List<Outlook.MAPIFolder>();
            foreach (Outlook.MAPIFolder folder in mapiFolder.Folders)
            {
                list.Add(folder);
                list.AddRange(GetSubFolders(folder));
            }
            return list;
        }

        public SyncItem GetSyncItem(Touch tch)
        {
            SyncItem syncItem = new SyncItem();
            if (tch != null)
            {
                Document docCalSync = tch.Documents.Find(d => d.UserType_Id == Guids.Doc_CalSync);
                if (docCalSync != null)
                {
                    //get the existing syncItem
                    try
                    {
                        syncItem = JsonConvert.DeserializeObject<SyncItem>(docCalSync.Value);
                    }
                    catch (Exception ex)
                    {
                        HandleError(ex, "GetSyncItem");
                    }
                }
            }

            return syncItem;
        }

        public void CaptureEml(Outlook.MailItem mail, Touch touch)
        {
            //capture copy
            string friendly = (string.IsNullOrWhiteSpace(mail.Subject) ? "original.pdf" : Utilities.CleanFriendlyname(mail.Subject) + ".pdf");
            string localfilename = string.Format("{0}\\{1}.mhtml", Path.GetTempPath(), Path.GetRandomFileName());
            string filename = string.Format("{0}.pdf", Path.GetRandomFileName());

            Document docExist = touch.Documents.Find(d => d.UserType_Id == Guids.Doc_EmailAttachment && d.Value.IndexOf(filename) > -1);
            if (docExist != null) docExist.changeType = ChangeType.Remove;


            mail.SaveAs(localfilename, Outlook.OlSaveAsType.olMHTML);

            Stream mhtmlStream = File.OpenRead(localfilename);
            byte[] fileBytes = new byte[mhtmlStream.Length];
            mhtmlStream.Read(fileBytes, 0, (int)mhtmlStream.Length);
            BinaryFilePost bfp = new BinaryFilePost()
            {
                contentType = "convertMHTMLtoPDF",
                collection_Id = touch.Collection_Id,
                fileBytes = fileBytes,
                filename = filename,
                friendlyName = friendly,
                userType_Id = Guids.Doc_EmailAttachment,
                ownedBy_Id = DataAPI.TheUser.Id,
                ownedByGroup_Id = touch.OwnedByGroup_Id
            };

            Document doc = DataAPI.PostBinaryFile(bfp);
            if (doc.Id != Guid.Empty) DataAPI.PostRelationship(new RelationshipPost()
            {
                entityType1 = EntityTypes.Touch,
                entityId1 = touch.Id,
                entityType2 = EntityTypes.Document,
                entityId2 = doc.Id
            });


        }

        public void UnsyncItem(object outlookItem)
        {

            Guid touchId = TouchId(outlookItem);

            if (touchId != Guid.Empty)
            {
                DataAPI.BreakSync(touchId);
                RemoveTouchId(outlookItem);
            }

        }

        internal Touch GetTouchForItem(object outlookItem)
        {
            Touch tch = new Touch();
            if (DataAPI.Online)
            {
                Guid touchId = Guid.Empty;
                Outlook.MailItem mail = null;
                Outlook.AppointmentItem appt = null;

                if (outlookItem is Outlook.MailItem)
                {
                    mail = (outlookItem as Outlook.MailItem);
                    touchId = TouchId(mail);
                    if (touchId != Guid.Empty) tch = DataAPI.GetTouch(touchId);
                    else tch = NewTouch(Outlook.OlItemType.olMailItem);

                }
                else if (outlookItem is Outlook.AppointmentItem || outlookItem is Outlook.MeetingItem)
                {
                    appt = (outlookItem as Outlook.AppointmentItem);
                    touchId = TouchId(appt);
                    if (touchId != Guid.Empty) tch = DataAPI.GetTouch(touchId);
                    else tch = NewTouch(Outlook.OlItemType.olAppointmentItem);

                }
                mail = null;
                appt = null;

            }

            return tch;
        }

        #endregion


        #region Outlook event handlers
        private void _folder_BeforeItemMove(object Item, Outlook.MAPIFolder MoveTo, ref bool Cancel)
        {
            try
            {
                Debug.WriteLine("_folder_BeforeItemMove");
                Guid touchId = Guid.Empty;
                DateTime dtNow = DateTime.UtcNow;

                if (Item is Outlook.AppointmentItem)
                {
                    Outlook.AppointmentItem appt = Item as Outlook.AppointmentItem;
                    if (appt != null)
                    {
                        // (MoveTo == null) = hard delete
                        if ((MoveTo == null) || (IsDeletedItemsFolder(MoveTo)))
                        {
                            Cancel = DeleteAppointment(appt);
                        }
                        else
                        {
                            Touch tch = GetTouchForItem(appt);
                            if (tch.Id != Guid.Empty) CompareAndSync(tch, appt);
                            Cancel = false;
                        }

                        appt = null;
                    }
                }
                else if (Item is Outlook.MailItem)
                {
                    Outlook.MailItem mail = Item as Outlook.MailItem;
                    if (mail != null)
                    {
                        if ((MoveTo == null) || (IsDeletedItemsFolder(MoveTo)))
                        {
                            Cancel = DeleteEmail(mail);
                        }
                        else
                        {
                            Touch tch = GetTouchForItem(mail);
                            if (tch.Id != Guid.Empty) CompareAndSync(tch, mail);
                            Cancel = false;
                        }

                        mail = null;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "_folder_BeforeItemMove");
            }
        }

        //private void Appt_PropertyChange(string Name)
        //{
        //    Debug.WriteLine("Appt PropChg: " + Name);
        //    switch (Name)
        //    {
        //        case "StartUTC":
        //        case "EndUTC":
        //            ExplorerItemUpdate();
        //            break;
        //    }
        //}

        //private void ExplorerItemUpdate()
        //{
        //    if (Application.ActiveExplorer().Selection != null && Application.ActiveExplorer().Selection.Count > 0)
        //    {
        //        Outlook.MailItem mail = null;
        //        Outlook.AppointmentItem appt = null;
        //        string entryID = "";

        //        if (Application.ActiveExplorer().Selection[1] is Outlook.MailItem)
        //        {
        //            mail = Application.ActiveExplorer().Selection[1] as Outlook.MailItem;
        //            entryID = mail.EntryID;
        //        }
        //        else if (Application.ActiveExplorer().Selection[1] is Outlook.AppointmentItem)
        //        {
        //            appt = Application.ActiveExplorer().Selection[1] as Outlook.AppointmentItem;
        //            entryID = appt.EntryID;
        //        }

        //        Touch tch = GetTouchByEntryId(entryID);
        //        if (tch.Id != Guid.Empty)
        //        {
        //            if (appt != null) CompareAndSync(tch, appt);
        //            else if (mail != null) CompareAndSync(tch, mail, false);
        //        }
        //    }

        //}

        private void _explorer_BeforeFolderSwitch(object NewFolder, ref bool Cancel)
        {

            try
            {
                //Debug.WriteLine("_explorer_BeforeFolderSwitch");
                Outlook.MAPIFolder folder = NewFolder as Outlook.MAPIFolder;
                if (folder != null)
                {
                    if (folder.DefaultItemType == Outlook.OlItemType.olAppointmentItem || folder.DefaultItemType == Outlook.OlItemType.olMailItem)
                    {
                        // Note: BeforeItemMove() only fires in default store, not in secondary stores
                        _folder.BeforeItemMove -= _folder_BeforeItemMove; // remove old BeforeItemMove handler
                        _folder = NewFolder as Outlook.Folder; // set new Folder object
                        _folder.BeforeItemMove += _folder_BeforeItemMove; // add new BeforeItemMove handler

                    }
                    folder = null;
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "_explorer_BeforeFolderSwitch");
            }
        }

        private void Application_AdvancedSearchComplete(Outlook.Search SearchObject)
        {
            Debug.WriteLine(string.Format("Search Object Type {0} Scope {1} and Count {2}", SearchObject.Tag, SearchObject.Scope, SearchObject.Results.Count));
            if (SearchObject.Results.Count > 0)
            {
                switch (SearchObject.Tag)
                {
                    case "MailSync":
                        foreach (var res in SearchObject.Results)
                        {
                            if (res is Outlook.MailItem)
                            {
                                Outlook.MailItem mail = res as Outlook.MailItem;
                                if (!mail.Sent)
                                {

                                    Touch touch = GetTouchForItem(mail);
                                    if (touch.Id != Guid.Empty)
                                    {
                                        CompareAndSync(touch, mail);
                                        //Debug.WriteLine(string.Format(" \n\nmail: {0} ", mail.Subject));
                                    }
                                    else
                                    {
                                        //we have touchId but no backing touch?
                                        Guid id = TouchId(mail);
                                        if (!DataAPI.EntityExists(id, EntityTypes.Touch))
                                        {
                                            //clear id from mail
                                            RemoveTouchId(mail);
                                        }
                                    }
                                }
                                mail = null;
                            }
                        }
                        break;
                    case "ApptSync":
                        foreach (var res in SearchObject.Results)
                        {
                            if (res is Outlook.AppointmentItem)
                            {
                                Outlook.AppointmentItem appt = res as Outlook.AppointmentItem;
                                Touch touch = GetTouchForItem(appt);
                                if (touch.Id != Guid.Empty)
                                {
                                    CompareAndSync(touch, appt);
                                    //Debug.WriteLine("appt: " + appt.Subject);
                                }
                                else
                                {
                                    //we have touchId but no backing touch?
                                    Guid id = TouchId(appt);
                                    if (!DataAPI.EntityExists(id, EntityTypes.Touch))
                                    {
                                        //clear id 
                                        RemoveTouchId(appt);
                                    }
                                }
                                appt = null;
                            }

                        }
                        break;
                }

            }
            _syncDeepSearchesCompleted++;

            if (_syncDeepSearchesStarted == _syncDeepSearchesCompleted)
            {
                Cursor.Current = Cursors.Default;
                Debug.WriteLine(string.Format("Synced {0} folders", _syncDeepSearchesStarted));
                //MessageBox.Show(string.Format("Synced {0} folders", _syncDeepSearchesStarted), "Sync Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                foreach (var rib in Globals.Ribbons)
                {
                    if (rib.GetType() == typeof(ribbonExplorer))
                    {
                        ((ribbonExplorer)rib).SyncAllComplete(string.Format("Synced {0} folders", _syncDeepSearchesStarted));
                    }
                }
            }


        }

        private void _inspectors_NewInspector(Outlook.Inspector Inspector)
        {
            try
            {

                // need to add Inspector wrapper for cases where Inspector.CurrentItem == appointment
                // add other handlers in Inspector.Activate() handler. Object references are weak until then and not fully hydrated. In Activate() add Write() [Save] handler for Item.
                // also need to add Item.Send and Item.Close handlers.
            }
            catch (Exception ex)
            {
                HandleError(ex, "_inspectors_NewInspector");

            }
        }

        private bool IsDeletedItemsFolder(Outlook.MAPIFolder folder)
        {
            bool ret = false;

            try
            {
                ret = _nameSpace.CompareEntryIDs(folder.EntryID, _deletedFolder.EntryID)
                        || folder.Name == "Trash";
            }
            catch (Exception ex)
            {
                HandleError(ex, "IsDeletedItemsFolder");
            }

            return ret;
        }

        #endregion

        #region Exception handlers
        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            try
            {
            }
            catch
            {
                // intentionally do nothing
            }
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
            }
            catch
            {
                // intentionally do nothing
            }
        }
        public void HandleError(string source, Exception ex)
        {
            HandleError(ex, source);
        }
        public void HandleError(string source, string errorMsg)
        {
            HandleError(new Exception(errorMsg), source);
        }
        public void HandleError(Exception ex, string source)
        {
            Debug.WriteLine("-------------------------------" + source);
            Debug.WriteLine("Error" + ex.Message);
            Debug.WriteLine("Stacktrace: " + ex.StackTrace);

        }
        #endregion

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion

        #region Old Code

        //private Outlook.Items GetCalItemsInRange(Outlook.Folder folder, DateTime startTime, DateTime endTime)
        //{
        //    string filter = "[Start] >= '"
        //        + startTime.ToString("g")
        //        + "' AND [End] <= '"
        //        + endTime.ToString("g") + "'";
        //    //Debug.WriteLine(filter);
        //    try
        //    {
        //        Outlook.Items theItems = folder.Items;
        //        theItems.IncludeRecurrences = true;
        //        theItems.Sort("[Start]", Type.Missing);
        //        Outlook.Items restrictItems = theItems.Restrict(filter);
        //        if (restrictItems.Count > 0)
        //        {
        //            return restrictItems;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch { return null; }
        //}
        //private Outlook.Items GetEmailItemsInRange(Outlook.Folder folder, DateTime startTime, DateTime endTime)
        //{
        //    string filter = "([SentOn] >= '"
        //        + startTime.ToString("g")
        //        + "' AND [SentOn] <= '"
        //        + endTime.ToString("g") + "') ";
        //    filter += "OR ([ReceivedTime] >= '"
        //        + startTime.ToString("g")
        //        + "' AND [ReceivedTime] <= '"
        //        + endTime.ToString("g") + "')";
        //    //Debug.WriteLine(filter);
        //    try
        //    {
        //        Outlook.Items theItems = folder.Items;
        //        Debug.WriteLine(theItems.Count);
        //        theItems.IncludeRecurrences = true;
        //        theItems.Sort("[SentOn]", Type.Missing);
        //        Outlook.Items restrictItems = theItems.Restrict(filter);
        //        if (restrictItems.Count > 0)
        //        {
        //            return restrictItems;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch { return null; }
        //}

        //private void KensSearchComplete(Outlook.Search SearchObject)
        //{
        //    // logic need to be extended to account for items already deleted
        //    // need to send CalSyncLast after all synching complete
        //    try
        //    {
        //        Touch tch = new Touch();

        //        Outlooks results = SearchObjects;
        //        if (results.Count > 0)
        //        {
        //            tch = _touchesToSync.Find(x => x.Id.ToString() == SearchObject.Tag);
        //            if (tch != null)
        //            {
        //                // this is anticipating only 1 item in the result
        //                Outlook.AppointmentItem appt = results.GetFirst();
        //                appt.StartUTC = tch.DateStart;
        //                if (tch.DateEnd != null)
        //                {
        //                    appt.EndUTC = (DateTime)tch.DateEnd;
        //                }
        //                appt.Subject = tch.Name;
        //                appt.Body = tch.Description;

        //                Utilities.ProcessApptTouchLink(appt);

        //                appt.Save();

        //                appt = null;
        //            }

        //            //TODO this should "unsync" NOT delete
        //            tch = _touchesToDele.Find(x => x.Id.ToString() == SearchObject.Tag);
        //            if (tch != null)
        //            {
        //                Outlook.AppointmentItem appt = results.GetFirst();
        //                if (appt != null)
        //                {
        //                    //appt.Delete();

        //                    appt = null;
        //                }

        //            }
        //        }
        //        else
        //        {
        //            // appointment doesn't exist, create it
        //            tch = _touchesToSync.Find(x => x.Id.ToString() == SearchObject.Tag);
        //            if (tch != null)
        //            {
        //                Outlook.AppointmentItem appt = Application.CreateItem(Outlook.OlItemType.olAppointmentItem);
        //                appt.StartUTC = tch.DateStart;
        //                appt.EndUTC = (DateTime)tch.DateEnd;
        //                appt.Subject = tch.Name;
        //                appt.Body = tch.Description;

        //                Outlook.UserProperties props = appt.UserProperties;

        //                Outlook.UserProperty prop = props.Find(Properties.Resources.ShuriTouchID, true);
        //                if (prop == null)
        //                {
        //                    prop = props.Add(Properties.Resources.ShuriTouchID, Outlook.OlUserPropertyType.olText, false);
        //                    prop.Value = SearchObject.Tag;
        //                }

        //                prop = null;

        //                prop = props.Find(Properties.Resources.ShuriTouchID, true);
        //                if (prop == null)
        //                {
        //                    prop = props.Add(Properties.Resources.ShuriTouchID, Outlook.OlUserPropertyType.olText, false);
        //                    prop.Value = SearchObject.Tag;
        //                }

        //                prop = null;

        //                prop = props.Find(Properties.Resources.ShuriLastSync, true);
        //                if (prop == null)
        //                {
        //                    prop = props.Add(Properties.Resources.ShuriLastSync, Outlook.OlUserPropertyType.olDateTime, false);
        //                    prop.Value = DateTime.UtcNow;
        //                }

        //                prop = null;

        //                props = null;

        //                appt.Save();

        //                appt = null;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(ex.Message);
        //    }

        //}

        #endregion
    }
}
