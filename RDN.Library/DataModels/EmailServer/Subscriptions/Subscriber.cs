using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.EmailServer.Subscriptions
{
    [Table("RDN_Email_Subscription_Subscriber")]
    public class Subscriber : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SubscriberId { get; set; }

        public string Name { get; set; }

        public string Data { get; set; }

        public long EmailsSent { get; set; }

       // public long SubscriberTypeEnum { get; set; }

        public bool IsRemoved { get; set; }

        public bool OptedOut { get; set; }

        public int BounceCount { get; set; }

        public DateTime? OptedOutDateTime { get; set; }

        public virtual SubscriptionList List { get; set; }
    }
}
