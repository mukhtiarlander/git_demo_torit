using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Calendar
{

    /// <summary>
    /// shows who the owners of the league are.
    /// </summary>
    [Table("RDN_Calendar_Member_Owners")]
    public class CalendarMemberOwnership : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public int OwnerType { get; set; }

        #region references
        [Required]
        public virtual Calendar Calendar { get; set; }
        [Required]
        public virtual Member.Member Member { get; set; }
        #endregion

    }
}
