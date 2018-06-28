using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NumbeoMiddleware;

using Wox.Plugin;

namespace WoxCurrencyExchange
{
    public class Main : IPlugin
    {
        static HttpClient client;
        // where the DLL files will be when the plugin is loaded
        static string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static string apiCurrencyPath = assemblyFolder + ConfigurationManager.AppSettings["resourceDir"] + ConfigurationManager.AppSettings["currencyJson"];

        /// <summary>
        /// Initialises the Main Application.
        /// </summary>
        public void Init(PluginInitContext context)
        {
            client = new HttpClient();
        }

        /// <summary>
        /// Processes the queries entered into Wox.
        /// </summary>
        /// <param name="query">The query that was entered into Wox.</param>
        /// <returns></returns>
        public List<Result> Query(Query query)
        {
            List<Result> results = new List<Result>();
            String fromCurrencyQuery = query.FirstSearch.ToUpper();
            String exchangeValue = query.SecondSearch;            
            String toCurrencyQuery = query.ThirdSearch.ToUpper();

            // convert the currency code entered into a Currency object
            Currency currency = new Currency(apiCurrencyPath);

            Currency.CurrencyInformation fromCurrency = currency.getCurrencyByCode(fromCurrencyQuery);
            Currency.CurrencyInformation toCurrency = currency.getCurrencyByCode(toCurrencyQuery);

            // if the mapping failed, assume that the entered currency is invalid
            if (fromCurrency == null || toCurrency == null)
            {
                results.Add(new Result()
                {
                    Title = "Currency Exchange",
                    SubTitle = "Invalid Currency.",
                    IcoPath = "images\\wox-cx.png",
                    Action = e =>
                    {
                        return false;
                    }
                });
                return results;
            }

            // request in real time for currency exchange value
            var apiRequest = String.Format(ConfigurationManager.AppSettings["currencyExchangeApiUrl"] + "?q={0}_{1}&compact=y", fromCurrencyQuery, toCurrencyQuery);
            ExchangeRate exchangeRate = new ExchangeRate(apiRequest);
            // value defaults to 0
            float rate = exchangeRate.getExchangeRate(String.Format("{0}_{1}", fromCurrencyQuery, toCurrencyQuery));

            // if rate is 0, assume failed to retrieve currency exchange
            if (rate == 0)
            {             
                results.Add(new Result()
                {
                    Title = "Currency Exchange",
                    SubTitle = "Unable to retrieve exchange rate, Please try again later.",
                    IcoPath = "images\\wox-cx.png",
                    Action = e =>
                    {
                        return false;
                    }
                });
                return results;
            }

            float calculatedValue = 0;
            float.TryParse(exchangeValue, out calculatedValue);

            // calculate exchange value based on the retrieve rates
            calculatedValue = calculatedValue * rate;

            // first result shows exchange value based on user entry
            results.Add(new Result()
            {
                Title = fromCurrency.Name + " to " + toCurrency.Name,
                SubTitle = "= " + toCurrency.Symbol + calculatedValue,
                IcoPath = "images\\wox-cx.png",
                Action = e =>
                {
                    return false;
                }
            });

            // second result show the rate of exchange
            results.Add(new Result()
            {
                Title = "Current Rate : " + rate,
                SubTitle = fromCurrency.Symbol + "1 = " + toCurrency.Symbol + (1 * rate),
                IcoPath = "images\\wox-cx.png",
                Action = e =>
                {
                    return false;
                }
            });

            // retrieve cost of living object from the numbeo middleware
            var middleware = new NumbeoMiddleware.NumbeoMiddleware(fromCurrency.Code);
            float colValue = middleware.getValueByCountryName(toCurrency.CountryName);

            // only show the third panel if there are results
            // as some currency do not have cost of living data
            if (colValue != 0)
            {
                results.Add(new Result()
                {
                    Title = "Cost of Living in " + toCurrency.CountryName,
                    SubTitle = "Loaf of Bread (500g) cost: " + fromCurrency.Symbol + colValue,
                    IcoPath = "images\\wox-cx.png",
                    Action = e =>
                    {
                        return false;
                    }
                });

            }
            return results;
        }

        
    }
}
