using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.RN.Funds.Monthly
{
  public   class TotalPayment
    {
      public Guid UserId { get; set; }
      public double TotalPercentageBeingPaid { get; set; }
      public double TotalActiveInAccount { get; set; }
      public double TotalAddedToAccount { get; set; }
      public double TotalPageViewsThisMonth { get; set; }
    }
}
