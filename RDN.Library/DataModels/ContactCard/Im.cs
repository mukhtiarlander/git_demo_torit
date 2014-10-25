using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;

namespace RDN.Library.DataModels.ContactCard
{
    [Table("RDN_ContactCard_Ims")]
    public class Im : BaseDb
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ImId { get; set; }
        public Guid ContactCardId { get; set; }
        public int ImTypeId { get; set; }
        public string Data { get; set; }

        #region References

        //[ForeignKey("ContactCardId")]
        public virtual ContactCard ContactCard { get; set; }
        //[ForeignKey("ImTypeId")]
        public virtual ImType ImType { get; set; }

        #endregion
    }
}
