using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Imports.Rinxter
{
    [Table("RDN_Rinxter_Penalties")]
    public class RPenalties : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PenaltyId { get; set; }
        public int ID { get; set; }
        public int TeamID { get; set; }
        public int BoutID { get; set; }
        public int SkaterID { get; set; }
        public string MinorA1 { get; set; }
        public string MinorA2 { get; set; }
        public string MinorA3 { get; set; }
        public string MinorA4 { get; set; }
        public string MinorB1 { get; set; }
        public string MinorB2 { get; set; }
        public string MinorB3 { get; set; }
        public string MinorB4 { get; set; }
        public string MinorC1 { get; set; }
        public string MinorC2 { get; set; }
        public string MinorC3 { get; set; }
        public string MinorC4 { get; set; }
        public string MinorD1 { get; set; }
        public string MinorD2 { get; set; }
        public string MinorD3 { get; set; }
        public string MinorD4 { get; set; }
        public string Major1 { get; set; }
        public string Major2 { get; set; }
        public string Major3 { get; set; }
        public string Major4 { get; set; }
        public string Major5 { get; set; }
        public string Major6 { get; set; }
        public string Major7 { get; set; }
        public bool IsDeleted { get; set; }

        public virtual RBouts Bouts { get; set; }

        public RPenalties()
        {
            IsDeleted = false;
        }

    }
}
