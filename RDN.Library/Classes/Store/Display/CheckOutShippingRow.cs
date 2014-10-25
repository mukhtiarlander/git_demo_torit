using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Payment.Enums;

namespace RDN.Library.Classes.Store.Display
{
    public class CheckOutShippingRow
    {
        public ShippingType Name { get; set; }
        public decimal Price { get; set; }        
    }
}
