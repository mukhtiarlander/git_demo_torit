using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.Officials
{
    [Table("RDN_Ref_JobBoard_Request")]
    public class RefereeRequest : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RequestId { get; set; }
        public string TeamsPlaying { get; set; }
        public DateTime? Date { get; set; }
        public string FirstWhistle { get; set; }
        public bool IsRegulation { get; set; }
        public bool EvaluationsProvided { get; set; }
      
        public bool IsSnacksProvided { get; set; }
        public long NoRefNeded { get; set; }
        public long NonsoNeded { get; set; }
        public bool IsReimbursement { get; set; }
        public decimal TravelStipendForNSO { get; set; }
        public decimal TravelStipendForRefs { get; set; }
        public int RuleSetId { get; set; }
        public string Description { get; set; }
        public bool IsDelete { get; set; }

        #region References
        public virtual Location.Location Location { get; set; }
        public virtual RDN.Library.DataModels.Member.Member RequestCreator { get; set; }
        #endregion

    }
}
