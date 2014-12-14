<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="_default" %>


<asp:content id="Content1" contentplaceholderid="cphBody" runat="Server">
    <ul id="webticker" >	
        <% int t = 1;
           if (Tweets != null) { 
           foreach (var item in Tweets)
           {
               if (item != null)
               { 
                %>
    <li id='item<%= t%>'>
		<%= item.TextAsHtml%>
	</li>   
         <%
                   t += 1;
               }
           } }%>
	
</ul>
    <%---  
    <div class="scoresAndComing">
        <div >
            <div class="scoresAndComingTitle">#derbyscores</div>
            <div class="scoresAndComingContent">
            <ul>
            <% foreach (var game in this.Games)
               {  %>
            <li>
                <% if (game.GameLocationFrom == RDN.Portable.Models.Json.Games.Enums.GameLocationFromEnum.SCOREBOARD)
                   {  %>
             <a href="<%=game.GameUrl %>" target="_blank"><%= game.StartTime.ToString("M/d") %>: <%= game.Team1Name %> <%= game.Team1Score %> - <%= game.Team2Score%> <%= game.Team2Name%></a>
                   <%}
                   else
                   {  %>
             <%= game.StartTime.ToString("M/d") %>: <%= game.Team1Name %> <%= game.Team1Score %> - <%= game.Team2Score %> <%= game.Team2Name %>
                <%} %>
            </li>
            
            <%} %>
                </ul>
                </div>
            </div>

    </div>--%>
    <div id="sliderFrame">
    <div id="slider2">
        <ul>
        <% foreach (var post in this.Posts)
           {  %>
            <li>
                <a   href="<%= post.RelativeLink %>"><img  src="<%= post.MainImageUrl %>" width="100%" alt="<%= post.Title %>"  /></a>
            </li>
                <%} %>
            </ul>
    </div>
</div>
    <div class="frontPageColumn1">
          <% int i = 0;
             foreach (var post in this.PostsColumn1)
             {  %>
         <% if (i == 0)
            {  %>
        <div class="frontPagePostContainer">
          <div class="center">
<%--<!-- RN Large Left Side 1 -->
<ins class="adsbygoogle"
     style="display:inline-block;width:300px;height:250px"
     data-ad-client="ca-pub-1376896373478670"
     data-ad-slot="1302951083"></ins>
<script>
    (adsbygoogle = window.adsbygoogle || []).push({});
</script>--%>
              <div class="adsbyathleticads" style="display: inline-block; width: 300px; height: 250px" data-ad-client="66ae111b-0858-e411-a349-00155db10512" data-ad-slot="1"></div>
              </div>
            </div>
        <%} %>
           <% if (i == 11)
              {  %>
        <div class="frontPagePostContainer">
          <div class="center">
<!-- RN Large Left Side 2 -->
<ins class="adsbygoogle"
     style="display:inline-block;width:300px;height:250px"
     data-ad-client="ca-pub-1376896373478670"
     data-ad-slot="4256417483"></ins>
<script>
    (adsbygoogle = window.adsbygoogle || []).push({});
</script>
              </div>
            </div>
        <%} %>
            <% if (i == 17)
               {  %>
        <div class="frontPagePostContainer">
          <div class="center">
<!-- RN Left Side 3 -->
<ins class="adsbygoogle"
     style="display:inline-block;width:300px;height:250px"
     data-ad-client="ca-pub-1376896373478670"
     data-ad-slot="6930682284"></ins>
<script>
    (adsbygoogle = window.adsbygoogle || []).push({});
</script>
              </div></div>
        <%} %>
     
   <% if (i == 1 && this.SkaterOfWeek != null)
      {  %>
    <div class="PlayerProfileFrontPageContainer">
        <div class="frontPageProfileTitle">Featured Member <a class="featuredLink"  href="http://rollinnews.com/post/want-to-be-featured">?</a></div> 
        <div class="PlayerProfileFrontPagePersonContainer">
           <div class="PlayerProfileFrontPagePersonName"><a href="<%= this.SkaterOfWeek.DerbyNameUrl %>" target="_blank"><%= this.SkaterOfWeek.DerbyNumber +" "+ this.SkaterOfWeek.DerbyName %></a></div>
            <div class="PlayerProfileFrontPageImgContainer"><a href="<%= this.SkaterOfWeek.DerbyNameUrl %>" target="_blank"><img src="<%= this.SkaterOfWeek.photoUrl %>" /></a></div>
        <div>
            <table class="PlayerProfileFrontPageStatsTable">
                <tr>
                    <td colspan="2" class="PlayerProfileFrontPageLeagueName"><a href="<%= this.SkaterOfWeek.LeagueUrl %>" target="_blank"><%= this.SkaterOfWeek.LeagueName %></a></td>
                </tr>
                <tr><td class="PlayerProfileFrontPageStatsTitle">Games: </td>
                    <td class="PlayerProfileFrontPageStatsDetail"><%= this.SkaterOfWeek.GamesCount %></td>
                </tr>
                                <tr><td class="PlayerProfileFrontPageStatsTitle">Wins: </td>
                    <td class="PlayerProfileFrontPageStatsDetail"><%= this.SkaterOfWeek.Wins %></td>
                </tr>
                                <tr><td class="PlayerProfileFrontPageStatsTitle">Losses: </td>
                    <td class="PlayerProfileFrontPageStatsDetail"><%= this.SkaterOfWeek.Losses %></td>
                </tr>
                           </table>
            <div class="PlayerProfileFrontPageBio"><%= this.SkaterOfWeek.Bio%></div>
        </div>
        </div>
    </div>
    <% } %>
    
                <div class="frontPagePostContainer">
                    <div class="frontPagePostTitle"><a href="<%= post.RelativeLink %>"><%= post.Title %></a></div>
                    <div class="frontPagePostImage">
                        <a href="<%= post.RelativeLink %>"><img src="<%= post.InitialImageUrl %>" /></a>
                    </div>

                    <div>
                        Comments (<%= post.Comments[1] %>) | <% if (post.Categories.FirstOrDefault() != null)
                                                                {%>         
                        <span> <%= post.Categories.FirstOrDefault().Title %></span>
                        <% }%>
                    </div>
                </div>


                <% i += 1;
             } %>

    </div>
      <div class="frontPageColumn2">
            <% 
                int k = 0;
                foreach (var post in this.PostsColumn2)
                {  %>
      <% if (k == 3)
         {  %>
        <div class="frontPagePostContainer">
          
  <%var item = StoreItem.StoreItems[0];
    
        %>
                  <div class="frontPagePostTitle">
                        <a target="_blank" href="<%= RDN.Portable.Config.ServerConfig.WEBSITE_STORE_DEFAULT_LOCATION + "/roller-derby-item/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(item.Name) + "/" + item.StoreItemId%>"><%=item.Name %></a>
                        </div>
                    <div class="frontPagePostImage">
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
        <%} %>
    <% if (k == 9)
       {  %>
        <div class="frontPagePostContainer">
          
  <%var item = StoreItem.StoreItems[1];
    
        %>
                  <div class="frontPagePostTitle">
                        <a target="_blank" href="<%= RDN.Portable.Config.ServerConfig.WEBSITE_STORE_DEFAULT_LOCATION + "/roller-derby-item/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(item.Name) + "/" + item.StoreItemId%>"><%=item.Name %></a>
                        </div>
                    <div class="frontPagePostImage">
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
        <%} %>
    <% if (k == 18)
       {  %>
        <div class="frontPagePostContainer">
          
  <%var item = StoreItem.StoreItems[2];
    
        %>
                  <div class="frontPagePostTitle">
                        <a target="_blank" href="<%= RDN.Portable.Config.ServerConfig.WEBSITE_STORE_DEFAULT_LOCATION + "/roller-derby-item/" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(item.Name) + "/" + item.StoreItemId%>"><%=item.Name %></a>
                        </div>
                    <div class="frontPagePostImage">
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
        <%} %>
    
                <div class="frontPagePostContainer">
                    <div class="frontPagePostTitle"><a href="<%= post.RelativeLink %>"><%= post.Title %></a></div>
                    <div class="frontPagePostImage">
                        <a href="<%= post.RelativeLink %>"><img src="<%= post.InitialImageUrl %>" /></a>
                    </div>

                    <div>
                        Comments (<%= post.Comments[1] %>) | <% if (post.Categories.FirstOrDefault() != null)
                                                                {%>         
                        <span>  <%= post.Categories.FirstOrDefault().Title %></span>
                        <% }%>
                    </div>
                </div>


                <% 
                                                                k += 1;
                } %>

    </div>
      <div class="frontPageColumn3">
            <% int j = 0;
               foreach (var post in this.PostsColumn3)
               {  %>
      <% if (j == 0)
         {  %>
          <div class="frontPagePostContainer">
          <div class="center">
<!-- RN Right Side 1 Large -->
<ins class="adsbygoogle"
     style="display:inline-block;width:300px;height:600px"
     data-ad-client="ca-pub-1376896373478670"
     data-ad-slot="5872751489"></ins>
<script>
    (adsbygoogle = window.adsbygoogle || []).push({});
</script>
              </div>
              </div>
        <%} %>
          <% if (j == 9)
             {  %>
          <div class="frontPagePostContainer">
       <div class="center">
<!-- RN Right Side 2 -->
<ins class="adsbygoogle"
     style="display:inline-block;width:300px;height:250px"
     data-ad-client="ca-pub-1376896373478670"
     data-ad-slot="4396018289"></ins>
<script>
    (adsbygoogle = window.adsbygoogle || []).push({});
</script>
           </div>
              </div>
          <%} %>
           <%-- <% if (j == 18)
               {  %>
          <div class="frontPagePostContainer">
       <div class="center">
<!-- RN Right Side 3 -->
<ins class="adsbygoogle"
     style="display:inline-block;width:300px;height:250px"
     data-ad-client="ca-pub-1376896373478670"
     data-ad-slot="5453949087"></ins>
<script>
    (adsbygoogle = window.adsbygoogle || []).push({});
</script>
           </div>
              </div>
          <%} %>--%>
      <% if (j == 1 && this.LeagueOfWeek != null)
         {  %>
    <div class="LeagueProfileFrontPageContainer">
        <div class="frontPageProfileTitle">Featured League <a class="featuredLink"  href="http://rollinnews.com/post/want-to-be-featured">?</a></div> 
             <div class="LeagueProfileFrontPageLeagueContainer">
           
            <div class="LeagueProfileFrontPageImgContainer"><a href="<%= RDN.Portable.Config.ServerConfig.WEBSITE_DEFAULT_LOCATION_FOR_LEAGUES +  RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly( this.LeagueOfWeek.Name ) +"/"+this.LeagueOfWeek.LeagueId.ToString().Replace("-","")%>" target="_blank"><img src="<%= this.LeagueOfWeek.Logo.ImageUrl %>" /></a></div>
       
                 <div class="LeagueProfileFrontPageName"><a href="<%= RDN.Portable.Config.ServerConfig.WEBSITE_DEFAULT_LOCATION_FOR_LEAGUES +  RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly( this.LeagueOfWeek.Name ) +"/"+this.LeagueOfWeek.LeagueId.ToString().Replace("-","")%>" target="_blank"><%=  this.LeagueOfWeek.Name%></a></div>
       <div class="LeagueProfileFrontPageLocation"><%=  this.LeagueOfWeek.City +", " + this.LeagueOfWeek.State %></div>
                            <div>
              </div>
            <table class="LeagueProfileFrontPageStatsTable">
                <% if (!String.IsNullOrEmpty(this.LeagueOfWeek.Website))
                   {  %>
                <tr>
                    <td class="LeagueProfileFrontPageStatsDetail"><a href="<%= this.LeagueOfWeek.Website %>" target="_blank"><img src="<%= this.LeagueOfWeek.Logo.ImageThumbUrl %>" /> Website</a></td>
                </tr>
                <%} %>
                 <% if (!String.IsNullOrEmpty(this.LeagueOfWeek.Facebook))
                    {  %>
                                <tr>
                    <td class="LeagueProfileFrontPageStatsDetail"><a href="<%= this.LeagueOfWeek.Facebook %>" target="_blank"><img src="<%=Page.ResolveUrl("~/Content/images/icons/facebook.png")  %>" /> Facebook</a></td>
                </tr>
                <%} %>
                 <% if (!String.IsNullOrEmpty(this.LeagueOfWeek.Twitter))
                    {  %>
                                <tr>
                    <td class="LeagueProfileFrontPageStatsDetail"><a href="<%= this.LeagueOfWeek.Twitter %>" target="_blank"><img src="<%=Page.ResolveUrl("~/Content/images/icons/twitter.png")  %>" /> Twitter</a></td>
                </tr>
                <%} %>
                 <% if (!String.IsNullOrEmpty(this.LeagueOfWeek.Instagram))
                    {  %>
                           <tr>
                    <td class="LeagueProfileFrontPageStatsDetail"><a href="<%= this.LeagueOfWeek.Instagram %>" target="_blank"><img src="<%=Page.ResolveUrl("~/Content/images/icons/instagram.png")  %>" /> Instagram</a></td>
                </tr>
                <%} %>
                 <% if (!String.IsNullOrEmpty(this.LeagueOfWeek.ShopUrl))
                    {  %>
                           <tr>
                    <td class="LeagueProfileFrontPageStatsDetail"><a href="<%= this.LeagueOfWeek.ShopUrl %>" target="_blank"><img src="<%=Page.ResolveUrl("~/Content/images/icons/shop.png")  %>" /> Merchandise</a></td>
                </tr>
                <%} %>
                           </table>
            
        </div>
      
    </div>
    <% } %>
                <div class="frontPagePostContainer">
                    <div class="frontPagePostTitle"><a href="<%= post.RelativeLink %>"><%= post.Title %></a></div>
                    <div class="frontPagePostImage">
                        <a href="<%= post.RelativeLink %>"><img src="<%= post.InitialImageUrl %>" /></a>
                    </div>

                    <div>
                        Comments (<%= post.Comments[1] %>) | <% if (post.Categories.FirstOrDefault() != null)
                                                                {%>         
                        <span> <%= post.Categories.FirstOrDefault().Title %></span>
                        <% }%>
                    </div>
                </div>


                <% j += 1;
               } %>

    </div>

<script type="text/ecmascript">
    //    var sliderOptions =
    //{
    //    sliderId: "slider",
    //    startSlide: 0,
    //    effect: "9",
    //    effectRandom: false,
    //    pauseTime: 3500,
    //    transitionTime: 500,
    //    slices: 12,
    //    boxes: 5,
    //    hoverPause: 1,
    //    autoAdvance: true,
    //    captionOpacity: 0.8,
    //    captionEffect: "fade",
    //    thumbnailsWrapperId: "thumbs",
    //    m: false,
    //    license: "b6t80"
    //};

    //    var imageSlider = new mcImgSlider(sliderOptions);
    $(document).ready(function () {

        $("#slider2").easySlider({
            auto: true,
            continuous: true,
            speed:3500
        });
    });


    $("#webticker").webTicker({ speed: 50, startEmpty: false });
</script>
  
</asp:content>
