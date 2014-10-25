using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RDNationLibrary.Static.Enums;

namespace RDNationLibrary.ViewModel
{
   public  class PenaltyViewModel
    {
       public PenaltiesEnum PenaltyType { get; set; }
       public long GameTimeInMilliseconds { get; set; }
       public long JamTimeInMilliseconds { get; set; }
       public int JamNumber { get; set; }
       public TeamMembersViewModel PenaltyAgainstMember { get; set; }
       
    }
}
