<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CommentView.ascx.cs" EnableViewState="false" Inherits="UserControls.CommentView" %>
<%@ Import Namespace="BlogEngine.Core" %>

<div class="well-global ">
    <div class="CommentContainer">
   
        <div class="commentCounterIcon">
        <img src="<%=Utils.RelativeWebRoot %>content/images/commentIcon.png"  />
            </div>
        <div class="commentTitleContatiner">
    <div id="comment" class="commentTitle">
        <%=Resources.labels.comments %> 
        
         <% if (CommentCounter > 0)
       { %>
        (<%=CommentCounter%>)<%} %>
    </div>
        <div class="commentCountDesc">Share what you think</div>
            </div>
        <div class="clear" style="margin-bottom:10px;"></div>
    

        
    <asp:PlaceHolder runat="Server" ID="phAddComment">

        <div id="comment-form">
            <img src="<%=Utils.RelativeWebRoot %>pics/ajax-loader.gif" width="24" height="24" alt="Saving the comment" style="display: none" id="ajaxLoader" />
            <span id="status"></span>

            <% if (!Security.IsAuthenticated)
               {  %>
            <div class="commentForm">
                <h3 id="H1"><%=Resources.labels.addComment%></h3>
                <p>
                    <span class="b" id="commentWarning">Please <a href="<%= Page.ResolveUrl("~/Account/login.aspx?u=f&returnUrl=" + Request.RawUrl + "#comment-form")%>">Login</a> or <a href="https://rdnation.com/SignUp?returnSite=rollinNews&ReturnUrl=<%= Request.Url%>#comment-form">Register</a> To Add A Comment</span>
                    <textarea class="txt-content" tabindex="7" id="txtContent" cols="50" rows="4" placeholder="Start a conversation..." name="txtContent"></textarea>
                </p>
            </div>
            <script type="text/javascript">
                $("#commentWarning").hide();
                $("#txtContent").focus(function () {
                    $("#commentWarning").show();
                });
            </script>

            <% }
               else
               {  %>
            <div class="commentForm">
                <h3 id="addcomment"><%=Resources.labels.addComment %></h3>
                <img class="commentInitialGravatar" id="commentImg" runat="server" />

                <textarea class="txt-content" tabindex="7" id="txtContent" placeholder="Start a conversation..." rows="3" name="txtContent"></textarea>
                <p>
                    <input type="checkbox" id="cbNotify" class="cmnt-frm-notify" style="width: auto" tabindex="8" />
                    <label for="cbNotify" style="width: auto; float: none; display: inline; padding-left: 5px"><%=Resources.labels.notifyOnNewComments %></label>
                </p>
                <p>
                    <input type="button" id="btnSaveAjax" class="btn-save" style="margin-top: 10px" value="<%=Resources.labels.saveComment %>" onclick="return BlogEngine.validateAndSubmitCommentForm()" tabindex="10" />
                </p>
            </div>
            <%} %>

            <% if (NestingSupported)
               { %>
            <asp:HiddenField runat="Server" ID="hiddenReplyTo" />
            <p id="cancelReply" style="display: none;">
                <a href="javascript:void(0);" onclick="BlogEngine.cancelReply();"><%=Resources.labels.cancelReply %></a>
            </p>
            <%} %>
            <% if (Security.IsAuthenticated)
               {  %>
            <blog:SimpleCaptchaControl ID="simplecaptcha" runat="server" />
            <blog:RecaptchaControl ID="recaptcha" runat="server" />
            <asp:HiddenField runat="server" ID="hfCaptcha" />
            <%} %>
        </div>

        <script type="text/javascript">
    <!--//
    BlogEngine.comments.flagImage = BlogEngine.$("imgFlag");
    BlogEngine.comments.contentBox = BlogEngine.$("txtContent");
    BlogEngine.comments.moderation = <%=BlogSettings.Instance.EnableCommentsModeration.ToString().ToLowerInvariant() %>;
	    BlogEngine.comments.checkName = <%=(!Security.IsAuthenticated).ToString().ToLowerInvariant() %>;
	    BlogEngine.comments.postAuthor = "<%=Post.Author %>";
    BlogEngine.comments.nameBox = BlogEngine.$("txtName");
    BlogEngine.comments.emailBox = BlogEngine.$("txtEmail");
    BlogEngine.comments.websiteBox = BlogEngine.$("txtWebsite");
    BlogEngine.comments.countryDropDown = BlogEngine.$("ddlCountry");
    BlogEngine.comments.controlId = '<%=UniqueID %>';
    BlogEngine.comments.captchaField = BlogEngine.$('<%=hfCaptcha.ClientID %>');
            BlogEngine.comments.replyToId = BlogEngine.$("<%=hiddenReplyTo.ClientID %>");
            //-->
        </script>
       
    </asp:PlaceHolder>
    </div>

    <div id="commentlist" style="display: block">
        <asp:PlaceHolder runat="server" ID="phComments" />

    </div>

</div>

<asp:PlaceHolder runat="server" ID="phTrckbacks"></asp:PlaceHolder>

<script type="text/javascript">
    function toggle_visibility(id, id2) {
        var e = document.getElementById(id);
        var h = document.getElementById(id2);
        if (e.style.display == 'block') {
            e.style.display = 'none';
            h.innerHTML = "+";
        }
        else {
            e.style.display = 'block';
            h.innerHTML = "-";
        }
    }
</script>

<asp:Label runat="server" ID="lbCommentsDisabled" CssClass="lbl-CommentsDisabled" Visible="false">
    <%=Resources.labels.commentsAreClosed %>
</asp:Label>