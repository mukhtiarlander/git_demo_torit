using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Imports.Rinxter
{
    [Table("RDN_Rinxter_Scores")]
    public class RScores : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ScoreId { get; set; }
        public int ID { get; set; }
        public string LD1 { get; set; }
        public string Points1 { get; set; }
        public string LD2 { get; set; }
        public string Points2 { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public int BoutID { get; set; }
        public bool IsDeleted { get; set; }

        public virtual RBouts Bouts { get; set; }

        public RScores()
        {
            IsDeleted = false;
        }
    }
}
