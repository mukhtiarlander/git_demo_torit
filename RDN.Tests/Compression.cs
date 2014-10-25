using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RDN.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class Compression
    {

        [TestMethod]
        public void TestMethod1()
        {
            FileInfo f = new FileInfo(@"c:\temp\temp2.xml");
            Compress(f);
        }

        public static void Compress(FileInfo fi)
        {
            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                // Prevent compressing hidden and 
                // already compressed files.
                if ((File.GetAttributes(fi.FullName)
                     & FileAttributes.Hidden)
                    != FileAttributes.Hidden & fi.Extension != ".gz")
                {
                    // Create the compressed file.
                    using (FileStream outFile =
                        File.Create(fi.FullName + ".gz"))
                    {
                        using (GZipStream Compress = new GZipStream(outFile, CompressionMode.Compress))
                        {
                            // Copy the source file into 
                            // the compression stream.
                            inFile.CopyTo(Compress);

                            Console.WriteLine("Compressed {0} from {1} to {2} bytes.",
                                              fi.Name, fi.Length.ToString(), outFile.Length.ToString());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void Decrypt()
        {
            var key1 = "'il,.:;fj!I[]rt\"()-J?LTcksz|¦ghnopquvxy0123456789EFKabde$*/\\PRSZ}ABCDG";


            var key2 = "'il,.ABfj!IC]rt\"DG-J?LTcksz|}ghnopquvxy0123456789EFKabde$*/\\PRSZ";


            var toDecrypt = "5!Zj,/rfe}Rp.ZB8FA1Gvi57Zinnv9oIb0c.Ptv,fnn0sJvxEJdS2CvP'Kj76kLe$z-scChIZRP0?6I!TL";



            //var base64 = DecodeFrom64(toDecrypt);
            //var des1 = DecryptFile(toDecrypt, key1);
            var des2 = DecryptString(toDecrypt, key2);
            //var des3 = DecryptFile(des1, key2);
            var des4 = DecryptString(des2, key1);
        }

        static public string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes
                = System.Convert.FromBase64String(encodedData);
            string returnValue =
               System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }

        public static string DecryptString(string Message, string Passphrase)
        {

            byte[] Results;

            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();



            // Step 1. We hash the passphrase using MD5

            // We use the MD5 hash generator as the result is a 128 bit byte array

            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();

            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));



            // Step 2. Create a new TripleDESCryptoServiceProvider object

            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the decoder

            TDESAlgorithm.Key = TDESKey;

            TDESAlgorithm.Mode = CipherMode.ECB;

            TDESAlgorithm.Padding = PaddingMode.PKCS7;



            // Step 4. Convert the input string to a byte[]

            byte[] DataToDecrypt = System.Text.ASCIIEncoding.ASCII.GetBytes(Message);
            // Step 5. Attempt to decrypt the string
            byte [] d = new byte[8];
            d[0] = DataToDecrypt[0];
            d[1] = DataToDecrypt[1];
            d[2] = DataToDecrypt[2];
            d[3] = DataToDecrypt[3];
            d[4] = DataToDecrypt[4];
            d[5] = DataToDecrypt[5];
            d[6] = DataToDecrypt[6];
            d[7] = DataToDecrypt[7];

            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(d, 0, d.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information

                TDESAlgorithm.Clear();

                HashProvider.Clear();

            }

            // Step 6. Return the decrypted string in UTF8 format

            return UTF8.GetString(Results);

        }
    }
}
