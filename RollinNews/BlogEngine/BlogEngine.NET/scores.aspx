<%@ Page Language="C#" AutoEventWireup="true" CodeFile="scores.aspx.cs" Inherits="scores" EnableViewState="false" %>

<asp:content id="Content1" contentplaceholderid="cphBody" runat="Server">
  <div id="archive" class="archive-page page-global">
      balh
    <h2 class="archive-page-title page-global-title"><%=Resources.labels.archive %></h2>
    <ul runat="server" id="ulMenu" class="archive-page-menu" />
    <div class="archive-page-content">
        <asp:placeholder runat="server" id="phArchive" />
    </div>
    <br />
    <div id="totals" class="archive-page-total">
      <h3><%=Resources.labels.total %></h3>
      <span><asp:literal runat="server" id="ltPosts" /></span><br />
      <asp:literal runat="server" id="ltComments" />
      <span><asp:literal runat="server" id="ltRaters" /></span>
    </div>
  </div>
</asp:content>
