using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Classes.Insurance
{
    public class InsuranceNumber
    {
        public string Number { get; set; }

        public DateTime? Expires { get; set; }

        public InsuranceType Type { get; set; }
    }
}
