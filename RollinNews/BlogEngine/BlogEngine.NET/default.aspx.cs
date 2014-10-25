#region Using

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Collections.Generic;
using BlogEngine.Core;
using System.Web.UI;
using BlogEngine.Core.Data;
using BlogEngine.Core.Data.Models;
using RDN.Library.Classes.Error;
using RDN.Library.Cache;
using RDN.Library.Classes.Store.Display;
using RDN.Library.Cache.Singletons;
using System.Configuration;

#endregion

public partial class _default : BlogEngine.Core.Web.Controls.BlogBasePage
{
    public IEnumerable<PostItem> Posts
    {
        get;
        set;
    }
    public List<PostItem> PostsColumn1
    {
        get;
        set;
    }
    public List<PostItem> PostsColumn2
    {
        get;
        set;
    }
    public List<PostItem> PostsColumn3
    {
        get;
        set;
    }
    public IEnumerable<TweetSharp.TwitterStatus> Tweets
    { get; set; }

    public DisplayStore StoreItem { get; set; }

    ////Disabled as #derbyscores no longer needed.
    //public List<RDN.Portable.Models.Json.Games.CurrentGameJson> Games { get; set; }
    public RDN.Portable.Classes.League.Classes.League LeagueOfWeek { get; set; }
    public RDN.Portable.Models.Json.SkaterJson SkaterOfWeek { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Page.IsCallback)
                return;


            if (Request.RawUrl.ToLowerInvariant().Contains("/news/"))
            {
                DisplayCategories();
            }
            else if (Request.RawUrl.ToLowerInvariant().Contains("/author/"))
            {
                DisplayAuthors();
            }
            else if (Request.RawUrl.ToLowerInvariant().Contains("?tag="))
            {
                DisplayTag();
            }
            else
            {
                PostRepository repository = new PostRepository();
                Posts = repository.FindPublic(take: 60);
            }
            StoreItem = SiteCache.GetRandomPublishedStoreItems(3);
            PostsColumn1 = new List<PostItem>();
            PostsColumn2 = new List<PostItem>();
            PostsColumn3 = new List<PostItem>();

           // var resul = SiteSingleton.Instance.GetLastTweets("#derbyscores");// Search by HashTag

            var resul = SiteSingleton.Instance.GetTweetsStatus("@rollinnews"); //Get 150 tweets from Home Timeline.

            Tweets = resul;
            if (Tweets == null)
                Tweets = new List<TweetSharp.TwitterStatus>();

            int i = 0;
            foreach (var post in Posts)
            {
                if (i % 3 == 0)
                    PostsColumn1.Add(post);
                else if (i % 3 == 1)
                    PostsColumn2.Add(post);
                else if (i % 3 == 2)
                    PostsColumn3.Add(post);
                i += 1;
            }
            //Disabled as #derbyscores no longer needed.
            //Games = RDN.Library.Cache.SiteCache.GetPastGames(0, 15);

            LeagueOfWeek = SiteCache.GetLeagueOfWeek();
            if (LeagueOfWeek != null)
            {
                if (!String.IsNullOrEmpty(LeagueOfWeek.Logo.ImageThumbUrl))
                    LeagueOfWeek.Logo.ImageThumbUrl = LeagueOfWeek.Logo.ImageUrl;
            }
            SkaterOfWeek = SiteCache.GetSkaterOfWeek();
            if (SkaterOfWeek != null)
            {
                if (SkaterOfWeek.Bio.Length > 230)
                    SkaterOfWeek.Bio = SkaterOfWeek.Bio.Remove(230) + "...";
            }
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, exception.GetType());
        }

    }

    private void DisplayCategories()
    {
        try
        {
            if (!String.IsNullOrEmpty(Request.QueryString["id"]))
            {
                Guid categoryId = new Guid(Request.QueryString["id"]);
                Category category = Category.GetCategory(categoryId, BlogEngine.Core.Blog.CurrentInstance.IsSiteAggregation);
                Posts = Post.GetPostsByCategory(category, Convert.ToInt32(ConfigurationManager.AppSettings["FrontPagePostsCount"]));
                Page.Title = category.Title;
                base.AddMetaTag("description", string.IsNullOrWhiteSpace(category.Description) ? Server.HtmlEncode(BlogSettings.Instance.Description + ", " + category.Title) : category.Description);
            }
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, exception.GetType());
        }
    }

    private void DisplayTag()
    {
        try
        {
            if (!String.IsNullOrEmpty(Request.QueryString["tag"]))
            {
                string tag = Request.QueryString["tag"];

                Posts = Post.GetPostsByTagFront(tag, Convert.ToInt32(ConfigurationManager.AppSettings["FrontPagePostsCount"]));
                Page.Title = tag;

            }
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, exception.GetType());
        }
    }

    private void DisplayAuthors()
    {
        try
        {
            if (!string.IsNullOrEmpty(Request.QueryString["name"]))
            {
                string author = Server.UrlDecode(Request.QueryString["name"]);
                Posts = Post.GetPostsByAuthor(author, Convert.ToInt32(ConfigurationManager.AppSettings["FrontPagePostsCount"]));
                Title = Resources.labels.AllPostsBy + " " + Server.HtmlEncode(author);
                base.AddMetaTag("description", Server.HtmlEncode(BlogSettings.Instance.Description + ", " + Title));
            }
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, exception.GetType());
        }
    }


    /// <summary>
    /// Permanently redirects to the correct URL format if the page is requested with
    /// the old URL: /default.aspx?year=2007&month=12
    /// <remarks>
    /// The redirection is important so that we don't end up having 2 URLs 
    /// to the same resource. It's for SEO purposes.
    /// </remarks>
    /// </summary>
    private void Redirect()
    {
        try
        {
            string year = Request.QueryString["year"];
            string month = Request.QueryString["month"];
            string date = Request.QueryString["date"];
            string page = string.IsNullOrEmpty(Request.QueryString["page"]) ? string.Empty : "?page=" + Request.QueryString["page"];
            string rewrite = null;

            if (!string.IsNullOrEmpty(date))
            {
                DateTime dateParsed = DateTime.Parse(date);
                rewrite = Utils.RelativeWebRoot + dateParsed.Year + "/" + dateParsed.Month + "/" + dateParsed.Day + "/default.aspx";
            }
            else if (!string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(month))
            {
                rewrite = Utils.RelativeWebRoot + year + "/" + month + "/default.aspx";
            }
            else if (!string.IsNullOrEmpty(year))
            {
                rewrite = Utils.RelativeWebRoot + year + "/default.aspx";
            }

            if (rewrite != null)
            {
                Response.Clear();
                Response.StatusCode = 301;
                Response.AppendHeader("location", rewrite + page);
                Response.End();
            }
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, exception.GetType());
        }
    }

    private static readonly Regex YEAR_MONTH = new Regex("/([0-9][0-9][0-9][0-9])/([0-1][0-9])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex YEAR_MONTH_DAY = new Regex("/([0-9][0-9][0-9][0-9])/([0-1][0-9])/([0-3][0-9])", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Adds the post's tags as meta keywords.
    /// </summary>
    private void AddMetaKeywords()
    {
        try
        {
            if (Category.Categories.Count > 0)
            {
                string[] categories = new string[Category.Categories.Count];
                for (int i = 0; i < Category.Categories.Count; i++)
                {
                    categories[i] = Category.Categories[i].Title;
                }

                string metakeywords = Server.HtmlEncode(string.Join(",", categories));
                System.Web.UI.HtmlControls.HtmlMeta tag = null;
                foreach (Control c in Page.Header.Controls)
                {
                    if (c is System.Web.UI.HtmlControls.HtmlMeta && (c as System.Web.UI.HtmlControls.HtmlMeta).Name.ToLower() == "keywords")
                    {
                        tag = c as System.Web.UI.HtmlControls.HtmlMeta;
                        tag.Content += ", " + metakeywords;
                        break;
                    }
                }
                if (tag == null)
                {
                    base.AddMetaTag("keywords", metakeywords);
                }
            }
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, exception.GetType());
        }
    }


}
