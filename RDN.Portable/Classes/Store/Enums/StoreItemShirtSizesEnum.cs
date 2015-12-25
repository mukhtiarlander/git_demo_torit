using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Classes.Store.Enums
{
    [Flags]
    public enum StoreItemShirtSizesEnum
    {
        None = 0,
        X_Small = 1,
        Small = 2,
        Medium = 4,
        Large = 8,
        X_Large = 16,
        XX_Large = 32,
        XXX_Large = 64
    }
}
