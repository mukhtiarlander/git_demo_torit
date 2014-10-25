using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Utilities.Distance
{
   public  class Conversion
    {
       public static double ConvertMetersToMiles(double meters)
       {
           return (meters / 1609.344);
       }
    }
}
