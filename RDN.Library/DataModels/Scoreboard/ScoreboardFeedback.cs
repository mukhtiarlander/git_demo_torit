using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Scoreboard
{
    [Table("RDN_Scoreboards_Feedback")]
    public class ScoreboardFeedback : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long FeedBackId { get; set; }
        [Required]
        public string Feedback { get; set; }
        [MaxLength(200)]
        public string Email { get; set; }
        [MaxLength(200)]
        public string League { get; set; }
        public byte FeedbackTypeEnum { get; set; }
        public bool IsArchived { get; set; }

        public virtual ScoreboardInstance Instance { get; set; }
        
    }
}
