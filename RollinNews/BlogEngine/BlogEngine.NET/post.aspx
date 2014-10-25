<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="False" CodeFile="post.aspx.cs"  EnableViewState="false" Inherits="post" %>

<%@ Register Src="User controls/CommentView.ascx" TagName="CommentView" TagPrefix="uc" %>
<asp:content id="Content1" contentplaceholderid="cphBody" runat="Server">

 
 <div class="postViewContainer">
  <asp:placeholder runat="server" id="pwPost" />
     
  <asp:placeholder runat="server" id="phRelatedPosts" />
  
  <asp:placeholder runat="server" id="phRDF">

  </asp:placeholder>

     <div class="youMayLikeContainer">
              <div class="youMayLikeTitle">
If you liked that, you'll love this!
     </div>

              <% 
                  foreach (var p in YouMayLikePosts)
                  {  %>
                    <div class="youMayLikeItem" >
                        
                        <a href="<%= p.RelativeLink %>">
                            <div class="youMayLikeText">

                        <img src="<%= p.InitialImageUrl %>"" height="115" width="154" alt="<%= p.Title %>">
                        <span >
                            <%= p.Title %>
                        </span></div>
                         </a></div>
                  <% } %>
     <div class="clear"></div>        
          </div>

  
     <div class="clear"></div>
  
     <div  class="postAdContainer" >
                  <div class="PagePostContainer">
          
  <%var item1 = StoreItem.StoreItems[1];
    
        %>
                  <div class="frontPagePostTitle">
                        <a target="_blank" href="<%= RDN.Portable.Config.ServerConfig.WEBSITE_STORE_DEFAULT_LOCATION + "/roller-derby-item/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(item1.Name) + "/" + item1.StoreItemId%>"><%=item1.Name %></a>
                        </div>
                    <div class="PagePostImage">
                        <a target="_blank" href="<%= RDN.Portable.Config.ServerConfig.WEBSITE_STORE_DEFAULT_LOCATION + "/roller-derby-item/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(item1.Name) + "/" + item1.StoreItemId %>">
                        <%if (item1.Photos.FirstOrDefault() != null)
                          { 
                                   %>
                                <img src="<%= item1.Photos.FirstOrDefault().ImageUrl%>"  alt="<%=item1.Photos.FirstOrDefault().Alt %>" />
                            <%} %>
                            </a>
                    </div>

                <div class="storeItemPrice">
                        <% if (item1.Currency == "USD")
                           { %>
                        <span>$</span><%}%>
                        
                        <%= item1.Price.ToString("N2") %> <span class="usd"><%= item1.Currency %></span>
                    
                    |    <a target="_blank" href="<%= RDN.Portable.Config.ServerConfig.WEBSITE_STORE_DEFAULT_LOCATION + "/roller-derby-shop/" + item1.Store.MerchantId.ToString().Replace("-", "") + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(item1.Store.Name)%>"><%= item1.Store.Name%></a>
                    
                   
                     </div>
          
        
              
              </div>
<!-- YouMayLikeLeft -->
<ins class="adsbygoogle"
     style="display:inline-block;width:300px;height:250px; margin:0px auto;"
     data-ad-client="ca-pub-1376896373478670"
     data-ad-slot="6731653881"></ins>
<script>
    (adsbygoogle = window.adsbygoogle || []).push({});
</script>
     
         </div>
        <div class="clear"></div>
  
              <div id="aboutTheAuthor" class="postViewSideAuthorContainer">
          <% if (Member != null)
             { %>
          <h2 class="center">About The Author</h2>
          <% if (Member.Photos.Count > 0)
             {  %>
          <div  class="aboutAuthorPictureContainer">
              <% if (!String.IsNullOrEmpty(Member.Photos.FirstOrDefault().ImageThumbUrl))
                 {%>
          <img src="<%= Member.Photos.FirstOrDefault().ImageThumbUrl %>" />
              <%}
                 else if (!String.IsNullOrEmpty(Member.Photos.FirstOrDefault().ImageUrl))
                 {  %>
              <img src="<%= Member.Photos.FirstOrDefault().ImageUrl %>" />
              <%} %>
              </div>
          <%} %>
               <div class="authorBioInfoContainer">
          <div><span class="b">Name: </span><a href="<%= Member.DerbyNameUrl %>" target="_blank"><%= Member.DerbyName +" - "+ Member.PlayerNumber%></a> </div>
          <% if (Member.DOB != new DateTime())
             { %>
          <div><span class="b">DOB: </span><%= Member.DOB.ToShortDateString() %></div>
          <%} %>
          <% if (Member.Leagues.Count > 0)
             {  %>
          <span class="b">Leagues: </span>
          <% foreach (var league in Member.Leagues)
             { %>
          
          <div><a href="<%= league.NameUrl %>" target="_blank"><%= league.Name%></a></div>
          <%}
             }%>

          <span class="b">Bio: </span>
          <div><%= Member.Bio %></div>
          
          <%} %>
                   </div>
          <br />
          <div class="clear"></div>
      </div>

     
     <uc:CommentView ID="CommentView1" runat="server" />
    
  </div>
  

     <div class="postViewSideContainer">
<!-- RN Article Side 2 -->
<ins class="adsbygoogle"
     style="display:inline-block;width:300px;height:250px"
     data-ad-client="ca-pub-1376896373478670"
     data-ad-slot="7209883885"></ins>
<script>
    (adsbygoogle = window.adsbygoogle || []).push({});
</script>

              <div class="PagePostContainer">
          
  <%var item = StoreItem.StoreItems[0];
    
        %>
                  <div class="frontPagePostTitle">
                        <a target="_blank" href="<%= RDN.Portable.Config.ServerConfig.WEBSITE_STORE_DEFAULT_LOCATION + "/roller-derby-item/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(item.Name) + "/" + item.StoreItemId%>"><%=item.Name %></a>
                        </div>
                    <div class="PagePostImage">
                        <a target="_blank" href="<%= RDN.Portable.Config.ServerConfig.WEBSITE_STORE_DEFAULT_LOCATION + "/roller-derby-item/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(item.Name) + "/" + item.StoreItemId %>">
                        <%if (item.Photos.FirstOrDefault() != null)
                          { 
                                   %>
                                <img src="<%= item.Photos.FirstOrDefault().ImageUrl%>"  alt="<%=item.Photos.FirstOrDefault().Alt %>" />
                            <%} %>
                            </a>
                    </div>

                <div class="storeItemPrice">
                        <% if (item.Currency == "USD")
                           { %>
                        <span>$</span><%}%>
                        
                        <%= item.Price.ToString("N2") %> <span class="usd"><%= item.Currency %></span>
                    
                    |    <a target="_blank" href="<%= RDN.Portable.Config.ServerConfig.WEBSITE_STORE_DEFAULT_LOCATION + "/roller-derby-shop/" + item.Store.MerchantId.ToString().Replace("-", "") + "/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(item.Store.Name)%>"><%= item.Store.Name%></a>
                    
                   
                     </div>
          
        
              
              </div>
               <ins class="adsbygoogle"
     style="display:inline-block;width:300px;height:250px"
     data-ad-client="ca-pub-1376896373478670"
     data-ad-slot="4396018289"></ins>
<script>
    (adsbygoogle = window.adsbygoogle || []).push({});
</script>
        <div><div class="derbyTodayTitle">DERBY TODAY</div>
              <ul class="derbyToday">
              <% 
                  int i = 0;
                  foreach (var p in RecommendedPosts)
                  {  %>
                    <li >
                        
                        <a href="<%= p.RelativeLink %>">
                            <div class="sideText">

                        <img src="<%= p.InitialImageUrl %>" height="115" width="154" alt="<%= p.Title %>">
                        <span >
                            <%= p.Title %>
                        </span></div>
                         </a></li>
                  <% if (i == 11)
                     { %>
              <li>
         <!-- RN Right Side 3 -->
<ins class="adsbygoogle"
     style="display:inline-block;width:300px;height:250px"
     data-ad-client="ca-pub-1376896373478670"
     data-ad-slot="5453949087"></ins>
<script>
    (adsbygoogle = window.adsbygoogle || []).push({});
</script></li>
                  <% }
                     else if (i == 23)
                     { 
                     %>
                  <!-- RN Large Left Side 1 -->
<ins class="adsbygoogle"
     style="display:inline-block;width:300px;height:250px"
     data-ad-client="ca-pub-1376896373478670"
     data-ad-slot="1302951083"></ins>
<script>
    (adsbygoogle = window.adsbygoogle || []).push({});
</script>
                  <%
                     }
                     i += 1;
                  } %>
                  </ul>
          </div>
      <div class="postViewSideContainerInner">

        


          </div>
  </div>
</asp:content>
