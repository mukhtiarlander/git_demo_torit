using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RDN.Library.Classes.Messages.Classes;
using System.Web.Mvc;

namespace RDN.League.Models.Messages
{
    public class MessageModel : MessageDisplay
    {
        [AllowHtml]
        public string MessageTextWriting { get; set; }
        public bool IsCarrierVerified { get; set; }
        //public GroupOwnerTypeEnum OwnerEntity { get; set; }
        public Guid OwnerId { get; set; }
    }
}