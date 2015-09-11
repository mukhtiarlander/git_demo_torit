using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.Exception
{
    [Table("RDN_Error_Exception_Detail")]
    [Obsolete]
    public class ExceptionDetail
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExceptionDetailId { get; set; }
        [MaxLength(255)]
        public string MethodName { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }        
        public int Depth { get; set; }

        [Required]
        public virtual Exception Exception { get; set; }
    }
}
