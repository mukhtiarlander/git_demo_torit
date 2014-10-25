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
    [Table("RDN_Timezone_Country")]
    public class CountryTimeZone : InheritDb
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CountryId{ get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        

        #region References

        #endregion

        #region Constructor

        public CountryTimeZone()
        {
     
        }

        #endregion
    }
}
