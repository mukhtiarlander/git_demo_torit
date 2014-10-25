using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Game
{
        /// <summary>
        /// tells who the lead jammers are for the game.
        /// </summary>
        [Table("RDN_Game_Jams_Lead_Jammers")]
    public class GameLeadJammer : InheritDb
        {
            [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public long LeadJammerId { get; set; }
            public long GameJamLeadId { get; set; }
            [Required]
            public long GameTimeInMilliseconds { get; set; }
            [Required]
            public long JamTimeInMilliseconds { get; set; }

            [Required]
            public Guid GameMemberId { get; set; }


            #region References

            
            [Required]
            public virtual GameJam GameJam { get; set; }

            #endregion
        }
    }
