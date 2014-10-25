namespace UserControls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using BlogEngine.Core;
    using BlogEngine.Core.Web.Controls;
    using BlogEngine.Core.Web.Extensions;
    using RDN.Library.Classes.Error;
    using RDN.Portable.Config;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The comment view.
    /// </summary>
    public partial class CommentView : UserControl, ICallbackEventHandler
    {
        Regex tagRegex = new Regex(@"<[^>]+>");

        #region Constants and Fields

        /// <summary>
        ///     The callback.
        /// </summary>
        private string callback;

        /// <summary>
        ///     The nesting supported.
        /// </summary>
        private bool? nestingSupported;

        /// <summary>
        ///     Initializes a new instance of the <see cref = "CommentView" /> class.
        /// </summary>
        public CommentView()
        {
            NameInputId = string.Empty;
            DefaultName = string.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a value indicating whether NestingSupported.
        /// </summary>
        public bool NestingSupported
        {
            get
            {
                try
                {
                    if (!nestingSupported.HasValue)
                    {
                        if (!BlogSettings.Instance.IsCommentNestingEnabled)
                        {
                            nestingSupported = false;
                        }
                        else
                        {
                            var path = string.Format(
                                "{0}themes/{1}/CommentView.ascx", Utils.ApplicationRelativeWebRoot, BlogSettings.Instance.GetThemeWithAdjustments(null));

                            // test comment control for nesting placeholder (for backwards compatibility with older themes)
                            var commentTester = (CommentViewBase)LoadControl(path);
                            var subComments = commentTester.FindControl("phSubComments") as PlaceHolder;
                            nestingSupported = subComments != null;
                        }
                    }

                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, GetType());
                }
                return nestingSupported.Value;
            }
        }

        public bool DisplayCommentForm
        {
            get
            {
                try
                {
                    if (BlogSettings.Instance.IsCommentsEnabled && Security.IsAuthorizedTo(Rights.CreateComments))
                    {
                        if (Post != null && (!Post.HasCommentsEnabled || (BlogSettings.Instance.DaysCommentsAreEnabled > 0 &&
                            Post.DateCreated.AddDays(BlogSettings.Instance.DaysCommentsAreEnabled) < DateTime.Now.Date)))
                        {
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, GetType());
                }
                return false;
            }
        }

        /// <summary>
        ///     Gets or sets the post from which the comments are parsed.
        /// </summary>
        public Post Post { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [any captcha enabled].
        /// </summary>
        /// <value><c>true</c> if [any captcha enabled]; otherwise, <c>false</c>.</value>
        protected bool AnyCaptchaEnabled { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [any captcha necessary].
        /// </summary>
        /// <value><c>true</c> if [any captcha necessary]; otherwise, <c>false</c>.</value>
        protected bool AnyCaptchaNecessary { get; set; }

        /// <summary>
        ///     Gets or sets the comment counter.
        /// </summary>
        /// <value>The comment counter.</value>
        protected int CommentCounter { get; set; }

        /// <summary>
        ///     Gets or sets the default name.
        /// </summary>
        /// <value>The default name.</value>
        protected string DefaultName { get; set; }

        /// <summary>
        ///     Gets or sets the name input id.
        /// </summary>
        /// <value>The name input id.</value>
        protected string NameInputId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [re captcha enabled].
        /// </summary>
        /// <value><c>true</c> if [re captcha enabled]; otherwise, <c>false</c>.</value>
        protected bool ReCaptchaEnabled { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [simple captcha enabled].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [simple captcha enabled]; otherwise, <c>false</c>.
        /// </value>
        protected bool SimpleCaptchaEnabled { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resolves the region based on the browser language.
        /// </summary>
        /// <returns>
        /// The region info.
        /// </returns>
        public static RegionInfo ResolveRegion()
        {
            var languages = HttpContext.Current.Request.UserLanguages;

            if (languages == null || languages.Length == 0)
            {
                return new RegionInfo(CultureInfo.CurrentCulture.LCID);
            }

            try
            {
                var language = languages[0].ToLowerInvariant().Trim();
                var culture = CultureInfo.CreateSpecificCulture(language);
                return new RegionInfo(culture.LCID);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new RegionInfo(CultureInfo.CurrentCulture.LCID);
        }

        /// <summary>
        /// Binds the country dropdown list with countries retrieved
        ///     from the .NET Framework.
        /// </summary>
        public void BindCountries()
        {
        }

        #endregion

        #region Implemented Interfaces

        #region ICallbackEventHandler

        /// <summary>
        /// Returns the results of a callback event that targets a control.
        /// </summary>
        /// <returns>
        /// The result of the callback.
        /// </returns>
        public string GetCallbackResult()
        {
            return callback;
        }

        /// <summary>
        /// Processes a callback event that targets a control.
        /// </summary>
        /// <param name="eventArgument">
        /// A string that represents an event argument to pass to the event handler.
        /// </param>
        /// 
   
        public void RaiseCallbackEvent(string eventArgument)
        {
            try
            {
                if (!tagRegex.IsMatch(eventArgument))
                {
                    if (!BlogSettings.Instance.IsCommentsEnabled || !Security.IsAuthorizedTo(Rights.CreateComments))
                    {
                        return;
                    }
                    if (!Security.IsAuthenticated)
                        return;
                    var memId = RDN.Library.Classes.Account.User.GetMemberId();
                    var mem = RDN.Library.Cache.SiteCache.GetPublicMemberFull(memId);

                    var args = eventArgument.Split(new[] { "-|-" }, StringSplitOptions.None);
                    var author = args[0];
                    var email = args[1];
                    var website = args[2];
                    var country = args[3];
                    var content = args[4];
                    var notify = bool.Parse(args[5]);
                    var preview = bool.Parse(args[6]);

                    // If there is no "reply to" comment, args[8] is empty
                    var replyToCommentId = String.IsNullOrEmpty(args[8]) ? Guid.Empty : new Guid(args[8]);
                    var avatar = args[9];
                    string DerbyName = string.Empty;
                    Guid derbyId = new Guid();
                    if (mem != null)
                    {
                        DerbyName = mem.DerbyName;
                        derbyId = mem.MemberId;
                    }
                    var comment = new Comment
                        {
                            Id = Guid.NewGuid(),
                            ParentId = replyToCommentId,
                            //   Author = Server.HtmlEncode(author),
                            Author = HttpUtility.HtmlAttributeEncode(DerbyName),
                            Email = Security.CurrentUser.Identity.Name,
                            //   Content = Server.HtmlEncode(content),
                            Content = HttpUtility.HtmlAttributeEncode(content),
                            IP = Utils.GetClientIP(),
                            Country = Server.HtmlEncode(country),
                            DateCreated = DateTime.Now,
                            Parent = Post,
                            IsApproved = !BlogSettings.Instance.EnableCommentsModeration,
                            Avatar = Server.HtmlEncode(avatar.Trim()),
                            Website = new Uri(ServerConfig.WEBSITE_DEFAULT_LOCATION_FOR_MEMBER + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(DerbyName) + "/" + derbyId.ToString().Replace("-", ""))
                        };


                    comment.IsApproved = true;


                    if (!preview)
                    {
                        if (notify && !Post.NotificationEmails.Contains(comment.Email))
                        {
                            Post.NotificationEmails.Add(comment.Email);
                        }
                        else if (!notify && Post.NotificationEmails.Contains(comment.Email))
                        {
                            Post.NotificationEmails.Remove(comment.Email);
                        }

                        Post.AddComment(comment);
                    }

                    var path = string.Format(
                        "{0}themes/{1}/CommentView.ascx", Utils.ApplicationRelativeWebRoot, BlogSettings.Instance.GetThemeWithAdjustments(null));

                    var control = (CommentViewBase)LoadControl(path);
                    control.Comment = comment;
                    control.Post = Post;
                    control.RenderComment();

                    using (var sw = new StringWriter())
                    {
                        control.RenderControl(new HtmlTextWriter(sw));
                        callback = sw.ToString();
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType(), additionalInformation: eventArgument);
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Displays a delete link to visitors that is authenticated
        ///     using the default membership provider.
        /// </summary>
        /// <param name="id">
        /// The id of the comment.
        /// </param>
        /// <returns>
        /// The admin link.
        /// </returns>
        protected string AdminLink(string id)
        {
            try
            {
                if (Security.IsAuthenticated)
                {
                    var sb = new StringBuilder();
                    foreach (var comment in Post.Comments.Where(comment => comment.Id.ToString() == id))
                    {
                        sb.AppendFormat(" | <a href=\"mailto:{0}\">{0}</a>", comment.Email);
                    }

                    if (Security.IsAuthorizedTo(Rights.ModerateComments))
                    {
                        string ConfirmDelete = Resources.labels.areYouSureDeleteComment;
                        sb.AppendFormat(
                            " | <a href=\"?deletecomment={0}\" onclick=\"return confirm('{1}?')\">{2}</a>",
                            id,
                            ConfirmDelete,
                            Resources.labels.delete);

                    }
                    return sb.ToString();
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return string.Empty;
        }

        /// <summary>
        /// Displays BBCodes dynamically loaded from settings.
        /// </summary>
        /// <returns>
        /// The bb codes.
        /// </returns>
        protected string BBCodes()
        {

            var sb = new StringBuilder();
            try
            {
                var settings = ExtensionManager.GetSettings("BBCode");
                if (settings != null)
                {
                    var table = settings.GetDataTable();

                    foreach (DataRow row in table.Rows)
                    {
                        var code = (string)row["Code"];
                        var title = string.Format("[{0}][/{1}]", code, code);
                        sb.AppendFormat(
                            "<a title=\"{0}\" href=\"javascript:void(BlogEngine.addBbCode('{1}'))\">{2}</a>",
                            title,
                            code,
                            code);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return sb.ToString();

        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                bool isOdd = true;

                if (Post == null)
                {
                    Response.Redirect(Utils.RelativeWebRoot, false);
                    return;
                }

                NameInputId = string.Format("txtName{0}", DateTime.Now.Ticks);
                EnableCaptchas();

                if (!Page.IsPostBack && !Page.IsCallback)
                {
                    if (Security.IsAuthorizedTo(Rights.ModerateComments))
                    {
                        if (Request.QueryString["deletecomment"] != null)
                        {
                            DeleteComment();
                        }

                        else if (Request.QueryString["deletecommentandchildren"] != null)
                        {
                            DeleteCommentAndChildren();
                        }

                        else if (!string.IsNullOrEmpty(Request.QueryString["approvecomment"]))
                        {
                            ApproveComment();
                        }

                        else if (!string.IsNullOrEmpty(Request.QueryString["approveallcomments"]))
                        {
                            ApproveAllComments();
                        }
                    }

                    var path = string.Format(
                        "{0}themes/{1}/CommentView.ascx", Utils.ApplicationRelativeWebRoot, BlogSettings.Instance.GetThemeWithAdjustments(null));

                    bool canViewUnpublishedPosts = Security.IsAuthorizedTo(AuthorizationCheck.HasAny, new[] { Rights.ViewUnmoderatedComments, Rights.ModerateComments });

                    if (NestingSupported)
                    {
                        // newer, nested comments
                        if (Post != null)
                        {
                            AddNestedComments(path, Post.NestedComments, phComments, canViewUnpublishedPosts);
                        }
                    }
                    else
                    {
                        // old, non nested code
                        // Add approved Comments

                        isOdd = true;

                        foreach (var comment in
                            Post.Comments.Where(
                                comment => comment.Email != "pingback" && comment.Email != "trackback"))
                        {
                            if (comment.IsApproved)
                            {
                                CommentCounter++;
                            }

                            if (!comment.IsApproved && BlogSettings.Instance.EnableCommentsModeration)
                            {
                                continue;
                            }

                            isOdd = !isOdd;
                            var control = (CommentViewBase)LoadControl(path);
                            control.Comment = comment;
                            control.Post = Post;
                            control.IsOdd = isOdd;
                            phComments.Controls.Add(control);
                        }

                        // Add unapproved comments
                        if (canViewUnpublishedPosts)
                        {
                            foreach (var comment in Post.Comments)
                            {
                                if (comment.Email == "pingback" || comment.Email == "trackback")
                                {
                                    continue;
                                }

                                if (comment.IsApproved)
                                {
                                    continue;
                                }

                                isOdd = !isOdd;
                                var control = (CommentViewBase)LoadControl(path);
                                control.Comment = comment;
                                control.Post = Post;
                                control.IsOdd = isOdd;
                                phComments.Controls.Add(control);
                            }
                        }
                    }

                    var pingbacks = new List<CommentViewBase>();

                    isOdd = true;
                    foreach (var comment in Post.Comments)
                    {
                        var control = (CommentViewBase)LoadControl(path);

                        if (comment.Email != "pingback" && comment.Email != "trackback")
                        {
                            continue;
                        }

                        isOdd = !isOdd;
                        control.Comment = comment;
                        control.Post = Post;
                        control.IsOdd = isOdd;
                        pingbacks.Add(control);
                    }

                    if (pingbacks.Count > 0)
                    {
                        var litTrackback = new Literal();
                        var sb = new StringBuilder();
                        sb.AppendFormat("<h3 id=\"trackbackheader\">Pingbacks and trackbacks ({0})", pingbacks.Count);
                        sb.Append(
                            "<a id=\"trackbacktoggle\" style=\"float:right;width:20px;height:20px;border:1px solid #ccc;text-decoration:none;text-align:center\"");
                        sb.Append(" href=\"javascript:toggle_visibility('trackbacks','trackbacktoggle');\">+</a>");
                        sb.Append("</h3><div id=\"trackbacks\" style=\"display:none\">");
                        litTrackback.Text = sb.ToString();
                        phTrckbacks.Controls.Add(litTrackback);

                        foreach (var c in pingbacks)
                        {
                            phTrckbacks.Controls.Add(c);
                        }

                        var closingDiv = new Literal { Text = @"</div>" };
                        phTrckbacks.Controls.Add(closingDiv);
                    }
                    else
                    {
                        phTrckbacks.Visible = false;
                    }

                    if (DisplayCommentForm)
                    {
                        GetCookie();
                        recaptcha.UserUniqueIdentifier = hfCaptcha.Value = Guid.NewGuid().ToString();

                        phAddComment.Visible = true;
                        lbCommentsDisabled.Visible = false;
                        LoadCommentForm();
                        var user = RDN.Library.Cache.SiteCache.GetPublicMemberFull(Security.CurrentUser.Identity.Name);
                        if (user != null)
                        {

                            var uPhoto = user.Photos.FirstOrDefault();
                            if (uPhoto != null)
                            {
                                if (!String.IsNullOrEmpty(uPhoto.ImageThumbUrl))
                                    commentImg.Src = uPhoto.ImageThumbUrl;
                                else
                                    commentImg.Src = uPhoto.ImageUrl;
                            }
                        }
                    }
                    else
                    {
                        phAddComment.Visible = false;
                        lbCommentsDisabled.Visible = true;
                    }
                }

                Page.ClientScript.GetCallbackEventReference(this, "arg", null, string.Empty);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        private void LoadCommentForm()
        {
            try
            {
                string path = FromThemeFolder("CommentForm.ascx");
                if (!File.Exists(Server.MapPath(path)))
                {
                    path = Utils.ApplicationRelativeWebRoot + "User controls/Defaults/CommentForm.ascx";
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }

        }



        private string FromThemeFolder(string file)
        {
            return Utils.ApplicationRelativeWebRoot + "themes/" + BlogSettings.Instance.GetThemeWithAdjustments(null) + "/" + file;
        }

        /// <summary>
        /// Adds the nested comments.
        /// </summary>
        /// <param name="path">
        /// The path string.
        /// </param>
        /// <param name="nestedComments">
        /// The nested comments.
        /// </param>
        /// <param name="commentsPlaceHolder">
        /// The comments place holder.
        /// </param>
        private void AddNestedComments(string path, IEnumerable<Comment> nestedComments, Control commentsPlaceHolder, bool canViewUnpublishedPosts)
        {
            try
            {
                bool enableCommentModeration = BlogSettings.Instance.EnableCommentsModeration;

                bool isOdd = true;
                foreach (var comment in nestedComments)
                {
                    if ((!comment.IsApproved && enableCommentModeration) &&
                        (comment.IsApproved || !canViewUnpublishedPosts))
                    {
                        continue;
                    }

                    // if comment is spam, only authorized can see it
                    if (comment.IsSpam && !canViewUnpublishedPosts)
                    {
                        continue;
                    }

                    if (comment.Email == "pingback" || comment.Email == "trackback")
                    {
                        continue;
                    }

                    isOdd = !isOdd;

                    var control = (CommentViewBase)LoadControl(path);
                    control.Comment = comment;
                    control.Post = Post;
                    control.IsOdd = isOdd;

                    if (comment.IsApproved)
                    {
                        CommentCounter++;
                    }

                    if (comment.Comments.Count > 0)
                    {
                        // find the next placeholder and add the subcomments to it
                        var subCommentsPlaceHolder = control.FindControl("phSubComments") as PlaceHolder;
                        if (subCommentsPlaceHolder != null)
                        {
                            AddNestedComments(path, comment.Comments, subCommentsPlaceHolder, canViewUnpublishedPosts);
                        }
                    }

                    commentsPlaceHolder.Controls.Add(control);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
        }

        /// <summary>
        /// Approves all comments.
        /// </summary>
        private void ApproveAllComments()
        {
            try
            {
                Security.DemandUserHasRight(Rights.ModerateComments, true);

                Post.ApproveAllComments();

                var index = Request.RawUrl.IndexOf("?");
                var url = Request.RawUrl.Substring(0, index);
                Response.Redirect(url);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
        }

        /// <summary>
        /// Approves the comment.
        /// </summary>
        private void ApproveComment()
        {
            try
            {
                Security.DemandUserHasRight(Rights.ModerateComments, true);

                foreach (var comment in
                    Post.NotApprovedComments.Where(
                        comment => comment.Id == new Guid(Request.QueryString["approvecomment"])))
                {
                    Post.ApproveComment(comment);

                    var index = Request.RawUrl.IndexOf("?");
                    var url = Request.RawUrl.Substring(0, index);
                    Response.Redirect(url);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
        }

        /// <summary>
        /// Collects the comment to delete.
        /// </summary>
        /// <param name="comment">
        /// The comment.
        /// </param>
        /// <param name="commentsToDelete">
        /// The comments to delete.
        /// </param>
        private void CollectCommentToDelete(Comment comment, List<Comment> commentsToDelete)
        {
            try
            {
                commentsToDelete.Add(comment);

                // recursive collection
                foreach (var subComment in comment.Comments)
                {
                    CollectCommentToDelete(subComment, commentsToDelete);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
        }

        /// <summary>
        /// Deletes the comment.
        /// </summary>
        private void DeleteComment()
        {
            try
            {
                Security.DemandUserHasRight(Rights.ModerateComments, true);

                foreach (var comment in
                    Post.Comments.Where(comment => comment.Id == new Guid(Request.QueryString["deletecomment"])))
                {
                    Post.RemoveComment(comment);

                    var index = Request.RawUrl.IndexOf("?");
                    var url = string.Format("{0}#comment", Request.RawUrl.Substring(0, index));
                    Response.Redirect(url, false);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
        }

        /// <summary>
        /// Deletes the comment and children.
        /// </summary>
        private void DeleteCommentAndChildren()
        {
            try
            {
                Security.DemandUserHasRight(Rights.ModerateComments, true);

                var deletecommentandchildren = new Guid(Request.QueryString["deletecommentandchildren"]);

                foreach (var comment in Post.Comments)
                {
                    if (comment.Id != deletecommentandchildren)
                    {
                        continue;
                    }

                    // collect comments to delete first so the Nesting isn't lost
                    var commentsToDelete = new List<Comment>();

                    CollectCommentToDelete(comment, commentsToDelete);

                    foreach (var commentToDelete in commentsToDelete)
                    {
                        Post.RemoveComment(commentToDelete);
                    }

                    var index = Request.RawUrl.IndexOf("?");
                    var url = string.Format("{0}#comment", Request.RawUrl.Substring(0, index));
                    Response.Redirect(url);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
        }

        /// <summary>
        /// Enables the captchas.
        /// </summary>
        private void EnableCaptchas()
        {
            try
            {
                ReCaptchaEnabled = ExtensionManager.ExtensionEnabled("Recaptcha");
                SimpleCaptchaEnabled = ExtensionManager.ExtensionEnabled("SimpleCaptcha");
                if (ReCaptchaEnabled && SimpleCaptchaEnabled)
                {
                    var simpleCaptchaExtension = ExtensionManager.GetExtension("SimpleCaptcha");
                    var recaptchaExtension = ExtensionManager.GetExtension("Recaptcha");
                    if (simpleCaptchaExtension.Priority < recaptchaExtension.Priority)
                    {
                        EnableRecaptcha();
                    }
                    else
                    {
                        EnableSimpleCaptcha();
                    }
                }
                else if (ReCaptchaEnabled)
                {
                    EnableRecaptcha();
                }
                else if (SimpleCaptchaEnabled)
                {
                    EnableSimpleCaptcha();
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
        }

        /// <summary>
        /// Enables the recaptcha.
        /// </summary>
        private void EnableRecaptcha()
        {
            try
            {
                AnyCaptchaEnabled = true;
                AnyCaptchaNecessary = recaptcha.RecaptchaNecessary;
                recaptcha.Visible = true;
                simplecaptcha.Visible = false;
                SimpleCaptchaEnabled = false;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
        }

        /// <summary>
        /// Enables the simple captcha.
        /// </summary>
        private void EnableSimpleCaptcha()
        {
            AnyCaptchaEnabled = true;
            AnyCaptchaNecessary = simplecaptcha.SimpleCaptchaNecessary;
            simplecaptcha.Visible = true;
            recaptcha.Visible = false;
            ReCaptchaEnabled = false;
        }

        /// <summary>
        /// Gets the cookie with visitor information if any is set.
        ///     Then fills the contact information fields in the form.
        /// </summary>
        private void GetCookie()
        {
            var cookie = Request.Cookies["comment"];
            try
            {
                //if (cookie != null)
                //{
                //    DefaultName = Server.UrlDecode(cookie.Values["name"]);
                //    txtEmail.Text = cookie.Values["email"];
                //    txtWebsite.Text = cookie.Values["url"];
                //    ddlCountry.SelectedValue = cookie.Values["country"];
                //}
                //else if (Security.IsAuthenticated)
                //{
                //    var user = Membership.GetUser();
                //    if (user != null)
                //    {
                //        DefaultName = user.UserName;
                //        txtEmail.Text = user.Email;
                //    }

                //    txtWebsite.Text = Request.Url.Host;
                //}
            }
            catch (Exception)
            {
                // Couldn't retrieve info on the visitor/user
            }
        }

        /// <summary>
        /// Sets a cookie with the entered visitor information
        ///     so it can be prefilled on next visit.
        /// </summary>
        /// <param name="name">
        /// The cookie name.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="website">
        /// The website.
        /// </param>
        /// <param name="country">
        /// The country.
        /// </param>
        private void SetCookie(string name, string email, string website, string country)
        {
            var cookie = new HttpCookie("comment") { Expires = DateTime.Now.AddMonths(24) };
            cookie.Values.Add("name", Server.UrlEncode(name.Trim()));
            cookie.Values.Add("email", email.Trim());
            cookie.Values.Add("url", website.Trim());
            cookie.Values.Add("country", country);
            Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// Sets the flag image URL.
        /// </summary>
        protected string FlagUrl()
        {
            //return !string.IsNullOrEmpty(ddlCountry.SelectedValue)
            //    ? string.Format("{0}pics/flags/{1}.png", Utils.RelativeWebRoot, ddlCountry.SelectedValue)
            //    : string.Format("{0}pics/pixel.png", Utils.RelativeWebRoot);
            return string.Format("{0}pics/pixel.png", Utils.RelativeWebRoot);
        }

        #endregion
    }
}