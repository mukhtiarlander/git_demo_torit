using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.Library.Classes.Payment.Paypal.IPN
{
    public enum PaymentStatusEnum
    {
        Completed,
        Pending,
        Failed,
        Denied
    }
}
