using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngine.Core.Data.Models.Posts
{
  public   class PostModel
    {
      public long TotalPostCount { get; set; }


      public IEnumerable<PostItem> Posts { get; set; }
    }
}
