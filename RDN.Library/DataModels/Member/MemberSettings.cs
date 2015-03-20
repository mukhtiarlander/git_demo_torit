using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Member
{
    [Table("RDN_Member_Settings")]
    public class MemberSettings : InheritDb
    {
        /// <summary>
        /// the settings Id will be the actual memberId
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SettingsId { get; set; }

        public byte CalendarViewSetting { get; set; }

        public long MemberPrivacySettingsEnum { get; set; }

        public bool ForumDescending { get; set; }
    }
}
