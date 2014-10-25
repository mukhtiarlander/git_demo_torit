using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.Exception
{
    [Obsolete("Use RDN.Core.*")]
    [Table("RDN_Error_Exception_Datas")]
    public class ExceptionData
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExceptionDataId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }        
        public string Data { get; set; }
        public byte DataType { get; set; }
        
        [Required]
        public virtual Exception Exception { get; set; }
    }
}
