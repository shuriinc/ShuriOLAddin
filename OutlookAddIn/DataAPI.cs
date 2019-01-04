using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;
using System.Deployment.Application;

namespace ShuriOutlookAddIn
{
    public class DataAPI
    {
        #region Globals
        private static AppUser _appuser = null;
        private static bool _offline = true;
        private static bool _ready = false;
        private static Dictionary<string, string> _prefs = null;
        private static string _authToken = null;
        private static bool? _useProxy = null;
        private static HttpClientHandler _proxyHandler = null;


        #endregion

        #region Properties

        public static AppUser TheUser
        {
            get
            {
                if (_appuser == null) _appuser = new AppUser();
                return _appuser;
            }
        }

        public static bool DBsFiltered
        {
            get
            {
                bool result = false;
                if (_appuser != null && _appuser.SubscriptionIds.Count > 0)
                {
                    int cntId = 0, cntSub = 0;
                    foreach (Guid id in _appuser.SubscriptionIds) if (id != Guid.Empty && id != Guids.System) cntId++;
                    foreach (Subscription sub in _appuser.Subscriptions) if (sub.Group_Id != Guid.Empty && sub.Group_Id != Guids.System) cntSub++;
                    result = (cntId != cntSub);
                }
                return result;
            }
        }

        internal static bool UseProxy
        {
            get
            {
                if (_useProxy == null)
                {
                    _useProxy = false;
                    try
                    {
                        string uprx = Utilities.ReadRegStringValue(RegKeys.UseProxy);
                        if (!String.IsNullOrWhiteSpace(uprx)) _useProxy = Convert.ToBoolean(uprx);
                    }
                    catch { }
                }
                return (bool)_useProxy;
            }
        }

        internal static HttpClientHandler ProxyHandler
        {
            get
            {
                if (_proxyHandler == null)
                {
                    string proxyHost = "", proxyPortS = "", proxyUn = "", proxyPw = "", passEncrypt = "";
                    proxyHost = Utilities.ReadRegStringValue(RegKeys.ProxyHost);
                    proxyPortS = Utilities.ReadRegStringValue(RegKeys.ProxyPort);
                    proxyUn = Utilities.ReadRegStringValue(RegKeys.ProxyUser);
                    passEncrypt = Utilities.ReadRegStringValue(RegKeys.ProxyPass);

                    if (!String.IsNullOrEmpty(passEncrypt)) proxyPw = Utilities.Decrypt(passEncrypt);

                    int proxyPort = 8080;
                    int.TryParse(proxyPortS, out proxyPort);
                    _proxyHandler = new HttpClientHandler()
                    {
                        Proxy = new WebProxy(string.Format("{0}:{1}", proxyHost, proxyPort), false),
                        PreAuthenticate = true,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(proxyUn, proxyPw),
                    };
                }
                return _proxyHandler;
            }

        }

        //public static HttpClient client.
        //{
        //    return TheHttpClient(_clientTimeout);
        //}
        //    public static HttpClient TheHttpClient(int timeoutMilliseconds)
        //    {
        //        HttpClient client = new HttpClient(); 

        //        if (UseProxy) client = new HttpClient(handler: ProxyHandler, disposeHandler: true);

        //        client.BaseAddress = new Uri(Globals.ThisAddIn.APIEnv.Url);
        //        client.Timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
        //        client.DefaultRequestHeaders.Add("x-api-key", Globals.ThisAddIn.APIEnv.Key);
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);

        //        //if (Debugger.IsAttached) client.Timeout = TimeSpan.FromSeconds(_clientTimeout);

        //        return client;
        //}

        public static Dictionary<string, string> UserPreferences
        {
            get
            {
                if (_prefs != null) return _prefs;
                else
                {
                    SetUser();
                    return _prefs;
                }
            }

        }

        public static bool Online
        {
            get { return !(_offline); }
            set { _offline = !(value); }
        }
        #endregion

        public static bool Ready()
        {
            if (!Globals.ThisAddIn.Enabled) return true;
            else return _ready;
        }

        #region Admin

        public static bool Login(bool withCheck)
        {
            bool result = false;
            if (!Globals.ThisAddIn.LoggingIn)
            {
                bool needsLogin = true;
                Globals.ThisAddIn.LoggingIn = true;
                try
                {
                    string token = Utilities.ReadRegStringValue(Properties.Resources.OAuthToken);
                    if (token != null && withCheck)
                    {
                        //check it
                        SetToken(token);
                        ShClient client = new ShClient(_authToken);

                        try
                        {
                            HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(API_Interfaces.CheckAuth)).Result;
                            {
                                if (response.IsSuccessStatusCode)
                                {
                                    _offline = needsLogin = false;
                                    Globals.ThisAddIn.InitTheSync();
                                }
                                else
                                {
                                    _offline = true;
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            //API off-line
                            _offline = true;
                            needsLogin = false;
                            string msg = "Unable to contact Shuri API\r\n\r\nPlease try again shortly or work offline.";
                            DialogResult res = MessageBox.Show(msg, "Login timed out", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            Globals.ThisAddIn.LoggingIn = false;
                            Debug.WriteLine(ex.Message, ex.StackTrace);
                        }
                        finally
                        {
                            client.Dispose();
                        }

                    }

                    if (needsLogin)
                    {
                        LoginForm login = new LoginForm();
                        login.StartPosition = FormStartPosition.CenterScreen;
                        DialogResult res = login.ShowDialog();
                        switch (res)
                        {
                            case DialogResult.Cancel:
                            case DialogResult.No:
                                _offline = true;
                                break;
                            case DialogResult.Yes:
                            case DialogResult.OK:
                                _offline = false;
                                Globals.ThisAddIn.InitTheSync();
                                Globals.ThisAddIn.RefreshUI();
                                _ready = true;
                                break;
                        }

                    }
                    else _ready = true;
                    result = _ready;
                }
                catch (Exception exOuter)
                {
                    result = _ready;
                    Debug.WriteLine(exOuter.Message, exOuter.StackTrace);
                }

                Globals.ThisAddIn.LoggingIn = false;
            }
            return result;
        }


        public static void SetToken(string token)
        {
            _authToken = token;
        }


        public static bool SetUser()
        {
            AppUser user = new AppUser();
            if (Online)
            {
                ShClient client = new ShClient(_authToken, 120000);
                try
                {
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(API_Interfaces.AppUser)).Result;
                    string respString = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        user = JsonConvert.DeserializeObject<AppUser>(respString);
                    }
                    else
                    {
                        Debug.WriteLine(respString);
                    }
                    _appuser = user;

                    //getuser prefs
                    _prefs = new Dictionary<string, string>();
                    HttpResponseMessage response2 = Task.Run<HttpResponseMessage>(() => client.GetAsync(API_Interfaces.UserPrefs)).Result;

                    if (response2.IsSuccessStatusCode)
                    {
                        string respString2 = response2.Content.ReadAsStringAsync().Result;
                        List<Document> prefs = JsonConvert.DeserializeObject<List<Document>>(respString2);
                        foreach (Document doc in prefs)
                        {
                            _prefs.Add(doc.Name, doc.Value);
                        }
                    }

                }
                catch (Exception ex)
                {
                    HandleError("GetUser", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return true;
        }

        //[Route("Pageview")]
        //[HttpGet]
        //public static HttpResponseMessage Pageview(string page, EntityTypes entityType, Guid entityId)

        public static bool Pageview(string page, EntityTypes entityType, Guid entityId)
        {
            bool res = false;
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    string url = string.Format(API_Interfaces.Pageview, page, entityType, entityId);
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(url)).Result;

                    res = response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    HandleError("Pageview", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return res;
        }
        public static bool PostUserPreference(string prefName, string prefvalue)
        {
            bool res = false;
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    string url = string.Format(API_Interfaces.UserPreferencePost, prefName, prefvalue);
                    StringContent postData = new StringContent("");
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.PostAsync(url, postData)).Result;
                    res = response.IsSuccessStatusCode;
                    if (res) RefreshUser();
                }
                catch (Exception ex)
                {
                    HandleError("PostUserPreference", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return res;
        }

        public static bool Logout()
        {
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync("RefreshAppUser")).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        _appuser = null;
                        _offline = true;
                        Utilities.DeleteRegKey(Properties.Resources.OAuthToken);
                        return true;
                    }

                }
                catch (Exception ex)
                {
                    HandleError("Logout", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return false;
        }


        public static bool IsValidEmail(string emailAddress)
        {
            return Regex.IsMatch(emailAddress, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }

        public static bool RefreshUser()
        {
            _appuser = null;
            SetUser();

            return true;
        }

        public static bool EntityExists(Guid entityId, EntityTypes entityType)
        {
            bool result = false;
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    string url = string.Format("EntityExists?entityId={0}&entityType={1}",entityId, entityType);
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(url)).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string respString = response.Content.ReadAsStringAsync().Result;
                        result = Convert.ToBoolean(respString);
                    }
                }
                catch (Exception ex)
                {
                    HandleError("EntityExists", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }

            return result;
        }

        public static bool ClearDBFilters()
        {
            bool res = false;
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    StringContent postData = new StringContent("");
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.PutAsync("resetSubscriptionIds", postData)).Result;
                    res = response.IsSuccessStatusCode;
                    if (res) RefreshUser();
                }
                catch (Exception ex)
                {
                    HandleError("ClearDBFilters", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return res;
        }



        #endregion

        #region Calendar Syncing

        //public static bool SetCalSync()
        //{
        //    if (Online)
        //    {
        //        ShClient client = new ShClient(_authToken);
        //        try
        //        {
        //            StringContent postData = new StringContent("");
        //            HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.PostAsync(API_Interfaces.CalSync, postData)).Result;

        //            if (response.IsSuccessStatusCode) return true;
        //            else return false;

        //        }
        //        catch (Exception ex)
        //        {
        //            HandleError("SetCalSync", ex);
        //            return false;
        //        }
        //        finally
        //        {
        //            client.Dispose();
        //        }

        //    }
        //    else return false;

        //}

        public static bool BreakSync(Guid touchId)
        {
            bool res = false;
            if (Online)
            {
                Touch tch = GetTouch(touchId);
                foreach (Document doc in tch.Documents)
                {
                    if (doc.UserType_Id == Guids.Doc_CalSync)
                    {
                        DeleteDocument(doc.Id);
                        res = true;
                    }
                }
            }
            return res;
        }

        #endregion

        public static Guid PostCP(ContactPoint cp)
        {
            Guid id = Guid.Empty;
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    StringContent postData = new StringContent(JsonConvert.SerializeObject(cp), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.PostAsync(API_Interfaces.PostCP, postData)).Result;

                    string respString = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        id = Guid.Parse(respString.Replace("\"", ""));
                    }

                }
                catch (Exception ex)
                {
                    HandleError("PostCP", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return id;
        }

        public static bool AssociateEmail(Guid entityId, EntityTypes entityType, string emailAddress)
        {
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    StringContent postData = new StringContent("", Encoding.UTF8, "application/json");
                    string url = string.Format(API_Interfaces.AssociateEmailPost, entityId, entityType, emailAddress);
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.PostAsync(url, postData)).Result;
                    string respString = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return Convert.ToBoolean(respString);
                    }
                }
                catch (Exception ex)
                {
                    HandleError("AssociateEmail", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return false;
        }

        public static string EmailForEntity(Guid entityId, EntityTypes entityType)
        {
            string result = "";
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    string url = string.Format(API_Interfaces.EmailForEntity, entityId, entityType);
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(url)).Result;

                    string msg = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode) result = msg.Replace("\"", "");
                    else result += "Error: " + msg;

                }
                catch (Exception ex)
                {
                    HandleError("EmailForEntity", ex);
                    result += "Error: " + ex.Message;
                }
                finally
                {
                    client.Dispose();
                }

            }

            return result;
        }



        #region Docs
        public static Document GetDocument(Guid id)
        {
            Document doc = new Document();
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    string url = string.Format(API_Interfaces.Document, id);
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(url)).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string respString = response.Content.ReadAsStringAsync().Result;
                        doc = JsonConvert.DeserializeObject<Document>(respString);
                    }

                }
                catch (Exception ex)
                {
                    HandleError("GetDocument", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }

            return doc;
        }

        public static string GetFile(string url)
        {
            string localpath = "error";

            if (Online)
            {
                HttpClient client = new HttpClient();
                try
                {

                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(url)).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] bytes = response.Content.ReadAsByteArrayAsync().Result;
                        //localpath = Path.GetTempFileName();
                        localpath = Application.LocalUserAppDataPath + Path.GetFileName(url);
                        File.WriteAllBytes(localpath, bytes);

                    }

                }
                catch (Exception ex)
                {
                    HandleError("GetFile", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }

            return localpath;
        }


        public static Guid PostDocument(Document document)
        {
            Guid id = Guid.Empty;
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    if (document.OwnedBy_Id == Guid.Empty) document.OwnedBy_Id = TheUser.Id;

                    StringContent postData = new StringContent(JsonConvert.SerializeObject(document), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.PostAsync(API_Interfaces.Documents, postData)).Result;

                    string respString = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        id = Guid.Parse(respString.Replace("\"", ""));
                    }

                }
                catch (Exception ex)
                {
                    HandleError("PostDocument", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return id;
        }

        public static bool DeleteDocument(Guid id)
        {
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.DeleteAsync(String.Format(API_Interfaces.DeleteDocument, id))).Result;

                    if (response.IsSuccessStatusCode) return true;

                }
                catch (Exception ex)
                {
                    HandleError("DeleteDocument", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return false;
        }

        public class AttachmentPost
        {
            public string attachmentBase64String { get; set; }
            public string filename { get; set; }
            public string contentType { get; set; }
            public Guid touchId { get; set; }

        }
        public static Document PostAttachment(AttachmentPost att)
        {
            Document doc = new Document();
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    StringContent postData = new StringContent(JsonConvert.SerializeObject(att), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.PostAsync(API_Interfaces.EmailAttachment, postData)).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string respString = response.Content.ReadAsStringAsync().Result;
                        doc = JsonConvert.DeserializeObject<Document>(respString);
                    }
                }
                catch (Exception ex)
                {
                    HandleError("PostAttachment", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return doc;
        }

        public static Document PostBinaryFile(BinaryFilePost post)
        {
            Document doc = new Document();
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    StringContent postData = new StringContent(JsonConvert.SerializeObject(post), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.PostAsync(API_Interfaces.PostBinaryFile, postData)).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string respString = response.Content.ReadAsStringAsync().Result;
                        doc = JsonConvert.DeserializeObject<Document>(respString);
                    }


                }
                catch (Exception ex)
                {
                    HandleError("PostBinaryFile", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return doc;
        }


        #endregion

        #region Groups
        public static List<Group> EditTeamsDB(Guid dbId)
        {
            List<Group> teams = new List<Group>();
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    string url = String.Format(API_Interfaces.EditTeamsDB, dbId);
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(url)).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string respString = response.Content.ReadAsStringAsync().Result;
                        teams = JsonConvert.DeserializeObject<List<Group>>(respString);
                    }

                }
                catch (Exception ex)
                {
                    HandleError("EditTeamsDB", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }

            return teams;
        }

        public static Group GetGroup(Guid id, bool forEmail)
        {
            Group grp = new Group();
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    string url = String.Format((forEmail) ? API_Interfaces.GroupForEmail : API_Interfaces.Group, id);
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(url)).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string respString = response.Content.ReadAsStringAsync().Result;
                        grp = JsonConvert.DeserializeObject<Group>(respString);
                    }

                }
                catch (Exception ex)
                {
                    HandleError("GetGroup", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }

            return grp;
        }
        public static Group GetOrganization(Guid id)
        {
            Group grp = new Group();
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    string url = String.Format(API_Interfaces.Organization, id);
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(url)).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string respString = response.Content.ReadAsStringAsync().Result;
                        grp = JsonConvert.DeserializeObject<Group>(respString);
                    }

                }
                catch (Exception ex)
                {
                    HandleError("GetOrganization", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }

            return grp;
        }
        public static Guid PostGroup(Group group)
        {
            Guid id = Guid.Empty;
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    StringContent postData = new StringContent(JsonConvert.SerializeObject(group), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.PostAsync(API_Interfaces.PostGroup, postData)).Result;

                    string respString = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        id = Guid.Parse(respString.Replace("\"", ""));
                    }
                    else HandleError("PostGroup", new Exception(respString));

                }
                catch (Exception ex)
                {
                    HandleError("PostGroup", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return id;
        }

        #endregion

        #region Location
        public static Guid PostLocation(Location loc)
        {
            Guid id = Guid.Empty;
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    StringContent postData = new StringContent(JsonConvert.SerializeObject(loc), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.PostAsync(API_Interfaces.PostLocation, postData)).Result;

                    string respString = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        id = Guid.Parse(respString.Replace("\"", ""));
                    }

                }
                catch (Exception ex)
                {
                    HandleError("PostLocation", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return id;
        }
        public static bool DeleteLocation(Guid id)
        {
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    string url = String.Format(API_Interfaces.DeleteLocation, id);
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.DeleteAsync(url)).Result;

                    if (response.IsSuccessStatusCode) return true;

                }
                catch (Exception ex)
                {
                    HandleError("DeleteLocation", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return false;
        }

        #endregion

        #region People
        public static Person GetPerson(Guid id)
        {
            Person touch = new Person();
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    string url = string.Format(API_Interfaces.Person, id);
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(url)).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string respString = response.Content.ReadAsStringAsync().Result;
                        touch = JsonConvert.DeserializeObject<Person>(respString);
                    }

                }
                catch (Exception ex)
                {
                    HandleError("GetPerson", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }

            return touch;
        }

        public static Guid PostPerson(Person per)
        {
            Guid id = Guid.Empty;
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    StringContent postData = new StringContent(JsonConvert.SerializeObject(per), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.PostAsync(API_Interfaces.PostPerson, postData)).Result;

                    string respString = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        id = Guid.Parse(respString.Replace("\"", ""));
                    }
                    else HandleError("PostPerson", new Exception(respString));

                }
                catch (Exception ex)
                {
                    HandleError("PostPerson", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return id;
        }

        public static bool PostRelationship(RelationshipPost post)
        {
            bool result = false;
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    StringContent postData = new StringContent(JsonConvert.SerializeObject(post), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.PostAsync(API_Interfaces.Relationship, postData)).Result;

                    string respString = response.Content.ReadAsStringAsync().Result;
                    return response.IsSuccessStatusCode;

                }
                catch (Exception ex)
                {
                    HandleError("PostRelationship", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }


            return result;
        }

        public static bool DeleteRelationship(RelationshipPost post)
        {
            bool result = false;
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    string postData = JsonConvert.SerializeObject(post);
                    string url = string.Format(API_Interfaces.DeleteRelationship, post.entityId1, post.entityType1, post.entityId2, post.entityType2);
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.DeleteAsync(url)).Result;

                    string respString = response.Content.ReadAsStringAsync().Result;
                    return response.IsSuccessStatusCode;

                }
                catch (Exception ex)
                {
                    HandleError("DeleteRelationship", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }


            return result;
        }


        public static List<AutocompleteResult> AutocompleteEmailRecipients(string prefix, int noRecs)
        {
            List<AutocompleteResult> results = new List<AutocompleteResult>();
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    string url = string.Format(API_Interfaces.AutocompleteEmailRecipients, prefix, noRecs);
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(url)).Result;

                    string respString = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        results = JsonConvert.DeserializeObject<List<AutocompleteResult>>(respString);
                    }
                    else Debug.WriteLine("Error in AutocompleteEmailRecipients " + respString);

                }
                catch (Exception ex)
                {
                    HandleError("AutocompleteEmailRecipients", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }

            return results;
        }

        #endregion

        #region Tags
        public static List<AutocompleteResult> GetTagsAC(string prefix)
        {
            List<AutocompleteResult> tags = new List<AutocompleteResult>();
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    string url = string.Format(API_Interfaces.TagsAutocomplete, prefix);
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(url)).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string respString = response.Content.ReadAsStringAsync().Result;
                        tags = JsonConvert.DeserializeObject<List<AutocompleteResult>>(respString);
                    }

                }
                catch (Exception ex)
                {
                    HandleError("GetTagsAC", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }

            return tags;
        }

        public static List<UserType> GetTags()
        {
            List<UserType> tags = new List<UserType>();
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    string url = string.Format(API_Interfaces.Tags);
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(url)).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string respString = response.Content.ReadAsStringAsync().Result;
                        tags = JsonConvert.DeserializeObject<List<UserType>>(respString);
                    }

                }
                catch (Exception ex)
                {
                    HandleError("GetTags", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }

            return tags;
        }

        #endregion


        #region Touches
        public static Touch GetTouch(Guid id)
        {
            Touch touch = new Touch();
            if (Online && id != Guid.Empty)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    string url = string.Format(API_Interfaces.Touch, id);
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(url)).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string respString = response.Content.ReadAsStringAsync().Result;
                        touch = JsonConvert.DeserializeObject<Touch>(respString);

                        //var syncDoc = touch.Documents.Find(d => d.UserType_Id == Guids.Doc_CalSync);
                        //if (syncDoc == null)
                        //{
                        //    string msg = string.Format("Touch {0} {1} showed up without a syncDoc", touch.Id, touch.Name);
                        //    Debug.WriteLine(msg);
                        //}
                    }
                    else HandleError("GetTouch", new Exception(response.ReasonPhrase + " \nContent: " + response.Content.ToString()));
                }
                catch (Exception ex)
                {
                    HandleError("GetTouch", ex);
                }
                finally
                {
                    client.Dispose();
                }
            }

            return touch;
        }

        public static List<Touch> SyncTouches()
        {
            List<Touch> touches = new List<Touch>();
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(API_Interfaces.SyncTouches)).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string respString = response.Content.ReadAsStringAsync().Result;
                        touches = JsonConvert.DeserializeObject<List<Touch>>(respString);
                    }

                }
                catch (Exception ex)
                {
                    HandleError("GetTouches", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }

            return touches;
        }

        public static List<UserType> GetToucheTypes()
        {
            List<UserType> types = new List<UserType>();
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(API_Interfaces.TouchTypes)).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string respString = response.Content.ReadAsStringAsync().Result;
                        types = JsonConvert.DeserializeObject<List<UserType>>(respString);
                    }

                }
                catch (Exception ex)
                {
                    HandleError("GetToucheTypes", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return types;
        }

        public static List<Document> GetDeletedSyncs()
        {
            List<Document> docs = new List<Document>();
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(API_Interfaces.DeletedSyncs)).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string respString = response.Content.ReadAsStringAsync().Result;
                        docs = JsonConvert.DeserializeObject<List<Document>>(respString);
                    }

                }
                catch (Exception ex)
                {
                    HandleError("GetDeletedSyncs", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return docs;
        }
        public static List<Document> GetSyncRemoves()
        {
            List<Document> docs = new List<Document>();
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync("SyncRemoves")).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string respString = response.Content.ReadAsStringAsync().Result;
                        docs = JsonConvert.DeserializeObject<List<Document>>(respString);
                    }

                }
                catch (Exception ex)
                {
                    HandleError("GetSyncRemoves", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return docs;
        }

        public static Guid TouchIdByEntryId(string entryId)
        {
            Guid touchId = Guid.Empty;
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    string url = string.Format("TouchIdByEntryId?entryId={0}", entryId);
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(url)).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string respString = response.Content.ReadAsStringAsync().Result;
                        Guid.TryParse(respString, out touchId);
                    }
                    else HandleError("GetTouchIdByEntryId", new Exception(response.ReasonPhrase + " \nContent: " + response.Content.ToString()));

                }
                catch (Exception ex)
                {
                    HandleError("GetTouchIdByEntryId", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }

            return touchId;
        }

        public static Touch Resolver(List<string> resolveStrings)
        {
            Touch result = new Touch();
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    StringContent postData = new StringContent(JsonConvert.SerializeObject(resolveStrings),
                    Encoding.UTF8,
                    "application/json");

                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.PostAsync(API_Interfaces.ResolveEmailStrings, postData)).Result;

                    string respString = response.Content.ReadAsStringAsync().Result;
                    result = JsonConvert.DeserializeObject<Touch>(respString);
                    //mark them for update, as they'll be added to another touch for post
                    foreach (Group org in result.Groups) org.changeType = ChangeType.Update;
                    foreach (Person per in result.People) per.changeType = ChangeType.Update;

                }
                catch (Exception ex)
                {
                    HandleError("Resolver", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return result;
        }

        public static List<AutocompleteResult> AutocompletePeopleOrgs(string str)
        {
            List<AutocompleteResult> result = new List<AutocompleteResult>();
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {

                    string url = string.Format(API_Interfaces.AutocompletePeopleOrgs, str);

                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(url)).Result;

                    string respString = response.Content.ReadAsStringAsync().Result;
                    result = JsonConvert.DeserializeObject<List<AutocompleteResult>>(respString);

                }
                catch (Exception ex)
                {
                    HandleError("AutocompletePeopleOrgs", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return result;

        }

        public static List<AutocompleteResult> ResolveEmail(string emailAddress)
        {
            List<AutocompleteResult> result = new List<AutocompleteResult>();
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {

                    string url = string.Format(API_Interfaces.ResolveEmailAddress, emailAddress);

                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.GetAsync(url)).Result;

                    string respString = response.Content.ReadAsStringAsync().Result;
                    result = JsonConvert.DeserializeObject<List<AutocompleteResult>>(respString);

                }
                catch (Exception ex)
                {
                    HandleError("ResolveEmail", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return result;

        }

        class postAudit
        {
            public string name { get; set; }
            public string description { get; set; }
        }
        public static Guid PostAudit(string name, string description)
        {
            Guid id = Guid.Empty;
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    postAudit post = new postAudit() { name = name, description = description };
                    StringContent postData = new StringContent(JsonConvert.SerializeObject(post),
                                        Encoding.UTF8,
                                        "application/json");


                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.PostAsync("AddinAudit", postData)).Result;

                    string respString = response.Content.ReadAsStringAsync().Result;
                }
                catch //(Exception ex)
                {
                    //do nothing
                    //HandleError("PostAudit", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return id;
        }

        public static Guid PostTouch(Touch touch, bool postPageview)
        {
            Guid id = Guid.Empty;
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    //stash the unresolved strings
                    bool needsStash = (touch.resolveStrings.Count > 0);
                    string[] stash = new string[touch.resolveStrings.Count];
                    if (needsStash)
                    {
                        touch.resolveStrings.CopyTo(stash);
                        touch.resolveStrings.Clear();
                    }

                    StringContent postData = new StringContent(JsonConvert.SerializeObject(touch),
                                        Encoding.UTF8,
                                        "application/json");

                    if (needsStash) touch.resolveStrings.AddRange(stash);

                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.PostAsync(API_Interfaces.PostTouches, postData)).Result;

                    string respString = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        id = Guid.Parse(respString.Replace("\"", ""));
                        if (postPageview) Pageview("touch", EntityTypes.Touch, id);
                    }
                    else HandleError("PostTouch", new Exception(respString));

                }
                catch (Exception ex)
                {
                    HandleError("PostTouch", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return id;
        }

        public static bool PostEmailConversation(EmailConversationPost post)
        {
            bool result = false;
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    //clear out any unresolved strings
                    StringContent postData = new StringContent(JsonConvert.SerializeObject(post),
                                        Encoding.UTF8,
                                        "application/json");


                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.PostAsync(API_Interfaces.PostEmailConversation, postData)).Result;

                    string respString = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        result = true;
                    }

                }
                catch (Exception ex)
                {
                    HandleError("PostTouch", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return result;
        }
        public static bool DeleteTouch(Guid id)
        {
            if (Online)
            {
                ShClient client = new ShClient(_authToken);
                try
                {
                    BreakSync(id);
                    string url = String.Format(API_Interfaces.DeleteTouch, id);
                    HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.DeleteAsync(url)).Result;

                    if (response.IsSuccessStatusCode) return true;

                }
                catch (Exception ex)
                {
                    HandleError("DeleteTouch", ex);
                }
                finally
                {
                    client.Dispose();
                }

            }
            return false;
        }

        #endregion

        #region Private Methods
        public static void HandleError(string source, Exception ex)
        {
            Debug.WriteLine("Error in " + source + ": " + ex.Message);
            Debug.WriteLine("Stacktrace: " + ex.StackTrace);

            if (ex.GetType() == typeof(System.AggregateException))
            {
                //probably offline
                _offline = true;
                string msg = "Unable to contact Shuri\r\n\r\nRetry login or press Cancel to work offline.";
                DialogResult res = MessageBox.Show(msg, "API timed out", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);
                if (res == DialogResult.Cancel)
                {
                    Globals.ThisAddIn.RefreshUI();
                }
                else
                {
                    try { Login(false); }
                    catch { }
                }

            }
            else
            {
                string msg = "Outlook Addin Error: " + TheUser.Name;
                string desc = "Error in " + source + ": \n" + ex.Message + "\n\nStacktrace: " + ex.StackTrace;
                MessageBox.Show(desc, msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                PostAudit(msg, desc);
            }
        }

        #endregion

    }
}
