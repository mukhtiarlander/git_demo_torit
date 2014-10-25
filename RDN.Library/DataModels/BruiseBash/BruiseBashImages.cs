using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.BruiseBash
{
    /// <summary>
    /// This table contains the images for bruisebash.
    /// </summary>
    [Table("RDN_BruiseBash_Images")]
    public class BruiseBashImage : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ImageId { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public string SaveLocation { get; set; }
        [Required]
        public string Name { get; set; }

    
        #region References
        
        #endregion

    }
}
