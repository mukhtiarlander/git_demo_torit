using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.Tags;

namespace RDN.Library.DataModels.League.Documents
{
    [Table("RDN_League_Documents_Tags")]
    public class DocumentTag : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        #region references

    
        public virtual Tag Tag { get; set; }
         
        public virtual LeagueDocument LeagueDocument { get; set; }

         public bool IsRemoved { get; set; }

        #endregion


    }
}
