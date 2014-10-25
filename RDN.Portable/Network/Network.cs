using System.IO;
using System.Net;
using System;
using System.Text;
using ProtoBuf;
using System.Threading;
//using System.Net.Http;

namespace RDN.Portable.Network
{
    public class Network
    {
        public static Stream ConvertObjectToStream<T>(T ob)
        {
            Stream stream = new MemoryStream();
            Serializer.Serialize(stream, ob);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
        public static T LoadObject<T>(ref Stream stream)
        {
            stream.Position = 0;
            T output = default(T);
            output = Serializer.Deserialize<T>(stream);
            stream.Dispose();
            return output;
        }

        //public static async Task<string> SendPackage1(Stream package, string endPoint)
        //{
        //    var httpClient = new HttpClient();

        //    var response = await httpClient.PostAsync(endPoint, new StreamContent(package));
        //    return await response.Content.ReadAsStringAsync();
        //}

        /// <summary>
        /// Sends the package as a stream using the HttpWebRequest and the post method to the specified endpoint. The stream is closed afterwards.
        /// </summary>
        /// <param name="package">Package to send</param>
        /// <param name="endPoint">End point to send the package</param>
        /// <returns>Status code returned from the server</returns>
        public static HttpWebResponse SendPackage(Stream package, string endPoint)
        {
            HttpStatusCode statusCode = HttpStatusCode.ExpectationFailed;

            // Reset the stream to start position
            package.Position = 0;

            // Create a byte array and fill it with the bytes from the package
            var bytesToSend = new byte[package.Length];
            package.Read(bytesToSend, 0, (int)package.Length);
            package.Dispose();

            var request = (HttpWebRequest)WebRequest.Create(endPoint);

            HttpWebResponse response = null;

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.BeginGetRequestStream(ar =>
            {
                using (var os = request.EndGetRequestStream(ar))
                {
                    os.Write(bytesToSend, 0, bytesToSend.Length);
                }


                request.BeginGetResponse(
                    ar2 =>
                    {
                        response = request.EndGetResponse(ar2) as HttpWebResponse;

                    }, null);
            }, null);
            while (response == null)
            {
            }
            return response;
        }


    }
}
