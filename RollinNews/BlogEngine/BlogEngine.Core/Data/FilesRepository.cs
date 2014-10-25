using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Providers;
using RDN.Library.Classes.RN.Funds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Statistics
    /// </summary>
    public class FilesRepository : IFilesRepository
    {
        /// <summary>
        /// Get stats info
        /// </summary>
        /// <returns>Stats counters</returns>
        public List<FileSystem.File> Get()
        {
            List<FileSystem.File> files = new List<FileSystem.File>();
            XmlFileSystemProvider pr = new XmlFileSystemProvider();
            var dir = BlogService.GetDirectory("~/");
            files = pr.GetFiles(dir).ToList();
            return files;

        }

    }
}
