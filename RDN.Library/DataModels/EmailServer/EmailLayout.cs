using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.EmailServer
{
    /// <summary>
    /// Email layout. This will be parsed using an email and email properties and then sent through the email server.
    /// </summary>
     [Obsolete("Use RDN.Core.*")]
    [Table("RDN_EmailServer_EmailLayouts")]
    public class EmailLayout
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmailTypeId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        
        /// <summary>
        /// Location to the Layout file on the server. This file is the body of the layout and will be the body of the email
        /// </summary>
        public string LayoutPath { get; set; }
    }
}