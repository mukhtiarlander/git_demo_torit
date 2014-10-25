using RDN.Portable.Classes.Controls.Forum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Forum.Json
{
  public   class ForumSelectedJson
    {
      public List<ForumTopicJson> Topics { get; set; }
      public List<ForumCategory> Categories{ get; set; }
      public bool IsManager { get; set; }
    }
}
