using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.League.Documents;

namespace RDN.Library.Classes.League.Classes
{
    public class LeagueFolder
    {


        public LeagueFolder()
        {

        }

        public static long AddFolderToLeague(Guid leagueId, string folderName)
        {
            try
            {
                var dc = new ManagementContext();
                DocumentCategory folder = new DocumentCategory();
                folder.CategoryName = folderName;
                folder.League = dc.Leagues.Where(x => x.LeagueId == leagueId).FirstOrDefault();
                dc.LeagueDocumentFolders.Add(folder);
                int c = dc.SaveChanges();
                if (c > 0)
                    return folder.CategoryId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }

        public static bool DeleteFolderFromLeague(Guid leagueId, long folderId)
        {
            try
            {
                var dc = new ManagementContext();
                var fold = dc.LeagueDocumentFolders.Where(x => x.League.LeagueId == leagueId && x.CategoryId == folderId).FirstOrDefault();
                if (fold != null)
                {
                    fold.ParentFolder = null;
                    var docs = dc.LeagueDocuments.Where(x => x.Category.CategoryId == folderId);
                    foreach (var doc in docs)
                    {
                        doc.Category = null;
                    }
                    dc.LeagueDocumentFolders.Remove(fold);
                    int c = dc.SaveChanges();
                    return c > 0;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        //public static LeagueFolder GetFoldersForLeague(Guid leagueId)
        //{
        //    LeagueFolder folders = new LeagueFolder();
        //    try
        //    {
        //        var dc = new ManagementContext();
        //        var folds = (from xx in dc.LeagueDocumentFolders.Include("League")
        //                     where xx.League.LeagueId == leagueId
        //                     select new
        //                     {
        //                         folder = xx,
        //                         docSize = dc.LeagueDocuments.Where(x => x.Category.CategoryId == xx.CategoryId),
        //                         docCount = dc.LeagueDocuments.Where(x => x.Category.CategoryId == xx.CategoryId).Count()
        //                     });
        //        foreach (var doc in folds)
        //        {
        //            folders.LeagueId = doc.folder.League.LeagueId;
        //            folders.LeagueName = doc.folder.League.Name;

        //            folders.Folders.Add(folder);
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }
        //    return folders;
        //}
        public static Document.Folder DisplayFolder(DataModels.League.Documents.DocumentCategory folder)
        {
            Document.Folder f = new Document.Folder();
            try
            {
                f.FolderId = folder.CategoryId;
                f.FolderName = folder.CategoryName;
                if (folder.Group != null)
                {
                    f.GroupId = folder.Group.Id;
                    f.ParentGroupFolderId = f.GroupId;
                }
                if (folder.ParentFolder != null)
                {
                    f.ParentFolderId = folder.ParentFolder.CategoryId;
                    f.ParentGroupFolderId = f.ParentFolderId;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return f;
        }
    }
}
