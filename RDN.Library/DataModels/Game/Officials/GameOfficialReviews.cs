using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Game.Officials
{
    [Table("RDN_Game_Official_Reviews")]
    public class GameOfficialReview : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long OfficialReviewId{get;set;}
        public Guid OfficialReviewIdFromGame { get; set; }
        /// <summary>
        /// current date time
        /// </summary>
        public DateTime CurrentDateTimeReviewed { get; set; }
        /// <summary>
        /// remaining time in the period
        /// </summary>
        public long PeriodTimeRemaining { get; set; }
        /// <summary>
        /// period number
        /// </summary>
        public int Period { get; set; }
        /// <summary>
        /// jame number
        /// </summary>
        public int JamNumber { get; set; }

        public Guid JamId { get; set; }

        public byte TeamNumber { get; set; }

        public string Details { get; set; }
        public string Result { get; set; }
        public GameOfficialReview()
        {
            
        }
        
        #region References

        [Required]
        public virtual Game Game { get; set; }
        
        #endregion
    }
}
