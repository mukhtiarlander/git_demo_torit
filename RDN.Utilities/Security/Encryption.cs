using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using RDN.Utilities.IO;

namespace RDN.Utilities.Security
{
    public class Encryption
    {
        private const string PassKey = "182E7E79DC0A07D9FC3CA1D103571A67";
        private const string Iv = "D316823454888C02";

        /// <summary>   
        /// Decrypts the input file (strInputFileName) and creates a new decrypted file (strOutputFileName)   
        /// </summary>   
        /// <param name="strInputFileName">input file name</param>   
        /// <param name="strOutputFileName">output file name</param>   
        public static void DecryptFiletoFile(string strInputFileName, string strOutputFileName)
        {
            string strFileData = "";
            using (FileStream inputStream = new FileStream(strInputFileName, FileMode.Open, FileAccess.Read))
            {
                var cryptic = new TripleDESCryptoServiceProvider();
                cryptic.Key = FromHexString(PassKey);
                cryptic.IV = FromHexString(Iv);

                CryptoStream crStream = new CryptoStream(inputStream, cryptic.CreateDecryptor(), CryptoStreamMode.Read);

                StreamReader reader = new StreamReader(crStream);

                strFileData = reader.ReadToEnd();

                reader.Close();
                inputStream.Close();
            }

            if (File.Exists(strOutputFileName))
            {
                File.Delete(strOutputFileName);
            }
            using (StreamWriter outputStream = new StreamWriter(strOutputFileName))
            {
                outputStream.Write(strFileData, 0, strFileData.Length);

                outputStream.Close();
            }

        }

        /// <summary>   
        /// Decrypts the input file (strInputFileName) and writes it to a stream  
        /// </summary>   
        /// <param name="strInputFileName">input file name</param>   
        /// <param name="stream">stream to decrypt to</param>   
        public static bool DecryptFiletoStream(string strInputFileName, out Stream stream)
        {            
            if (!File.Exists(strInputFileName))
            {
                stream = null; 
                return false;
            }

            // Get the file into a new stream
            Stream inputStream = new FileStream(strInputFileName, FileMode.Open, FileAccess.Read);

            Stream decompressedStream;
            // Decompress the data and return it in the decompressedStream
            Compression.Decompress(ref inputStream, out decompressedStream);

            var cryptic = new TripleDESCryptoServiceProvider();
            cryptic.Key = FromHexString(PassKey);
            cryptic.IV = FromHexString(Iv);

            // Create an output stream and a crypto stream. The cryptostream will use the output stream to write data to.
            stream = new MemoryStream();
            var crStream = new CryptoStream(stream, cryptic.CreateDecryptor(), CryptoStreamMode.Write);
            // Reset the position of the decompressed stream
            decompressedStream.Position = 0;
            var buffer = new byte[decompressedStream.Length];
            int n;
            // Read the data from the decompressed stream and write that data to the output stream
            while ((n = decompressedStream.Read(buffer, 0, buffer.Length)) != 0)
                crStream.Write(buffer, 0, n);
            // Make sure to flush. Failure will result in bytes left over in the crypto stream and an incorrect object state.
            crStream.FlushFinalBlock();
            // Close the decompressed stream.
            
            decompressedStream.Close();            

            // The cryptostream will be closed when the output stream closes. If we were to close the crypto stream here then the output stream would also be closed.
            
            return true;
        }

        /// <summary>   
        /// Encrypts the input file(strInputFileName) and creates a new encrypted file(strOutputFileName)   
        /// </summary>   
        /// <param name="strInputFileName">input file name</param>   
        /// <param name="strOutputFileName">output file name</param>   
        public static void EncryptFiletoFile(string strInputFileName, string strOutputFileName)
        {
            byte[] fileBuffer;

            using (FileStream inputStream = new FileStream(strInputFileName, FileMode.Open, FileAccess.Read))
            {
                fileBuffer = new byte[inputStream.Length];

                inputStream.Read(fileBuffer, 0, fileBuffer.GetLength(0));


                inputStream.Close();
            }
            if (File.Exists(strOutputFileName))
            {
                File.Delete(strOutputFileName);
            }
            using (FileStream outputStream = new FileStream(strOutputFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var cryptic = new TripleDESCryptoServiceProvider();
                cryptic.Key = FromHexString(PassKey);
                cryptic.IV = FromHexString(Iv);

                CryptoStream crStream = new CryptoStream(outputStream, cryptic.CreateEncryptor(), CryptoStreamMode.Write);

                crStream.Write(fileBuffer, 0, fileBuffer.Length);

                crStream.Close();
            }
        }

        /// <summary>   
        /// Encrypts the stream and creates a new encrypted file(strOutputFileName). Closes the data stream afterwards. 
        /// </summary>   
        /// <param name="fileName">file name to save the file to</param>   
        /// <param name="data">stream containing the data</param>   
        public static bool EncryptStreamtoFile(string fileName, ref Stream data)
        {
            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);

                var cryptic = new TripleDESCryptoServiceProvider();
                cryptic.Key = FromHexString(PassKey);
                cryptic.IV = FromHexString(Iv);

                // New temporary stream to write encrypted data to
                Stream ms = new MemoryStream();
                // Get a crypto stream
                var crStream = new CryptoStream(ms, cryptic.CreateEncryptor(), CryptoStreamMode.Write);
                data.Position = 0;
                var buffer = new byte[data.Length];
                int n;
                // Read the data from the input data stream and write that data to the crypto and memory (ms) stream
                while ((n = data.Read(buffer, 0, buffer.Length)) != 0)
                    crStream.Write(buffer, 0, n);
                // Make sure that we flush. Failure will result in an invalid encrypted file.
                crStream.FlushFinalBlock();

                Stream compressedStream;
                // Compress the memory stream and get the compressed data back.
                Compression.Compress(ref ms, out compressedStream);

                // Create a new output stream with the filename specified.
                var outputStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write);
                // Reset the position of the compressedStream to 0
                compressedStream.Position = 0;
                buffer = new byte[compressedStream.Length];
                // Read the data from the compressed stream and write that data to the file stream
                while ((n = compressedStream.Read(buffer, 0, buffer.Length)) != 0)
                    outputStream.Write(buffer, 0, n);
                // Flush and close all open streams
                outputStream.Flush();

                crStream.Close();
                compressedStream.Close();
                outputStream.Close();
                data.Close();
            }
            catch { }
            return true;

        }

        /// <summary>   
        /// Encrypts the input string and creates a new encrypted file(strOutputFileName)   
        /// </summary>   
        /// <param name="strInputString">input string name</param>   
        /// <param name="strOutputFileName">output file name</param>   
        public static void EncryptStringtoFile(string strInputString, string strOutputFileName)
        {
            if (File.Exists(strOutputFileName))
            {
                File.Delete(strOutputFileName);
            }
            using (FileStream outputStream = new FileStream(strOutputFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var cryptic = new TripleDESCryptoServiceProvider();
                cryptic.Key = FromHexString(PassKey);
                cryptic.IV = FromHexString(Iv);

                CryptoStream crStream = new CryptoStream(outputStream, cryptic.CreateEncryptor(), CryptoStreamMode.Write);

                byte[] buffer = ASCIIEncoding.ASCII.GetBytes(strInputString);

                crStream.Write(buffer, 0, buffer.Length);

                crStream.Close();
            }
        }

        /// <summary>   
        /// Decrypts the input file (strInputFileName) and creates a new decrypted file (strOutputFileName)   
        /// </summary>   
        /// <param name="strInputFileName">input file name</param>   
        public static string DecryptFiletoString(string strInputFileName)
        {
            string strFileData = "";
            using (FileStream inputStream = new FileStream(strInputFileName, FileMode.Open, FileAccess.Read))
            {
                var cryptic = new TripleDESCryptoServiceProvider();
                cryptic.Key = FromHexString(PassKey);
                cryptic.IV = FromHexString(Iv);

                CryptoStream crStream = new CryptoStream(inputStream, cryptic.CreateDecryptor(), CryptoStreamMode.Read);

                StreamReader reader = new StreamReader(crStream);

                strFileData = reader.ReadToEnd();

                reader.Close();
                inputStream.Close();
            }

            return strFileData;
        }

        private static byte[] FromHexString(string str)
        {
            //null check a good idea
            var NumberChars = str.Length;
            var bytes = new byte[NumberChars / 2];
            for (var i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(str.Substring(i, 2), 16);
            return bytes;
        }
    }
}
