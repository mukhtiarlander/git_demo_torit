using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Classes.Forum.Enums
{
    /// <summary>
    /// tells the DB what type of forum to create and who will control it.
    /// </summary>
    public enum ForumOwnerTypeEnum
    {
        federation = 0,
        league = 1,
        main = 2
    }
}
