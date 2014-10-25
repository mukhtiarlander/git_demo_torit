using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Util;

namespace RDN.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
          
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
