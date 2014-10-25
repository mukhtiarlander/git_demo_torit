using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.WP.Resources;
using RDN.Portable.Util;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using Microsoft.Advertising.Mobile.UI;

namespace RDN.WP
{
    public partial class MainPage1 : PhoneApplicationPage
    {
        // Constructor
        public MainPage1()
        {
            try
            {
                InitializeComponent();

                // Sample code to localize the ApplicationBar
                //BuildLocalizedApplicationBar();
                LoggerMobile.Instance.logMessage("Opening App", Portable.Util.Log.Enums.LoggerEnum.message);

             
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        //Sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarButtonText;
            ApplicationBar.Buttons.Add(appBarButton);

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

        private void Scores_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new Uri("/View/Public/Game/Games.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void Events_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new Uri("/View/Public/Events.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void Skaters_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Skaters.xaml", UriKind.Relative));
        }

        private void Leagues_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/Public/Leagues.xaml", UriKind.Relative));
        }

        private void Shop_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/Public/Shop/ShopsItems.xaml", UriKind.Relative));
        }
    }
}