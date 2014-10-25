<%@ Page Title="Password Retrieval" Language="C#" MasterPageFile="~/Account/account.master" AutoEventWireup="true"
    CodeFile="password-retrieval.aspx.cs" Inherits="Account.PasswordRetrieval" %>

<%@ MasterType VirtualPath="~/Account/account.master" %>
<%@ Import Namespace="BlogEngine.Core" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="page-header clearfix">
        <h3>
            <%=Resources.labels.passwordRetrieval %></h3>
    </div>
    <p>
        <%=Resources.labels.passwordRetrievalInstructionMessage %>
    </p>
    <br />
    <div class="account-content">
        <div class="form-group">
            <asp:TextBox ID="txtEmail" runat="server" placeholder="someone@example.com" AutoCompleteType="None" CssClass="textEntry form-control "></asp:TextBox>
        </div>
        <div class="btn-wrapper text-right">
            <a href="<%= Utils.RelativeWebRoot %>Account/login.aspx" class="btn btn-default"><%=Resources.labels.cancel %></a>
            <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="<%$Resources:labels,send %>" CssClass="btn btn-primary" OnClick="LoginButton_Click" OnClientClick="return ValidatePasswordRetrieval()" />
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $("input[name$='txtEmail']").focus();
        });
    </script>
</asp:Content>
