using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Account;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Admin.Account;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.League.Documents;
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
