using BlogEngine.Core.Data;
using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Providers;
using HtmlAgilityPack;
using log4net.Core;
using RDN.Portable.Config;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BlogEngine.Core.Utilities
{
    public class DNNScraper
    {


        public void ScanAllPages()
        {

            ILogger log = LoggerManager.GetLogger(GetType().Assembly, this.GetType());
            for (int i = 2528; i < 7776; i++)
            {

                log.Log(GetType(), Level.Info, i, null);

                try
                {

                    WebClient client = new WebClient();
                    string html = client.DownloadString("http://www.derbynews.net/?p=" + i);

                    HtmlDocument doc = new HtmlDocument();
                    //doc.Load("http://www.derbynews.net/?p=" + i);


                    doc.LoadHtml(html);

                    var content = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'blog-wrapper')]");
                    if (content != null)
                    {
                        if (!String.IsNullOrEmpty(content.InnerHtml))
                        {
                            var header = doc.DocumentNode.SelectSingleNode("//h2[contains(@class, 'post-header')]");
                            PostDetail detail = new PostDetail();
                            detail.IsSavedForApproval = true;
                            detail.Tags = new List<TagItem>();
                            detail.Title = header.InnerText.Replace("&#8217;", "").Replace("&#038;","");
                            detail.Author = ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN;
                            detail.Categories = new List<CategoryItem>();
                            detail.HasCommentsEnabled = true;

                            int imgNumber = 0;

                            var images = doc.DocumentNode.SelectNodes("//img");
                            if (images != null)
                            {
                                foreach (var image in images)
                                {
                                    try
                                    {
                                        string src = image.Attributes["src"].Value;


                                        log.Log(GetType(), Level.Info, src, null);
                                        if (!String.IsNullOrEmpty(src) && !src.Contains("gravatar") && !src.Contains("http://scontent-a.cdninstagram") && !src.Contains("disquscdn"))
                                        {
                                            WebClient dlFile = new WebClient();
                                            dlFile.DownloadFile(src, HttpContext.Current.Server.MapPath(BlogService.GetDirectory("/").FullPath + "/DNNImages/" + i + "-" + imgNumber + VirtualPathUtility.GetExtension(src)));
                                            //dlFile.DownloadFile(src, "C:/Personal/" + i + "-" + imgNumber + VirtualPathUtility.GetExtension(src));
                                        }
                                            var newNodeStr = "<!-- IMG[" + i + "-" + imgNumber + "] -->";
                                            var newNode = HtmlNode.CreateNode(newNodeStr);
                                            doc.DocumentNode.InnerHtml = doc.DocumentNode.InnerHtml.Replace(image.OuterHtml, newNode.InnerHtml);
                                            //image.Remove();
                                        

                                    }
                                    catch (Exception exception)
                                    {

                                    }
                                }
                            }

                            var mainContent = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'main-content')]");


                            detail.Content = mainContent.InnerHtml;

                            var date = doc.DocumentNode.SelectSingleNode("//span[contains(@class, 'date-span')]");
                            //detail.DateCreated = date.InnerText.Replace("\n", "").Replace("\t", "").Trim();
                            var dateLink = date.SelectSingleNode("a");
                            DateTime dateCre = DateTime.UtcNow;
                            var datePulled = dateLink.InnerText;
                            var split = datePulled.Split(' ');
                            string day = split[0];
                            day = day.Replace("rd", "").Replace("th", "").Replace("nd", "").Replace("st", "");
                            datePulled = day + " " + split[1] + " " + split[2];


                            DateTime.TryParse(datePulled, out dateCre);

                            //if (DateTime.TryParse(datePulled, "dd MMMM yyyy", DateTimeStyles.None, out dateCre)) ;
                            detail.DateCreated = dateCre.ToString("yyyy-MM-dd HH:mm");
                            var authorForDNN = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'author-bio-content')]");
                            var dnnAuthorName = authorForDNN.SelectSingleNode("h4");
                            detail.BottomLineForConstribution = "By " + dnnAuthorName.InnerText + " for Derby News Network";

                            var tagCloud = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'tagcloud')]");
                            var tags = tagCloud.SelectNodes("a");
                            if (tags != null)
                                foreach (var tag in tags)
                                {
                                    detail.Tags.Add(new TagItem()
                                    {
                                        TagName = tag.InnerText
                                    });
                                }
                            detail.Tags.Add(new TagItem()
                            {
                                TagName = "Derby News Network"
                            });


                            detail.MainImageUrl = "http://rollinnews.com/FILES%2f2014%2f09%2f11%2fDerby-News-Network.jpg.axdx";
                            detail.InitialImageUrl = "http://rollinnews.com/FILES%2f2014%2f09%2f11%2fDerby-News-Network1.jpg.axdx";

                            PostRepository repo = new PostRepository();
                            detail = repo.Add(detail, "DNNSubmittal");

                        }
                    }

                }
                catch (Exception exception)
                {

                    log.Log(GetType(), Level.Error, "Error occured", exception);

                }




            }

        }



    }
}
