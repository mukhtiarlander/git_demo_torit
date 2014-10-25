using System.IO;
using System.Net;
using System;
using RDN.Utilities.Error;
using RDN.Utilities.Config;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

namespace RDN.Utilities.Network
{
    public class Network
    {
       private static string[] bots = new string[23]{
                "google",                "bot",
            "yahoo",     "spider",
            "archiver",   "curl",
            "python",     "nambu",
            "twitt",     "perl",
            "sphere",     "PEAR",
            "java",     "wordpress",
            "radian",     "crawl",
            "yandex",     "eventbox",
            "monitor",   "mechanize",
            "facebookexternal", "twitterbot", 
       "bingbot"};
        public static bool IsSearchBot(string userAgent)
        {

            if (String.IsNullOrEmpty(userAgent))
                return false;
            
            if (bots.Where(x => x.Contains(userAgent)).FirstOrDefault() != null)
            {
                return true;
            }
            return false;

        }

        public static Stream ConvertObjectToStream<T>(T ob)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            formatter.Serialize(stream, ob);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
        public static T LoadObject<T>(ref Stream stream)
        {
            stream.Position = 0;
            T output = default(T);
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            output = (T)formatter.Deserialize(stream);
            stream.Close();
            return output;
        }

        /// <summary>
        /// Sends the package as a stream using the HttpWebRequest and the post method to the specified endpoint. The stream is closed afterwards.
        /// </summary>
        /// <param name="package">Package to send</param>
        /// <param name="endPoint">End point to send the package</param>
        /// <returns>Status code returned from the server</returns>
        public static HttpStatusCode SendPackage(Stream package, string endPoint)
        {
            HttpStatusCode statusCode = HttpStatusCode.ExpectationFailed;

            // Reset the stream to start position
            package.Position = 0;

            // Create a byte array and fill it with the bytes from the package
            var bytesToSend = new byte[package.Length];
            package.Read(bytesToSend, 0, (int)package.Length);
            package.Close();

            // Create a web request
            var request = CreateWebRequest(endPoint, bytesToSend.Length);
            // Write the data to the web request
            using (var requestStream = request.GetRequestStream())
                requestStream.Write(bytesToSend, 0, bytesToSend.Length);

            // Get the status from the call

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                //using (Stream respStream = request.GetResponse().GetResponseStream())
                //{
                //    StreamReader read = new StreamReader(respStream);
                //    string blah = read.ReadToEnd();
                //    // read the error response
                //}
                statusCode = response.StatusCode;
            }

            // Return the status
            return statusCode;
        }
        public static HttpWebResponse SendPackageMobile(Stream package, string endPoint)
        {
            try
            {
                // Reset the stream to start position
                package.Position = 0;

                // Create a byte array and fill it with the bytes from the package
                var bytesToSend = new byte[package.Length];
                package.Read(bytesToSend, 0, (int)package.Length);
                package.Close();

                // Create a web request
                var request = CreateWebRequest(endPoint, bytesToSend.Length);
                // Write the data to the web request
                using (var requestStream = request.GetRequestStream())
                    requestStream.Write(bytesToSend, 0, bytesToSend.Length);

                // Get the status from the call

                // Return the status
                return (HttpWebResponse)request.GetResponse();
            }
            catch (WebException exc)
            {
                var resp = new StreamReader(exc.Response.GetResponseStream()).ReadToEnd();
            }
            return null;
        }


        private static HttpWebRequest CreateWebRequest(string endPoint, int contentLength)
        {
            var request = (HttpWebRequest)WebRequest.Create(endPoint);

            request.Method = "POST";
            request.ContentLength = contentLength;
            request.ContentType = "application/x-www-form-urlencoded";

            return request;
        }
    }
}
