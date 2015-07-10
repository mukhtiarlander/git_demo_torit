using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using RDN.Library.Classes.Error;

namespace RDN.Library.Util
{
    public class Tumblr
    {
        public static Tumblr GetBlog()
        {
            try
            {
                XDocument xmlDoc = XDocument.Load(RDN.Library.Classes.Config.LibraryConfig.BlogSite+ "/api/read");
                Tumblr tm = Tumblr.GetTumblr(RDN.Library.Classes.Config.LibraryConfig.BlogSite+ "/api/read", true);
                tm.Posts.Count = "3";

                return tm;
            }
            catch
            {

            }
            return new Tumblr();
        }

        public static string Strip(string text, bool stripHtml)
        {
            if (stripHtml)
                return Regex.Replace(text, @"<(.|\n)*?>", string.Empty);
            else
                return text;
        }

        public TumblrRssFeed Feed = new TumblrRssFeed();
        public TumblrPosts Posts = new TumblrPosts();
        public static Tumblr GetTumblr(string xmlDoc, bool stripHtmlFromBody)
        {
            XDocument xmlDocs = XDocument.Load(xmlDoc);

            Tumblr tm = new Tumblr();
            tm.Feed = (from t in xmlDocs.Root.Elements("tumblelog")
                       select new TumblrRssFeed()
                       {
                           Name = (string)t.Attribute("name"),
                           CName = (string)t.Attribute("cname"),
                           TimeZone = (string)t.Attribute("timezone"),
                           Title = (string)t.Attribute("title"),
                           Description = (string)t.Value
                       }).FirstOrDefault();

            tm.Posts = (from ps in xmlDocs.Root.Elements("posts")
                        select new TumblrPosts()
                        {
                            Count = (string)ps.Attribute("total"),
                            Start = (string)ps.Attribute("start"),
                            Post = new List<TumblrPost>()
                        }).FirstOrDefault();

            foreach (var pst in xmlDocs.Root.Elements("posts").Elements("post"))
            {
                TumblrPost post = new TumblrPost();
                post.Id = (string)pst.Attribute("id");
                post.Url = (string)pst.Attribute("url");
                post.UrlSlug = (string)pst.Attribute("url-with-slug");
                post.Type = (string)pst.Attribute("type");
                post.Date = (string)pst.Attribute("date");
                post.DateGMT = (string)pst.Attribute("date-gmt");
                post.UnixTime = (string)pst.Attribute("unix-timestamp");
                post.Format = (string)pst.Attribute("format");
                if (post.Type == "regular")
                {
                    post.Title = (string)pst.Element("regular-title").Value;
                    post.Body = Strip((string)pst.Element("regular-body").Value, stripHtmlFromBody);
                }
                else if (post.Type == "photo")
                {
                    if (pst.Element("photo-caption") != null)
                    {
                        post.Title = Strip((string)pst.Element("photo-caption").Value, stripHtmlFromBody);
                    }
                    foreach (var photo in pst.Elements("photo-url"))
                    {
                        if ((string)photo.Attribute("max-width") == "1280")
                        {
                            post.photoUrl1280 = photo.Value;
                        }
                        else if ((string)photo.Attribute("max-width") == "250")
                        {
                            post.photoUrl250 = photo.Value;
                        }
                    }
                }
                tm.Posts.Post.Add(post);
            }
            return tm;
        }

    }

    public class TumblrRssFeed
    {
        public string Name;
        public string TimeZone;
        public string CName;
        public string Title;
        public string Description;
    }
    public class TumblrPosts
    {
        public string Start;
        public string Count;
        public List<TumblrPost> Post;
    }
    public class TumblrPost
    {
        public string Id;
        public string Url;
        public string UrlSlug;
        public string Title;
        public string Body;
        public string Type;
        public string DateGMT;
        public string Date;
        public string UnixTime;
        public string Format;
        public string photoUrl1280;
        public string photoUrl250;


    }


}
