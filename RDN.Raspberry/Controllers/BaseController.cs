using System.Collections.Generic;
using System.Web.Mvc;
using RDN.Raspberry.Models.OutModel;
using RDN.Raspberry.Models.Utilities;

namespace RDN.Raspberry.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var result = filterContext.Result as ViewResult;
            if (result != null)
            {
                var model = result.Model as Base;                

                if (TempData.ContainsKey("Messages"))
                {
                    model.MessageList.AddRange((List<SiteMessage>)TempData["Messages"]);
                    TempData.Remove("Messages");
                }
            }

            base.OnResultExecuting(filterContext);
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
