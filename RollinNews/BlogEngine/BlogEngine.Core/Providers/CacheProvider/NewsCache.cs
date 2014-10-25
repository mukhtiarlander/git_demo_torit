using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using RDN.Library.Classes.Federation;
using RDN.Library.Classes.Federation.Enums;
using RDN.Library.DataModels.Federation;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.DataModels.League;
using RDN.Library.Classes.League.Enums;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.League.Classes;
using RDN.Utilities.Enums;
using RDN.Library.Classes.Calendar;
using RDN.Library.Classes.Document;
using System.Net;
using RDN.Portable.Config;
using RDN.Library.Classes.Account.Classes.Json;
using RDN.Portable.Models.Json;
using BlogEngine.Core.Data.Models;

namespace BlogEngine.Core.Providers.CacheProvider
{
    public class NewsCache : CacheLock
    {
        List<FileItem> PictureArchive { get; set; }

        NewsCache()
        {
            PictureArchive = new List<FileItem>();
        }

        public static List<FileItem> GetPictureArchive()
        {
            var cache = GetCache();
            return cache.PictureArchive;
        }

        /// <summary>
        /// gets the cache of the member
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static NewsCache GetCache()
        {
            try
            {
                NewsCache dataObject = (NewsCache)HttpContext.Current.Cache["NewsCache"];
                if (dataObject == null)
                {
                    lock (ThisLock)
                    {
                        dataObject = (NewsCache)HttpContext.Current.Cache["NewsCache"];

                        if (dataObject == null)
                        {
                            dataObject = new NewsCache();
                            XmlFileSystemProvider pr = new XmlFileSystemProvider();
                            var dir111 = BlogService.GetDirectory("~/");
                            dataObject.PictureArchive = pr.GetAllImages(dir111).ToList();
                            HttpContext.Current.Cache["NewsCache"] = dataObject;
                        }
                    }
                }
                return dataObject;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static NewsCache UpdateCache(NewsCache newsCache)
        {
            try
            {
                lock (ThisLock)
                {
                    HttpContext.Current.Cache["NewsCache"] = newsCache;
                }
                return newsCache;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
    }
}
