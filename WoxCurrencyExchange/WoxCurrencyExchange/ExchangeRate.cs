using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace WoxCurrencyExchange
{
    public class ExchangeRate
    {
        private dynamic exchangeRate;

        public ExchangeRate(string apiUrl)
        {
            // currencyList = new CurrencyList();
            exchangeRate = LoadExchangeRate(apiUrl);
        }

        /// <summary>
        /// Retrieves exchange rate from the api.
        /// </summary>
        /// <param name="uri">The url to the api.</param>
        /// <returns></returns>
        private dynamic LoadExchangeRate(string uri)
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

        public float getExchangeRate(string id)
        {
            // if the request was bad, treat as invalid
            if (exchangeRate["status"] != null && exchangeRate["status"] == 400)
            {
                return 0;
            }
            return exchangeRate[id].val;
        }
    }
}