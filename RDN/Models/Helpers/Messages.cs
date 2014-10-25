using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using RDN.Library.Util;

namespace RDN.Models.Helpers
{
    public class MessageHelper
    {
        public static string PrintMessages(List<SiteMessage> messages)
        {
            if (messages == null || messages.Count == 0)
                return string.Empty;

            //The current layout can NOT handle multiple small message bars with the same type. Like multiple error bars.

            // This is a cheat. 
            // -------------------------------------------------------------------------------------\\
            var output = new StringBuilder();
            StringBuilder messageText;
            SiteMessage firstMessage;

            if (messages.Count(x => x.MessageType == SiteMessageType.Info) > 0)
            {
                messageText = new StringBuilder();
                foreach (var message in messages.Where(x => x.MessageType == SiteMessageType.Info))
                    messageText.Append(message.Message + "<br />");
                firstMessage = messages.First(x => x.MessageType == SiteMessageType.Info);
                firstMessage.Message = messageText.ToString().Substring(0, messageText.Length - 6);
                output.Append(GenerateInfoMessage(firstMessage));
            }

            if (messages.Count(x => x.MessageType == SiteMessageType.Error) > 0)
            {
                messageText = new StringBuilder();
                foreach (var message in messages.Where(x => x.MessageType == SiteMessageType.Error))
                    messageText.Append(message.Message + "<br />");
                firstMessage = messages.First(x => x.MessageType == SiteMessageType.Error);
                firstMessage.Message = messageText.ToString().Substring(0, messageText.Length - 6);
                output.Append(GenerateErrorMessage(firstMessage));
            }

            if (messages.Count(x => x.MessageType == SiteMessageType.Success) > 0)
            {
                messageText = new StringBuilder();
                foreach (var message in messages.Where(x => x.MessageType == SiteMessageType.Success))
                    messageText.Append(message.Message + "<br />");
                firstMessage = messages.First(x => x.MessageType == SiteMessageType.Success);
                firstMessage.Message = messageText.ToString().Substring(0, messageText.Length - 6);
                output.Append(GenerateSuccessMessage(firstMessage));
            }

            if (messages.Count(x => x.MessageType == SiteMessageType.Warning) > 0)
            {
                messageText = new StringBuilder();
                foreach (var message in messages.Where(x => x.MessageType == SiteMessageType.Warning))
                    messageText.Append(message.Message + "<br />");
                firstMessage = messages.First(x => x.MessageType == SiteMessageType.Warning);
                firstMessage.Message = messageText.ToString().Substring(0, messageText.Length - 6);
                output.Append(GenerateWarningMessage(firstMessage));
            }

            return output.ToString();
            // -------------------------------------------------------------------------------------\\
            // End cheat \\


            // This code is to be used when the layout gets updated to support multiple bars with the same type
            //var output = new StringBuilder();
            //foreach (var message in messages)
            //{
            //    switch (message.MessageType)
            //    {
            //        case SiteMessageType.Success:
            //            output.Append(GenerateSuccessMessage(message));
            //            break;
            //        case SiteMessageType.Warning:
            //            output.Append(GenerateWarningMessage(message));
            //            break;
            //        case SiteMessageType.Error:
            //            output.Append(GenerateErrorMessage(message));
            //            break;
            //        case SiteMessageType.Info:
            //            output.Append(GenerateInfoMessage(message));
            //            break;
            //    }
            //}
            //return output.ToString();
        }

        private static string GenerateSuccessMessage(SiteMessage message)
        {
            var output = new StringBuilder();
            output.Append("<div class=\"messages\"><span class=\"mNotify\">Success: </span>");
            output.Append(message.Message + " " + (string.IsNullOrEmpty(message.Link) ? string.Empty : string.Format("<a href=\"{0}\">{1}</a>", message.Link, message.LinkText)));
            output.Append("</div>");
            return output.ToString();
        }

        private static string GenerateInfoMessage(SiteMessage message)
        {
            var output = new StringBuilder();
            output.Append("<div class=\"messages\"><span class=\"mNotify\">Notification: </span>");
            output.Append(message.Message + " " + (string.IsNullOrEmpty(message.Link) ? string.Empty : string.Format("<a href=\"{0}\">{1}</a>", message.Link, message.LinkText)));
            output.Append("</div>");
            return output.ToString();
        }

        private static string GenerateErrorMessage(SiteMessage message)
        {
            var output = new StringBuilder();
            output.Append("<div class=\"messages\"><span class=\"mNotify\">Error: </span>");
            output.Append(message.Message + " " + (string.IsNullOrEmpty(message.Link) ? string.Empty : string.Format("<a href=\"{0}\">{1}</a>", message.Link, message.LinkText)));
            output.Append("</div>");
            return output.ToString();
        }

        private static string GenerateWarningMessage(SiteMessage message)
        {
            var output = new StringBuilder();
                        output.Append("<div class=\"messages\"><span class=\"mNotify\">Warning: </span>");
            output.Append(message.Message + " " + (string.IsNullOrEmpty(message.Link) ? string.Empty : string.Format("<a href=\"{0}\">{1}</a>", message.Link, message.LinkText)));
            output.Append("</div>");
            return output.ToString();
        }
    }
}