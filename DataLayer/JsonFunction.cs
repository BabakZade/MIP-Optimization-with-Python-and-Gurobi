using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
//using Newtonsoft.Json;

namespace DataLayer
{
    public class JsonFunction
    {
        public JsonFunction() {
            inital();
        }
      
        public void inital()
        {
            var url = "https://spartatst.ugent.be/_New/json/";
           //var currencyRates = _download_serialized_json_data<JsonInfo>(url);
        }
        //public static T _download_serialized_json_data<T>(string url) where T : new()
        //{
        //    using (var w = new WebClient())
        //    {
        //        var json_data = string.Empty;
        //        // attempt to download JSON data as a string
        //        try
        //        {
        //            json_data = w.DownloadString(url);
        //        }
        //        catch (Exception) { }
        //        // if string with JSON data is not empty, deserialize it to class and return its instance 
        //        return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
        //    }
        //}
    }
}
