using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace NumbeoMiddleware
{
    public class ItemConstants
    {
        private string pageUrl = string.Empty;
        private string pageContents = string.Empty;

        private void Initialise(string baseUrl)
        {
            pageContents = string.Empty;
            pageUrl = baseUrl;
            //costOfLiving = getCosts();
        }

        /// <summary>
        /// Retrieves exchange rate from the api.
        /// </summary>
        /// <param name="uri">The url to the api.</param>
        /// <returns></returns>
        private dynamic LoadItems(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            string html = string.Empty;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            // converts the api response to a JSON Object
            return JsonConvert.DeserializeObject<dynamic>(html);
        }
    }
}
