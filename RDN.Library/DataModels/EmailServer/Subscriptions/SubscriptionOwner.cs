using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.EmailServer.Subscriptions
{
    [Table("RDN_Email_Subscription_Owners")]
 public    class SubscriptionOwner : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long OwnerId { get; set; }

        public bool IsRemoved { get; set; }

        public virtual League.League LeagueOwner { get; set; }

        public virtual SubscriptionList List { get; set; }

        public virtual Member.Member Owner { get; set; }
    }
}
