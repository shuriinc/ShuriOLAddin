using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShuriOutlookAddIn
{
    public class DataAPIAnon
    {
        #region Globals
        private static int _clientTimeout = 60000;
        private static bool? _useProxy = null;
        private static HttpClientHandler _proxyHandler = null;

        public class AnonResources
        {
            public static string TeamnameOK = "TeamnameOK?teamname={0}&id={1}";
            public static string LoginPost = "login";
            public static string RegisterPost = "register";
            public static string UsernameOK = "UsernameOK?username={0}"; 
        }

        internal static bool UseProxy
        {
            get
            {
                if (_useProxy == null) 
                {
                    _useProxy = false;
                    try { _useProxy = Convert.ToBoolean(Utilities.ReadRegStringValue(RegKeys.UseProxy)); }
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
        #endregion


        private static HttpClient TheApiClient
        {
            get {
                HttpClient client = null;

                if (UseProxy) client = new HttpClient(handler: ProxyHandler, disposeHandler: true);
                else client = new HttpClient();

                client.BaseAddress = new Uri(Globals.ThisAddIn.APIEnv.Url);
                client.Timeout = TimeSpan.FromMilliseconds(_clientTimeout);
                client.DefaultRequestHeaders.Add("x-api-key", Globals.ThisAddIn.APIEnv.Key);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return client;
            }



        }
 
        public static bool IsTeamnameOK(string teamname)
        {
            bool result = false;

            try
            {
                string url = String.Format(AnonResources.TeamnameOK, teamname, Guid.Empty);
                using (HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => TheApiClient.GetAsync(url)).Result)
                {
                    string respString = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return Convert.ToBoolean(respString);
                    }
                    else
                    {
                        HandleError("IsTeamnameOK", new Exception(respString));
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError("TeamnameOK", ex);
            }

            return result;
        }
        public static bool IsUsernameOK(string username)
        {
            bool result = false;

            try
            {
                string url = String.Format(AnonResources.UsernameOK, username);
                using (HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => TheApiClient.GetAsync(url)).Result)
                {
                    string respString = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return Convert.ToBoolean(respString);
                    }
                    else
                    {
                        HandleError("IsUsernameOK", new Exception(respString));
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError("IsUsernameOK", ex);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rModel"></param>
        /// <returns>Empty string when registraion succeeds.  Error message otheriwse.</returns>
        public static string Register(RegisterModel rModel)
        {

            try
            {
                StringContent postData = new StringContent(JsonConvert.SerializeObject(rModel), Encoding.UTF8, "application/json");
                using (HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => TheApiClient.PostAsync(AnonResources.RegisterPost, postData)).Result)
                {
                    string respString = response.Content.ReadAsStringAsync().Result;
                    return respString;
                }
            }
            catch (Exception ex)
            {
                HandleError("Register", ex);
                return ex.Message;
            }

        }
        
        /// <summary>
        /// Logs into the API, gets a bearing token; saves token, username, and pw (optionally) to registry
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        /// <param name="daysRemember"></param>
        /// <param name="savePassword"></param>
        /// <returns></returns>
        public static string GetAuthToken(string userName, string userPassword, int daysRemember, bool savePassword)
        {
            string theToken = "";
            try
            {
                LoginViewModel model = new LoginViewModel()
                {
                    UserName = userName,
                    Password = userPassword,
                    DaysRemember = daysRemember,
                    RememberMe = savePassword
                };

                StringContent postData = new StringContent(JsonConvert.SerializeObject(model),
                                    Encoding.UTF8,
                                    "application/json");
                //Debug.WriteLine(TheApiClient.BaseAddress);
                using (HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => TheApiClient.PostAsync(API_Interfaces.Login, postData)).Result)
                {
                    string respString = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        theToken = respString.Replace("\"", "");
                        Crypto crypt = new Crypto();
                        string iv = "";
                        string secret = "";

                        Utilities.SetRegistryValue(Properties.Resources.RegistryPath, Properties.Resources.OAuthToken, theToken, Microsoft.Win32.RegistryValueKind.String);

                        string encryptedUsername = crypt.AESEncrypt(userName, out secret, out iv);
                        encryptedUsername += ("~" + secret + "~" + iv);
                        Utilities.SetRegistryValue(Properties.Resources.RegistryPath, Properties.Resources.UserID, encryptedUsername, Microsoft.Win32.RegistryValueKind.String);

                        if (savePassword)
                        {

                            iv = "";
                            secret = "";
                            string encryptedPwd = crypt.AESEncrypt(userPassword, out secret, out iv);
                            encryptedPwd += ("~" + secret + "~" + iv);
                            Utilities.SetRegistryValue(Properties.Resources.RegistryPath, Properties.Resources.UserPassword, encryptedPwd, Microsoft.Win32.RegistryValueKind.String);

                        }
                        else
                        {
                            //remove the pwd if prev saved
                            Utilities.DeleteRegKey(Properties.Resources.UserPassword);
                        }

                    }
                    else
                    {
                        if (respString.IndexOf("{\"message\":\"") > -1)
                        {
                            theToken = "Error " + (JsonConvert.DeserializeObject<ErrorMessage>(respString)).message;
                        }
                        else theToken = "Error " + respString;
                    }
                }
            }
            catch
            {
                //server offline
                theToken = "offline";
            }
            return theToken;
        }

        #region Misc Methods
        public static void HandleError(string source, Exception ex)
        {
            Debug.WriteLine("Error in " + source + ": " + ex.Message);
            Debug.WriteLine("Stacktrace: " + ex.StackTrace);

            if (ex.GetType() == typeof(System.AggregateException))
            {
                //probably offline
                string msg = "Unable to contact Shuri \r\n\r\nOffline?";
                DialogResult res = MessageBox.Show(msg, "API timed out", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);
                //do something
            }
        }
        public static bool IsGoodPassword(string pw)
        {
            bool result = false;
            var hasUpper = false;
            var hasLower = false;
            var hasNumber = false;
            var hasSpec = false;
            if (pw.Length >= 6)
            {
                foreach (char c in pw.ToCharArray())
                {
                    var ascii = (int)c;
                    if (ascii >= 97 && ascii <= 122) hasLower = true;
                    else if (ascii >= 65 && ascii <= 90) hasUpper = true;
                    else if (ascii >= 48 && ascii <= 57) hasNumber = true;
                    else if (ascii > 32) hasSpec = true;
                }

                var n = 0;
                if (hasUpper) n++;
                if (hasLower) n++;
                if (hasNumber) n++;
                if (hasSpec) n++;

                result = (n >= 3);
            }

            return result;
        }

        #endregion

    }
}
