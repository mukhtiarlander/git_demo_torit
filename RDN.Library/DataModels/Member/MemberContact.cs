using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Member
{
    [Table("RDN_Member_Contact")]
    public class MemberContact : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ContactId { get; set; }

        [MaxLength(255)]
        public string Firstname { get; set; }
        [MaxLength(255)]
        public string Lastname { get; set; }

        public int ContactTypeEnum { get; set; }
        
        public virtual ContactCard.ContactCard ContactCard { get; set; }
    }
}
