using BlogEngine.Core;
using BlogEngine.Core.Data;
using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.RN.Funds;
using RDN.Portable.Classes.Controls.Dues.Enums;
using RDN.Portable.Config;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;

public class FundsController : ApiController
{
    readonly IFundsRepository repository;

    public FundsController(IFundsRepository repository)
    {
        this.repository = repository;
    }

    public Fund Get()
    {
        try
        {
            if (Security.IsAuthenticated)
                return repository.Get();
            else
                return null;
        }
        catch (UnauthorizedAccessException)
        {
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, GetType());
            throw new HttpResponseException(HttpStatusCode.InternalServerError);
        }
    }
    public HttpResponseMessage Put([FromBody]Fund funds, string action)
    {
        try
        {
            //paypal Withdraw
            if (action == "withdrawPP")
            {

                var user = Membership.GetUser(Security.CurrentUser.Identity.Name);
                var localFunds = Fund.GetCurrentFundsInformation((Guid)user.ProviderUserKey);
                if (localFunds.ActiveInUserAccount >= funds.AmountToWithdraw)
                {


                    var mem = SiteCache.GetPublicMemberFull(RDN.Library.Classes.Account.User.GetMemberId());
                    PaymentGateway pg = new PaymentGateway();

                    var f = pg.StartInvoiceWizard()
                        .Initalize(RollinNewsConfig.MERCHANT_ID, "USD", PaymentProvider.Paypal, PaymentMode.Test, ChargeTypeEnum.RollinNewsWriterPrePayout)
                        .SetInvoiceId(Guid.NewGuid())
                        .AddWriterPayout(new RDN.Library.Classes.Payment.Classes.Invoice.InvoiceWriterPayout
                        {
                            BasePrice = (decimal)funds.AmountToWithdraw,
                            Name = "Rollin News Payout " + DateTime.UtcNow.ToString("yyyy/MM/dd"),
                            PaymentRequestedDateTime = DateTime.UtcNow,
                            PayoutId = Guid.NewGuid(),
                            UserPaidId = (Guid)user.ProviderUserKey,
                            WhoPaysFees = WhoPaysProcessorFeesEnum.Receiver
                        })
                        .SetInvoiceContactData(new RDN.Library.Classes.Payment.Classes.Invoice.InvoiceContactInfo
                        {
                            Email = user.Email,
                            FirstName = mem.Firstname,
                            LastName = mem.LastName,
                            Phone = mem.PhoneNumber,
                        })
                                            .FinalizeInvoice();
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable);
                }
            }
            //bitcoinWithdraw
            else if (action == "withdrawBc")
            {

            }
            else if (action == "submitEarnings")
            {
                Security.DemandUserHasRight(Rights.AllowedToSetMonthlyEarnings, false);
                PostRepository repo = new PostRepository();
                var posts = repo.GetPostsAndAuthors();
                List<RDN.Library.Classes.RN.Posts.Classes.Post> PostsTemp = new List<RDN.Library.Classes.RN.Posts.Classes.Post>();
                foreach (var p in posts)
                {
                    try
                    {
                        RDN.Library.Classes.RN.Posts.Classes.Post postTemp = new RDN.Library.Classes.RN.Posts.Classes.Post();
                        postTemp.Id = p.Id;
                        postTemp.AuthorUserId = p.AuthorUserId;
                        PostsTemp.Add(postTemp);
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, GetType());
                    }
                }
                MonthlyStatementsFactory statement = new MonthlyStatementsFactory();
                var state = statement.Initialize(Convert.ToDouble(funds.TotalEarningsForMonth), DateTime.UtcNow)
                    .GetPostsThatHadViews()
                    .SetAuthorsForPosts(PostsTemp)
                    .CalculateAndSavePayouts()
                    .SendEmailsToAuthorsAboutPayouts()
                    .ZeroOutMonthlyViews();


            }
            else
            {
                repository.Update(funds);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        catch (UnauthorizedAccessException)
        {
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, GetType());
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
}
