using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.Exception
{
    [Table("RDN_Error_Exception")]
    public class Exception
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExceptionId { get; set; }
        [Required]
        public DateTime Created { get; set; }
        [MaxLength(255)]
        public string AssemblyName { get; set; }
        [MaxLength(255)]
        public string AssemblyVersion { get; set; }
        [MaxLength(255)]
        public string NameSpace { get; set; }
        [MaxLength(255)]
        public string ErrorNameSpace { get; set; }
        [MaxLength(255)]
        public string MethodName { get; set; }
        public string AdditionalInformation { get; set; }
        public byte? Severity { get; set; }
        public byte? Group { get; set; }

        #region References
        
        public virtual ICollection<ExceptionDetail> ExceptionDetails { get; set; }
        public virtual ICollection<ExceptionData> ExceptionData { get; set; } 

        #endregion

        #region Constructor

        public Exception()
        {
            ExceptionDetails = new Collection<ExceptionDetail>();
            ExceptionData = new Collection<ExceptionData>();
        }

        #endregion
    }
}
