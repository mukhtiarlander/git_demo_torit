using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.EmailServer.Subscriptions
{

    [Table("RDN_Email_Subscription_List")]
    public class SubscriptionList : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ListId { get; set; }

        public long SubscriberTypeEnum { get; set; }

        public string ListName { get; set; }

        public bool IsRemoved { get; set; }

        public List<SubscriptionOwner> Owners { get; set; }

        public List<Subscriber> Subscribers { get; set; }

        public SubscriptionList()
        {
            Owners = new List<SubscriptionOwner>();
            Subscribers = new List<Subscriber>();

        }

    }
}
