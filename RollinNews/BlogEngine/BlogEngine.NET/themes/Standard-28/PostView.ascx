<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="BlogEngine.Core.Web.Controls.PostViewBase" %>
<div class="post PostPad xfolkentry sharingDiv" id="post<%=Index %>">
    <h1 class="PostTitle">
        <%=Server.HtmlEncode(Post.Title) %>
    </h1>
    <div class="PostInfo Clear">
        <span class="PubDate"><%=Post.DateCreated.ToString("d. MMMM yyyy HH:mm") %></span>
        <span>/ </span>
        <span><a href="#aboutTheAuthor"><%=Post.AuthorDerbyName%></a></span>
        <span class="upperAdminLinks">
            <%=AdminLinks %>  
        </span>

        <a rel="nofollow" class="Right" href="<%=Post.RelativeOrAbsoluteLink %>#comment"><%=Resources.labels.comments %> (<%=Post.ApprovedComments.Count %>)</a>
        <div class="Clearer"></div>
    </div>
    <div class="postCenterImage">
        <% if (String.IsNullOrEmpty(Post.MainImageUrl))
           {  %>
        <img class="postMainImage" src="<%= Post.InitialImageUrl%>" />

        <% }
           else
           {  %>
        <img class="postMainImage" src="<%= Post.MainImageUrl%>" />

        <%} %>
    </div>

    <div class="postCategories">
        <span class="b">FOLLOW:</span> <span class="CatPost"><%=CategoryLinks(", ") %></span>
    </div>
    <div class="shareButtonsContainer">
        <ul class="postShareCounts">
            <li>
                <div class="fb-like" data-href="<%= Post.RelativeOrAbsoluteLink %>" data-layout="box_count" data-action="like" og: data-show-faces="false" data-share="false"></div>
            </li>
            <li>
                <div class="fb-share-button" data-href="<%= Post.RelativeOrAbsoluteLink %>" data-type="box_count"></div>
            </li>
            <li><a href="https://twitter.com/share" class="twitter-share-button" data-via="rollinnews" data-lang="en" data-related="anywhereTheJavascriptAPI" data-count="vertical">Tweet</a></li>
            <li>
                <div class="g-plus" data-action="share" data-annotation="vertical-bubble" data-height="60"></div>
            </li>
        </ul>

        <ul class="postShareBlank">
            <li>
                <a href="http://www.reddit.com/submit" onclick="window.location = 'http://www.reddit.com/submit?url=' + encodeURIComponent(window.location); return false">
                    <img src="http://www.reddit.com/static/spreddit5.gif" alt="submit to reddit" border="0" />
                </a>
            </li>
            <li>
                <su:badge layout="4"></su:badge>
            </li>
            <li><a href="http://www.tumblr.com/share" title="Share on Tumblr" style="display: inline-block; text-indent: -9999px; overflow: hidden; width: 20px; height: 20px; background: url('http://platform.tumblr.com/v1/share_4.png') top left no-repeat transparent;">Share on Tumblr</a></li>
        </ul>
        <div class="clear"></div>
    </div>

    <div class="shareScrollingContainer" id="header-anchor">
        <div id="header-scroller" class="shareHeaderScroller">
            <ul class="postShareCountsScroller" style="padding: 3px;">
                <li>
                    <div class="fb-like" data-href="<%= Post.RelativeOrAbsoluteLink %>" data-layout="box_count" data-action="like" data-show-faces="false" data-share="false"></div>
                </li>
                <li>
                    <div class="fb-share-button" data-href="<%= Post.RelativeOrAbsoluteLink %>" data-type="box_count"></div>
                </li>
                <li><a href="https://twitter.com/share" class="twitter-share-button" data-via="rollinnews" data-lang="en" data-related="anywhereTheJavascriptAPI" data-count="vertical">Tweet</a></li>
                <li>
                    <div class="g-plus" data-action="share" data-annotation="vertical-bubble" data-height="60"></div>
                </li>

                <li>
                    <a href="http://www.reddit.com/submit" onclick="window.location = 'http://www.reddit.com/submit?url=' + encodeURIComponent(window.location); return false">
                        <img src="http://www.reddit.com/static/spreddit5.gif" alt="submit to reddit" border="0" />
                    </a>
                    <su:badge layout="4"></su:badge>

                </li>
                <li><a href="http://www.tumblr.com/share" title="Share on Tumblr" style="display: inline-block; text-indent: -9999px; overflow: hidden; width: 20px; height: 20px; background: url('http://platform.tumblr.com/v1/share_4.png') top left no-repeat transparent;">Share on Tumblr</a></li>
            </ul>
        </div>
    </div>
      <% if (!String.IsNullOrEmpty(Post.BottomLineForConstribution))
       { %>
    <div class="PostTags bold">
        <br />
        <span>From the DNN Archive:</span>
        <br />
        <br />
    </div>
    <% } %>
    <% if (Post.FromFeed)
       { %>
    <div class="PostTags">
        <br />
        <span>This post was originally shared at <a target="_blank" href="<%= Post.FeedUrl %>"><%= Post.FeedName %></a></span>
        <br />
        <br />
    </div>
    <% } %>
    <div class="PostBody text">
        <asp:PlaceHolder ID="BodyContent" runat="server" />


    </div>
    <% if (!String.IsNullOrEmpty(Post.BottomLineForConstribution))
       { %>
    <div class="PostTags bold">
        <br />
        <span><%= Post.BottomLineForConstribution %></span>
        <br />
        <br />
    </div>
    <% } %>

    
    <div class="PostTags">
        <%=Resources.labels.tags %> : <%=TagLinks(", ") %>
    </div>

    <%=AdminLinks %>
    <script type="text/javascript">
        moveScroller("header-anchor", "header-scroller");
    </script>
</div>
