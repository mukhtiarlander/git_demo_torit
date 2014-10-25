using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;

namespace RDN.WP.View.MyProfile
{
    public partial class MyProfile: PhoneApplicationPage
    {
        public MyProfile()
        {
            InitializeComponent();
        }

        private void ForumButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new Uri("/View/MyLeague/ForumMain.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void MembersButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new Uri("/View/MyLeague/Members/Roster.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void MessagesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new Uri("/View/MyLeague/Messages/MessagesHome.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new Uri("/View/MyProfile/EditProfile.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
    }
}