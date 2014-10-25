<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" Inherits="YAF.ForumPageBase" %>

<%@ Register TagPrefix="YAF" Assembly="YAF" Namespace="YAF" %>
<script runat="server">
	
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="YafHead" runat="server">
    <meta id="YafMetaScriptingLanguage" http-equiv="Content-Script-Type" runat="server"
        name="scriptlanguage" content="text/javascript" />
    <meta id="YafMetaStyles" http-equiv="Content-Style-Type" runat="server" name="styles"
        content="text/css" />
    <meta id="YafMetaDescription" runat="server" name="description" content="Yet Another Forum.NET -- A bulletin board system written in ASP.NET" />
    <meta id="YafMetaKeywords" runat="server" name="keywords" content="Yet Another Forum.net, Forum, ASP.NET, BB, Bulletin Board, opensource" />
    <meta name="HandheldFriendly" content="true" />
    <meta name="viewport" content="width=device-width,user-scalable=yes" />
    <title></title>
</head>
<body style="margin: 0; padding: 5px">
    <asp:HyperLink runat="server" ID="BannerLink">
        <img src="http://rdnation.com/Content/Rollerball_pink_s500.png" runat="server" alt="logo"
            style="border: 0; width: 200px; float: left;" id="imgBanner" />
    </asp:HyperLink>
    <div id="fb-root">
    </div>
    <div style="float: right;">
        <div class="fb-like-box" data-href="http://www.facebook.com/rdnation" data-width="292"
            data-colorscheme="light" data-show-faces="false" data-stream="false" data-header="true">
        </div>
    </div>
    <script>        (function (d, s, id) {
            var js, fjs = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) return;
            js = d.createElement(s); js.id = id;
            js.src = "//connect.facebook.net/en_US/all.js#xfbml=1&appId=159367097423747";
            fjs.parentNode.insertBefore(js, fjs);
        } (document, 'script', 'facebook-jssdk'));</script>
    <div style=" position:absolute; left:210px; top:50px; font-size: 3.5em; margin: 0px 0px 0px 20px; font-family: Arial Black;
        color: #872f95;">
        Officials of Roller Derby
    </div>
    <br />
    <div style="clear: both;">
    </div>
    <form id="form1" runat="server" enctype="multipart/form-data">
    <YAF:Forum runat="server" ID="forum" BoardID="1"></YAF:Forum>
    </form>
    <script type="text/javascript">

        var _gaq = _gaq || [];
        _gaq.push(['_setAccount', 'UA-29843570-1']);
        _gaq.push(['_setDomainName', 'rdnation.com']);
        _gaq.push(['_trackPageview']);

        (function () {
            var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
            ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
        })();

    </script>
</body>
</html>
