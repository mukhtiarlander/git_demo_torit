using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using Scoreboard.Library.ViewModel;

namespace Scoreboard.Library.Util
{
    public class Compression
    {
        /// <summary>
        /// returns the file path string of the encrypted file
        /// </summary>
        /// <param name="fi"></param>
        /// <returns></returns>
        public static string Compress(FileInfo fi)
        {
            try
            {
                //there might be a problem with encryption, so we need to make sure the file exists.
                //before we complress.
                if (!fi.Exists)
                    return null;
                string dateString = DateTime.UtcNow.ToString("yyyyMMddmmss");
                // Get the stream of the source file.
                using (FileStream inFile = fi.OpenRead())
                {
                    // Prevent compressing hidden and 
                    // already compressed files.
                    if ((File.GetAttributes(fi.FullName) & FileAttributes.Hidden) != (FileAttributes.Hidden) & fi.Extension != ".gz")
                    {
                        // Create the compressed file.
                        FileStream outFile = File.Create(fi.FullName + dateString + ".gz");
                        using (GZipStream Compress = new GZipStream(outFile, CompressionMode.Compress))
                        {
                            inFile.CopyTo(Compress);
                        }
                        outFile.Close();
                        outFile.Dispose();
                        inFile.Close();
                        inFile.Dispose();
                        return fi.FullName + dateString + ".gz";
                    }

                }

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: fi.FullName);
            }
            return null;
        }

        public static string Decompress(FileInfo fi)
        {
            // Get original file extension, for example
            // "doc" from report.doc.gz.
            string curFile = fi.FullName;
            string origName = curFile.Remove(curFile.Length - fi.Extension.Length);
            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                //Create the decompressed file.
                FileStream outFile = File.Create(origName);
                using (GZipStream Decompress = new GZipStream(inFile, CompressionMode.Decompress))
                {
                    // Copy the decompression stream 
                    // into the output file.
                    Decompress.CopyTo(outFile);
                }
                outFile.Close();
                outFile.Dispose();
            }
            return origName;
        }
    }
}
