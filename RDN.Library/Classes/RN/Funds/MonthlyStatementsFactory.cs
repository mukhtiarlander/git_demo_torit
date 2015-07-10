using RDN.Library.Cache;
using RDN.Library.Classes.Config;
using RDN.Library.Classes.EmailServer;
using RDN.Library.Classes.RN.Funds.Monthly;
using RDN.Library.DataModels.EmailServer.Enums;
using RDN.Library.DataModels.RN.Financials;
using RDN.Portable.Config;
using RN.Library.DataModels.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.RN.Funds
{
    public class MonthlyStatementsFactory
    {
        private List<TotalPayment> TotalPayments { get; set; }
        private List<Statement> Statements { get; set; }
        private double PAYOUTPERCENTAGETOAUTHORS = .60;
        private Statement Statement { get; set; }
        private List<Posts.Classes.Post> PostsToPay { get; set; }
        private List<Posts.Classes.Post> PostsForAll { get; set; }
        public MonthlyStatementsFactory Initialize(double totalProfitMade, DateTime date)
        {
            Statement = new Statement();
            Statement.TotalProfitMade = totalProfitMade;
            Statement.StatementDateTime = date;
            return this;
        }
        public MonthlyStatementsFactory GetPostsThatHadViews()
        {
            var dc = new RNManagementContext();
            PostsToPay = (from xx in dc.Posts
                          where xx.CurrentMonthlyViews > 0
                          select new Posts.Classes.Post
                          {
                              Id = xx.PostId,
                              TotalMonthlyViews = xx.CurrentMonthlyViews,
                              TotalViews = xx.TotalViews,
                              DisablePaymentsForPost = xx.DisablePaymentsForPost
                          }).ToList();

            return this;
        }
        public MonthlyStatementsFactory ZeroOutMonthlyViews()
        {
            var dc = new RNManagementContext();
            foreach (var p in PostsForAll)
            {
                var post = dc.Posts.Where(x => x.PostId == p.Id).FirstOrDefault();
                if (post != null)
                {
                    post.CurrentMonthlyViews = 0;
                }
            }
            int c = dc.SaveChanges();

            return this;
        }
        public MonthlyStatementsFactory SetAuthorsForPosts(List<Posts.Classes.Post> posts)
        {
            PostsForAll = posts;
            //sets the userid of each post with views.
            foreach (var item in PostsToPay)
            {
                var p = posts.Where(x => x.Id == item.Id).FirstOrDefault();
                if (p != null)
                    item.AuthorUserId = p.AuthorUserId;
            }
            return this;
        }
        public MonthlyStatementsFactory CalculateAndSavePayouts()
        {

            double authorsCut = Statement.TotalProfitMade * PAYOUTPERCENTAGETOAUTHORS;

            //removes SpoiledTechies Posts cause he doesn't want to get paid.
            //removes posts that aren't allowed to pay, mainly sponsors.
            var allPosts = PostsToPay.Where(x => x.DisablePaymentsForPost == false).Where(x => x.AuthorUserId == LibraryConfig.DEFAULT_JAMIES_USER_ID || x.AuthorUserId == LibraryConfig.DEFAULT_SCOTTS_USER_ID || x.AuthorUserId == LibraryConfig.DEFAULT_ADMIN_USER_ID).ToList();
            foreach (var post in allPosts)
            {
                PostsToPay.Remove(post);
            }


            //gets the total views of all the posts in this month.
            double totalViews = PostsToPay.Sum(x => x.TotalMonthlyViews);
            //caculates the post percentages.
            foreach (var post in PostsToPay)
            {
                post.PercentageOfTotalViews = (double)post.TotalMonthlyViews / totalViews;
            }
            TotalPayments = (from p in PostsToPay
                             group p by p.AuthorUserId into g
                             select new TotalPayment
                             {
                                 UserId = g.Key,
                                 TotalPercentageBeingPaid = g.Sum(x => x.PercentageOfTotalViews),
                                 TotalPageViewsThisMonth = g.Sum(x => x.TotalMonthlyViews)
                             }).ToList();

            var dc = new RNManagementContext();
            MonthlyStatement newMonth = new MonthlyStatement();
            newMonth.StatementDateTime = DateTime.UtcNow;
            newMonth.TotalPageViews = (long)totalViews;
            newMonth.TotalProfitMade = Statement.TotalProfitMade;
            newMonth.TotalWritersPaid = TotalPayments.Count;
            newMonth.TotalWritersPayoutProfit = authorsCut;
            dc.MonthlyStatements.Add(newMonth);


            foreach (var group in TotalPayments)
            {
                //add the funds to the account.
                var fund = dc.Funds.Where(x => x.UserId == group.UserId).FirstOrDefault();
                group.TotalAddedToAccount = (group.TotalPercentageBeingPaid * authorsCut);
                if (fund != null)
                {
                    fund.ActiveInUserAccount += group.TotalAddedToAccount;
                    group.TotalActiveInAccount = fund.ActiveInUserAccount;

                }
                else
                {
                    FundsForWriter f = new FundsForWriter();
                    f.ActiveInUserAccount = group.TotalAddedToAccount;
                    f.UserId = group.UserId;
                    dc.Funds.Add(f);
                    group.TotalActiveInAccount = f.ActiveInUserAccount;

                }
            }
            int c = dc.SaveChanges();

            return this;
        }
        public MonthlyStatementsFactory SendEmailsToAuthorsAboutPayouts()
        {
            for (int i = 0; i < TotalPayments.Count; i++)
            {
                var member = SiteCache.GetPublicMemberFullWithUserId(TotalPayments[i].UserId);
                if (member != null)
                {
                    var emailData = new Dictionary<string, string> { 
                
                { "totalActive", TotalPayments[i].TotalActiveInAccount.ToString("N2")} ,
                { "totalAdded", TotalPayments[i].TotalAddedToAccount.ToString("N2") },
                { "totalPageViews", TotalPayments[i].TotalPageViewsThisMonth.ToString("N0")},
                { "link", RollinNewsConfig.WEBSITE_DEFAULT_LOCATION }
                };
                    if (!String.IsNullOrEmpty(member.DerbyName))
                        emailData.Add("name", member.DerbyName);
                    else
                        emailData.Add("name", member.Firstname + " " + member.LastName);
                    EmailServer.EmailServer.SendEmail(RollinNewsConfig.DEFAULT_EMAIL, RollinNewsConfig.DEFAULT_EMAIL_FROM_NAME, member.UserName, LibraryConfig.DefaultEmailSubject + " You just received more money!", emailData, EmailServerLayoutsEnum.RNMoneyAddedToAccount, EmailPriority.Normal);
                    EmailServer.EmailServer.SendEmail(RollinNewsConfig.DEFAULT_EMAIL, RollinNewsConfig.DEFAULT_EMAIL_FROM_NAME, LibraryConfig.DefaultAdminEmail, LibraryConfig.DefaultEmailSubject + " You just received more money!", emailData, EmailServerLayoutsEnum.RNMoneyAddedToAccount, EmailPriority.Normal);
                }
            }


            return this;
        }

        public bool AddNewStatement(Statement statement)
        {
            var dc = new RNManagementContext();
            MonthlyStatement s = new MonthlyStatement();
            s.StatementDateTime = DateTime.UtcNow;
            s.TotalPageViews = statement.TotalPageViews;
            s.TotalProfitMade = statement.TotalProfitMade;
            s.TotalWritersPaid = statement.TotalWritersPaid;
            s.TotalWritersPayoutProfit = statement.TotalWritersPayoutProfit;
            dc.MonthlyStatements.Add(s);
            int c = dc.SaveChanges();
            return c > 0;
        }
        public List<Statement> GetLatestStatements(int page, int count)
        {
            var dc = new RNManagementContext();
            return (from xx in dc.MonthlyStatements
                    select new Statement
                    {
                        StatementDateTime = xx.StatementDateTime,
                        StatementId = xx.StatementId,
                        TotalPageViews = xx.TotalPageViews,
                        TotalProfitMade = xx.TotalProfitMade,
                        TotalWritersPaid = xx.TotalWritersPaid,
                        TotalWritersPayoutProfit = xx.TotalWritersPayoutProfit
                    }).Skip(count * page).Take(count).ToList();
        }
    }
}
