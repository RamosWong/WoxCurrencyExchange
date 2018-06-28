using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace NumbeoMiddleware
{
    public class NumbeoMiddleware
    {
        private string _numbeoBaseUrl = "";
        private string pageUrl = string.Empty;
        private string pageContents = string.Empty;
        public Dictionary<string, float> costOfLiving = new Dictionary<string, float>();

        public NumbeoMiddleware(string baseUrl)
        {
            Initialise(baseUrl, "USD", "9");
        }

        public NumbeoMiddleware(string baseUrl, string currency)
        {
            Initialise(baseUrl, currency, "9");
        }

        public NumbeoMiddleware(string baseUrl, string currency, string itemId)
        {
            Initialise(baseUrl, currency, itemId);
        }

        private void Initialise(string baseUrl, string currency, string itemId)
        {
            _numbeoBaseUrl = baseUrl;
            pageContents = string.Empty;
            pageUrl = _numbeoBaseUrl + "?displayCurrency=" + currency + "&itemId=" + itemId;
            costOfLiving = getCosts();
        }

        public float getValueByCountryName(String countryName)
        {
            if (costOfLiving.Count == 0) return 0;
            try
            {
                return costOfLiving[countryName];
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        private Dictionary<string, float> getCosts()
        {
            Dictionary<string, float> obj = new Dictionary<string, float>();

            using (WebClient client = new WebClient())
            {
                //client.DownloadFile(pageUrl, @".\CostOfLiving\data.html");
                pageContents = client.DownloadString(pageUrl);

                string startIdentifier = "data.addRows(";
                string endIdentifier = ");";

                int startIndex = pageContents.IndexOf(startIdentifier) + startIdentifier.Length;
                int endIndex = pageContents.IndexOf(endIdentifier, startIndex);
                string rawResults = pageContents.Substring(startIndex, endIndex - startIndex);
                rawResults = Regex.Replace(rawResults, @"\s+", string.Empty);
                rawResults = rawResults.Substring(0, rawResults.LastIndexOf(','));


                string[] splitResults = rawResults.Split(']');
                //richTextBox1.Text = json.Where(x => x.;
                foreach (var opt in splitResults)
                {
                    try
                    {
                        var opt2 = opt.Substring(opt.IndexOf('\''));
                        var keyPairValues = opt2.Split(',');
                        float cost = 0F;
                        float.TryParse(keyPairValues[1], out cost);

                        string country = keyPairValues[0].Trim('\'');

                        obj.Add(country, cost);

                    }
                    catch (Exception e)
                    {

                    }

                }
            }
            return obj;
        }      
    }
}
