using App_Code;
using BlogEngine.Core;
using BlogEngine.Core.API.BlogML;
using BlogEngine.Core.Providers;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Linq;
using System.Web.Http;
using System.Collections.Generic;
using RDN.Utilities.Strings;
using System.Drawing.Imaging;

public class UploadController : ApiController
{
    public HttpResponseMessage Post(string action)
    {
        try
        {

            WebUtils.CheckRightsForAdminPostPages(false);
            if (HttpContext.Current.Request.Files.Count > 0)
            {
                HttpPostedFile file = HttpContext.Current.Request.Files[0];
                action = action.ToLower();

                if (file != null && file.ContentLength > 0)
                {

                    var dir = BlogService.GetDirectory("/" + DateTime.Now.ToString("yyyy") + "/" + DateTime.Now.ToString("MM") + "/" + DateTime.Now.ToString("dd"));
                    var retUrl = "";

                    if (action == "import")
                    {
                        if (Security.IsAdministrator)
                        {
                            return ImportBlogML();
                        }
                    }
                    if (action == "profile")
                    {
                        if (Security.IsAuthorizedTo(Rights.EditOwnUser))
                        {
                            // upload profile image
                            dir = BlogService.GetDirectory("/avatars");
                            var dot = file.FileName.IndexOf(".");
                            var ext = dot > 0 ? file.FileName.Substring(dot) : "";
                            var fileName = User.Identity.Name + ext;

                            var imgPath = HttpContext.Current.Server.MapPath(dir.FullPath + "/" + fileName);
                            var image = Image.FromStream(file.InputStream);
                            Image thumb = image.GetThumbnailImage(80, 80, () => false, IntPtr.Zero);
                            thumb.Save(imgPath);

                            return Request.CreateResponse(HttpStatusCode.Created, fileName);
                        }
                    }
                    if (action == "image" || action == "initial"||action == "main")
                    {
                        if (Security.IsAuthorizedTo(Rights.EditOwnPosts))
                        {
                            var image = Image.FromStream(file.InputStream);
                            var imgTemp = RDN.Utilities.Drawing.Images.ScaleDownImage(image, 1000, 2000);
                            MemoryStream stream = new MemoryStream();
                            // Save image to stream.
                            imgTemp.Save(stream, image.RawFormat);
                            var uploaded = BlogService.UploadImage(stream, StringExt.ToFileNameFriendlySize(file.FileName), dir, true);
                            retUrl = uploaded.FileDownloadPath.Replace("\"", "");
                            if (retUrl.StartsWith("/"))
                                retUrl = retUrl.Substring(1);
                            retUrl = Utils.RelativeWebRoot + retUrl;
                            return Request.CreateResponse(HttpStatusCode.Created, retUrl);
                        }
                    }
                    if (action == "file")
                    {
                        if (Security.IsAuthorizedTo(Rights.EditOwnPosts))
                        {
                            var uploaded = BlogService.UploadFile(file.InputStream, StringExt.ToFileNameFriendly(file.FileName), dir, true);
                            retUrl = uploaded.FileDownloadPath + "|" + StringExt.ToFileNameFriendly(file.FileName) + " (" + BytesToString(uploaded.FileSize) + ")";
                            retUrl = retUrl.Replace("\"", "");
                            if (retUrl.StartsWith("/"))
                                retUrl = retUrl.Substring(1);
                            retUrl = Utils.RelativeWebRoot + retUrl;
                            return Request.CreateResponse(HttpStatusCode.Created, retUrl);
                        }
                    }
                    if (action == "video")
                    {
                        if (Security.IsAuthorizedTo(Rights.EditOwnPosts))
                        {
                            // default media folder
                            var mediaFolder = "media";

                            // get the mediaplayer extension and use it's folder
                            var mediaPlayerExtension = BlogEngine.Core.Web.Extensions.ExtensionManager.GetExtension("MediaElementPlayer");
                            mediaFolder = mediaPlayerExtension.Settings[0].GetSingleValue("folder");

                            var folder = Utils.ApplicationRelativeWebRoot + mediaFolder + "/";
                            var fileName = file.FileName;

                            UploadVideo(folder, file, fileName);

                            return Request.CreateResponse(HttpStatusCode.Created, fileName);
                        }
                    }
                }
            }
        }
        catch (Exception exception)
        {
            RDN.Library.Classes.Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
        }
        return Request.CreateResponse(HttpStatusCode.BadRequest);
    }

    #region Private methods

    HttpResponseMessage ImportBlogML()
    {
        try
        {
            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            if (file != null && file.ContentLength > 0)
            {
                var reader = new BlogReader();
                var rdr = new StreamReader(file.InputStream);
                reader.XmlData = rdr.ReadToEnd();

                if (reader.Import())
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
        }
        catch (Exception exception)
        {
            RDN.Library.Classes.Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
        }
        return Request.CreateResponse(HttpStatusCode.InternalServerError);
    }

    static String BytesToString(long byteCount)
    {
        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
        if (byteCount == 0)
            return "0" + suf[0];
        long bytes = Math.Abs(byteCount);
        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        double num = Math.Round(bytes / Math.Pow(1024, place), 1);
        return (Math.Sign(byteCount) * num).ToString() + suf[place];
    }

    private void UploadVideo(string virtualFolder, HttpPostedFile file, string fileName)
    {
        try
        {
            var folder = HttpContext.Current.Server.MapPath(virtualFolder);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            file.SaveAs(folder + fileName);
        }
        catch (Exception exception)
        {
            RDN.Library.Classes.Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
        }
    }

    #endregion
}