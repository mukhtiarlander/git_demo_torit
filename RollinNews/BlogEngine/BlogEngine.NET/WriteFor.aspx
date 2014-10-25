<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WriteFor.aspx.cs" Inherits="writeFor" ValidateRequest="false" EnableEventValidation="false" %>

<%@ Import Namespace="BlogEngine.Core" %>

<asp:content id="Content1" contentplaceholderid="cphBody" runat="Server">

  <div id="contact" class="contact-page page-global">

      <div id="two-column-left" class="page_content">

                            
                            
								

	<header class="entry-header">
		<h1 class="entry-title">Write for Rollin News</h1>
	</header>

	<div class="entry-content">
		<p><strong>Are you interested in publishing your sports writing, and building your personal brand?</strong></p>
<p>Rollin News is a Roller Derby writing community built on passion and love for everything surrounding Roller Derby. If you bleed Roller Derby and have a love for sports broadcasting, journalism, or blogging your fellow fans want you to prove it.<strong><br>
</strong></p>
        
        <ul>
<li><strong><a href="<%=Utils.AbsoluteWebRoot %>write-for-rollin-news-faq/" target="_blank">Read the Facts about Our Writers</a></strong></li>
            <li><strong><a href="<%=Utils.AbsoluteWebRoot %>post/writing-for-rollin-news" target="_blank">Read how We Pay Our Writers</a></strong></li>

</ul>
        <br />
<p><strong>Editorial Process</strong></p>
<p>Rollin News’s editorial team, both in-house and remote, is responsible for reviewing all posts prior to publishing. All posts are checked for originality, sourcing, statistics and grammar. Sources must come in the form of a reputable publisher, website or individual.</p>
			</div>		
						</div>
      <br />
    <div id="divForm" runat="server">
      
      
        <div class="form-group">
          <label for="<%=txtName.ClientID %>"><%=Resources.labels.name %></label>
          <asp:TextBox runat="server" id="txtName" cssclass="field form-control" />
            <asp:requiredfieldvalidator runat="server" CssClass="required-field" controltovalidate="txtName" ErrorMessage="<%$Resources:labels, required %>" validationgroup="contact" />
        </div>
        <div class="form-group">
          <label for="<%=txtEmail.ClientID %>"><%=Resources.labels.email %></label> 
          <asp:TextBox runat="server" id="txtEmail" cssclass="field form-control" />
            <asp:requiredfieldvalidator runat="server"  CssClass="required-field" controltovalidate="txtEmail" ErrorMessage="<%$Resources:labels, required %>" validationgroup="contact" /> <asp:RegularExpressionValidator runat="server"   CssClass="required-field" ControlToValidate="txtEmail" display="dynamic" ErrorMessage="<%$Resources:labels, enterValidEmail %>" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" validationgroup="contact" />       
        </div>
        <div class="form-group">
          <label for="<%=txtSubject.ClientID %>"><%=Resources.labels.subject %></label> 
          <asp:TextBox runat="server" id="txtSubject" cssclass="field form-control" />
            <asp:requiredfieldvalidator runat="server"  CssClass="required-field" controltovalidate="txtSubject" ErrorMessage="<%$Resources:labels, required %>" validationgroup="contact" />
        </div>
        <div class="form-group">
          <label for="<%=txtMessage.ClientID %>"><%=Resources.labels.message %></label> 
          <asp:TextBox runat="server" id="txtMessage" textmode="multiline" cssclass="form-control" rows="5" columns="30" />
            <asp:requiredfieldvalidator runat="server"  CssClass="required-field" controltovalidate="txtMessage" ErrorMessage="<%$Resources:labels, required %>" display="dynamic" validationgroup="contact" />    
        </div>
       
      <blog:RecaptchaControl runat="server" ID="recaptcha" />
      <asp:HiddenField runat="server" ID="hfCaptcha" />
      <div class="text-right btn-wrapper">
        <asp:button runat="server" id="btnSend" class="btn btn-primary" Text="<%$Resources:labels, send %>" OnClientClick="return beginSendMessage();" validationgroup="contact" />    
        <asp:label runat="server" id="lblStatus" visible="false">This form does not work at the moment. Sorry for the inconvenience.</asp:label>
      </div>
    </div>
    <div id="thanks">
      <div id="divThank" runat="Server" visible="False">      
      <div class="">  <%=BlogSettings.Instance.ContactThankMessage %></div>
      </div>
    </div>
  </div>
  
  <script type="text/javascript">

     
      function beginSendMessage() {


          var recaptchaResponseField = document.getElementById('recaptcha_response_field');
          var recaptchaResponse = recaptchaResponseField ? recaptchaResponseField.value : "";

          var recaptchaChallengeField = document.getElementById('recaptcha_challenge_field');
          var recaptchaChallenge = recaptchaChallengeField ? recaptchaChallengeField.value : "";

          var name = BlogEngine.$('<%=txtName.ClientID %>').value;
          var email = BlogEngine.$('<%=txtEmail.ClientID %>').value;
          var subject = BlogEngine.$('<%=txtSubject.ClientID %>').value;
          
          var message = BlogEngine.$('<%=txtMessage.ClientID %>').value;
          if (BlogEngine.isValid(message)) {
              var sep = '-||-';
              var arg = name + sep + email + sep + subject + sep + message + sep + recaptchaResponse + sep + recaptchaChallenge;
              WebForm_DoCallback('__Page', arg, endSendMessage, 'contact', onSendError, false)

              BlogEngine.$('<%=btnSend.ClientID %>').disabled = true;
              return true;
          }
          else {
              alert("Please remove special characters from the form.");
          }
          return false;
      }

      function endSendMessage(arg, context) {

          if (arg == "RecaptchaIncorrect") {
              displayIncorrectCaptchaMessage();
              BlogEngine.$('<%=btnSend.ClientID %>').disabled = false;

              if (document.getElementById('recaptcha_response_field')) {
                  Recaptcha.reload();
              }
          }
          else {
              if (document.getElementById("spnCaptchaIncorrect")) document.getElementById("spnCaptchaIncorrect").style.display = "none";

              BlogEngine.$('<%=btnSend.ClientID %>').disabled = false;
              var form = BlogEngine.$('<%=divForm.ClientID %>')
              var thanks = BlogEngine.$('thanks');

              form.style.display = 'none';
              thanks.innerHTML = arg;
          }
      }

      function displayIncorrectCaptchaMessage() {
          if (document.getElementById("spnCaptchaIncorrect")) document.getElementById("spnCaptchaIncorrect").style.display = "";
      }

      function onSendError(err, context) {
          if (document.getElementById('recaptcha_response_field')) {
              Recaptcha.reload();
          }
          BlogEngine.$('<%=btnSend.ClientID %>').disabled = false;
          alert("Sorry, but the following occurred while attemping to send your message: " + err);
      }
  </script>
</asp:content>
