using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlogEngine.Core.Providers.CacheProvider
{
    /// <summary>
    /// this is an inherited only cache lock so we can lock this object when we insert into the cache object.
    /// </summary>
   public  class CacheLock
    {
       public static object ThisLock = new object();
         

    }
}
