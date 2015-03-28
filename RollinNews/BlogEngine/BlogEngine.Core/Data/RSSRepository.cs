using BlogEngine.Core.Data.Models;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.RN.Posts;
using RDN.Library.Classes.RN.RSS;
using RDN.Portable.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;

namespace BlogEngine.Core.Data
{
    public class RSSRepository
    {
        public static bool ScanRSSFeeds()
        {
            var fact = RSSFactory.Initilize();
            var feeds = fact.PullAllFeedsToScan(10000, 0);

            for (int i = 0; i < feeds.Count; i++)
            {
                int postCount = 0;
                try
                {
                    if (!String.IsNullOrEmpty(feeds[i].RSSUrl))
                    {
                        XmlReaderSettings settings = new XmlReaderSettings();
                        settings.ProhibitDtd = false;
                        XmlReader reader = XmlReader.Create(feeds[i].RSSUrl, settings);
                        SyndicationFeed feed = SyndicationFeed.Load(reader);
                        string authorName = ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN;

                        if (!String.IsNullOrEmpty(feeds[i].AuthorUserName))
                        {
                            authorName = feeds[i].AuthorUserName;
                        }

                        var latestPosts = feed.Items.Where(x => x.PublishDate >= feeds[i].LastChecked).ToList();
                        for (int j = 0; j < latestPosts.Count; j++)
                        {
                            try
                            {
                                PostDetail detail = new PostDetail();

                                var p = latestPosts[j];

                                detail.Author = authorName;

                                foreach (SyndicationElementExtension ext in p.ElementExtensions)
                                {
                                    if (ext.GetObject<XElement>().Name.LocalName == "encoded")
                                        detail.Content = ext.GetObject<XElement>().Value;
                                }

                                if (p.Content != null)
                                    detail.Content = (p.Content as TextSyndicationContent).Text;
                                if (p.Summary != null)
                                {
                                    detail.Description = (p.Summary as TextSyndicationContent).Text;
                                    if (String.IsNullOrEmpty(detail.Content))
                                        detail.Content = detail.Description;
                                }


                                detail.Title = p.Title.Text;
                                detail.DateCreated = DateTime.UtcNow.ToString();
                                detail.IsSavedForApproval = true;
                                detail.Tags = new List<TagItem>();
                                detail.Categories = new List<CategoryItem>();
                                detail.HasCommentsEnabled = true;


                                foreach (var cat in feeds[i].Categories)
                                {
                                    detail.Categories.Add(new CategoryItem()
                                    {
                                        Id = cat.CategoryRNId,
                                        Title = Category.GetCategory(cat.CategoryRNId).Title
                                    });
                                }
                                foreach (var tag in feeds[i].Tags)
                                {
                                    detail.Tags.Add(new TagItem()
                                    {
                                        TagName = tag.TagName
                                    });
                                }


                                detail.MainImageUrl = feeds[i].MainImageUrl;
                                detail.InitialImageUrl = feeds[i].InitialImageUrl;
                                detail.FeedId = feeds[i].FeedId;

                                PostRepository repo = new PostRepository();
                                detail = repo.Add(detail, "RNFeedSubmittal");
                                PostManager.AddPostForFeed(detail.Id, feeds[i].FeedId);
                                postCount += 1;
                            }
                            catch (Exception exception)
                            {

                            }
                        }
                    }



                }
                catch (Exception exception)
                {
                    if (!String.IsNullOrEmpty(exception.Message) && !exception.Message.Contains("(404) Not Found") && !exception.Message.Contains("The operation has timed out") && !exception.Message.Contains(@"c:\windows\system32\inetsrv\NA") && !exception.Message.Contains("Unable to connect to the remote serve") && !exception.Message.Contains("remote name could not be resolved"))
                        ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: feeds[i].RSSUrl);
                }
                fact.FinishFeedPolling(feeds[i].FeedId, postCount);
            }



            return true;

        }


    }
}
