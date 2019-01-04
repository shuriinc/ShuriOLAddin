using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;


namespace ShuriOutlookAddIn
{
    class GoogleAPI
    {
        #region Globals
        private static HttpClient _httpClient = null;
        private static string _key = "[Your Google Maps API key here]";

        public class GoogResources
        {
            public static string Geocode = "https://maps.googleapis.com/maps/api/geocode/json?address={0}&key=" + _key;
            public static string GeocodeRev = "https://maps.googleapis.com/maps/api/geocode/json?latlng={0}&key=" + _key;  //latlng = 40.714224,-73.961452
        }
        #endregion

        private static HttpClient TheApiClient
        {
            get
            {
                if (_httpClient == null)
                {
                    RefreshClient();
                }
                return _httpClient;
            }



        }
        public static void RefreshClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.Timeout = TimeSpan.FromMilliseconds(10000);
        }
        public static Location GeoCode(string address)
        {
            Location loc = new Location();
            try
            {
                address = HttpUtility.UrlEncode(address);
                string url = String.Format(GoogResources.Geocode, address);
                using (HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => TheApiClient.GetAsync(url)).Result)
                {
                    string respString = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        GoogleGeoCodeResponse geo = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(respString);
                        if (geo.status.ToUpper() == "OK") return LocFromGeo(geo);
                        else return loc;
                    }
                    else return loc;
                }
            }
            catch (Exception ex)
            {
                DataAPI.HandleError("GoogleAPI.GeoCode", ex);
                return loc;
            }

        }

        public static Location LocFromGeo(GoogleGeoCodeResponse geo)
        {
            Location loc = new Location();
            if (geo.status.ToUpper() == "OK" && geo.results.GetLength(0) > 0)
            {
                results result = geo.results[0];
                if (!string.IsNullOrWhiteSpace(result.formatted_address)) loc.Address = result.formatted_address;
                if (!string.IsNullOrWhiteSpace(result.place_id)) loc.Place_Id = result.place_id;
                if (!string.IsNullOrWhiteSpace(result.geometry.location.lat)) loc.Latitude = Convert.ToDecimal(result.geometry.location.lat);
                if (!string.IsNullOrWhiteSpace(result.geometry.location.lng)) loc.Longitude = Convert.ToDecimal(result.geometry.location.lng);
                foreach(address_component addr in result.address_components)
                {
                    foreach(string type in addr.types)
                        switch (type)
                        {
                            case "country":
                                loc.Country = addr.long_name;
                                break;
                            case "postal_code":
                                loc.Postal = addr.long_name;
                                break;
                            case "route":
                            case "street_address":
                                loc.Street = loc.Street + addr.long_name;
                                break;
                            case "postal_town":
                            case "locality":
                                loc.City = addr.long_name;
                                break;
                            case "administrative_area_level_1":
                                loc.State = addr.long_name;
                                break;
                            case "street_number":
                                loc.Street = addr.long_name + " " + loc.Street;
                                break;
                            //case "intersection":
                            //case "political":
                            //case "administrative_area_level_2":
                            //case "administrative_area_level_3":
                            //case "colloquial_area":
                            //case "sublocality":
                            //case "neighborhood":
                            //case "premise":
                            //case "subpremise":
                            //case "natural_feature":
                            //case "airport":
                            //case "park":
                            //case "point_of_interest":
                            //case "post_box":
                            //case "floor":
                            //case "room":
                            //case "establishment":
                            //case "sublocality_level_1":
                            //case "sublocality_level_2":
                            //case "sublocality_level_3":
                            //case "sublocality_level_4":
                            //case "sublocality_level_5":
                            //case "postal_code_suffix":
                            //    break;

                        }

                }
            }

            return loc;

        }
        public class GoogleGeoCodeResponse
        {

            public string status { get; set; }
            public results[] results { get; set; }

        }

        public class results
        {
            public string formatted_address { get; set; }
            public string place_id { get; set; }
            public geometry geometry { get; set; }
            public string[] types { get; set; }
            public address_component[] address_components { get; set; }
        }

        public class geometry
        {
            public string location_type { get; set; }
            public location location { get; set; }
        }

        public class location
        {
            public string lat { get; set; }
            public string lng { get; set; }
        }

        public class address_component
        {
            public string long_name { get; set; }
            public string short_name { get; set; }
            public string[] types { get; set; }
        }
    }


}
