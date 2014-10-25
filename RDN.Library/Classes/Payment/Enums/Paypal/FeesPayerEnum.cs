using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Payment.Enums.Paypal
{
    /// <summary>
    /// xs:string
    ///(Optional) The payer of PayPal fees. Allowable values are:
    ///SENDER – Sender pays all fees (for personal, implicit simple/parallel payments; do not use for chained or unilateral payments)
    ///PRIMARYRECEIVER – Primary receiver pays all fees (chained payments only)
    ///EACHRECEIVER – Each receiver pays their own fee (default, personal and unilateral payments)
    ///SECONDARYONLY – Secondary receivers pay all fees (use only for chained payments with one secondary receiver)
    /// </summary>
    public enum FeesPayerEnum
    {
        SENDER = 0,
        PRIMARYRECEIVER = 1,
        EACHRECEIVER = 2,
        SECONDARYONLY = 3
    }
}
