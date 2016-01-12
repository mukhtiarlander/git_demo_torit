using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.League
{
    [Obsolete]
    [Table("RDN_League_Colors")]
    public class LeagueColor : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ColorId { get; set; }
        
        public string ColorName { get; set; }

        public virtual Color.Color Color { get; set; }

        public virtual League League { get; set; }
    }
}
