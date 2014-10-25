using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.TimeZone
{
    /// <summary>
    /// Teams are used for the scoreboard system to save logos of the teams
    /// </summary>
    [Table("RDN_Timezone_TimeZone")]
    public class TimeZone : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ZoneId { get; set; }
        public string Location { get; set; }
        public string GMT { get; set; }
        public int GMTOffset { get; set; }

        #region References

        #endregion

        #region Constructor

        public TimeZone()
        {

        }

        #endregion
    }
}
