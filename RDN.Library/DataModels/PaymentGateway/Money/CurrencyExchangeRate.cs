using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.PaymentGateway.Money
{
    [Table("RDN_Currency_Exchange")]
    public class CurrencyExchangeRate : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyAbbrName { get; set; }
        public decimal CurrencyExchangePerUSD { get; set; }
        public bool IsEnabledForRDNation { get; set; }
    }
}
