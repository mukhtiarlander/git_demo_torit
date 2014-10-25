using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Site
{
    [Table("RDN_Site_Front_Page_History")]
    public class FrontPageHistory : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long HistoryId { get; set; }
        public Guid LeagueId { get; set; }
        public Guid MemberId { get; set; }


    }
}
