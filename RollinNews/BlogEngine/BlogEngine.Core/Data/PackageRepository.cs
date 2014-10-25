using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Packaging;
using BlogEngine.Core.Web.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Caching;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Package repository
    /// </summary>
    public class PackageRepository : IPackageRepository
    {
        /// <summary>
        /// Find packages
        /// </summary>
        /// <param name="take">Items to take</param>
        /// <param name="skip">Items to skip</param>
        /// <param name="filter">Filter expression</param>
        /// <param name="order">Sort order</param>
        /// <returns>List of packages</returns>
        public IEnumerable<Package> Find(int take = 10, int skip = 0, string filter = "", string order = "")
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.AccessAdminPages))
                return new List<Package>();

            if (take == 0) take = CachedPackages.Count();
            if (string.IsNullOrEmpty(filter)) filter = "1==1";
            if (string.IsNullOrEmpty(order)) order = "LastUpdated desc";

            var packages = new List<Package>();
            var query = CachedPackages.AsQueryable().Where(filter);        

            foreach (var item in query.OrderBy(order).Skip(skip).Take(take))
            {
                packages.Add(item);
            }
            return packages;
        }

        /// <summary>
        /// Package by ID
        /// </summary>
        /// <param name="id">Package ID</param>
        /// <returns>Package</returns>
        public Package FindById(string id)
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.AccessAdminPages))
                throw new System.UnauthorizedAccessException();

            return CachedPackages.FirstOrDefault(pkg => pkg.Id == id);
        }

        /// <summary>
        /// Update package metadata
        /// </summary>
        /// <param name="item">Package object</param>
        /// <returns>True if success</returns>
        public bool Update(Package item)
        {
            if (!Security.IsAdministrator)
                throw new System.UnauthorizedAccessException();

            if (item == null)
                return false;

            switch (item.PackageType)
            {
                case "Extension":
                    if (!string.IsNullOrEmpty(item.Id))
                    {
                        var ext = ExtensionManager.GetExtension(item.Id);

                        if (ext == null)
                        {
                            // handle when extension and package ID different
                            var map = Packaging.FileSystem.ExtansionMap();
                            foreach (var m in map)
                            {
                                if (m.Value.ToString() == item.Id)
                                {
                                    ext = ExtensionManager.GetExtension(m.Key);
                                    ExtensionManager.ChangeStatus(m.Key, item.Enabled);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            ExtensionManager.ChangeStatus(item.Id, item.Enabled);
                        }

                        ext.Priority = item.Priority;
                        ExtensionManager.SaveToStorage(ext);
                        Blog.CurrentInstance.Cache.Remove(Constants.CacheKey);
                    }
                    break;
                case "Theme":
                    break;
                case "Widget":
                    break;
                default:
                    break;
            }
            return true;
        }

        /// <summary>
        /// Install package
        /// </summary>
        /// <param name="id">Package id</param>
        /// <returns>True if success</returns>
        public bool Install(string id)
        {
            if (!Security.IsAdministrator || !Blog.CurrentInstance.IsPrimary)
                throw new System.UnauthorizedAccessException();

            return Installer.InstallPackage(id).Success;
        }

        /// <summary>
        /// Uninstall package
        /// </summary>
        /// <param name="id">Package id</param>
        /// <returns>True if success</returns>
        public bool Uninstall(string id)
        {
            if (!Security.IsAdministrator || !Blog.CurrentInstance.IsPrimary)
                throw new System.UnauthorizedAccessException();

            return Installer.UninstallPackage(id).Success;
        }

        #region Private methods

        static IEnumerable<Package> CachedPackages
        {
            get
            {
                // uncomment this line to disable gallery caching for debugging
                // Blog.CurrentInstance.Cache.Remove(Constants.CacheKey);

                if (Blog.CurrentInstance.Cache[Constants.CacheKey] == null)
                {
                    Blog.CurrentInstance.Cache.Add(
                        Constants.CacheKey,
                        LoadPackages(),
                        null,
                        Cache.NoAbsoluteExpiration,
                        new TimeSpan(0, 15, 0),
                        CacheItemPriority.Low,
                        null);
                }
                return (IEnumerable<Package>)Blog.CurrentInstance.Cache[Constants.CacheKey];
            }
        }

        static List<Package> LoadPackages()
        {
            var packages = new List<Package>();

            Gallery.Load(packages);
            //Trace("01: ", packages);
            Packaging.FileSystem.Load(packages);
            //Trace("02: ", packages);
            LoadExtensions(packages);
            //Trace("03: ", packages);
            Installer.MarkAsInstalled(packages);
            //Trace("04: ", packages);

            return packages;
        }

        static void LoadExtensions(List<Package> packages)
        {
            var extensions = ExtensionManager.Extensions.Where(x => x.Key != "MetaExtension").ToList();

            foreach (KeyValuePair<string, ManagedExtension> ext in extensions)
            {
                var x = ExtensionManager.GetExtension(ext.Key);

                var adminPage = string.IsNullOrEmpty(x.AdminPage) ?
                string.Format(Utils.RelativeWebRoot + "admin/Extensions/Settings.aspx?ext={0}&enb={1}", x.Name, x.Enabled) :
                string.Format(x.AdminPage, x.Name, x.Enabled);

                // If extension name in gallery differ from package ID
                // they will show as 2 extensions in the list.
                // To avoid, we can add mapping to /app_data/extensionmap.txt
                // in format "ExtensionId=PackageName" to map exension id to package id
                var map = Packaging.FileSystem.ExtansionMap();
                var extId = map.ContainsKey(x.Name) ? map[x.Name] : x.Name;
                var existingPackage = packages.Where(p => p.Id == extId).FirstOrDefault();

                if (existingPackage == null)
                {
                    var p = new Package
                    {
                        Id = x.Name,
                        PackageType = "Extension",
                        Title = x.Name,
                        Description = x.Description,
                        LocalVersion = x.Version,
                        Authors = x.Author,
                        IconUrl = "http://dnbegallery.org/cms/Themes/OrchardGallery/Content/Images/extensionDefaultIcon.png",
                        Enabled = x.Enabled,
                        Priority = x.Priority,
                        SettingsUrl = x.Settings.Count > 0 ? adminPage : "",
                        Location = "L"
                    };
                    packages.Add(p);
                }
                else
                {
                    existingPackage.LocalVersion = x.Version;
                    existingPackage.Enabled = x.Enabled;
                }
            }
        }

        static void Trace(string msg, List<Package> packages)
        {
            string s = "{0}|{1}|{2}|{3}|{4}|{5}";
            foreach (var p in packages)
            {
                System.Diagnostics.Debug.WriteLine(string.Format(s, msg, p.PackageType, p.Id, p.Location, p.LocalVersion, p.OnlineVersion));
            }
        }

        IEnumerable<Package> FromGallery()
        {
            return CachedPackages.Where(p => p.Location == "G" || p.Location == "I");
        }

        IEnumerable<Package> LocalPackages()
        {
            return CachedPackages.Where(p => p.Location != "G");
        }

        #endregion
    }
}
