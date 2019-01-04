using System;
using System.Windows.Forms;
using System.Drawing;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace ShuriOutlookAddIn
{
    public class API_Interfaces
    {
        // REST Call URI's
        public static string Login = "login"; // POST
        public static string SyncTouches = "syncTouches"; // POST
        public static string DeletedSyncs = "deletedSyncs"; // GET
        public static string Document = "document"; // GET
        public static string Documents = "documents"; // GET & POST
        public static string CheckAuth = "checkauth"; // GET
        public static string Touch = "touch?fullRecord=true&id={0}"; // GET
        public static string Person = "person?id={0}"; // GET 
        public static string Organization = "Organization?id={0}"; // GET 
        public static string GetTouches = "touches"; // GET 
        public static string PostBinaryFile = "BinaryFile";
        public static string PostCP = "ContactPoints";
        public static string PostGroup = "Groups";
        public static string PostLocation = "Locations";
        public static string PostPerson = "People";
        public static string PostTouches = "Touches"; 
        public static string Relationship = "Relationship";
        public static string TouchTypes = "userTypesFor?mode=touch"; // GET
        public static string Tags = "userTypesTags?entityType=-1"; // GET
        public static string AppUser = "RefreshAppuser"; // GET
        public static string UserPrefs = "documents?usertypeId=507CB5DE-DB0F-453A-A0E9-28EE3B99FCC4&page=1&pagesize=200"; // POST
        public static string ResolveEmailStrings = "ResolveEmailStrings"; //POST
        public static string EmailAttachment = "EmailAttachment"; //POST
        public static string OutlookAddinVersion = "OutlookAddinVersion"; 
        public static string PostEmailConversation = "PostEmailConversation"; //POST

        // these need formatting
        public static string AssociateEmailPost = "AssociateEmail?entityId={0}&entityType={1}&email={2}"; // POST
        public static string CalSyncLast = "userPreference?name=calsynclast&value={0}"; // POST
        public static string DeleteDocument = "document?id={0}"; // DELETE
        public static string DeleteLocation = "LocationSafe?id={0}"; // DELETE   Guid entityId1, EntityTypes entityType1, Guid entityId2, EntityTypes entityType2
        public static string DeleteTouch = "touch?id={0}"; // DELETE
        public static string DeleteRelationship = "Relationship?entityId1={0}&entityType1={1}&entityId2={2}&entityType2={3}"; // DELETE
        public static string EditTeamsDB = "EditTeamsDB?dbId={0}"; // GET
        public static string EmailForEntity = "EmailForEntity?entityId={0}&entityType={1}"; // GET
        public static string Group = "group?id={0}"; // GET
        public static string GroupForEmail = "group?forEmail=true&id={0}"; // GET
        public static string ResolveEmailAddress = "ResolveEmailAddress?address={0}"; //GET
        public static string UsernameOK = "UsernameOK?username={0}"; // GET
        public static string Pageview = "Pageview?page={0}&entityType={1}&entityId={2}";
        public static string UserPreferencePost = "userPreference?name={0}&value={1}"; // GET
        public static string TagsAutocomplete = "AutocompleteByEntity?entityType=5&noRecs=10&forEntityType=6&prefix={0}"; // GET
        public static string AutocompleteEmailRecipients = "AutocompleteEmailRecipients?prefix={0}&noRecs={1}"; // GET
        public static string AutocompletePeopleOrgs = "AutocompletePeopleOrgs?prefix={0}&noRecs=10";
    }

    public class Guids
    {
        public static Guid CP_Email = Guid.Parse("6C306E29-6702-46E7-A789-02BD144FA6FB");
        public static Guid Doc_CalSync = Guid.Parse("D130159F-7B11-4E7B-AFFF-13D05CE40C09");
        public static Guid Doc_CalSyncRemoval = Guid.Parse("E7E4E7F0-B575-4B28-B736-0D313B1A0A9B");
        public static Guid Doc_EmailAttachment = Guid.Parse("A6C2C03B-02D7-4E33-9ED1-62BDD1BFECC5");
        public static Guid Loc_Business = Guid.Parse("FC5B3E5D-9F76-44DF-8CBB-E0B000F114A6");
        public static Guid Tag_Loose = Guid.Parse("A2E53FB1-8120-4A90-9422-0D5A3B3C959D");
        public static Guid Tch_Appointment = Guid.Parse("78B7943D-0B23-4443-ADD7-C467CCF30455");
        public static Guid Tch_Email = Guid.Parse("4136ACB4-14BE-425C-98F0-B9A560C86742");
        public static Guid Tch_Meeting = Guid.Parse("B9FAD172-3914-4F86-BD7C-B7CE664F0F26");
        public static Guid System = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");

    }
    public class RegKeys
    {
        public static string DefaultDB = "DefaultDB";
        public static string DefaultOG = "DefaultOG";
        public static string TouchTypeAppt = "TouchTypeAppt";
        public static string TouchTypeMail = "TouchTypeMail";
        public static string ProxyHost = "ProxyHost";
        public static string ProxyPort = "ProxyPort";
        public static string ProxyUser = "ProxyUser";
        public static string ProxyPass = "ProxyPass";
        public static string UseProxy = "UseProxy";

        public static void Clear()
        {
            Utilities.DeleteRegKey(DefaultDB);
            Utilities.DeleteRegKey(DefaultOG);
            Utilities.DeleteRegKey(TouchTypeAppt);
            Utilities.DeleteRegKey(TouchTypeMail);
            Utilities.DeleteRegKey(UseProxy);
        }
    }

    public class AppColors
    {
        public static Color personBGLight = ColorTranslator.FromHtml("#F8EEDD");
        public static Color orgBGLight = ColorTranslator.FromHtml("#EEE8D9");
        public static Color personDark = Color.DarkOrange;
        public static Color orgDark = Color.DarkGoldenrod;
        public static Color tagBGLight = Color.FromArgb(243, 241, 249);
        public static Color tagDark = Color.Indigo;
    }
    public class ListPanel : Panel
    {
        public ListPanel(AutocompleteResult entity, Color bgColor, System.EventHandler onClick)
        {
            this.Size = new Size(490, 48);
            if (bgColor != null) this.BackColor = bgColor;

            PictureBox avatar = new PictureBox() { Location = new Point(6, 6), SizeMode= PictureBoxSizeMode.AutoSize };
            avatar.Image = Properties.Resources.icon_person42; 
            avatar.Cursor = Cursors.Hand;
            avatar.Click += onClick;
            avatar.Tag = entity;
            avatar.Paint += Avatar_Paint;
            this.Controls.Add(avatar);

            Label lbl = new Label() { Text = entity.Name, Location = new Point(60, 16), AutoSize = true };
            lbl.Font = new Font("Arial", 10, FontStyle.Regular);
            lbl.Click += onClick;
            lbl.Cursor = Cursors.Hand;
            lbl.Tag = entity;
            this.Controls.Add(lbl);

        }

        private void Avatar_Paint(object sender, PaintEventArgs e)
        {
            PictureBox avatar = (PictureBox)sender;
            AutocompleteResult entity = (AutocompleteResult)avatar.Tag;
            if (avatar.Image == Properties.Resources.icon_person42 && entity.ImageUrlThumb.IndexOf("http") > -1) avatar.Image = Utilities.AvatarFromUrl(entity.ImageUrlThumb);

        }
    }

    //public class DelayWriteObj
    //{
    //    public DelayWriteObj()
    //    {
    //        ItemType = Outlook.OlItemType.olAppointmentItem;
    //        EntryID = "";
    //        ModifiedDt = DateTime.MinValue;
    //    }
    //    public Outlook.OlItemType ItemType { get; set; }
    //    public Guid TouchId { get; set; }
    //    public string EntryID { get; set; }
    //    public bool IsRemove { get; set; }
    //    public DateTime ModifiedDt { get; set; }
    //}
    public class RelationshipPost
    {
        public EntityTypes entityType1 { get; set; }
        public Guid entityId1 { get; set; }
        public EntityTypes entityType2 { get; set; }
        public Guid entityId2 { get; set; }
    }
    public class BinaryFilePost
    {
        public byte[] fileBytes { get; set; }
        public string filename { get; set; }
        public string friendlyName { get; set; }
        public string contentType { get; set; }
        public Guid collection_Id { get; set; }
        public Guid userType_Id { get; set; }
        public Guid ownedBy_Id { get; set; }
        public Guid ownedByGroup_Id { get; set; }
    }

    public class AddinSyncItem
    {
        public AddinSyncItem()
        {
            entryId = "";
            touch = new Touch();
        }
        public string entryId { get; set; }
        public Guid touchId { get; set; }
        public Touch touch { get; set; }

    }

    public class SyncItem
    {
        public SyncItem()
        {
            id = platform = title = url = notes = loc = prevTitle = folderName = folderId = "";
            lastSyncAppt = lastSyncTouch = startDate = DateTime.MinValue;
            itemType = Outlook.OlItemType.olJournalItem;
        }
        public string platform { get; set; }
        public Outlook.OlItemType itemType { get; set; }
        public string id { get; set; }
        public DateTime? lastSyncAppt { get; set; }
        public DateTime lastSyncTouch { get; set; }

        //not used in Outlook
        public string folderName { get; set; }
        public string folderId { get; set; }
        public string title { get; set; }
        public DateTime startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string url { get; set; }
        public string notes { get; set; }
        public string loc { get; set; }
        public string prevTitle { get; set; }
        public DateTime? prevStartDate { get; set; }
        public DateTime? prevEndDate { get; set; }
    }

    public class SyncCalendar
    {
        public string name { get; set; }
        public string id { get; set; }
        public string storeId { get; set; }
    }


    public class ShClient : HttpClient
    {

        private int _clientTimeoutMS = 20000;
        public ShClient(string authToken, int msTimeout)
        {
            //if (UseProxy) client = new HttpClient(handler: ProxyHandler, disposeHandler: true);

            this.BaseAddress = new Uri(Globals.ThisAddIn.APIEnv.Url);
            this.Timeout = TimeSpan.FromMilliseconds(msTimeout);
            this.DefaultRequestHeaders.Add("x-api-key", Globals.ThisAddIn.APIEnv.Key);
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        }
        public ShClient(string authToken)
        {
            this.BaseAddress = new Uri(Globals.ThisAddIn.APIEnv.Url);
            this.Timeout = TimeSpan.FromMilliseconds(_clientTimeoutMS);
            if (Debugger.IsAttached) this.Timeout = TimeSpan.FromSeconds(120000);
            this.DefaultRequestHeaders.Add("x-api-key", Globals.ThisAddIn.APIEnv.Key);
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        }
        public ShClient()
        {
            this.BaseAddress = new Uri(Globals.ThisAddIn.APIEnv.Url);
            this.Timeout = TimeSpan.FromMilliseconds(_clientTimeoutMS);
            this.DefaultRequestHeaders.Add("x-api-key", Globals.ThisAddIn.APIEnv.Key);
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

    }


    #region Models
    public class ShuriEnvironment
    {
        public ShuriEnvironment(string name, string url, string key, string baseAppUrl, string baseWWWUrl)
        {
            Name = name;
            Url = url;
            Key = key;
            BaseAppUrl = baseAppUrl;
            BaseWWWUrl = baseWWWUrl;
        }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Key { get; set; }
        public string BaseAppUrl { get; set; }
        public string BaseWWWUrl { get; set; }
    }

    public class CalSync
    {
        public string calendarId { get; set; }
        public DateTime lastSync { get; set; }

    }

    public class EmailConversationPost
    {
        public EmailConversationPost()
        {
            conversationId = "";
            touchIds = new List<Guid>();
        }
        public string conversationId { get; set; }
        public List<Guid> touchIds { get; set; }

    }

    #endregion
}
