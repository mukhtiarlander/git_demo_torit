#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using System.Net.Mail;
using System.Text.RegularExpressions;
using RDN.Library.Classes.Facebook;
using RDN.Library.Classes.Facebook.Enum;
using System.Linq;

#endregion

public partial class writeFor : BlogBasePage, ICallbackEventHandler
{

    string[] spamWords = new string[] { "Air Jordan Retro", "louis vuitton", "fake oakleys", "cheap hockey jerseys",
        "phonesdaq.com", "gambling</a>","coach factory outlet","cheap retro jordans"};

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        ClientScript.GetCallbackEventReference(this, "arg", "callback", "context");
        btnSend.Click += new EventHandler(btnSend_Click);
        if (!Page.IsPostBack)
        {
            txtSubject.Text = Request.QueryString["subject"];
            txtName.Text = Request.QueryString["name"];
            txtEmail.Text = Request.QueryString["email"];

            GetCookie();
            SetFocus();
        }

        if (!IsPostBack && !IsCallback)
        {
            recaptcha.Visible = UseCaptcha;
            recaptcha.UserUniqueIdentifier = hfCaptcha.Value = Guid.NewGuid().ToString();
        }

        Page.Title = Server.HtmlEncode("Write For Rollin News");
        base.AddMetaTag("description", Utils.StripHtml(BlogSettings.Instance.ContactFormMessage));
    }

    /// <summary>
    /// Sets the focus on the first empty textbox.
    /// </summary>
    private void SetFocus()
    {
        if (string.IsNullOrEmpty(Request.QueryString["name"]) && txtName.Text == string.Empty)
        {
            txtName.Focus();
        }
        else if (string.IsNullOrEmpty(Request.QueryString["email"]) && txtEmail.Text == string.Empty)
        {
            txtEmail.Focus();
        }
        else if (string.IsNullOrEmpty(Request.QueryString["subject"]))
        {
            txtSubject.Focus();
        }
        else
        {
            txtMessage.Focus();
        }
    }

    /// <summary>
    /// Handles the Click event of the btnSend control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void btnSend_Click(object sender, EventArgs e)
    {

        if (Page.IsValid)
        {
            if (!UseCaptcha || IsCaptchaValid)
            {
                //if (!IsSpam())
                //{
                //    bool success = SendEmail(txtEmail.Text, txtName.Text, txtSubject.Text, txtMessage.Text);
                //    divForm.Visible = !success;
                //    lblStatus.Visible = !success;
                //    divThank.Visible = success;
                //    SetCookie();
                //}
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "captcha-incorrect", " displayIncorrectCaptchaMessage(); ", true);
            }
        }
    }

    private bool IsSpam(string subject, string text)
    {

        if (String.IsNullOrEmpty(text))
            return true;
        if (String.IsNullOrEmpty(subject))
            return true;


        if (spamWords.Any(subject.Contains))
            return true;
        if (spamWords.Any(text.Contains))
            return true;

        return false;
    }

    private bool SendEmail(string email, string name, string subject, string message)
    {
        try
        {
            string Body = "<div style=\"font: 11px verdana, arial\">";
            Body += Server.HtmlEncode(message).Replace("\n", "<br />") + "<br /><br />";
            Body += "<hr /><br />";
            Body += "<h3>" + Resources.labels.contactAuthorInformation + "</h3>";
            Body += "<div style=\"font-size:10px;line-height:16px\">";
            Body += "<strong>" + Resources.labels.name + ":</strong> " + Server.HtmlEncode(name) + "<br />";
            Body += "<strong>" + Resources.labels.email + ":</strong> " + Server.HtmlEncode(email) + "<br />";

            if (ViewState["url"] != null)
                Body += string.Format("<strong>" + Resources.labels.website + ":</strong> <a href=\"{0}\">{0}</a><br />", ViewState["url"]);

            if (ViewState["country"] != null)
                Body += "<strong>" + Resources.labels.countryCode + ":</strong> " + ((string)ViewState["country"]).ToUpperInvariant() + "<br />";

            if (HttpContext.Current != null)
            {
                Body += "<strong>" + Resources.labels.contactIPAddress + ":</strong> " + Utils.GetClientIP() + "<br />";
                Body += "<strong>" + Resources.labels.contactUserAgent + ":</strong> " + HttpContext.Current.Request.UserAgent;
            }
            RDN.Library.Classes.EmailServer.EmailServer.SendEmail(RDN.Portable.Config.RollinNewsConfig.DEFAULT_EMAIL, RDN.Portable.Config.RollinNewsConfig.DEFAULT_EMAIL_FROM_NAME, RDN.Portable.Config.RollinNewsConfig.DEFAULT_ADMIN_EMAIL, "Inquiry For Writing", Body, RDN.Library.DataModels.EmailServer.Enums.EmailPriority.Normal);
            RDN.Library.Classes.EmailServer.EmailServer.SendEmail(RDN.Portable.Config.RollinNewsConfig.DEFAULT_EMAIL, RDN.Portable.Config.RollinNewsConfig.DEFAULT_EMAIL_FROM_NAME, RDN.Portable.Config.ServerConfig.DEFAULT_KRIS_WORLIDGE_EMAIL_ADMIN, "Inquiry For Writing", Body, RDN.Library.DataModels.EmailServer.Enums.EmailPriority.Normal);
            RDN.Library.Classes.EmailServer.EmailServer.SendEmail(RDN.Portable.Config.RollinNewsConfig.DEFAULT_EMAIL, RDN.Portable.Config.RollinNewsConfig.DEFAULT_EMAIL_FROM_NAME, RDN.Portable.Config.ServerConfig.DEFAULT_ADMIN_EMAIL_ADMIN, "Inquiry For Writing", Body, RDN.Library.DataModels.EmailServer.Enums.EmailPriority.Normal);


            return true;
        }
        catch (Exception ex)
        {
            RDN.Library.Classes.Error.ErrorDatabaseManager.AddException(ex, GetType());

            return false;
        }
    }

    // comment test

    #region Cookies

    /// <summary>
    /// Gets the cookie with visitor information if any is set.
    /// Then fills the contact information fields in the form.
    /// </summary>
    private void GetCookie()
    {
        HttpCookie cookie = Request.Cookies["comment"];
        if (cookie != null)
        {
            txtName.Text = Server.UrlDecode(cookie.Values["name"]);
            txtEmail.Text = cookie.Values["email"];
            ViewState["url"] = cookie.Values["url"];
            ViewState["country"] = cookie.Values["country"];
        }
    }

    /// <summary>
    /// Sets a cookie with the entered visitor information
    /// so it can be prefilled on next visit.
    /// </summary>
    private void SetCookie()
    {
        HttpCookie cookie = new HttpCookie("comment");
        cookie.Expires = DateTime.Now.AddMonths(24);
        cookie.Values.Add("name", Server.UrlEncode(txtName.Text));
        cookie.Values.Add("email", txtEmail.Text);
        cookie.Values.Add("url", string.Empty);
        cookie.Values.Add("country", string.Empty);
        Response.Cookies.Add(cookie);
    }

    #endregion

    #region CAPTCHA

    /// <summary> 
    /// Gets whether or not the user is human 
    /// </summary> 
    private bool IsCaptchaValid
    {
        get
        {
            recaptcha.Validate();
            return recaptcha.IsValid;
        }
    }

    private bool UseCaptcha
    {
        get
        {
            return
                BlogSettings.Instance.EnableRecaptchaOnContactForm &&
                recaptcha.RecaptchaEnabled &&
                recaptcha.RecaptchaNecessary;
        }
    }

    #endregion


    #region ICallbackEventHandler Members

    private string _Callback;

    public string GetCallbackResult()
    {
        return _Callback;
    }

    public void RaiseCallbackEvent(string eventArgument)
    {
        string[] arg = eventArgument.Split(new string[] { "-||-" }, StringSplitOptions.None);
        if (arg.Length == 6)
        {
            string name = arg[0];
            string email = arg[1];
            string subject = arg[2];
            string message = arg[3];

            string recaptchaResponse = arg[4];
            string recaptchaChallenge = arg[5];

            recaptcha.UserUniqueIdentifier = hfCaptcha.Value;
            if (UseCaptcha)
            {
                if (!recaptcha.ValidateAsync(recaptchaResponse, recaptchaChallenge))
                {
                    _Callback = "RecaptchaIncorrect";
                    return;
                }
            }

            if (!IsSpam(subject, message) && SendEmail(email, name, subject, message))
            {
                _Callback = BlogSettings.Instance.ContactThankMessage;
            }
            else
            {
                _Callback = "This form does not work at the moment. Sorry for the inconvenience.";
            }
        }
        else
        {
            _Callback = "This form does not work at the moment. Sorry for the inconvenience.";
        }
    }

    #endregion

}