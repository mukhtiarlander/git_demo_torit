using System;
using System.Configuration;
using System.Net.Mail;
using System.Text;

namespace RDN.EmailServer
{
    public static class Email
    {
        // Message logger, see app.config for configuration
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("EmailServerLogger");


        private static readonly SmtpClient Smtp = new SmtpClient(ConfigurationManager.AppSettings["Server"])
        {
            UseDefaultCredentials = false,
            Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserName"], ConfigurationManager.AppSettings["Password"]),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Port = Int32.Parse(ConfigurationManager.AppSettings["Port"])

        };
        public static bool SendEmail(string toUserEmail, string fromEmail, string displayName, string subject, string body)
        {
            try
            {
                var mail = new MailMessage
                               {
                                   IsBodyHtml = true,
                                   BodyEncoding = Encoding.UTF8,
                                   From = new MailAddress(fromEmail, displayName),
                                   Subject = subject,
                                   Body = body
                               };
                //if (!String.IsNullOrEmpty(replyTo))
                //    mail.Headers.Add("Reply-To", replyTo);
                mail.To.Add(new MailAddress(toUserEmail));

                SendEmailMessage(mail, false, null);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void SendEmailMessage(MailMessage message, bool sendAsync, string asyncToken)
        {
            if (!sendAsync)
            {
                Smtp.SendCompleted += Smtp_SendCompleted;
                Smtp.Send(message);
            }
            else
            {
                if (String.IsNullOrEmpty(asyncToken))
                    asyncToken = Guid.NewGuid().ToString();

                try
                {
                    Smtp.SendCompleted += Smtp_SendCompleted;
                    Smtp.SendAsync(message, asyncToken);
                }
                catch (Exception e)
                {
                    logger.Error("Inside SendEmailMessage", e);
                    throw;
                }
            }
        }

        static void Smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}