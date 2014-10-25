using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.EmailServer.Subscriptions
{
     [Table("RDN_Email_Subscription_Email_Blast")]
    public class EmailBlast: InheritDb
    {
         [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
         public long BlastId { get; set; }

         public string BlastTitle { get; set; }

         public long SubscriptionId { get; set; }

         public string EmailBody { get; set; }

         public bool TestEmail { get; set; }

         public string TestEmailField { get; set; }
    }
}
