using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Document.Enums;

namespace RDN.Library.Classes.Document.Json
{
    public class DocumentJson
    {
        /// <summary>
        /// the document ID
        /// </summary>
        public Guid DocumentId { get; set; }
        public long CommentCount { get; set; }
        public long SizeOfDocument { get; set; }
        public Folder Folder { get; set; }
        public string UploadedHuman { get; set; }
        /// <summary>
        /// document ID for the league
        /// </summary>
        public long OwnerDocId { get; set; }
        public string DocumentName { get; set; }

        public MimeType MimeType { get; set; }

        public Guid OwnerId { get; set; }
        public List<CommentForDocument> Comments { get; set; }
        public int SearchMatches { get; set; }
        public DocumentJson(Document doc)
        {
            Folder = new Folder();
            Comments = new List<CommentForDocument>();
            DocumentId = doc.DocumentId;
            CommentCount = doc.CommentCount;
            SizeOfDocument = doc.SizeOfDocument;
            if (doc.Folder != null)
                Folder = new Classes.Document.Folder() { DocumentCount = doc.Folder.DocumentCount, FolderId = doc.Folder.FolderId, FolderName= doc.Folder.FolderName };
            UploadedHuman = doc.UploadedHuman;
            OwnerDocId = doc.OwnerDocId;
            DocumentName = doc.DocumentName;
            MimeType = doc.MimeType;
            OwnerId = doc.OwnerId;
            SearchMatches = doc.SearchMatches;
        }
    }
}
