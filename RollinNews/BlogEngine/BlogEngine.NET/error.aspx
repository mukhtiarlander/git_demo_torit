<%@ Page Language="C#" AutoEventWireup="true" CodeFile="error.aspx.cs" Inherits="error_occurred" %>

<asp:content id="Content1" contentplaceholderid="cphBody" runat="Server">
  <div class="post page-global">
    <h2 class="page-global-title">Ooops! An unexpected error has occurred.</h2>
        <div>
      <p>
        This one's down to me! Please accept my apologies for this - I'll see to it
        that the developer responsible for this happening is given 20 lashes 
        (but only after he or she has fixed this problem).
      </p>
    </div>
        <div id="divErrorDetails" runat="server" visible="false">
        <h2>Error Details:</h2>
        <p id="pDetails" runat="server"></p>
    </div>
    
  </div>
</asp:content>
