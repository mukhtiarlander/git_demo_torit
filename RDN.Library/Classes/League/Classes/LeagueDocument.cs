using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RDN.Library.Cache;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Admin.Account;
using RDN.Library.Classes.Document;
using RDN.Library.Classes.Document.Enums;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Portable.Classes.Account.Classes;

namespace RDN.Library.Classes.League.Classes
{
    public class LeagueDocument
    {
        public static Document.Document DisplayDocument(DataModels.League.Documents.LeagueDocument docsDb, bool pullComments)
        {
            try
            {
                Document.Document doc = new Document.Document();
                doc.DocumentId = docsDb.Document.DocumentId;
                doc.DocumentName = docsDb.Name;
                doc.OwnerDocId = docsDb.DocumentId;
                doc.SizeOfDocument = docsDb.Document.DocumentSize;
                if (docsDb.League != null)
                    doc.OwnerId = docsDb.League.LeagueId;
                doc.IsRemoved = docsDb.IsRemoved;
                doc.MimeType = GetDocumentMimeType(docsDb.Name);
                doc.SaveLocation = docsDb.Document.SaveLocation;
                doc.FullText = docsDb.Document.FullText;
                doc.IsArchive = docsDb.IsArchived;
                doc.HasScannedText = docsDb.Document.HasScannedText;
                if (docsDb.Group != null)
                {
                    doc.GroupId = docsDb.Group.Id;
                    doc.GroupName = docsDb.Group.GroupName;
                }
                doc.UploadedHuman = RDN.Portable.Util.DateTimes.DateTimeExt.RelativeDateTime(docsDb.Created);
                doc.Created = docsDb.Created;
                if (docsDb.Category != null)
                {
                    doc.Folder = new Document.Folder();
                    doc.Folder.FolderId = docsDb.Category.CategoryId;
                    doc.Folder.FolderName = docsDb.Category.CategoryName;
                    if (docsDb.Category.Group != null)
                        doc.Folder.GroupId = docsDb.Category.Group.Id;
                    if (docsDb.Category.ParentFolder != null)
                        doc.Folder.ParentFolderId = docsDb.Category.ParentFolder.CategoryId;
                }
                var comments = docsDb.Comments.Where(x => x.IsRemoved == false).OrderByDescending(x => x.Created);
                doc.CommentCount = comments.Count();
                if (pullComments)
                {
                    try
                    {
                        foreach (var co in comments)
                        {
                            var comment = new Document.CommentForDocument()
                                    {
                                        CreatedHuman = RDN.Portable.Util.DateTimes.DateTimeExt.RelativeDateTime(co.Created),
                                        Created = co.Created,
                                        Comment = co.Comment,
                                        CommentId = co.CommentId,
                                    };
                            if (co.Commentor != null)
                            {
                                comment.Commentor = new MemberDisplay()
                                     {
                                         MemberId = co.Commentor.MemberId,
                                         DerbyName = co.Commentor.DerbyName,
                                         PlayerNumber = co.Commentor.PlayerNumber
                                     };
                            }
                            var leagueId = MemberCache.GetLeagueIdOfMember(RDN.Library.Classes.Account.User.GetMemberId());
                            var lstTags = CommentForDocument.FetchLeagueTags(leagueId).Select(x => x.Tag.TagName);
                            string.Join(",", lstTags);
                            doc.Comments.Add(comment);
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }

                return doc;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public static MimeType GetDocumentMimeType(string docName)
        {
            try
            {
                FileInfo fi = new FileInfo(docName);
                switch (fi.Extension.ToLower())
                {
                    case ".odf":
                        return MimeType.odf;
                    case ".pdf":
                        return MimeType.pdf;
                    case ".doc":
                    case ".docx":
                        return MimeType.doc;
                    case ".dotx":
                        return MimeType.dotx;
                    case ".xls":
                        return Document.Enums.MimeType.excelOld;
                    case ".xlsx":
                        return Document.Enums.MimeType.excel;
                    case ".jpg":
                        return MimeType.jpg;
                    case ".jpeg":
                        return MimeType.jpeg;
                    case ".png":
                        return MimeType.png;
                    case ".bmp":
                        return MimeType.bmp;
                    case ".tif":
                        return Document.Enums.MimeType.tif;
                    case ".zip":
                        return MimeType.zip;
                    case ".txt":
                        return MimeType.txt;
                    case ".xml":
                        return MimeType.xml;
                    case ".ppt":
                    case ".pptx":
                        return MimeType.ppt;
                    case ".pub":
                        return MimeType.pub;
                    case ".rtf":
                        return MimeType.rtf;
                    case ".eps":
                        return MimeType.eps;
                    case ".odt":
                        return MimeType.odt;
                    case ".psd":
                        return MimeType.psd;
                    case ".gsheet":
                        return MimeType.gsheet;
                    case ".gdraw":
                        return MimeType.gdraw;
                    case ".gdoc":
                        return MimeType.gdoc;
                    case ".exe":
                        return MimeType.exe;
                    case ".svg":
                        return MimeType.svg;
                    case ".gif":
                        return MimeType.gif;
                    case ".ai":
                        return MimeType.ai;
                    case ".html":
                    case ".htm":
                        return MimeType.html;
                    case ".pages":
                        return MimeType.pages;
                    case ".mht":
                        return MimeType.mht;
                    case ".xlsm":
                        return MimeType.xlsm;
                    case ".csv":
                        return MimeType.csv;
                    case ".odp":
                        return MimeType.odp;
                    case "":
                        return MimeType.None;
                    case ".ods":
                        return MimeType.ods;
                    case ".mp3":
                        return MimeType.mp3;
                    case ".wmv":
                        return MimeType.wmv;
                    case ".ps":
                        return MimeType.ps;
                    case ".wps":
                        return MimeType.wps;
                    case ".xps":
                        return MimeType.xps;
                    case ".gform":
                        return MimeType.gform;
                    case ".vsd":
                        return MimeType.vsd;
                    default:
                        {
                            ErrorDatabaseManager.AddException(new Exception("Mime Type Not Found: " + fi.Extension + ":" + docName), new Exception().GetType());
                            return MimeType.None;
                        }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return MimeType.None;

        }

    }
}
