using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.EmailServer;
using RDN.Library.DataModels.EmailServer.Enums;
using RDN.Utilities.Config;
using RDN.Portable.Config;

namespace RDN.EmailServer
{
    internal class EmailServer : ServiceBase
    {
        // Message logger, see app.config for configuration
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("EmailServerLogger");

        // Timer for when to send normal and priority emails
        private Timer _normalEmails;
        private Timer _priorityEmails;
        // Holds all email layouts
        static Dictionary<string, string> _layouts = new Dictionary<string, string>();
        static object layoutLock = new object();
        private static int NumberOfNormalEmailToSendAtOnce = Int32.Parse(ConfigurationManager.AppSettings["NumberOfNormalEmailToSendAtOnce"]);

        // Initialize the email server
        public void Init()
        {
            try
            {
                logger.Info("Initialize in progress....");

                // Priority emails send every thirty seconds and normal emails send every ten minutes
                _normalEmails = new Timer(Int32.Parse(ConfigurationManager.AppSettings["NormalEmailSendTimerTick"]));
                _priorityEmails = new Timer(Int32.Parse(ConfigurationManager.AppSettings["ImportantEmailSendTimerTick"]));
                LoadLayouts();

                // Start new threads to send the normal and important emails.
                _normalEmails.Elapsed += (sender, args) =>
                {
                    _normalEmails.Stop();
                    Task<bool>.Factory.StartNew(ExecuteNormalEmails);
                    _normalEmails.Start();
                };
                _priorityEmails.Elapsed += (sender, args) =>
                {
                    _priorityEmails.Stop();
                    Task<bool>.Factory.StartNew(ExecuteImportantEmails);
                    _priorityEmails.Start();
                };
                logger.Info("Initialize finished.");
            }
            catch (Exception e)
            {
                logger.Error("Loading layouts", e);
                ErrorDatabaseManager.AddException(e, typeof(EmailServer), additionalInformation: "RDN Email Server Service failure, Load layouts failed");
            }
        }

        /// <summary>
        /// Used to load email layouts. First get the layout details from the db then load the file content.
        /// </summary>
        static void LoadLayouts()
        {
            lock (layoutLock)
            {
                try
                {
                    logger.Info("Loading layouts....");
                    _layouts.Clear();
                    var layouts = System.IO.Directory.GetFiles(ServerConfig.EMAIL_LAYOUTS_LOCATION);
                    foreach (var layout in layouts)
                    {
                        var fileContents = File.ReadAllText(layout);
                        _layouts.Add(Path.GetFileNameWithoutExtension(layout), fileContents); // The id in the dictionary mirrors the database id and that is also what will be found in the email
                    }
                    logger.Info("Layouts finished. Loaded " + layouts.Count().ToString() + " layouts.");
                }
                catch (Exception e)
                {
                    logger.Error("Loading layouts", e);
                    ErrorDatabaseManager.AddException(e, typeof(EmailServer), additionalInformation: "RDN Email Server Service failure, Load layouts failed");
                }
            }
        }

        static bool ExecuteNormalEmails()
        {
            try
            {
                //logger.Info("Starting task: send normal emails");
                SendEmails(EmailPriority.Normal);
                //logger.Info("Task finished: send normal emails");
            }
            catch (Exception e)
            {
                logger.Error("Task: send normal emails", e);
                ErrorDatabaseManager.AddException(e, typeof(EmailServer), additionalInformation: "RDN Email Server Service failure, EmailPriority: Normal");
                return false;
            }
            return true;
        }

        static bool ExecuteImportantEmails()
        {
            try
            {
                //logger.Info("Starting task: send important emails");
                SendEmails(EmailPriority.Important);
                //logger.Info("Task finished: send important emails");
            }
            catch (Exception e)
            {
                logger.Error("Task: send important emails", e);
                ErrorDatabaseManager.AddException(e, typeof(EmailServer), additionalInformation: "RDN Email Server Service failure, EmailPriority: Important");
                return false;
            }
            return true;
        }

        // Start the timers
        public void Start()
        {
            logger.Info("Timers started");
            _normalEmails.Start();
            _priorityEmails.Start();
        }

        // Triggers when the windows service is started
        protected override void OnStart(string[] args)
        {
            logger.Info("Service OnStart triggered");
            base.OnStart(args);
            Init();
            Start();
        }

        // Triggers when the windows service is stopped
        protected override void OnStop()
        {
            logger.Info("Service OnStop triggered");
            if (_priorityEmails != null)
                _priorityEmails.Stop();
            if (_normalEmails != null)
                _normalEmails.Stop();
            base.OnStop();
        }

        // Triggers when the timers wants to send emails
        static void SendEmails(EmailPriority priority)
        {
            List<EmailSendItem> items; // Emails to send
            if (priority == EmailPriority.Important)
                items = Library.Classes.EmailServer.EmailServer.PullPriorityItemsToSend(); // Get all important emails and send them
            else
                items = Library.Classes.EmailServer.EmailServer.PullItemsToSend(NumberOfNormalEmailToSendAtOnce); // Get the specific amount of normal emails

            //this is the broadcast message that sits at the bottom of the emails.
            string mess = Library.Classes.EmailServer.EmailServer.PullLatestEmailMessage();
            var property = new EmailProperty() { Key = "BROADCASTMESSAGE", Value = mess };

            if (items.Count == 0) return; // If no emails were found, do nothing

            logger.Info("Fetching emails, priority: " + priority.ToString());
            logger.Info(string.Format("{0} email(s) found", items.Count));
            var itemsToDelete = new List<int>(); // Keep track of sent emails and delete them afterwards
            foreach (var item in items)
            {
                try
                {
                    item.Properties.Add(property);
                    // Parse the email properties into the layout specified and get that as the email body
                    var body = ParseEmailIntoLayout(item.EmailLayout, item.Properties.ToList());
                    Email.SendEmail(item.Reciever, item.From, item.DisplayNameFrom, item.Subject, body); // Send the email
                    itemsToDelete.Add(item.EmailSendItemId); // Add to the delete list
                }
                catch (Exception e)
                {
                    logger.Info("Email send error", e);
                    ErrorDatabaseManager.AddException(e, e.GetType());

                }
            }
            Library.Classes.EmailServer.EmailServer.DeleteItems(itemsToDelete); // Delete all emails that has been sent
            logger.Info(string.Format("Finished, {0} email(s) sent", items.Count));
        }

        /// <summary>
        /// Loads the layout and then parses the properties from the email into the layout. If no layout is found it prints the properties using a very basic layout.
        /// </summary>
        /// <param name="layoutId">Layout id (id from the table RDN_EmailServer_Layouts)</param>
        /// <param name="properties">Email properties to be parsed into the layout</param>
        /// <returns>Body string</returns>
        static string ParseEmailIntoLayout(string layoutId, List<EmailProperty> properties)
        {
            var output = new StringBuilder();
            bool layoutLoaded = false; // If a layout is loaded, then parse the data at the bottom.

            // Is the layout in the system?
            if (layoutId == RDN.Library.Classes.EmailServer.EmailServerLayoutsEnum.TextMessage.ToString())
            {
                output.Append(_layouts[layoutId]);

                layoutLoaded = true;
            }
            else if (_layouts.ContainsKey(layoutId))
            {
                output.Append(_layouts["AATemplateForAllEmails"]);

                output.Replace("%TEMPLATE%", _layouts[layoutId]);

                layoutLoaded = true;
            }
            else // Layout is not on the system
            {
                // Try to reload the layouts. Maybe it got added after the email server started.
                LoadLayouts();
                if (_layouts.ContainsKey(layoutId)) // Did we find it?
                {
                    output.Append(_layouts["AATemplateForAllEmails"]);

                    output.Replace("%TEMPLATE%", _layouts[layoutId]);

                    layoutLoaded = true;
                }
                else // No layout was found
                {
                    // If we have a body tag. Print that info and then the properties a few lines below
                    var body = properties.FirstOrDefault(x => x.Key.Equals("Body"));
                    if (body != null)
                    {
                        output.Append("<div style=\"font: 11px verdana, arial\">");
                        output.Append(body);
                        output.Append("<br /><br />");
                        foreach (var emailProperty in properties.Where(x => !x.Key.Equals("Body")))
                            output.Append(string.Format("{0}: {1}<br />", emailProperty.Key, emailProperty.Value));
                        output.Append("</div><br /><br />");
                    }
                    else
                    {
                        // No body tag was found. Print all properties
                        output.Append("<div style=\"font: 11px verdana, arial\">");
                        foreach (var emailProperty in properties)
                            output.Append(string.Format("{0}: {1}<br />", emailProperty.Key, emailProperty.Value));
                        output.Append("</div><br /><br />");
                    }
                }
            }

            // Did we find a layout?
            if (!layoutLoaded) return output.ToString();

            // Parse all properties into the layout
            foreach (var emailProperty in properties)
                output.Replace(string.Format("%{0}%", emailProperty.Key), emailProperty.Value);

            return output.ToString();
        }
    }
}
