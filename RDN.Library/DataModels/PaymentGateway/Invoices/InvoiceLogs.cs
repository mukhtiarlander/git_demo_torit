using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Invoices
{
    [Table("RDN_Invoice_Logs")]
    public class InvoiceLogs// : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceLogsId { get; set; }
        [Required]
        public string Log { get; set; }
        public DateTime Created { get; set; }

        [Required]
        public virtual Invoice Invoice { get; set; }

        public InvoiceLogs()
        {
            Created = DateTime.Now;
        }
    }
}
