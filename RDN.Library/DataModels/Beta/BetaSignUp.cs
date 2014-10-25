using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Beta
{
    [Table("RDN_Beta_SignUp")]
        public class BetaSignUp: InheritDb
        {
            [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public long SignUpId { get; set; }
            [Required]
            public string Email { get; set; }

            public bool Emailed { get; set; }
    }
}
