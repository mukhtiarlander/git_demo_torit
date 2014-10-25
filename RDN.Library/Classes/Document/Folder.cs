using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Document
{
   public  class Folder
    {
        
       public long FolderId { get; set; }
       public string GroupFolderId { get; set; }
       public string FolderName { get; set; }
       public int DocumentCount { get; set; }
       public string FolderSizeHuman { get; set; }
       public long FolderSize { get; set; }
       
       public long GroupId { get; set; }
       public long ParentFolderId { get; set; }
       /// <summary>
       /// can be considered the Group or Parent Folder Id with the settings.
       /// </summary>
       public long ParentGroupFolderId { get; set; }
    }
}
