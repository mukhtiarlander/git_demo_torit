using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace RDN.Utilities.IO
{
    public class Compression
    {
        /// <summary>
        /// Compress the content of a stream. The 
        /// </summary>
        /// <param name="data">Data to compress</param>
        /// <param name="compressedData">Stream with the compressed data</param>        
        public static void Compress(ref Stream data, out Stream compressedData)
        {
            compressedData = new MemoryStream();
            var compress = new GZipStream(compressedData, CompressionMode.Compress);

            data.Position = 0;
            var buffer = new byte[data.Length];
            int n;
            while ((n = data.Read(buffer, 0, buffer.Length)) != 0)
                compress.Write(buffer, 0, n);   

            compress.Flush();
            data.Close();
        }

        /// <summary>
        /// returns the file path string of the encrypted file
        /// </summary>
        /// <param name="fi"></param>
        /// <returns></returns>
        public static string Compress(FileInfo fi)
        {
            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                // Prevent compressing hidden and 
                // already compressed files.
                if ((File.GetAttributes(fi.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden & fi.Extension != ".gz")
                {
                    // Create the compressed file.
                    FileStream outFile = File.Create(fi.FullName + ".gz");
                    using (GZipStream Compress = new GZipStream(outFile, CompressionMode.Compress))
                    {
                        inFile.CopyTo(Compress);
                    }
                    outFile.Close();
                    outFile.Dispose();
                    return fi.FullName + ".gz";
                }
            }
            return null;
        }

        /// <summary>
        /// Decompresses the content of a stream. The data stream will be closed
        /// </summary>
        /// <param name="data">Data to decompress</param>
        /// <param name="decompressedData">Stream with the decompressed data</param>        
        public static void Decompress(ref Stream data, out Stream decompressedData)
        {
            decompressedData = new MemoryStream();
            var decompress = new GZipStream(data, CompressionMode.Decompress);
            decompress.CopyTo(decompressedData);
            decompress.Close();
            data.Close();
        }

        public static string Decompress(FileInfo fi)
        {
            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                // Get original file extension, for example
                // "doc" from report.doc.gz.
                string curFile = fi.FullName;
                string origName = curFile.Remove(curFile.Length - fi.Extension.Length);

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
                return origName;
            }

        }
    }
}
