using System.Collections.Generic;
using System.Text;
using RDN.Raspberry.Models.Utilities;

namespace RDN.Raspberry.Models.Helpers
{
    public class MessageHelper
    {
        public static string PrintMessages(List<SiteMessage> messages)
        {
            if (messages == null || messages.Count == 0)
                return string.Empty;


            var output = new StringBuilder();
            output.Append("<br />");
            foreach (var message in messages)
            {
                switch (message.MessageType)
                {
                    case SiteMessageType.Success:
                        output.Append(GenerateSuccessMessage(message));
                        break;
                    case SiteMessageType.Warning:
                        output.Append(GenerateWarningMessage(message));
                        break;
                    case SiteMessageType.Error:
                        output.Append(GenerateErrorMessage(message));
                        break;
                    case SiteMessageType.Info:
                        output.Append(GenerateInfoMessage(message));
                        break;
                }
            }
            output.Append("<br />");
            return output.ToString();
        }

        private static string GenerateSuccessMessage(SiteMessage message)
        {
            var output = new StringBuilder();
            output.Append("<div><span style=\"color: green;font-size:larger;font-weight:bold;\">" + message.Message + " " + (string.IsNullOrEmpty(message.Link) ? string.Empty : string.Format("<a href=\"{0}\">{1}</a>", message.Link, message.LinkText)) + "</span></div>");            
            return output.ToString();
        }

        private static string GenerateInfoMessage(SiteMessage message)
        {
            var output = new StringBuilder();
            output.Append("<div><span style=\"color: blue;font-size:larger;font-weight:bold;\">" + message.Message + " " + (string.IsNullOrEmpty(message.Link) ? string.Empty : string.Format("<a href=\"{0}\">{1}</a>", message.Link, message.LinkText)) + "</span></div>");            
            return output.ToString();
        }

        private static string GenerateErrorMessage(SiteMessage message)
        {
            var output = new StringBuilder();
            output.Append("<div><span style=\"color: red;font-size:larger;font-weight:bold;\">" + message.Message + " " + (string.IsNullOrEmpty(message.Link) ? string.Empty : string.Format("<a href=\"{0}\">{1}</a>", message.Link, message.LinkText)) + "</span></div>");            
            return output.ToString();
        }

        private static string GenerateWarningMessage(SiteMessage message)
        {
            var output = new StringBuilder();
            output.Append("<div><span style=\"color: orange;font-size:larger;font-weight:bold;\">" + message.Message + " " + (string.IsNullOrEmpty(message.Link) ? string.Empty : string.Format("<a href=\"{0}\">{1}</a>", message.Link, message.LinkText)) + "</span></div>");            
            return output.ToString();
        }
    }
}