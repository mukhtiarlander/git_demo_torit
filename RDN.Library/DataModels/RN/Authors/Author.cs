using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.Library.DataModels.RN.Posts
{
    [Table("RN_Authors")]
    public class Author : InheritDb
    {
        /// <summary>
        /// author/userid.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid UserId{ get; set; }

        /// <summary>
        /// total times been pushed to facebook automatically.
        /// </summary>
        public bool DoesAuthorGenerateRevenue{ get; set; }

    
    }
}
