using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.EmailServer
{
    /// <summary>
    /// Defines an email property for the email server. A property is a value that later will be parsed into the email layout.
    /// </summary>
    [Table("RDN_EmailServer_EmailProperty")]
    internal class EmailProperty
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmailPropertyId { get; set; }
        /// <summary>
        /// Property key, max length 100
        /// </summary>
        [MaxLength(100)]
        public string Key { get; set; }
        /// <summary>
        /// Property value
        /// </summary>
        public string Value { get; set; }

        [Required]
        public virtual EmailSendItem EmailSendItem { get; set; }
    }
}
