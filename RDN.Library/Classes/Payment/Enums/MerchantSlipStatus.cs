using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Payment.Enums
{
    public enum MerchantSlipStatus
    {
        IsRdn = 0,
        IsRdnArchived = 1,
        Active = 2,
        Archived = 3,
        Paid = 4
    }
}
