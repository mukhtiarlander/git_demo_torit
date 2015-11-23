using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Document.Enums;

namespace RDN.Library.Classes.Document
{
    public class Document
    {
        /// <summary>
        /// the document ID
        /// </summary>
        public Guid DocumentId { get; set; }
        public long CommentCount { get; set; }
        public long SizeOfDocument { get; set; }
        public Folder Folder { get; set; }
        public string UploadedHuman { get; set; }
        public DateTime Created{ get; set; }
        /// <summary>
        /// document ID for the league
        /// </summary>
        public long OwnerDocId { get; set; }
        public long GroupId { get; set; }
        public string GroupName { get; set; }
        public string DocumentName { get; set; }

        public MimeType MimeType { get; set; }
        
        public string SaveLocation { get; set; }
        public Guid OwnerId { get; set; }
        public List<CommentForDocument> Comments { get; set; }
        public string FullText { get; set; }
        public bool HasScannedText { get; set; }
        public int SearchMatches { get; set; }
        public bool IsRemoved { get; set; }
        public bool IsArchive { get; set; }
        public Document()
        {
            Folder = new Folder();
            Comments = new List<CommentForDocument>();
        }
    }
}
