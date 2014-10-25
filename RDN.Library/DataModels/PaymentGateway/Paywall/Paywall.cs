using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.Game;
using RDN.Library.DataModels.Game.Tournaments;
using RDN.Library.DataModels.PaymentGateway.Invoices;
using RDN.Library.DataModels.PaymentGateway.Merchants;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Paywall
{
    [Table("RDN_Paywalls")]
    public class Paywall : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PaywallId { get; set; }

        public string DescriptionOfPaywall { get; set; }
        /// <summary>
        /// if the start date and end date have multiple days, this is the 
        /// price for each day entered by the user if they buy just one day.
        /// </summary>
        public decimal DailyPrice { get; set; }
        /// <summary>
        /// this is the price of the full timespan.  User pays once and the entire paywall is paid.
        /// </summary>
        public decimal TimespanPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsEnabled { get; set; }
        public bool IsRemoved { get; set; }
        public virtual IList<Game.Game> Games { get; set; }
        public virtual IList<InvoicePaywall> PaywallInvoices{ get; set; }
        public virtual IList<GameTournament> Tournaments { get; set; }
        [Required]
        public virtual Merchant Merchant { get; set; }

        public Paywall()
        {
            PaywallInvoices = new List<InvoicePaywall>();
            Games = new List<Game.Game>();
            Tournaments = new List<GameTournament>();   
        }
    }
}
