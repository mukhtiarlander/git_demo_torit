using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using OfficeOpenXml;
using RDN.Library.Cache;
using RDN.Library.Classes.Document.Json;
using RDN.Library.Classes.Error;

namespace RDN.Library.Classes.Document
{
    public class DocumentSearch
    {
        public static List<DocumentJson> FullTextSearchForLeague(Guid leagueId, string text, long folderId = 0, long groupId = 0)
        {
            List<DocumentJson> temps = new List<DocumentJson>();
            try
            {
                var searchText = text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < searchText.Count(); i++)
                {
                    searchText[i] = searchText[i].Trim();
                }
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var docs = RDN.Library.Classes.Document.DocumentRepository.GetLeagueDocumentRepository(leagueId, memId, folderId, groupId);

                foreach (var d in docs.Documents)
                {
                    try
                    {
                        //need to zero out maches because we are pulling from the cached referenced object.
                        d.SearchMatches = 0;
                        if (!String.IsNullOrEmpty(d.FullText))
                        {
                            SearchText(searchText, d, d.FullText);
                        }
                        else
                        {
                            //need 
                            //odt
                            //gsheet
                            switch (d.MimeType)
                            {
                                case Enums.MimeType.doc:
                                case Enums.MimeType.dotx:
                                case Enums.MimeType.txt:
                                case Enums.MimeType.xml:
                                case Enums.MimeType.csv:
                                case Enums.MimeType.wps:
                                    SearchDocument(searchText, d);
                                    if (!d.HasScannedText && !String.IsNullOrEmpty(d.FullText))
                                    {
                                        bool success = DocumentRepository.UpdateFullTextForDocument(d.DocumentId, d.FullText);
                                        if (success)
                                            MemberCache.ClearLeagueDocument(memId);
                                        d.HasScannedText = true;
                                    }
                                    break;
                                case Enums.MimeType.excel:
                                    SearchExcel(searchText, d);
                                    if (!d.HasScannedText && !String.IsNullOrEmpty(d.FullText))
                                    {
                                        bool success = DocumentRepository.UpdateFullTextForDocument(d.DocumentId, d.FullText);
                                        if (success)
                                            MemberCache.ClearLeagueDocument(memId);
                                        d.HasScannedText = true;
                                    }
                                    break;
                                case Enums.MimeType.excelOld:
                                case Enums.MimeType.xlsm:
                                    SearchExcelOld(searchText, d);
                                    if (!d.HasScannedText && !String.IsNullOrEmpty(d.FullText))
                                    {
                                        bool success = DocumentRepository.UpdateFullTextForDocument(d.DocumentId, d.FullText);
                                        if (success)
                                            MemberCache.ClearLeagueDocument(memId);
                                        d.HasScannedText = true;
                                    }
                                    break;
                                case Enums.MimeType.pdf:
                                    SearchPDF(searchText, d);
                                    if (!d.HasScannedText && !String.IsNullOrEmpty(d.FullText))
                                    {
                                        bool success = DocumentRepository.UpdateFullTextForDocument(d.DocumentId, d.FullText);
                                        if (success)
                                            MemberCache.ClearLeagueDocument(memId);
                                        d.HasScannedText = true;
                                    }
                                    break;
                                case Enums.MimeType.ppt:
                                case Enums.MimeType.odp:
                                    SearchPPT(searchText, d);
                                    if (!d.HasScannedText && !String.IsNullOrEmpty(d.FullText))
                                    {
                                        bool success = DocumentRepository.UpdateFullTextForDocument(d.DocumentId, d.FullText);
                                        if (success)
                                            MemberCache.ClearLeagueDocument(memId);
                                        d.HasScannedText = true;
                                    }
                                    break;
                                case Enums.MimeType.rtf:
                                    SearchRTF(searchText, d);
                                    if (!d.HasScannedText && !String.IsNullOrEmpty(d.FullText))
                                    {
                                        bool success = DocumentRepository.UpdateFullTextForDocument(d.DocumentId, d.FullText);
                                        if (success)
                                            MemberCache.ClearLeagueDocument(memId);
                                        d.HasScannedText = true;
                                    }
                                    break;
                            }

                        }


                        //don't need to show save location to folks outside.
                        //d.SaveLocation = String.Empty;
                        if (d.SearchMatches > 0)
                        {
                            temps.Add(new DocumentJson(d));
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                        //we set this as true because of document locks in place.
                        //Can not open the package. Package is an OLE compound document. If this is an encrypted package, please supply the password
                        d.HasScannedText = true;
                    }
                }
                return temps;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return temps;
        }

        private static void SearchDocument(string[] searchText, Document d)
        {
            if (new FileInfo(d.SaveLocation).Exists)
            {
                using (StreamReader sr = new StreamReader(d.SaveLocation))
                {
                    string fileContent = sr.ReadToEnd();
                    SearchText(searchText, d, fileContent);
                }
            }
        }

        private static void SearchText(string[] searchText, Document d, string fileContent)
        {
            d.FullText = fileContent;
            //its been shown that Index Ordinal is faster than boyer-Moore.
            //because of the managed code.
            //http://www.blackbeltcoder.com/Articles/algorithms/fast-text-search-with-boyer-moore
            foreach (var t in searchText)
            {
                int pos = fileContent.IndexOf(t, 0, StringComparison.OrdinalIgnoreCase);
                while (pos != -1)
                {
                    d.SearchMatches++;
                    pos = fileContent.IndexOf(t, pos + t.Length, StringComparison.OrdinalIgnoreCase);
                }
            }
        }
        /// <summary>
        /// searches .xlsx files
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="d"></param>
        private static void SearchExcel(string[] searchText, Document d)
        {
            try
            {
                FileInfo fi = new FileInfo(d.SaveLocation);
                if (fi.Exists)
                {
                    ExcelPackage workbook = new ExcelPackage(fi);
                    string fileContent = String.Empty;
                    try
                    {
                        for (int i = 1; i < workbook.Workbook.Worksheets.Count; i++)
                        {
                            try
                            {
                                foreach (var cell in workbook.Workbook.Worksheets[i].Cells)
                                {
                                    fileContent += cell.Value + " ";
                                }
                            }
                            catch (Exception exception)
                            {
                                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: d.SaveLocation);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: d.SaveLocation);
                    }
                    SearchText(searchText, d, fileContent);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: d.SaveLocation);
            }
        }

        /// <summary>
        /// searches .xls files.
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="d"></param>
        private static void SearchExcelOld(string[] searchText, Document d)
        {
            try
            {

                ExcelLibrary.SpreadSheet.Workbook workbook = ExcelLibrary.SpreadSheet.Workbook.Load(d.SaveLocation);
                string fileContent = String.Empty;
                for (int i = 1; i < workbook.Worksheets.Count; i++)
                {
                    foreach (var cell in workbook.Worksheets[i].Cells)
                    {
                        fileContent += cell.Right.Value + " ";
                    }
                }
                SearchText(searchText, d, fileContent);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: d.SaveLocation);
            }
        }
        private static void SearchRTF(string[] searchText, Document d)
        {
            FileInfo fi = new FileInfo(d.SaveLocation);
            if (fi.Exists)
            {
                //Create the RichTextBox. (Requires a reference to System.Windows.Forms.)
                System.Windows.Forms.RichTextBox rtBox = new System.Windows.Forms.RichTextBox();

                // Get the contents of the RTF file. When the contents of the file are   
                // stored in the string (rtfText), the contents are encoded as UTF-16.  
                string rtfText = System.IO.File.ReadAllText(d.SaveLocation);

                // Use the RichTextBox to convert the RTF code to plain text.
                rtBox.Rtf = rtfText;
                string fileContent = rtBox.Text;

                SearchText(searchText, d, fileContent);
            }

        }
        private static void SearchPDF(string[] searchText, Document d)
        {
            try
            {
                string fileContent = String.Empty;
                FileInfo fi = new FileInfo(d.SaveLocation);
                if (fi.Exists)
                {
                    PdfReader reader = new PdfReader(d.SaveLocation);

                    for (int i = 1; i <= reader.NumberOfPages; i++)
                        fileContent += PdfTextExtractor.GetTextFromPage(reader, i, new SimpleTextExtractionStrategy());

                    SearchText(searchText, d, fileContent);
                }
            }
            catch (Exception exception)
            {
                if (exception.Message.Contains("Bad user password"))
                    d.FullText = "Password Protected";
                else
                    ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: d.SaveLocation);
            }
        }
        private static void SearchPPT(string[] searchText, Document d)
        {
            try
            {
                string fileContent = String.Empty;
                FileInfo fi = new FileInfo(d.SaveLocation);
                if (fi.Exists)
                {
                    int numberOfSlides = CountSlides(d.SaveLocation);
                    string slideText;
                    for (int i = 0; i < numberOfSlides; i++)
                    {
                        fileContent += GetSlideIdAndText(d.SaveLocation, i);
                    }
                    SearchText(searchText, d, fileContent);
                }
            }
            catch (Exception exception)
            {
                if (exception.Message.Contains("File contains corrupted data"))
                    d.FullText = "File Can't Be Scanned.  Corruption Issue.";
                else
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        // Count the slides in the presentation.
        private static int CountSlides(PresentationDocument presentationDocument)
        {
            // Check for a null document object.
            if (presentationDocument == null)
            {
                throw new ArgumentNullException("presentationDocument");
            }

            int slidesCount = 0;

            // Get the presentation part of document.
            PresentationPart presentationPart = presentationDocument.PresentationPart;
            // Get the slide count from the SlideParts.
            if (presentationPart != null)
            {
                slidesCount = presentationPart.SlideParts.Count();
            }
            // Return the slide count to the previous method.
            return slidesCount;
        }
        private static int CountSlides(string presentationFilePath)
        {
            // Open the presentation as read-only.
            using (PresentationDocument presentationDocument = PresentationDocument.Open(presentationFilePath, false))
            {
                // Pass the presentation to the next CountSlides method
                // and return the slide count.
                return CountSlides(presentationDocument);
            }
        }

        private static string GetSlideIdAndText(string filePath, int index)
        {
            StringBuilder paragraphText = new StringBuilder();
            try
            {
                using (PresentationDocument ppt = PresentationDocument.Open(filePath, false))
                {
                    // Get the relationship ID of the first slide.
                    PresentationPart part = ppt.PresentationPart;
                    OpenXmlElementList slideIds = part.Presentation.SlideIdList.ChildElements;

                    string relId = (slideIds[index] as SlideId).RelationshipId;

                    // Get the slide part from the relationship ID.
                    SlidePart slide = (SlidePart)part.GetPartById(relId);

                    // Iterate through all the paragraphs in the slide.
                    foreach (DocumentFormat.OpenXml.Drawing.Paragraph paragraph in
                        slide.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>())
                    {
                        // Iterate through the lines of the paragraph.
                        foreach (DocumentFormat.OpenXml.Drawing.Text text in
                            paragraph.Descendants<DocumentFormat.OpenXml.Drawing.Text>())
                        {
                            // Append each line to the previous lines.
                            paragraphText.Append(text.Text);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return paragraphText.ToString();
        }
    }
}
