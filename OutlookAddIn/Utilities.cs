#region Header

/*
 * Slovak Technical Services, Inc.
 * Ken Slovak
 * 5/18/17
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Outlook = Microsoft.Office.Interop.Outlook;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;

namespace ShuriOutlookAddIn
{
    class Utilities
    {
        internal const int REG_VALUE_TRUE = 1;
        internal const int REG_VALUE_FALSE = 0;


        public static string CleanFriendlyname(string name)
        {
            string s = "";
            foreach (char c in name)
            {
                int x = (int)c;
                if ((x >= 48 && x <= 57)
                    || (x >= 65 && x <= 90)
                    || (x >= 97 && x <= 122)
                    || (x == 32)
                    || (x == 45)
                    //|| (x == 46) //period
                    || (x == 95))
                    s += c;
            }
            return s;
        }

        public static string CleanWhitespace(string s)
        {
            s = Regex.Replace(s, "<.*?>", string.Empty, RegexOptions.Singleline);
            while (s.IndexOf("\r\n\r\n") > -1) s = s.Replace("\r\n\r\n", "\r\n");
            while (s.IndexOf("\r\n \r\n") > -1) s = s.Replace("\r\n \r\n", "\r\n");
            while (s.IndexOf("\n\n") > -1) s = s.Replace("\n\n", "\n");
            while (s.IndexOf("<br /><br />") > -1) s = s.Replace("<br /><br />", "<br />");
            return s;
        }
        public static Stream GetEmlStream(Outlook.MailItem mail)
        {
            Type converter = Type.GetTypeFromCLSID(MAPIMethods.CLSID_IConverterSession);
            object obj = Activator.CreateInstance(converter);
            MAPIMethods.IConverterSession session = (MAPIMethods.IConverterSession)obj;

            if (session != null)
            {
                uint hr = session.SetEncoding(MAPIMethods.ENCODINGTYPE.IET_QP);
                hr = session.SetSaveFormat(MAPIMethods.MIMESAVETYPE.SAVE_RFC822);
                var stream = new ComMemoryStream();
                hr = session.MAPIToMIMEStm((MAPIMethods.IMessage)mail.MAPIOBJECT, stream, MAPIMethods.MAPITOMIMEFLAGS.CCSF_SMTP);
                if (hr != 0)
                    throw new ArgumentException("There are some invalid COM arguments");


                stream.Position = 0;

                return stream;
            }

            return null;
        }

        public static List<string> EmailAddressesInString(string str)
        {
            List<string> result = new List<string>();
            Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
            RegexOptions.IgnoreCase);
            //find items that matches with our pattern
            MatchCollection emailMatches = emailRegex.Matches(str);


            foreach (Match emailMatch in emailMatches)
            {
                result.Add(emailMatch.Value);
            }
            return result;
        }

        public static string AppLink(object obj)
        {
            string url = "";
            if (obj is Person)
            {
                url += "person/" + (obj as Person).Id;
            }
            else if (obj is Group)
            {
                url += "group/" + (obj as Group).Id;
            }
            else if (obj is Tag)
            {
                url += "tag/" + (obj as Tag).Id.ToString();
            }

            if (url == "") return "";
            else return Globals.ThisAddIn.APIEnv.BaseAppUrl + "#/home/" + url;
        }

        public static Image ResizeFromUrl(string url, Size size)
        {
            Image newImage = null;
            WebClient client = new WebClient();
            Stream ms = client.OpenRead(url);
            Image img = Image.FromStream(ms);

            //resize
            newImage = FixedSize(img, size.Width, size.Height);

            img.Dispose();
            ms.Dispose();
            client.Dispose();
            return newImage;
        }

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            if ((image.Height > maxHeight || image.Width > maxWidth) && (image.Height > 0 && image.Width > 0))
            {
                var ratioX = (double)maxWidth / image.Width;
                var ratioY = (double)maxHeight / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);

                var newImage = new Bitmap(newWidth, newHeight);
                Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
                return newImage;
            }
            else return image;
        }
        public static Image CropToCircle(Image srcImage)
        {
            Image dstImage = new Bitmap(srcImage.Width, srcImage.Height, srcImage.PixelFormat);
            Graphics g = Graphics.FromImage(dstImage);
            using (Brush br = new SolidBrush(Color.Transparent))
            {
                g.FillRectangle(br, 0, 0, dstImage.Width, dstImage.Height);
            }
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, dstImage.Width, dstImage.Height);
            g.SetClip(path);
            g.DrawImage(srcImage, 0, 0);

            return dstImage;
        }

        static Image FixedSize(System.Drawing.Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width - (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height - (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height, imgPhoto.PixelFormat);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.WhiteSmoke);
            grPhoto.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
        public static Image AvatarFromUrl(string url)
        {
            Image newImage = Properties.Resources.icon_person42;
            WebClient client = new WebClient();
            Stream ms = null;
            try { ms = client.OpenRead(url); }
            catch { }
            if (ms != null)
            {
                newImage = Image.FromStream(ms);
                int startSize = 56;
                int endSize = 42;

                //resize
                newImage = ScaleImage(newImage, startSize, startSize);
                newImage = FixedSize(newImage, startSize, startSize);
                newImage = Crop(newImage, endSize, endSize, Convert.ToInt32((startSize - endSize) / 2), Convert.ToInt32((startSize - endSize) / 2));

                newImage = CropToCircle(newImage);

                ms.Dispose();
            }
            client.Dispose();
            return newImage;
        }


        public static Image Crop(Image image, int width, int height, int x, int y)
        {
            try
            {
                Bitmap bmp = new Bitmap(width, height, image.PixelFormat);
                bmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                Graphics gfx = Graphics.FromImage(bmp);
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gfx.DrawImage(image, new Rectangle(0, 0, width, height), x, y, width, height, GraphicsUnit.Pixel);
                // Dispose to free up resources
                image.Dispose();
                gfx.Dispose();

                return bmp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public static bool IsValidEmail(string emailAddress)
        {
            return Regex.IsMatch(emailAddress, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }

        #region Outlook items
        internal static Outlook.Items GetAppointmentsInRange(Outlook.Folder folder, DateTime startTime, DateTime endTime)
        {
            string filter = "[Start] >= '"
                + startTime.ToString("g")
                + "' AND [End] <= '"
                + endTime.ToString("g") + "'";
            Debug.WriteLine(filter);
            try
            {
                Outlook.Items calItems = folder.Items;
                calItems.IncludeRecurrences = true;
                calItems.Sort("[Start]", Type.Missing);
                Outlook.Items restrictItems = calItems.Restrict(filter);
                if (restrictItems.Count > 0)
                {
                    return restrictItems;
                }
                else
                {
                    return null;
                }
            }
            catch { return null; }
        }

        #endregion


        internal static string Decrypt(string encryptedtext)
        {
            string ret = "";

            try
            {
                if (!String.IsNullOrEmpty(encryptedtext))
                {
                    Crypto crypt = new Crypto();

                    char[] separator = new char[] { '~' };

                    if (!String.IsNullOrEmpty(encryptedtext))
                    {
                        string[] id = encryptedtext.Split(separator); // 0==id, 1==secret, 2==iv
                        if (id.Length == 3)
                        {
                            ret = crypt.AESDecrypt(id[0], id[1], id[2]);
                        }

                    }
                }
            }
            catch
            {
                ret = "";
            }

            return ret;
        }

        internal static string XMLEscape(string inputXML)
        {
            string outXML = "";

            try
            {
                string ampersand = inputXML.Replace("&", "&amp;"); // this replacement must be first
                string lessThan = ampersand.Replace("<", "&lt;");
                string greaterThan = lessThan.Replace(">", "&gt;");
                string backslash = greaterThan.Replace(@"\""", "&quot;");
                string quote = backslash.Replace(@"""", "&quot;");

                outXML = quote.Replace("'", "&apos;");
            }
            catch
            {
                //Logger.LogException(ex, ex.Message);
            }

            Debug.WriteLine(outXML);

            return outXML;
        }

        internal static string XMLUnescape(string inputXML)
        {
            string outXML = "";

            try
            {
                string apos = inputXML.Replace("&apos;", "'");
                string quote = apos.Replace("&quot;", "\"");
                string greaterThan = quote.Replace("&gt;", ">");
                string lessThan = greaterThan.Replace("&lt;", "<");

                outXML = lessThan.Replace("&amp;", "&");
            }
            catch
            {
                //Logger.LogException(ex, ex.Message);
            }

            Debug.WriteLine(outXML);

            return outXML;
        }

        internal static void WriteDoNotDisableKeyToRegistry()
        {
            try
            {
                int outlookVersion = GetOutlookVersion();

                if (outlookVersion >= 15) // Outlook 2013 is Outlook 15.
                {

                    string versionString = outlookVersion.ToString();
                    string doNotDisablePath = String.Format(Properties.Resources.DoNotDisablePath, versionString);

                    RegistryKey k = Registry.CurrentUser.OpenSubKey(doNotDisablePath, true);
                    if (k == null)
                    {
                        k = Registry.CurrentUser.CreateSubKey(doNotDisablePath);
                    }

                    if (k != null)
                    {
                        k.SetValue(Properties.Resources.AddinName, 1, RegistryValueKind.DWord);
                    }
                }
            }
            catch
            {
                //Logger.LogException(ex, "Error while Creating DoNotDisableAddinList key");
            }
        }

        internal static int DeleteRegKey(string regName)
        {
            int retVal = REG_VALUE_TRUE;

            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(Properties.Resources.RegistryPath, true);
                regKey.DeleteValue(regName, false);
            }
            catch
            {
                retVal = REG_VALUE_FALSE;
            }

            return retVal;
        }

        internal static void SetRegKey(string name, string value)
        {
            SetRegistryValue(Properties.Resources.RegistryPath, name, value, RegistryValueKind.String);
        }
        internal static void SetRegistryValue(string regPath, string regValueName, object regValue, RegistryValueKind valueKind)
        {
            if (regPath != "")
            {
                RegistryKey regKey = null;
                try
                {
                    regKey = Registry.CurrentUser.OpenSubKey(regPath, true);

                    if (regKey == null)
                    {
                        regKey = Registry.CurrentUser.CreateSubKey(regPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
                    }

                    if (regKey != null)
                    {
                        regKey.SetValue(regValueName, regValue, valueKind);
                    }
                    else
                    {
                    }

                }
                catch
                {
                }
                finally
                {
                    if (regKey != null)
                    {
                        regKey.Close();
                        regKey = null;
                    }
                }
            }
            else
            {
            }
        }

        internal static int ReadRegBoolValue(string regName)
        {
            // HKCU\Software\Shuri

            int retVal = REG_VALUE_FALSE;

            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(Properties.Resources.RegistryPath, false);
                if (regKey != null)
                {
                    retVal = Convert.ToInt32(regKey.GetValue(regName, REG_VALUE_FALSE)); // default False if new registry key

                }
                else
                {
                    RegistryKey newKey = Registry.CurrentUser.CreateSubKey(Properties.Resources.RegistryPath);
                    if (newKey != null)
                    {
                        newKey.SetValue(regName, REG_VALUE_FALSE, RegistryValueKind.DWord);
                        newKey.Close();

                        retVal = REG_VALUE_FALSE;
                    }
                }

            }
            catch
            {
                retVal = REG_VALUE_FALSE;
            }

            return retVal;
        }

        internal static string ReadRegStringValue(string regName)
        {
            // HKCU\Software\Shuri

            string retVal = "";

            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(Properties.Resources.RegistryPath, false);
                if (regKey != null)
                {
                    retVal = regKey.GetValue(regName, "").ToString();

                }
                else
                {
                    RegistryKey newKey = Registry.CurrentUser.CreateSubKey(Properties.Resources.RegistryPath);
                    if (newKey != null)
                    {
                        newKey.SetValue(regName, "", RegistryValueKind.String);
                        newKey.Close();

                        retVal = "";
                    }
                }

            }
            catch
            {
                retVal = "";
            }

            return retVal;
        }

        private static int GetOutlookVersion()
        {
            int outlookVersion = 0;

            try
            {
                string versionMajor = "";

                if (Globals.ThisAddIn.Application != null)
                {
                    string version = Globals.ThisAddIn.Application.Version;
                    int dotPos = version.IndexOf(".");
                    if (dotPos > 0)
                    {
                        versionMajor = version.Substring(0, dotPos);
                        try
                        {
                            outlookVersion = Convert.ToInt32(versionMajor);
                        }
                        catch (Exception ex)
                        {
                            //Logger.LogException(ex, ex.Message);
                            Debug.WriteLine(ex.Message, ex.StackTrace);
                            outlookVersion = 0;
                        }
                    }
                    else
                    {
                        outlookVersion = 0;
                    }
                }
                else
                {
                    outlookVersion = 0;
                }
            }
            catch
            {
                //Logger.LogException(ex1, ex1.Message);

                outlookVersion = 0;
            }

            return outlookVersion;
        }

        internal static string DefaultEmailAddress()
        {
            string defaultAddress = "";
            try
            {
                Outlook.Recipient user = Globals.ThisAddIn._nameSpace.CurrentUser;
                Outlook.AddressEntry ae = user.AddressEntry;

                if ((ae.AddressEntryUserType == Outlook.OlAddressEntryUserType.olExchangeUserAddressEntry) ||
                    (ae.AddressEntryUserType == Outlook.OlAddressEntryUserType.olExchangeRemoteUserAddressEntry))
                {
                    Outlook.ExchangeUser exUser = ae.GetExchangeUser();
                    if (exUser != null)
                    {
                        defaultAddress = exUser.PrimarySmtpAddress;
                        if (String.IsNullOrEmpty(defaultAddress))
                        {
                            defaultAddress = ae.Address;
                        }

                        exUser = null;
                    }
                    else
                    {
                        defaultAddress = ae.Address;
                    }
                }
                else
                {
                    defaultAddress = ae.Address;
                }

                ae = null;
                user = null;
            }
            catch
            {
                defaultAddress = "";
            }

            return defaultAddress;
        }

        internal static string Base64Decode(string encodedText)
        {
            try
            {
                byte[] plainTextBytes = System.Convert.FromBase64String(encodedText);
                return Encoding.UTF8.GetString(plainTextBytes);
            }
            catch
            {
                return "";
            }
        }

        internal static string Base64Encode(string plainText)
        {
            try
            {
                byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                return System.Convert.ToBase64String(plainTextBytes);
            }
            catch
            {
                return "";
            }
        }

        internal static string GetDecodedString(string inputToDecode)
        {
            string retVal = "";

            if (!String.IsNullOrEmpty(inputToDecode))
            {
                try
                {
                    byte[] bytesToDecode = Convert.FromBase64String(inputToDecode);
                    retVal = ASCIIEncoding.ASCII.GetString(bytesToDecode);
                }
                catch
                {
                    retVal = "";
                }
            }

            return retVal;
        }



        internal static string FormatDateForOutlook(DateTime toFormat)
        {
            // no seconds for Outlook
            string toCompare = "";

            try
            {
                DateTime newDate1 = new DateTime(toFormat.Year, toFormat.Month, toFormat.Day, toFormat.Hour, toFormat.Minute, 0);
                DateTime newDate = new DateTime(toFormat.Year, toFormat.Month, toFormat.Day, toFormat.Hour, toFormat.Minute, 0, DateTimeKind.Utc);

                string result1 = newDate1.ToString("g");
                string result = newDate.ToString("g");

                toCompare = result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                toCompare = "";
            }

            return toCompare;
        }
    }
}
