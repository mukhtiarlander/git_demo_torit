using RDN.Library.Classes.EmailServer;
using RDN.Library.Classes.Social.Enums;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Social;
using RDN.Portable.Config;
using RN.Library.DataModels.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace RDN.Library.Classes.Social.Facebook
{
    public class FacebookData
    {

        public static void ResetFacebookConnection()
        {
            var users = Roles.GetUsersInRole("Chiefs");
            foreach (var user in users)
            {
                var id = RDN.Library.Classes.Account.User.GetMemberId(user);
                var member = RDN.Library.Cache.SiteCache.GetPublicMember(id);
                if (member != null)
                {
                    var emailData = new Dictionary<string, string>
                                            {
                   
                                            { "body","Please Refresh Facebook Connection by clicking on link:http://rollinnews.com/admin/#/social"}
                                              };

                    EmailServer.EmailServer.SendEmail(RollinNewsConfig.DEFAULT_EMAIL, RollinNewsConfig.DEFAULT_EMAIL_FROM_NAME, user, EmailServer.EmailServer.DEFAULT_SUBJECT_ROLLIN_NEWS + " Reset Facebook Connection", emailData, EmailServerLayoutsEnum.Blank);
                }
            }
        }

        public static string GetLatestAccessToken()
        {
            var dc = new ManagementContext();
            long id = (long)SiteTypeEnum.Facebook;
            var key = dc.SocialAuthKeys.Where(x => x.SocialSiteType == id).OrderByDescending(x => x.AuthId).FirstOrDefault();

            if (key == null)
            {
                return string.Empty;
            }
            return key.LatestAuthToken;
        }

        public static bool InsertNewAccessToken(string token, DateTime expires)
        {
            var dc = new ManagementContext();
            long id = (long)SiteTypeEnum.Facebook;
            SocialAuthKeys key = new SocialAuthKeys();
            key.LatestAuthToken = token;
            key.SocialSiteType = id;
            if (expires != new DateTime())
                key.Expires = expires;
            else
                key.Expires = DateTime.UtcNow.AddDays(5);

            dc.SocialAuthKeys.Add(key);
            int c = dc.SaveChanges();
            return c > 0;

        }

        public static Guid GetRandomPostToSocialize(List<Guid> published, DateTime dateAfter)
        {
            var dc = new RNManagementContext();

            var key = (from xx in dc.Posts
                       where xx.Created > dateAfter
                       where xx.DisabledAutoPosting == false
                       where published.Contains(xx.PostId)
                       select xx).ToList();

            var post = key.OrderBy(x => x.LastTimePostedToFacebook).FirstOrDefault();
            post.LastTimePostedToFacebook = DateTime.UtcNow;
            post.TotalFacebookPosts += 1;
            int c = dc.SaveChanges();
            return post.PostId;
        }

        public static Guid RecordNewFBShare(Guid postId)
        {
            var dc = new RNManagementContext();

            var key = (from xx in dc.Posts
                       where xx.PostId == postId
                       select xx).FirstOrDefault();

            key.LastTimePostedToFacebook = DateTime.UtcNow;
            key.TotalFacebookPosts += 1;
            int c = dc.SaveChanges();
            return key.PostId;
        }

    }
}
