using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Store;
using RDN.Library.Util;

namespace RDN.Store.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            CartCount(HttpContext.Session);
        }
        protected void AddItemToCart(HttpSessionStateBase session, int cartItems)
        {
            if (session["cartCount"] == null || string.IsNullOrEmpty(session["cartCount"].ToString()))
            {
                session["cartCount"] = cartItems;
            }
            else
            {
                int count = Convert.ToInt32(session["cartCount"].ToString());
                count += cartItems;
                session["cartCount"] = count;
            }

        }
        protected void CartCount(HttpSessionStateBase session)
        {
            if (session["cartCount"] == null || string.IsNullOrEmpty(session["cartCount"].ToString()))
            {
                var shoppingCartId = StoreGateway.GetShoppingCartId(HttpContext);
                if (shoppingCartId != null)
                {
                    var sg = new StoreGateway();
                    var cart = sg.GetShoppingCart(shoppingCartId.Value);
                    if (cart != null)
                    {
                        session["cartCount"] = cart.ItemsCount;
                        return;
                    }
                }
                session["cartCount"] = 0;
            }

        }
        protected void ClearCart(HttpSessionStateBase session)
        {
            session.Remove("cartCount");

        }

        protected void AddMessages(List<SiteMessage> messages)
        {
            List<SiteMessage> messageList;
            if (TempData.ContainsKey("Messages"))
                messageList = (List<SiteMessage>)TempData["Messages"];
            else
                messageList = new List<SiteMessage>();

            messages.ForEach(x => messageList.Add(x));

            TempData["Messages"] = messages;
        }

        protected void AddMessage(SiteMessage message)
        {
            if (TempData.ContainsKey("Messages"))
            {
                var messages = (List<SiteMessage>)TempData["Messages"];
                messages.Add(message);
                TempData["Messages"] = messages;
            }
            else
            {
                var messages = new List<SiteMessage>();
                messages.Add(message);
                TempData["Messages"] = messages;
            }
        }
    }
}
