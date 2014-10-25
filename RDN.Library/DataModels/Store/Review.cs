using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.PaymentGateway.Invoices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.Store
{
    [Table("RDN_Store_ReviewTable")]
    public class Review : InheritDb
    {
         [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
         public long ReviewId { get; set; }

         public string title { get; set; }
         public string comment { get; set; }
         public string rate { get; set; }
         //[DataType(DataType.Date)]
         //public DateTime commentPublishDate { get; set; }
         public bool IsPublished { get; set; }
         public bool IsDeleted { get; set; }

         #region References
         public virtual RDN.Library.DataModels.Member.Member Member { get; set; }
         [Required]
         public virtual StoreItem StoreItem { get; set; }
         public virtual InvoiceItem ItemReviewed { get; set; }
         #endregion
    }
}