using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using OfficeOpenXml;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.League.Classes;
using RDN.Library.DataModels.Context;
using RDN.Portable.Classes.League.Classes;
using RDN.Library.Classes.Config;


namespace RDN.Library.Classes.Document
{
    public class DocumentRepository
    {

        public string OwnerName { get; set; }
        public Guid OwnerId { get; set; }
        public List<Folder> Folders { get; set; }
        public List<Document> Documents { get; set; }
        public List<LeagueGroup> Groups { get; set; }
        public List<LeagueGroup> GroupsApartOf { get; set; }



        /// <summary>
        /// here just for the setttings page of the documents section.
        /// </summary>
        public List<LeagueGroup> GroupFolderSettings { get; set; }
        public DocumentRepository()
        {
            Groups = new List<LeagueGroup>();

            GroupFolderSettings = new List<LeagueGroup>();
            GroupsApartOf = new List<LeagueGroup>();
            Folders = new List<Folder>();
            Documents = new List<Document>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="leagueId"></param>
        /// <param name="fileStream"></param>
        /// <param name="nameOfFile"></param>
        /// <returns>document id of the league</returns>
        public static Document UploadLeagueDocument(Guid leagueId, Guid memId, Stream fileStream, string nameOfFile, string folderName = "", long groupId = 0, long folderId = 0)
        {
            try
            {
                nameOfFile = RDN.Utilities.Strings.StringExt.ToFileNameFriendly(nameOfFile);

                ManagementContext dc = new ManagementContext();
                var league = dc.Leagues.Where(x => x.LeagueId == leagueId).FirstOrDefault();
                //time stamp for the save location
                DateTime timeOfSave = DateTime.UtcNow;
                DataModels.League.Documents.DocumentCategory folderDb = null;
                if (folderName != "")
                {
                    folderDb = dc.LeagueDocumentFolders.Where(x => x.League.LeagueId == leagueId && x.CategoryName == folderName && x.IsRemoved == false).FirstOrDefault();
                    if (folderDb == null)
                    {
                        folderDb = new DataModels.League.Documents.DocumentCategory();
                        folderDb.CategoryName = folderName;
                        folderDb.League = league;
                        dc.LeagueDocumentFolders.Add(folderDb);
                    }
                }
                FileInfo info = new FileInfo(nameOfFile);

                string saveLocation = LibraryConfig.DocumentsSaveFolder + @"\" + timeOfSave.Year + @"\" + timeOfSave.Month + @"\" + timeOfSave.Day + @"\";

                if (!Directory.Exists(saveLocation))
                    Directory.CreateDirectory(saveLocation);

                DataModels.Document.Document doc = new DataModels.Document.Document();
                doc.DocumentSize = (int)fileStream.Length;

                doc.SaveLocation = "WebSiteDocuments";
                dc.Documents.Add(doc);
                int c = dc.SaveChanges();
                doc.SaveLocation = saveLocation + doc.DocumentId.ToString().Replace("-", "") + info.Extension;
                c = dc.SaveChanges();

                DataModels.League.Documents.LeagueDocument docL = new DataModels.League.Documents.LeagueDocument();
                docL.Document = doc;
                docL.League = league;
                if (folderName != "")
                    docL.Category = folderDb;
                docL.Name = info.Name;
                docL.UploaderMember = dc.Members.FirstOrDefault(x => x.MemberId == memId);
                if (groupId > 0)
                    docL.Group = dc.LeagueGroups.Where(x => x.Id == groupId).FirstOrDefault();
                if (folderId > 0)
                    docL.Category = dc.LeagueDocumentFolders.Where(x => x.CategoryId == folderId).FirstOrDefault();
                dc.LeagueDocuments.Add(docL);
                c = dc.SaveChanges();

                using (var newfileStream = new FileStream(doc.SaveLocation, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fileStream.CopyTo(newfileStream);
                }
                Document newDoc = new Document();
                newDoc.OwnerDocId = docL.DocumentId;
                newDoc.DocumentId = doc.DocumentId;
                newDoc.SaveLocation = doc.SaveLocation;
                return newDoc;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: nameOfFile);
            }
            return null;
        }


        public static bool HasAccessToDocument(Guid documentId, Guid memId)
        {
            try
            {
                var dc = new ManagementContext();
                var docs = (from xx in dc.LeagueDocuments
                            where xx.Document.DocumentId == documentId
                            select xx.League.LeagueId).ToList();
                foreach (var doc in docs)
                {
                    bool isMem = MemberCache.IsMemberApartOfLeague(memId, doc);
                    if (isMem)
                        return true;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool DeleteDocument(Guid ownerId, long docId)
        {
            try
            {
                Document doc = new Document();
                var dc = new ManagementContext();
                var docs = dc.LeagueDocuments.Where(x => x.League.LeagueId == ownerId && x.DocumentId == docId).FirstOrDefault();
                bool isMem = MemberCache.IsMemberApartOfLeague(RDN.Library.Classes.Account.User.GetMemberId(), docs.League.LeagueId);
                if (isMem)
                {
                    if (docs != null)
                    {
                        docs.Document = docs.Document;
                        docs.Document.FullText = "";
                        docs.IsRemoved = true;
                        int c = dc.SaveChanges();

                        return c > 0;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool ArchiveDocument(Guid ownerId, long docId, bool isArchived)
        {
            try
            {
                Document doc = new Document();
                var dc = new ManagementContext();
                var docs = dc.LeagueDocuments.FirstOrDefault(x => x.League.LeagueId == ownerId && x.DocumentId == docId);
                if (docs != null)
                {
                    bool isMem = MemberCache.IsMemberApartOfLeague(RDN.Library.Classes.Account.User.GetMemberId(),
                        docs.League.LeagueId);
                    if (isMem)
                    {
                        docs.Document = docs.Document;
                        docs.IsArchived = !isArchived;
                        int c = dc.SaveChanges();
                        return c > 0;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool RestoreDocument(long docId)
        {
            try
            {
                Document doc = new Document();
                var dc = new ManagementContext();
                var docs = dc.LeagueDocuments.Where(x => x.DocumentId == docId).FirstOrDefault();
                docs.Document = docs.Document;
                docs.Document.FullText = "";
                docs.IsRemoved = false;
                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static Document RenameDocument(Guid ownerId, long docId, string documentName)
        {
            try
            {
                Document doc = new Document();
                var dc = new ManagementContext();

                documentName = Path.GetFileNameWithoutExtension(documentName);
                var docs = dc.LeagueDocuments.Where(x => x.League.LeagueId == ownerId && x.DocumentId == docId).FirstOrDefault();
                bool isMem = MemberCache.IsMemberApartOfLeague(RDN.Library.Classes.Account.User.GetMemberId(), docs.League.LeagueId);
                if (isMem)
                {
                    if (docs != null)
                    {
                        docs.Document = docs.Document;
                        FileInfo f = new FileInfo(docs.Name);
                        docs.Name = documentName + f.Extension;
                        int c = dc.SaveChanges();
                        Document d = new Document();
                        d.DocumentId = docs.Document.DocumentId;
                        d.DocumentName = docs.Name;
                        return d;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        /// <summary>
        /// moves the document to another folder.
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="folderId"></param>
        /// <param name="docId"></param>
        /// <returns></returns>
        public static bool MoveDocumentFolder(Guid ownerId, long folderId, long docId)
        {
            try
            {
                Document doc = new Document();
                var dc = new ManagementContext();
                var docs = dc.LeagueDocuments.Where(x => x.League.LeagueId == ownerId && x.DocumentId == docId).FirstOrDefault();
                bool isMem = MemberCache.IsMemberApartOfLeague(RDN.Library.Classes.Account.User.GetMemberId(), docs.League.LeagueId);
                if (isMem)
                {
                    if (docs != null)
                    {
                        docs.Document = docs.Document;
                        docs.Category = dc.LeagueDocumentFolders.Where(x => x.CategoryId == folderId).FirstOrDefault();
                        dc.Entry(docs).Reference(x => x.Group).CurrentValue = null;
                        int c = dc.SaveChanges();

                        return c > 0;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool MoveDocumentToGroup(Guid ownerId, long groupId, long docId)
        {
            try
            {
                Document doc = new Document();
                var dc = new ManagementContext();
                var docs = dc.LeagueDocuments.Where(x => x.League.LeagueId == ownerId && x.DocumentId == docId).FirstOrDefault();
                bool isMem = MemberCache.IsMemberApartOfLeague(RDN.Library.Classes.Account.User.GetMemberId(), docs.League.LeagueId);
                if (isMem)
                {
                    if (docs != null)
                    {
                        docs.Document = docs.Document;
                        docs.Group = dc.LeagueGroups.Where(x => x.Id == groupId).FirstOrDefault();
                        dc.Entry(docs).Reference(x => x.Category).CurrentValue = null;
                        int c = dc.SaveChanges();

                        return c > 0;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool MoveFolderToAnotherGroup(Guid ownerId, long folderId, long groupId)
        {
            try
            {
                var dc = new ManagementContext();

                var docs = dc.LeagueDocumentFolders.Where(x => x.League.LeagueId == ownerId && x.CategoryId == folderId).FirstOrDefault();
                bool isMem = MemberCache.IsMemberApartOfLeague(RDN.Library.Classes.Account.User.GetMemberId(), docs.League.LeagueId);
                if (isMem)
                {
                    if (docs != null)
                    {
                        if (groupId == 0)
                        {
                            dc.Entry(docs).Reference(x => x.Group).CurrentValue = null;
                            dc.Entry(docs).Reference(x => x.ParentFolder).CurrentValue = null;
                        }
                        else
                            docs.Group = dc.LeagueGroups.Where(x => x.Id == groupId).FirstOrDefault();
                        int c = dc.SaveChanges();

                        return c > 0;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool MoveFolderToAnotherFolder(Guid ownerId, long folderId, long parentFolderId)
        {
            try
            {
                var dc = new ManagementContext();

                var docs = dc.LeagueDocumentFolders.Where(x => x.League.LeagueId == ownerId && x.CategoryId == folderId).FirstOrDefault();
                bool isMem = MemberCache.IsMemberApartOfLeague(RDN.Library.Classes.Account.User.GetMemberId(), docs.League.LeagueId);
                if (isMem)
                {
                    if (docs != null)
                    {
                        if (parentFolderId == 0 || parentFolderId == folderId)
                        {
                            dc.Entry(docs).Reference(x => x.ParentFolder).CurrentValue = null;
                            dc.Entry(docs).Reference(x => x.Group).CurrentValue = null;
                        }
                        else
                            docs.ParentFolder = dc.LeagueDocumentFolders.Where(x => x.CategoryId == parentFolderId).FirstOrDefault();
                        int c = dc.SaveChanges();

                        return c > 0;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static Document GetLeagueDocument(Guid documentId, long leagueDocId)
        {
            try
            {
                var dc = new ManagementContext();
                var docs = dc.LeagueDocuments.Include("Comments").Include("Comments.Commentor").Where(x => x.Document.DocumentId == documentId && x.DocumentId == leagueDocId).FirstOrDefault();
                return LeagueDocument.DisplayDocument(docs, true);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static List<Document> GetLeagueDocuments(Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                var docsDb = dc.LeagueDocuments.Include("Document").Include("Comments").Include("Comments.Commentor").Where(x => x.League.LeagueId == leagueId && x.IsRemoved == false).ToList();
                List<Document> docs = new List<Document>();
                foreach (var d in docsDb)
                    docs.Add(LeagueDocument.DisplayDocument(d, true));
                return docs;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<Document>();
        }

        public static List<Document> GetLeagueDocumentsAll(Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                var docsDb = dc.LeagueDocuments.Include("Document").Include("Comments").Include("Comments.Commentor").Where(x => x.League.LeagueId == leagueId).ToList();
                List<Document> docs = new List<Document>();
                foreach (var d in docsDb)
                    docs.Add(LeagueDocument.DisplayDocument(d, true));
                return docs;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<Document>();
        }
        public static bool UpdateFullTextForDocument(Guid documentId, string fullText)
        {
            try
            {
                if (!String.IsNullOrEmpty(fullText))
                {
                    Document doc = new Document();
                    var dc = new ManagementContext();
                    var docs = dc.Documents.Where(x => x.DocumentId == documentId).FirstOrDefault();

                    if (docs != null)
                    {
                        docs.FullText = fullText;
                        if (!String.IsNullOrEmpty(fullText))
                            docs.HasScannedText = true;
                        docs.SaveLocation = docs.SaveLocation;
                        return dc.SaveChanges() > 0;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static Document GetDocumentLocation(Guid documentId)
        {
            try
            {
                Document doc = new Document();
                var dc = new ManagementContext();
                var docs = dc.LeagueDocuments.Where(x => x.Document.DocumentId == documentId).FirstOrDefault();

                if (docs != null)
                {
                    return LeagueDocument.DisplayDocument(docs, true);
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public static DocumentRepository GetArchivedDocuments(Guid leagueId, Guid memId, long folderId = 0, long groupId = 0)
        {
            try
            {
                DocumentRepository repo = new DocumentRepository();

                var dc = new ManagementContext();
                var docs = dc.Leagues.Include("Documents").Include("Documents.Category.Group").Include("Documents.Category").Include("Folders").Where(x => x.LeagueId == leagueId).FirstOrDefault();

                if (docs != null)
                {
                    repo.OwnerId = docs.LeagueId;
                    repo.OwnerName = docs.Name;
                    repo.Groups = MemberCache.GetLeagueGroupsOfMember().OrderBy(x => x.GroupName).ToList();
                    for (int i = 0; i < repo.Groups.Count; i++)
                    {
                        repo.GroupFolderSettings.Add(new LeagueGroup() { Id = repo.Groups[i].Id, GroupName = "G-" + repo.Groups[i].GroupName });
                    }
                    var fols = docs.Folders.OrderBy(x => x.CategoryName);
                    foreach (var doc in fols)
                    {
                        repo.Folders.Add(LeagueFolder.DisplayFolder(doc));
                        repo.GroupFolderSettings.Add(new LeagueGroup() { Id = doc.CategoryId, GroupName = "F-" + doc.CategoryName });
                    }
                    List<DataModels.League.Documents.LeagueDocument> documents = new List<DataModels.League.Documents.LeagueDocument>();
                    if (folderId > 0)
                        documents = docs.Documents.Where(x => x.Category != null && x.Category.CategoryId == folderId && x.IsRemoved == false && x.IsArchived).ToList();
                    else if (groupId > 0)
                    {//gets all documents within category group.
                        documents = docs.Documents.Where(x => x.Category != null && x.Category.Group != null && x.Category.Group.Id == groupId && x.IsRemoved == false && x.IsArchived == true).ToList();
                        //adds documents within JUST group.
                        var docs2 = docs.Documents.Where(x => x.Group != null && x.Group.Id == groupId && x.IsRemoved == false && x.IsArchived == true).ToList();
                        for (int i = 0; i < docs2.Count; i++)
                        {
                            if (documents.Where(x => x.DocumentId == docs2[i].DocumentId).FirstOrDefault() == null)
                                documents.Add(docs2[i]);
                        }
                        var docs3 = docs.Documents.Where(x => x.Category != null && x.Category.ParentFolder != null && x.Category.ParentFolder.Group != null && x.Category.ParentFolder.Group.Id == groupId && x.IsRemoved == false).ToList();
                        for (int i = 0; i < docs3.Count; i++)
                        {
                            if (documents.Where(x => x.DocumentId == docs3[i].DocumentId).FirstOrDefault() == null)
                                documents.Add(docs3[i]);
                        }
                    }
                    else
                        documents = docs.Documents.Where(x => x.IsRemoved == false && x.IsArchived == true).ToList();
                    var groups = MemberCache.GetGroupsApartOf(memId);
                    foreach (var doc in documents)
                    {
                        bool addDocDirtyBit = false;
                        var document = LeagueDocument.DisplayDocument(doc, false);
                        //check if the document is apart of a group.
                        if (document.Folder != null && document.Folder.GroupId > 0)
                        {
                            //check if member is apart of group doc is in.
                            if (groups.Where(x => x.Id == document.Folder.GroupId).FirstOrDefault() != null)
                                addDocDirtyBit = true;
                        }
                        else if (document.GroupId > 0)
                        {//if the document is apart of the group, check if the user is in the group.
                            if (groups.Where(x => x.Id == document.GroupId).FirstOrDefault() != null)
                                addDocDirtyBit = true;
                        }
                        else if (document.Folder != null && document.Folder.ParentFolderId > 0)
                        {
                            //if the folder/parent folder is in a group, we check if the user is apart of the group.
                            var temp = repo.Folders.Where(x => x.FolderId == document.Folder.ParentFolderId).FirstOrDefault();
                            if (temp.GroupId > 0)
                            {
                                if (groups.Where(x => x.Id == temp.GroupId).FirstOrDefault() != null)
                                    addDocDirtyBit = true;
                            }
                            else //we add the document to the collection since the parent folder doesn't have a group.
                                addDocDirtyBit = true;
                        }
                        else
                            addDocDirtyBit = true;

                        if (addDocDirtyBit)
                        {
                            repo.Documents.Add(document);
                            if (doc.Category != null)
                            {
                                var fol = repo.Folders.Where(x => x.FolderId == doc.Category.CategoryId).FirstOrDefault();
                                fol.DocumentCount += 1;
                                fol.FolderSize += doc.Document.DocumentSize;
                                fol.FolderSizeHuman = RDN.Utilities.Strings.StringExt.FormatBytes(fol.FolderSize);
                            }
                        }
                    }
                    repo.Documents = repo.Documents.OrderByDescending(x => x.Created).ToList();
                    return repo;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public static DocumentRepository GetLeagueDocumentRepository(Guid leagueId, Guid memId, long folderId = 0, long groupId = 0)
        {
            try
            {
                DocumentRepository repo = new DocumentRepository();

                var dc = new ManagementContext();
                var docs = dc.Leagues.Include("Documents").Include("Documents.Category.Group").Include("Documents.Category").Include("Folders").Where(x => x.LeagueId == leagueId).FirstOrDefault();

                if (docs != null)
                {
                    repo.OwnerId = docs.LeagueId;
                    repo.OwnerName = docs.Name;
                    repo.Groups = MemberCache.GetLeagueGroupsOfMember().OrderBy(x => x.GroupName).ToList();
                    for (int i = 0; i < repo.Groups.Count; i++)
                    {
                        repo.GroupFolderSettings.Add(new LeagueGroup() { Id = repo.Groups[i].Id, GroupName = "G-" + repo.Groups[i].GroupName });
                    }
                    var fols = docs.Folders.OrderBy(x => x.CategoryName);
                    foreach (var doc in fols)
                    {
                        repo.Folders.Add(LeagueFolder.DisplayFolder(doc));
                        repo.GroupFolderSettings.Add(new LeagueGroup() { Id = doc.CategoryId, GroupName = "F-" + doc.CategoryName });
                    }
                    List<DataModels.League.Documents.LeagueDocument> documents = new List<DataModels.League.Documents.LeagueDocument>();
                    if (folderId > 0)
                        documents = docs.Documents.Where(x => x.Category != null && x.Category.CategoryId == folderId && x.IsRemoved == false).ToList();
                    else if (groupId > 0)
                    {//gets all documents within category group.
                        documents = docs.Documents.Where(x => x.Category != null && x.Category.Group != null && x.Category.Group.Id == groupId && x.IsRemoved == false && x.IsArchived == false).ToList();
                        //adds documents within JUST group.
                        var docs2 = docs.Documents.Where(x => x.Group != null && x.Group.Id == groupId && x.IsRemoved == false && x.IsArchived == false).ToList();
                        for (int i = 0; i < docs2.Count; i++)
                        {
                            if (documents.Where(x => x.DocumentId == docs2[i].DocumentId).FirstOrDefault() == null)
                                documents.Add(docs2[i]);
                        }
                        var docs3 = docs.Documents.Where(x => x.Category != null && x.Category.ParentFolder != null && x.Category.ParentFolder.Group != null && x.Category.ParentFolder.Group.Id == groupId && x.IsRemoved == false).ToList();
                        for (int i = 0; i < docs3.Count; i++)
                        {
                            if (documents.Where(x => x.DocumentId == docs3[i].DocumentId).FirstOrDefault() == null)
                                documents.Add(docs3[i]);
                        }
                    }
                    else
                        documents = docs.Documents.Where(x => x.IsRemoved == false && x.IsArchived == false).ToList();
                    var groups = MemberCache.GetGroupsApartOf(memId);
                    foreach (var doc in documents)
                    {
                        bool addDocDirtyBit = false;
                        var document = LeagueDocument.DisplayDocument(doc, false);
                        if (document.UploaderMemberId != Guid.Empty)
                        {
                            if (document.UploaderMemberId == memId)
                            {
                                document.IsUploaderMember = true;
                            }
                        }

                        //check if the document is apart of a group.
                        if (document.Folder != null && document.Folder.GroupId > 0)
                        {
                            //check if member is apart of group doc is in.
                            if (groups.Where(x => x.Id == document.Folder.GroupId).FirstOrDefault() != null)
                                addDocDirtyBit = true;
                        }
                        else if (document.GroupId > 0)
                        {//if the document is apart of the group, check if the user is in the group.
                            if (groups.Where(x => x.Id == document.GroupId).FirstOrDefault() != null)
                                addDocDirtyBit = true;
                        }
                        else if (document.Folder != null && document.Folder.ParentFolderId > 0)
                        {
                            //if the folder/parent folder is in a group, we check if the user is apart of the group.
                            var temp = repo.Folders.Where(x => x.FolderId == document.Folder.ParentFolderId).FirstOrDefault();
                            if (temp.GroupId > 0)
                            {
                                if (groups.Where(x => x.Id == temp.GroupId).FirstOrDefault() != null)
                                    addDocDirtyBit = true;
                            }
                            else //we add the document to the collection since the parent folder doesn't have a group.
                                addDocDirtyBit = true;
                        }
                        else
                            addDocDirtyBit = true;

                        if (addDocDirtyBit)
                        {
                            repo.Documents.Add(document);
                            if (doc.Category != null)
                            {
                                var fol = repo.Folders.Where(x => x.FolderId == doc.Category.CategoryId).FirstOrDefault();
                                fol.DocumentCount += 1;
                                fol.FolderSize += doc.Document.DocumentSize;
                                fol.FolderSizeHuman = RDN.Utilities.Strings.StringExt.FormatBytes(fol.FolderSize);
                            }
                        }
                    }
                    repo.Documents = repo.Documents.OrderByDescending(x => x.Created).ToList();
                    return repo;
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public static bool DeleteOldLeagueDocuments()
        {
            try
            {
                DateTime dt = DateTime.UtcNow.AddYears(-2);
                List<RDN.Portable.Classes.League.Classes.League> leagues = new List<Portable.Classes.League.Classes.League>();
                var dc = new ManagementContext();
                var docsDb = dc.Leagues.Include("Documents").Where(x => x.SubscriptionPeriodEnds.GetValueOrDefault() < dt).ToList();

                if (docsDb != null)
                {
                    foreach (var d in docsDb)
                    {
                        //check see to current League have any docuement if so then move 
                        if (d.Documents.Count > 0)
                        {
                            //find all the active docs from list of League Documents 
                            var activeDocs = d.Documents.Where(fd => fd.Document.IsDeleted == false).ToList();

                            foreach (var doc in activeDocs)
                            {
                                try
                                {
                                    DeleteDocument(doc.Document.DocumentId);
                                }
                                catch (Exception exception)
                                {
                                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                                }
                            }

                        }
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        private static bool DeleteDocument(Guid docId)
        {
            var dc = new ManagementContext();
            int c = 0;

            var document = dc.Documents.Where(d => d.DocumentId == docId).FirstOrDefault();

            if (document != null)
            {
                FileInfo file = new FileInfo(document.SaveLocation);
                if (file.Exists)
                    file.Delete();

                document.IsDeleted = true;

                c = dc.SaveChanges();
            }
            return c > 0;
        }
    }
}
