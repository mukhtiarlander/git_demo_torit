#region Using

using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using System.Net.Mail;
using System.Text.RegularExpressions;

#endregion

public partial class writeForfaq : BlogBasePage
{

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        Page.Title = Server.HtmlEncode("Write For Rollin News FAQ");
        base.AddMetaTag("description", Utils.StripHtml(BlogSettings.Instance.ContactFormMessage));
    }






}