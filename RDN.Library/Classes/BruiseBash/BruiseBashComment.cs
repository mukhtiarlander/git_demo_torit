using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.BruiseBash
{
   public class BruiseBashComment
    {
       public long CommentId { get; set; }
       public string Comment { get; set; }
       public Guid OwnerId { get; set; }
       public DateTime Created { get; set; }
    }
}
