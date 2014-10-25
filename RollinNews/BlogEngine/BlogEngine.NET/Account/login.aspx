<%@ Page Language="C#" MasterPageFile="account.master" AutoEventWireup="true" ClientIDMode="Static" CodeFile="login.aspx.cs" Inherits="Account.Login" %>

<%@ MasterType VirtualPath="~/Account/account.master" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:Login ID="LoginUser" runat="server" EnableViewState="false" RenderOuterTable="false" OnAuthenticate="LoginUser_OnAuthenticate">
        <LayoutTemplate>
            <div class="page-header clearfix">
                <h3>
                    <asp:Label runat="server" ID="lblTitle" Text="Login With RDNation Account" /></h3>
            </div>
            <div class="account-content">
                <div class="form-group">
                    <asp:TextBox ID="UserName" runat="server" AutoCompleteType="None" placeholder="username" CssClass="form-control input-lg textEntry ltr-dir"></asp:TextBox>
                </div>
                <div class="form-group">
                    <asp:TextBox ID="Password" runat="server" placeholder="*******" CssClass="form-control input-lg passwordEntry  ltr-dir" TextMode="Password"></asp:TextBox>
                </div>
                <div class="checkbox">
                    <asp:CheckBox ID="RememberMe" runat="server" />
                    <asp:Label ID="RememberMeLabel" runat="server" AssociatedControlID="RememberMe" CssClass="inline "><%=Resources.labels.rememberMe %></asp:Label>
                </div>
                <div class="btn-wrapper text-center">
                    <asp:Button ID="LoginButton" runat="server" CommandName="Login" CssClass="btn btn-primary btn-block btn-lg" Text="<%$Resources:labels,login %>" OnClientClick="return ValidateLogin();" />

                    <asp:PlaceHolder ID="phResetPassword" runat="server">
                        <hr />
                        <asp:HyperLink runat="server" ID="linkForgotPassword" CssClass="text-muted" Text="<%$ Resources:labels,forgotPassword %>" />
                    </asp:PlaceHolder>
                </div>
            </div>
        </LayoutTemplate>
    </asp:Login>

    <br />
    <a href="<%= RDN.Portable.Config.ServerConfig.WEBSITE_LOST_PASSWORD_RECOVER %>">Lost Password?</a> | 
    <a href="https://rdnation.com/SignUp?u=f&returnSite=rollinNews&ReturnUrl=<%= Request.Url%>">Register</a>

    <script type="text/javascript">
        $(document).ready(function () {
            $("input[name$='UserName']").focus();
        });
    </script>
</asp:Content>
