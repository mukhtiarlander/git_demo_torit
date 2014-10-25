using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Utilities;

namespace RDN.Library.Classes.Utilities
{

    public enum SitemapHelperEnum
    {
        always = 1,
        hourly = 2,
        daily = 3,
        weekly = 4,
        monthly = 5,
        yearly = 6,
        never = 7
    }
    public class SitemapHelper
    {
        public SitemapHelper()
        { }
        /// <summary>
        /// deletes a line item in the sitemap
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool DeleteSiteMapItem(string url)
        {
            var dc = new ManagementContext();
            var item = dc.SiteMap.Where(x => x.Url.ToLower() == url.ToLower()).FirstOrDefault();
            if (item != null)
                dc.SiteMap.Remove(item);
            dc.SaveChanges();
            return true;
        }

        /// <summary>
        /// returns the entire sitemap.
        /// </summary>
        /// <returns></returns>
        public static List<DataModels.Utilities.Sitemap> GetSitemap()
        {
            var dc = new ManagementContext();
            var site = dc.SiteMap.ToList();
            return site;
        }

        /// <summary>
        /// Adds a Node to the Site Map
        /// </summary>
        /// <param name="url">URL to add to SiteMap</param>
        /// <param name="modified">true oo false if the item has just been modified.</param>
        public static void AddNode(string url, bool modified)
        {
            var dc = new ManagementContext();
            var site = dc.SiteMap.Where(x => x.Url.ToLower() == url.ToLower()).FirstOrDefault();
            if (site == null)
            {
                Sitemap siteMap = new Sitemap();
                siteMap.ChangeFrequency = Convert.ToInt32(SitemapHelperEnum.monthly);
                siteMap.LastModified = DateTime.UtcNow;
                siteMap.Url = url.ToLower();
                dc.SiteMap.Add(siteMap);
                dc.SaveChanges();
            }
            else
            {
                if (modified)
                {
                    site.LastModified = DateTime.UtcNow;

                    TimeSpan span = DateTime.UtcNow.Subtract(site.LastModified.Value);
                    if (span.Days > 365)
                        site.ChangeFrequency = Convert.ToInt32(SitemapHelperEnum.yearly);
                    else if (span.Days > 31)
                        site.ChangeFrequency = Convert.ToInt32(SitemapHelperEnum.monthly);
                    else if (span.Days > 7)
                        site.ChangeFrequency = Convert.ToInt32(SitemapHelperEnum.weekly);
                    else if (span.Hours > 24)
                        site.ChangeFrequency = Convert.ToInt32(SitemapHelperEnum.daily);
                    else
                        site.ChangeFrequency = Convert.ToInt32(SitemapHelperEnum.hourly);
                    dc.SaveChanges();
                }
            }

        }

    }
}
