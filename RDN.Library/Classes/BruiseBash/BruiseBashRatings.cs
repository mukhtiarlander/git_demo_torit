using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.BruiseBash
{
   public class BruiseBashRatings
    {
       public long RatingId { get; set; }
       public DateTime Created { get; set; }
       public Guid RaterId { get; set; }
       public BruiseBash Winner { get; set; }
       public BruiseBash Loser { get; set; }

   
    }
}
