using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Classes.Games.Officiating
{
 public    class RequestDA
    {

        public RequestDA()
        {

        }
        public long RequestId { get; set; }
        public string TeamsPlaying { get; set; }
        public DateTime? Date { get; set; }
        public string FirstWhistle { get; set; } //Time
        public bool IsRegulation { get; set; }
        public bool EvaluationsProvided { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public bool IsSnacksProvided { get; set; }
        public long NoRefNeded { get; set; }
        public long NonsoNeded { get; set; }
        public bool IsReimbursement { get; set; }
        public decimal TravelStipendForNSO { get; set; }
        public decimal TravelStipendForRefs { get; set; }
        public int RuleSetId { get; set; }
        public string Description { get; set; }

        public string UrlToRequest { get; set; }
        public string UrlToContact { get; set; }

        #region References
        public Guid RequestCreator { get; set; }
        #endregion


    }
}
