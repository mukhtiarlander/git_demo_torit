using System;
using System.Web.UI;
using BlogEngine.Core;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using BlogEngine.Core.Data;
using BlogEngine.Core.Data.Models;
using RDN.Library.Classes.RN.RSS;
using RDN.Library.Cache.Site;
using RDN.Portable.Classes.RN;

public partial class StandardSite28 : System.Web.UI.MasterPage
{
    private static Regex reg = new Regex(@"(?<=[^])\t{2,}|(?<=[>])\s{2,}(?=[<])|(?<=[>])\s{2,11}(?=[<])|(?=[\n])\s{2,}");

    protected static string ShRoot = Utils.ApplicationRelativeWebRoot + "editors/tiny_mce_3_5_8/plugins/syntaxhighlighter/";
    public List<RSSFeedItem> Feeds = new List<RSSFeedItem>();

    protected void Page_Load(object sender, EventArgs e)
    {
        
        // for supported of RTL languages
        if (Resources.labels.LangDirection.Equals("rtl", StringComparison.OrdinalIgnoreCase))
        {
            var lc = new LiteralControl("<link href=\"/themes/standard/include/rtl.css\" rel=\"stylesheet\" />");
            HeadContent.Controls.Add(lc);
        }

        // needed to make <%# %> binding work in the page header
        Page.Header.DataBind();
        if (!Utils.IsMono)
        {
            var lc = new LiteralControl("\n<!--[if lt IE 9]>" +
                "\n<script type=\"text/javascript\" src=\"/themes/standard/include/html5.js\"></script>" +
                "\n<![endif]-->\n");
            HeadContent.Controls.Add(lc);
        }
        if (Security.IsAuthenticated)
        {
            aLogin.InnerText = Resources.labels.logoff;
            aLogin.HRef = Utils.RelativeWebRoot + "Account/login.aspx?logoff";
            aLogin1.InnerText = Resources.labels.logoff;
            aLogin1.HRef = Utils.RelativeWebRoot + "Account/login.aspx?logoff";
        }
        else
        {
            aLogin.HRef = Utils.RelativeWebRoot + "Account/login.aspx";
            aLogin.InnerText = Resources.labels.login;
            aLogin1.InnerText = Resources.labels.logoff;
            aLogin1.HRef = Utils.RelativeWebRoot + "Account/login.aspx";
            aLogin2.HRef = Utils.RelativeWebRoot + "Account/login.aspx";
            aLogin2.InnerText = Resources.labels.login;
        }

        Feeds = RNCache.GetRSSFeeds();
    }

    protected override void Render(HtmlTextWriter writer)
    {
        using (HtmlTextWriter htmlwriter = new HtmlTextWriter(new System.IO.StringWriter()))
        {
            base.Render(htmlwriter);
            writer.Write(htmlwriter.InnerWriter.ToString());
        }
    }

}
