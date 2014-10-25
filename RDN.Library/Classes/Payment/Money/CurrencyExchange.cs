using Newtonsoft.Json;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.PaymentGateway.Money;
using RDN.Portable.Classes.Payment.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace RDN.Library.Classes.Payment.Money
{
    public class CurrencyExchangeFactory
    {
        private static readonly string CURRENCY_NAMES_API = "http://openexchangerates.org/currencies.json";
        private static readonly string CURRENCY_EXCHANGE_RATES = "http://openexchangerates.org/api/latest.json?app_id=8eb9936fac7041bbb658bf2863c688b8";
        public static bool PullCurrencyNames()
        {
            WebClient wc = new WebClient();

            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(wc.DownloadString(CURRENCY_NAMES_API));
            var dc = new ManagementContext();

            foreach (var item in dict)
            {
                var currency = dc.ExchangeRates.Where(x => x.CurrencyAbbrName == item.Key).FirstOrDefault();
                if (currency == null)
                {
                    CurrencyExchangeRate rate = new CurrencyExchangeRate();
                    rate.CurrencyAbbrName = item.Key;
                    rate.CurrencyName = item.Value;
                    dc.ExchangeRates.Add(rate);
                }
            }
            int c = dc.SaveChanges();
            return c > 0;
        }
        public static List<CurrencyExchange> GetCurrencyExchangeRates()
        {
            List<CurrencyExchange> tempRates = new List<CurrencyExchange>();
            try
            {
                var dc = new ManagementContext();
                var rates = dc.ExchangeRates.Where(x => x.IsEnabledForRDNation).ToList();
                foreach (var rate in rates)
                {
                    CurrencyExchange r = new CurrencyExchange();
                    r.CurrencyAbbrName = rate.CurrencyAbbrName;
                    r.CurrencyExchangeRate = rate.CurrencyExchangePerUSD;
                    r.CurrencyNameDisplay = rate.CurrencyAbbrName + " - " + rate.CurrencyName;
                    tempRates.Add(r);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return tempRates;
        }

        public static bool UpdateCurrencyExchangeRates()
        {
            try
            {
                PullCurrencyNames();

                var dc = new ManagementContext();
                WebClient wc = new WebClient();

                var dict = JsonConvert.DeserializeObject<ExchangeRateJson>(wc.DownloadString(CURRENCY_EXCHANGE_RATES));

                foreach (var item in dict.Rates)
                {
                    var currency = dc.ExchangeRates.Where(x => x.CurrencyAbbrName == item.Key).FirstOrDefault();
                    if (currency == null)
                    {
                        CurrencyExchangeRate rate = new CurrencyExchangeRate();
                        rate.CurrencyAbbrName = item.Key;
                        rate.CurrencyExchangePerUSD = item.Value;
                        dc.ExchangeRates.Add(rate);
                    }
                    else
                    {
                        currency.CurrencyExchangePerUSD = item.Value;
                    }
                }

                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
    }
}
