using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BlogEngine.Core.Data.Models
{
    public class FileItem
    {
        private string[] ImageExtensnios = { ".jpg", ".png", ".jpeg", ".tiff", ".gif", ".bmp" };
        public string FullPath { get; set; }
        public string Name { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime DateCreated { get; set; }
        public string Id { get; set; }
        public DateTime LastAccessTime { get; set; }
        public string ParentDirectory { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string SafeFilePath
        {
            get
            {
                return HttpUtility.UrlEncode(this.FilePath);
            }
        }
        public string Extension
        {
            get
            {
                return Path.GetExtension(Name);
            }
        }

    
        public string FileDownloadPath
        {
            get
            {
                
                    return string.Format("{0}FILES{1}.axdx", Utils.RelativeWebRoot, this.SafeFilePath);
            }
        }

        
    }
}
