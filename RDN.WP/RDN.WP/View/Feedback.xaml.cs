using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.Portable.Settings;
using System.Threading.Tasks;
using RDN.WP.Library.Classes.Utilities;
using Coding4Fun.Toolkit.Controls;
using RDN.Portable.Config.Enums;
using RDN.WP.Classes.Error;
using RDN.Portable.Classes.Utilities.Enums;

namespace RDN.WP.View
{
    public partial class Feedback : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        RDN.Portable.Classes.Utilities.Feedback feedBack;
        public Feedback()
        {
            InitializeComponent();

            feedBack = new Portable.Classes.Utilities.Feedback();
            progressIndicator = SystemTray.ProgressIndicator;
            progressIndicator = new ProgressIndicator();

            SystemTray.SetProgressIndicator(this, progressIndicator);
            progressIndicator.IsIndeterminate = true;
            progressIndicator.Text = "Sending Feedback...";


            if (SettingsMobile.Instance.User != null)
                Email.Text = SettingsMobile.Instance.User.UserName;
        }
        private void SendPost_Click(object sender, EventArgs e)
        {
            progressIndicator.Text = "Sending Feedback...";
            progressIndicator.IsVisible = true;
            try
            {
                feedBack.FeedbackText = feedbackText.Text;
                feedBack.Email = Email.Text;
                feedBack.DateTime = DateTime.UtcNow;
                feedBack.FeedbackType = FeedbackTypeEnum.Windows_Phone;

                Task.Run(new Action(() =>
                {
                    try
                    {
                        var m = FeedbackMobile.SendFeedback(feedBack);


                        Dispatcher.BeginInvoke(delegate
                        {
                            try
                            {
                                if (m.IsSuccessful)
                                {
                                    ToastPrompt t = new ToastPrompt
                                    {
                                        Title = "Feedback Sent",
                                        TextOrientation = System.Windows.Controls.Orientation.Vertical
                                    };

                                    t.Show();
                                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
                                }
                                else
                                {
                                    ToastPrompt t = new ToastPrompt
                                    {
                                        Title = "Something Happened, please try again later.",
                                        TextOrientation = System.Windows.Controls.Orientation.Vertical
                                    };

                                    t.Show();
                                }
                                progressIndicator.IsVisible = false;
                            }
                            catch (Exception exception)
                            {
                                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                            }
                        });
                    }
                    catch (Exception exception)
                    {
                        ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                    }
                }));

            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }



    }
}