using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.Collections.ObjectModel;
using RDN.Library.DataModels.PaymentGateway.Money;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.MemberFees
{
    [Table("RDN_Fee_Management")]
    public class FeeManagement : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid FeeManagementId { get; set; }

        public double FeeCostDefault { get; set; }
        public int FeeTypeEnum { get; set; }

        public int DayOfMonthToCollectDefault { get; set; }
        public int DaysBeforeDeadlineToNotifyDefault { get; set; }
        public string PayPalEmailAddress { get; set; }
        public bool AcceptPaymentsOnline{ get; set; }
        public int WhoPaysProcessorFeesEnum { get; set; }
        public bool LockDownManagementToManagersOnly { get; set; }

        public string EmailResponse { get; set; }
        public virtual CurrencyExchangeRate CurrencyRate { get; set; }
        public virtual League.League LeagueOwner { get; set; }
        public virtual ICollection<FeeItem> Fees { get; set; }
        public virtual ICollection<FeeClassification> FeeClassifications { get; set; }

        public FeeManagement()
        {
            Fees = new Collection<FeeItem>();
        }
    }
}
