using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.Library.DataModels.RN.Financials
{
    [Table("RN_Funds_For_Writers")]
    public class FundsForWriter : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid UserId { get; set; }

        public double TotalPaidToUser { get; set; }
        public double ActiveInUserAccount { get; set; }
        public string BitCoinId { get; set; }
        public string PaypalAddress { get; set; }
    }
}
