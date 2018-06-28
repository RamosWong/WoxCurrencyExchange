using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.IO;

namespace WoxCurrencyExchange
{
    public class Currency
    {
        public List<CurrencyInformation> currencyList;

        public Currency(string filePath)
        {
            // currencyList = new CurrencyList();
            currencyList = LoadCurrencyInformation(filePath);
        }

        public class CurrencyInformation
        {
            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("symbol")]
            public string Symbol { get; set; }

            [JsonProperty("country_name")]
            public string CountryName { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("symbol_native")]
            public string SymbolNative { get; set; }

            [JsonProperty("decimal_digits")]
            public int DecimalDigits { get; set; }

            [JsonProperty("rounding")]
            public float Rounding { get; set; }

            [JsonProperty("name_plural")]
            public string NamePlural { get; set; }
        }

        public List<CurrencyInformation> LoadCurrencyInformation(string filePath)
        {
            // read file into a string and deserialize JSON to a type
            var test = System.IO.File.ReadAllLines(filePath);
            return JsonConvert.DeserializeObject<List<CurrencyInformation>>(File.ReadAllText(filePath));
        }
        
        public CurrencyInformation getCurrencyByCode(string code)
        {
            return currencyList.FirstOrDefault(c => c.Code == code);
        }
    }
}