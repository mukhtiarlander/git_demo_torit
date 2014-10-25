using System.IO;
using System.Net;
using System;
using ProtoBuf;

//namespace RDN.WP.Library.Network
//{
//    //public class Network
//    //{
//    //    //public static Stream ConvertObjectToStream<T>(T ob)
//    //    //{
//    //    //    Stream stream = new MemoryStream();
//    //    //    Serializer.Serialize(stream, ob);
//    //    //    stream.Seek(0, SeekOrigin.Begin);
//    //    //    return stream;
//    //    //}
//    //    //public static T LoadObject<T>(ref Stream stream)
//    //    //{
//    //    //    stream.Position = 0;
//    //    //    T output = Serializer.Deserialize<T>(stream);
//    //    //    stream.Close();
//    //    //    return output;
//    //    //}

//    //    /// <summary>
//    //    /// Sends the package as a stream using the HttpWebRequest and the post method to the specified endpoint. The stream is closed afterwards.
//    //    /// </summary>
//    //    /// <param name="package">Package to send</param>
//    //    /// <param name="endPoint">End point to send the package</param>
//    //    /// <returns>Status code returned from the server</returns>
//    //    //public static HttpStatusCode SendPackage(Stream package, string endPoint)
//    //    //{
//    //    //    HttpStatusCode statusCode = HttpStatusCode.ExpectationFailed;

//    //    //    // Reset the stream to start position
//    //    //    package.Position = 0;

//    //    //    // Create a byte array and fill it with the bytes from the package
//    //    //    var bytesToSend = new byte[package.Length];
//    //    //    package.Read(bytesToSend, 0, (int)package.Length);
//    //    //    package.Close();

//    //    //    // Create a web request
//    //    //    var request = CreateWebRequest(endPoint, bytesToSend.Length);
//    //    //    // Write the data to the web request
//    //    //    using (var requestStream = request.GetRequestStream())
//    //    //        requestStream.Write(bytesToSend, 0, bytesToSend.Length);

//    //    //    // Get the status from the call

//    //    //    using (var response = (HttpWebResponse)request.GetResponse())
//    //    //    {
//    //    //        //using (Stream respStream = request.GetResponse().GetResponseStream())
//    //    //        //{
//    //    //        //    StreamReader read = new StreamReader(respStream);
//    //    //        //    string blah = read.ReadToEnd();
//    //    //        //    // read the error response
//    //    //        //}
//    //    //        statusCode = response.StatusCode;
//    //    //    }

//    //    //    // Return the status
//    //    //    return statusCode;
//    //    //}
//    //    //public static HttpWebResponse SendPackageMobile(Stream package, string endPoint)
//    //    //{
//    //    //    try
//    //    //    {
//    //    //        // Reset the stream to start position
//    //    //        package.Position = 0;

//    //    //        // Create a byte array and fill it with the bytes from the package
//    //    //        var bytesToSend = new byte[package.Length];
//    //    //        package.Read(bytesToSend, 0, (int)package.Length);
//    //    //        package.Close();

//    //    //        // Create a web request
//    //    //        var request = CreateWebRequest(endPoint, bytesToSend.Length);
//    //    //        // Write the data to the web request
//    //    //        using (var requestStream = request.GetRequestStream())
//    //    //            requestStream.Write(bytesToSend, 0, bytesToSend.Length);

//    //    //        // Get the status from the call

//    //    //        // Return the status
//    //    //        return (HttpWebResponse)request.GetResponse();
//    //    //    }
//    //    //    catch (WebException exc)
//    //    //    {
//    //    //        var resp = new StreamReader(exc.Response.GetResponseStream()).ReadToEnd();
//    //    //    }
//    //    //    return null;
//    //    //}

//    //    //public static HttpWebResponse SendPackage(Stream package, string endPoint)
//    //    //{
//    //    //    HttpStatusCode statusCode = HttpStatusCode.ExpectationFailed;

//    //    //    // Reset the stream to start position
//    //    //    package.Position = 0;

//    //    //    // Create a byte array and fill it with the bytes from the package
//    //    //    var bytesToSend = new byte[package.Length];
//    //    //    package.Read(bytesToSend, 0, (int)package.Length);
//    //    //    package.Dispose();

//    //    //    var request = (HttpWebRequest)WebRequest.Create(endPoint);
//    //    //    HttpWebResponse response = null;
//    //    //    request.Method = "POST";
//    //    //    request.ContentType = "application/x-www-form-urlencoded";

//    //    //    request.BeginGetRequestStream(ar =>
//    //    //    {
//    //    //        try
//    //    //        {
//    //    //            using (var os = request.EndGetRequestStream(ar))
//    //    //            {
//    //    //                os.Write(bytesToSend, 0, bytesToSend.Length);
//    //    //            }
//    //    //        }
//    //    //        catch (Exception ex)
//    //    //        {
//    //    //        }

//    //    //        request.BeginGetResponse(
//    //    //            ar2 =>
//    //    //            {
//    //    //                try
//    //    //                {
//    //    //                    response = request.EndGetResponse(ar2) as HttpWebResponse;

//    //    //                }
//    //    //                catch (Exception ex)
//    //    //                {
//    //    //                    //MessageBox.Show("Unsuccessful");
//    //    //                }
//    //    //            }, null);
//    //    //    }, null);


//    //    //    // Return the status
//    //    //    return response;
//    //    //}
//    //    //public static Stream GetPackageStream(string url)
//    //    //{
//    //    //    HttpStatusCode statusCode = HttpStatusCode.ExpectationFailed;

            
//    //    //    var request = (HttpWebRequest)WebRequest.Create(url);
//    //    //    HttpWebResponse response = null;
//    //    //    request.Method = "POST";
//    //    //    request.Accept = "application/json";
//    //    //    request.ContentType = "application/json; charset=utf-8";


//    //    //    request.BeginGetResponse(ar =>
//    //    //    {
//    //    //        try
//    //    //        {
//    //    //            using (var os = request.EndGetRequestStream(ar))
//    //    //            {

//    //    //            }
//    //    //        }
//    //    //        catch (Exception ex)
//    //    //        {
//    //    //        }

//    //    //        request.BeginGetResponse(
//    //    //            ar2 =>
//    //    //            {
//    //    //                try
//    //    //                {
//    //    //                    response = request.EndGetResponse(ar2) as HttpWebResponse;

//    //    //                }
//    //    //                catch (Exception ex)
//    //    //                {
//    //    //                    //MessageBox.Show("Unsuccessful");
//    //    //                }
//    //    //            }, null);
//    //    //    }, null);


//    //    //    // Return the status
//    //    //    return response.GetResponseStream();
//    //    //}
//    //    ////private static HttpWebRequest CreateWebRequest(string endPoint, int contentLength)
//    //    //{
//    //    //    var request = (HttpWebRequest)WebRequest.Create(endPoint);

//    //    //    request.Method = "POST";
//    //    //    request.ContentLength = contentLength;
//    //    //    request.ContentType = "application/x-www-form-urlencoded";

//    //    //    return request;
//    //    //}
//    //}
//}
