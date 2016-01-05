using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Geocoding;
using RDN.Library.Classes.Account;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Admin.Account;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.League.Documents;
using RDN.Library.DataModels.Tags;
using RDN.Portable.Classes.Account.Classes;

namespace RDN.Library.Classes.Document
{
    public class CommentForDocument
    {
        public long CommentId { get; set; }
        public string Comment { get; set; }
        public MemberDisplay Commentor { get; set; }
        public DateTime Created { get; set; }
        public string CreatedHuman { get; set; }

        public static long AddCommentToLeagueDocument(Guid documentId, long ownerId, string comment, Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var doc = dc.LeagueDocuments.Where(x => x.DocumentId == ownerId && x.Document.DocumentId == documentId).FirstOrDefault();
                DocumentComment com = new DocumentComment();
                com.Comment = comment;
                com.Commentor = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                com.LeagueDocument = doc;
                doc.Comments.Add(com);
                int c = dc.SaveChanges();
                if (c > 0)
                    return com.CommentId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }
        public static long AddTagToDocumentTag(Guid documentId, long ownerId, string tagName)
        {
            try
            {
                var dc = new ManagementContext();
                var tag = dc.Tags.FirstOrDefault(x => x.TagName == tagName);
                if (tag != null)
                {
                    var docTag =
                        dc.DocumentTags.FirstOrDefault(
                            x => x.LeagueDocument.DocumentId == ownerId && x.Tag.TagId == tag.TagId && x.IsRemoved == false);
                    if (docTag != null)
                        return docTag.Id;
                }
                else
                {
                    var newTag = new Tag();
                    newTag.TagName = tagName;
                    dc.Tags.Add(newTag);
                    int d = dc.SaveChanges();
                    if (d > 0)
                        tag = newTag;
                }
                var doc = dc.LeagueDocuments.FirstOrDefault(x => x.DocumentId == ownerId && x.Document.DocumentId == documentId);
                var newdocTag = new DocumentTag();
                newdocTag.Tag = tag;
                newdocTag.LeagueDocument = doc;
                if (doc != null) doc.DocumentTags.Add(newdocTag);
                int c = dc.SaveChanges();
                if (c > 0)
                    return newdocTag.Id;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }

        public static List<DocumentTag> FetchDocumentTags(long docOwnerId)
        {
            try
            {
                var dc = new ManagementContext();
                return
                    dc.DocumentTags.Where(x => x.LeagueDocument.DocumentId == docOwnerId && x.IsRemoved == false)
                        .ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<DocumentTag>();
        }
        public static List<DocumentTag> FetchLeagueTags(Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                return dc.DocumentTags.Where(x => x.LeagueDocument.League.LeagueId == leagueId && x.IsRemoved == false).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<DocumentTag>();
        }

        public static bool UpdateLeagueTags(long ownerId, string tags)
        {
            try
            {
                var dc = new ManagementContext();
                var tagItems = tags.Split(',');
                var tagsIdsToRemove =
                    dc.DocumentTags.Where(
                        x =>
                            x.LeagueDocument.DocumentId == ownerId && x.IsRemoved == false &&
                            !tagItems.Contains(x.Tag.TagName)).Select(x => x.Id).ToArray();
                var tagsToRemove = dc.DocumentTags.Where(x => tagsIdsToRemove.Contains(x.Id));
                tagsToRemove.ForEach(x => x.IsRemoved = true);
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool DeleteCommentFromLeagueDocument(long docOwnerId, long commentId)
        {
            try
            {
                var dc = new ManagementContext();
                var com = dc.DocumentComments.Where(x => x.CommentId == commentId && x.LeagueDocument.DocumentId == docOwnerId).FirstOrDefault();
                com.IsRemoved = true;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
    }
}
