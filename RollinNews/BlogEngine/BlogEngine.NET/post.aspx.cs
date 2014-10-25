#region Using

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using System.Collections.Generic;
using RDN.Library.Classes.Error;
using RDN.Utilities.Network;
using RDN.Library.Classes.RN.Posts;
using RDN.Library.Classes.Account.Classes;
using System.Linq;
using System.Threading;
using MoreLinq;
using RDN.Portable.Classes.Account.Classes;
using RDN.Library.Classes.Store.Display;
using RDN.Library.Cache;
//using RN.Library.Classes.Posts;

#endregion

public partial class post : BlogEngine.Core.Web.Controls.BlogBasePage
{
    public MemberDisplay Member = new MemberDisplay();
    public List<Post> RecommendedPosts = new List<Post>();
    public List<Post> YouMayLikePosts = new List<Post>();
    public DisplayStore StoreItem { get; set; }
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        try
        {
            bool shouldThrow404 = false;
            CommentView1.Visible = ShowCommentsForm;
            //disqus_box.Visible = ShowDisqusForm;

            var requestId = Request.QueryString["id"];
            Guid id;

            if ((!Utils.StringIsNullOrWhitespace(requestId)) && requestId.TryParse(out id))
            {
                Post post = Post.ApplicablePosts.Find(p => p.Id == id);
                if (post != null)
                {
                    if ((!HttpContext.Current.User.Identity.IsAuthenticated && !post.IsPublished) && !Security.IsAuthorizedTo(Rights.ViewUnpublishedPosts))
                    {


                        Response.Redirect(Utils.RelativeWebRoot, false);
                        Server.Transfer(Utils.RelativeWebRoot + "default.aspx");

                    }
                    else
                    {
                        //author Posts.
                        RecommendedPosts.AddRange(Post.ApplicablePosts.FindAll(x => x.Author == post.Author && x.IsPublished).Take(3));
                        //lowests viewed posts of month
                        RecommendedPosts.AddRange(Post.ApplicablePosts.FindAll(x => x.IsPublished).OrderBy(x => x.TotalMonthsViews).Take(3));
                        //same tagged posts.
                        RecommendedPosts.AddRange(Post.ApplicablePosts.FindAll(x => x.IsPublished && x.Tags.Intersect(post.Tags).Any()).Take(5));
                        //same category
                        RecommendedPosts.AddRange(Post.ApplicablePosts.FindAll(x => x.IsPublished && x.Categories.Select(z => z.Title).Intersect(post.Categories.Select(y => y.Title)).Any()).Take(5));
                        //highest viewed posts of month
                        RecommendedPosts.AddRange(Post.ApplicablePosts.FindAll(x => x.IsPublished).OrderByDescending(x => x.TotalMonthsViews).Take(3));
                        //highest viewed posts of year
                        RecommendedPosts.AddRange(Post.ApplicablePosts.FindAll(x => x.IsPublished).OrderByDescending(x => x.TotalViews).Take(2));
                        //lowest viewed posts of year
                        RecommendedPosts.AddRange(Post.ApplicablePosts.FindAll(x => x.IsPublished).OrderBy(x => x.TotalViews).Take(5));
                        //all other posts.
                        RecommendedPosts.AddRange(Post.ApplicablePosts.FindAll(x => x.IsPublished).OrderBy(x => Guid.NewGuid()).Take(100));
                        //order the posts.
                        RecommendedPosts = RecommendedPosts.DistinctBy(x => x.Id).OrderByDescending(x => x.DateCreated).ToList();

                        YouMayLikePosts.AddRange(Post.ApplicablePosts.FindAll(x => x.IsPublished && x.Tags.Intersect(post.Tags).Any()).Take(10));
                        YouMayLikePosts.AddRange(Post.ApplicablePosts.FindAll(x => x.IsPublished && x.Categories.Select(z => z.Title).Intersect(post.Categories.Select(y => y.Title)).Any()).Take(10));

                        if (YouMayLikePosts.Where(x => x.Id == post.Id).FirstOrDefault() != null)
                            YouMayLikePosts.Remove(YouMayLikePosts.Where(x => x.Id == post.Id).FirstOrDefault());

                        if (YouMayLikePosts.Count < 8)
                            YouMayLikePosts.AddRange(Post.ApplicablePosts.FindAll(x => x.IsPublished).OrderBy(x => Guid.NewGuid()).Take(8 - YouMayLikePosts.Count));

                        if (YouMayLikePosts.Count > 8)
                            YouMayLikePosts = YouMayLikePosts.Take(8).ToList();

                        StoreItem = SiteCache.GetRandomPublishedStoreItems(2);

                        Member = RDN.Library.Cache.SiteCache.GetPublicMemberFull(post.Author);
                        if (Member != null)
                        {
                            post.AuthorDerbyName = Member.DerbyName;
                            post.AuthorDerbyId = Member.MemberId;
                        }
                        this.Post = post;
                      

                        // SEO redirct, discussion #446011
                        int idx = Request.RawUrl.IndexOf("?");
                        var rawUrl = idx > 0 ? Request.RawUrl.Substring(0, idx) : Request.RawUrl;
                        if (rawUrl != post.RelativeLink.ToString())
                        {
                            Response.Clear();
                            Response.StatusCode = 301;
                            Response.AppendHeader("location", post.RelativeLink.ToString());
                            Response.End();
                        }

                        var settings = BlogSettings.Instance;
                        string encodedPostTitle = Server.HtmlEncode(Post.Title);
                        string path = Utils.ApplicationRelativeWebRoot + "themes/" + BlogSettings.Instance.GetThemeWithAdjustments(null) + "/PostView.ascx";

                        PostViewBase postView = (PostViewBase)LoadControl(path);
                        postView.Post = Post;
                        postView.ID = Post.Id.ToString().Replace("-", string.Empty);
                        postView.Location = ServingLocation.SinglePost;
                        pwPost.Controls.Add(postView);

                        if (!Network.IsSearchBot(HttpContext.Current.Request.UserAgent) && (HttpContext.Current.User.Identity.Name != post.Author) && post.IsVisibleToPublic && !IsPostBack)
                            PostManager.AddViewToPost(id);

                        CommentView1.Post = Post;

                        Page.Title = encodedPostTitle;
                        AddMetaKeywords();
                        AddMetaDescription();
                        //base.AddMetaTag("author", Server.HtmlEncode(Post.AuthorProfile == null ? Post.Author : Post.AuthorProfile.FullName));

                        //List<Post> visiblePosts = Post.Posts.FindAll(delegate(Post p) { return p.IsVisible; });
                        //if (visiblePosts.Count > 0)
                        //{
                        //    AddGenericLink("last", visiblePosts[0].Title, visiblePosts[0].RelativeLink);
                        //    AddGenericLink("first", visiblePosts[visiblePosts.Count - 1].Title, visiblePosts[visiblePosts.Count - 1].RelativeLink);
                        //}

                        //InitNavigationLinks();

                        phRDF.Visible = false;

                        base.AddGenericLink("application/rss+xml", "alternate", encodedPostTitle + " (RSS)", postView.CommentFeed + "?format=ATOM");
                        base.AddGenericLink("application/rss+xml", "alternate", encodedPostTitle + " (ATOM)", postView.CommentFeed + "?format=ATOM");

                        if (BlogSettings.Instance.EnablePingBackReceive)
                        {
                            Response.AppendHeader("x-pingback", "http://" + Request.Url.Authority + Utils.RelativeWebRoot + "pingback.axd");
                        }


                        HtmlGenericControl newControl = new HtmlGenericControl("meta");
                        newControl.Attributes["property"] = "og:title";
                        newControl.Attributes["content"] = post.Title;
                        Page.Header.Controls.Add(newControl);

                        HtmlGenericControl newControl2 = new HtmlGenericControl("meta");
                        newControl2.Attributes["property"] = "og:type";
                        newControl2.Attributes["content"] = "website";
                        Page.Header.Controls.Add(newControl2);

                        HtmlGenericControl newControl3 = new HtmlGenericControl("meta");
                        newControl3.Attributes["property"] = "og:image";
                        if (!String.IsNullOrEmpty(post.InitialImageUrl) && post.InitialImageUrl.Contains("http://"))
                            newControl3.Attributes["content"] = post.InitialImageUrl;
                        else
                            newControl3.Attributes["content"] = post.BaseHost.TrimEnd(new[] { '/' }) + post.InitialImageUrl;
                        Page.Header.Controls.Add(newControl3);

                        HtmlGenericControl newControl9 = new HtmlGenericControl("meta");
                        newControl9.Attributes["property"] = "og:image";
                        if (!String.IsNullOrEmpty(post.MainImageUrl) &&  post.MainImageUrl.Contains("http://"))
                            newControl9.Attributes["content"] = post.MainImageUrl;
                        else
                            newControl9.Attributes["content"] = post.BaseHost.TrimEnd(new[] { '/' }) + post.MainImageUrl;
                        Page.Header.Controls.Add(newControl9);

                        HtmlGenericControl newControl4 = new HtmlGenericControl("meta");
                        newControl4.Attributes["property"] = "og:url";
                        newControl4.Attributes["content"] = post.BaseHost.TrimEnd(new[] { '/' }) + post.RelativeOrAbsoluteLink;
                        Page.Header.Controls.Add(newControl4);

                        HtmlGenericControl newControl5 = new HtmlGenericControl("meta");
                        newControl5.Attributes["property"] = "og:site_name";
                        newControl5.Attributes["content"] = System.Configuration.ConfigurationManager.AppSettings["SiteName"];
                        Page.Header.Controls.Add(newControl5);

                        HtmlGenericControl newControl6 = new HtmlGenericControl("meta");
                        newControl6.Attributes["property"] = "fb:app_id";
                        newControl6.Attributes["content"] = System.Configuration.ConfigurationManager.AppSettings["FacebookAppId"];
                        Page.Header.Controls.Add(newControl6);


                    }

                }

            }

            else
            {
                shouldThrow404 = true;
            }

            if (shouldThrow404)
            {
                Response.Redirect(Utils.RelativeWebRoot + "error404.aspx", true);
            }
        }
        catch (ThreadAbortException)
        { }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, exception.GetType());
        }
    }

    /// <summary>
    /// Gets the next post filtered for invisible posts.
    /// </summary>
    private Post GetNextPost(Post post)
    {
        try
        {
            if (post.Next == null)
                return null;

            if (post.Next.IsVisible)
                return post.Next;
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, exception.GetType());
        }
        return GetNextPost(post.Next);
    }

    /// <summary>
    /// Gets the prev post filtered for invisible posts.
    /// </summary>
    private Post GetPrevPost(Post post)
    {
        try
        {
            if (post.Previous == null)
                return null;

            if (post.Previous.IsVisible)
                return post.Previous;
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, exception.GetType());
        }
        return GetPrevPost(post.Previous);
    }


    /// <summary>
    /// Adds the post's description as the description metatag.
    /// </summary>
    private void AddMetaDescription()
    {
        base.AddMetaTag("description", Server.HtmlEncode(Post.Description));
    }

    /// <summary>
    /// Adds the post's tags as meta keywords.
    /// </summary>
    private void AddMetaKeywords()
    {
        if (Post.Tags.Count > 0)
        {
            base.AddMetaTag("keywords", Server.HtmlEncode(string.Join(",", Post.Tags.ToArray())));
        }
    }

    public Post Post;

    public static bool ShowCommentsForm
    {
        get
        {
            return BlogSettings.Instance.ModerationType != BlogSettings.Moderation.Disqus;
        }
    }

    public static bool ShowDisqusForm
    {
        get
        {
            return BlogSettings.Instance.ModerationType == BlogSettings.Moderation.Disqus;
        }
    }
}
