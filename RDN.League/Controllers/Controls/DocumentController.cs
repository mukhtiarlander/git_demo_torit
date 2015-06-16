using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Cache;
using RDN.Library.Classes.Document;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.League.Classes;
using RDN.Library.Util.Enum;
using RDN.Utilities.IO;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class DocumentController : Controller
    {
        [Authorize]
        public ActionResult FullTextSearchLeague(string leagueId, string text, string folderId, string groupId)
        {
            try
            {
                long foldId = 0;
                long gId = 0;
                if (!String.IsNullOrEmpty(folderId))
                    foldId = Convert.ToInt64(folderId);
                if (!String.IsNullOrEmpty(groupId))
                    gId = Convert.ToInt64(groupId);
                var succ = DocumentSearch.FullTextSearchForLeague(new Guid(leagueId), text, foldId, gId);
                return Json(new { isSuccess = true, results = succ }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult SeachByDocumentName(string leagueId, string text, string folderId, string groupId)
        {
            try
            {
                long foldId = 0;
                long gId = 0;
                if (!String.IsNullOrEmpty(folderId))
                    foldId = Convert.ToInt64(folderId);
                if (!String.IsNullOrEmpty(groupId))
                    gId = Convert.ToInt64(groupId);
                var succ = DocumentSearch.SearchDocumentByName(new Guid(leagueId), text, foldId, gId);
                return Json(new { isSuccess = true, results = succ }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult DeleteDocument(string ownerId, string doc)
        {
            try
            {
                 bool success = true;
                string[] docs = doc.Split(',');
                for(int i=0; i<docs.Count(); i++)
                {
                    bool succ = DocumentRepository.DeleteDocument(new Guid(ownerId), Convert.ToInt64(docs[i]));
                    if (succ == false)
                        success = false;
                }
                MemberCache.ClearLeagueDocument(RDN.Library.Classes.Account.User.GetMemberId());
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult RenameDocument(string ownerId, string doc, string newName)
        {
            try
            {
                var document = DocumentRepository.RenameDocument(new Guid(ownerId), Convert.ToInt64(doc), newName);
                MemberCache.ClearLeagueDocument(RDN.Library.Classes.Account.User.GetMemberId());
                if (document != null)
                    return Json(new { isSuccess = true, link = "<a href='" + Url.Content("~/document/download/" + document.DocumentId.ToString().Replace("-", "")) + "'>" + document.DocumentName + "</a>" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult MoveFileTo(string ownerId, string moveTo, string moveToName, string doc)
        {
            try
            {
                bool success = true;

                string[] docs = doc.Split(',');
                for (int i = 0; i < docs.Count(); i++)
                {
                    bool succ = true;
                    if (String.IsNullOrEmpty(moveTo))
                        moveTo = "0";
                    if (!moveToName.Contains("G-"))
                        succ = DocumentRepository.MoveDocumentFolder(new Guid(ownerId), Convert.ToInt64(moveTo), Convert.ToInt64(docs[i]));
                    else
                        succ = DocumentRepository.MoveDocumentToGroup(new Guid(ownerId), Convert.ToInt64(moveTo), Convert.ToInt64(docs[i]));

                    if (succ == false)
                        success = false;
                }
               
                MemberCache.ClearLeagueDocument(RDN.Library.Classes.Account.User.GetMemberId());
                return Json(new { isSuccess = success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: ownerId + ":" + moveTo + ":" + moveToName + ":" + doc);
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult MoveFolderToGroup(string ownerId, string moveType, string moveTo, string fold)
        {
            try
            {
                bool succ = false;
                if (String.IsNullOrEmpty(moveTo))
                    moveTo = "0";
                if (moveType.Contains("G-"))
                    succ = DocumentRepository.MoveFolderToAnotherGroup(new Guid(ownerId), Convert.ToInt64(fold), Convert.ToInt64(moveTo));
                else
                    succ = DocumentRepository.MoveFolderToAnotherFolder(new Guid(ownerId), Convert.ToInt64(fold), Convert.ToInt64(moveTo));
                MemberCache.ClearLeagueDocument(RDN.Library.Classes.Account.User.GetMemberId());
                return Json(new { isSuccess = succ }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: ownerId + " " + moveTo + " " + fold);
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult DownloadFile(string documentId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (RDN.Library.Cache.MemberCache.IsAdministrator(memId) || DocumentRepository.HasAccessToDocument(new Guid(documentId), memId))
                {
                    Document doc = DocumentRepository.GetDocumentLocation(new Guid(documentId));
                    if (doc != null)
                    {
                        string mime = FileExt.GetMIMEType(doc.SaveLocation);
                        return File(doc.SaveLocation, mime, doc.DocumentName);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect("~/?u=" + SiteMessagesEnum.sww);
        }
        [Authorize]
        public ActionResult ViewFile(string documentId)
        {
            try
            {
                documentId = documentId.Split('.')[0];
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                bool hasAccess = DocumentRepository.HasAccessToDocument(new Guid(documentId.Split('.')[0]), memId);
                if (hasAccess)
                {
                    Document doc = SiteCache.GetDocument(new Guid(documentId));
                    if (doc != null)
                    {
                        string mime = FileExt.GetMIMEType(doc.SaveLocation);
                        return File(doc.SaveLocation, mime, doc.DocumentName);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect("~/?u=" + SiteMessagesEnum.sww);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UploadLeagueDocuments(DocumentRepository repo, IEnumerable<HttpPostedFileBase> files)
        {

            string additionalInfo = String.Empty;
            try
            {
                var folderId = HttpContext.Request.Form["uploadDocsDD"];
                long gId = 0;
                long fId = 0;
                if (!String.IsNullOrEmpty(folderId))
                {
                    if (folderId.Contains("G-"))
                        gId = Convert.ToInt64(folderId.Replace("G-", ""));
                    else
                        fId = Convert.ToInt64(folderId);
                }
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            additionalInfo += file.FileName + ":";
                            DocumentRepository.UploadLeagueDocument(repo.OwnerId, file.InputStream, file.FileName, groupId: gId, folderId: fId);
                        }
                    }
                    MemberCache.ClearLeagueDocument(RDN.Library.Classes.Account.User.GetMemberId());
                    return Redirect("~/league/documents/" + repo.OwnerId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.s);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: additionalInfo);
            }
            return Redirect("~/?u=" + SiteMessagesEnum.sww);
        }
        [Authorize]
        public ActionResult AddFolderToLeagueDocuments(Guid leagueId, string folderName)
        {
            try
            {
                long folderId = LeagueFolder.AddFolderToLeague(leagueId, folderName);
                MemberCache.ClearLeagueDocument(RDN.Library.Classes.Account.User.GetMemberId());
                return Json(new { isSuccess = true, folderId = folderId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult AddCommentToLeagueDocument(Guid docId, long docOwnerId, string comment)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                long commentId = CommentForDocument.AddCommentToLeagueDocument(docId, docOwnerId, comment, memId);
                MemberCache.ClearLeagueDocument(memId);
                return Json(new { isSuccess = true, commentId = commentId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult DeleteFolderFromLeagueDocuments(Guid leagueId, int folderId)
        {
            try
            {
                bool suc = LeagueFolder.DeleteFolderFromLeague(leagueId, Convert.ToInt64(folderId));
                MemberCache.ClearLeagueDocument(RDN.Library.Classes.Account.User.GetMemberId());
                return Json(new { isSuccess = suc }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult DeleteCommentForDocument(long commentId, long docOwnerId)
        {
            try
            {
                bool suc = CommentForDocument.DeleteCommentFromLeagueDocument(docOwnerId, commentId);
                MemberCache.ClearLeagueDocument(RDN.Library.Classes.Account.User.GetMemberId());
                return Json(new { isSuccess = suc }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

    }
}
