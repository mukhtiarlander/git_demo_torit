using RDN.Portable.Classes.Payment.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Payment.Money
{
    public class CurrencyConverter
    {
        private List<CurrencyExchange> ExchangeRates { get; set; }

        public void LoadCurrencies(List<CurrencyExchange> currencies)
        {
            ExchangeRates = currencies;
        }
        public decimal Convert(string fromCurrency, string toCurrency, decimal price)
        {

            var rate = ExchangeRates.Where(x => x.CurrencyAbbrName == fromCurrency).FirstOrDefault();
            var finalRate = ExchangeRates.Where(x => x.CurrencyAbbrName == toCurrency).FirstOrDefault();
            //if(rate.CurrencyExchangeRate< 1)

            decimal Ta;
            decimal Fa = price;
            decimal Fe = rate.CurrencyExchangeRate;
            
            if (Fe < 1)
                Fe += 1;
            else if (Fe > 1)
                Fe -= 1;

            Ta = Fa * Fe;

            Fa = Ta;
            Fe = finalRate.CurrencyExchangeRate;

            Ta = Fa * Fe;
            return Ta;

        }
    }
}
