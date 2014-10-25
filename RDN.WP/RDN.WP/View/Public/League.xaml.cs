using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Data;
using RDN.WP.Library.ViewModels.Public;
using RDN.Portable.Classes.Utilities;
using RDN.Portable.Models.Json;
using RDN.Portable.Models.Json.Public;
using RDN.Portable.Models.Json.Calendar;
using RDN.Portable.Util;
using RDN.Portable.Config.Enums;
using RDN.WP.Classes.Error;

namespace RDN.WP.View.Public
{
    public partial class League : PhoneApplicationPage
    {
        LeagueViewModel _viewModel;
        public League()
        {
            try
            {
                InitializeComponent();
                LoggerMobile.Instance.logMessage("Opening League", Portable.Util.Log.Enums.LoggerEnum.message);
                this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel = (LeagueViewModel)Resources["viewModel"];
                var progressIndicator = SystemTray.ProgressIndicator;
                if (progressIndicator != null)
                {
                    return;
                }

                progressIndicator = new ProgressIndicator();

                SystemTray.SetProgressIndicator(this, progressIndicator);

               Binding binding = new Binding("IsLoading") { Source = _viewModel };
                BindingOperations.SetBinding(progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);

                progressIndicator.Text = "Loading league...";
                var skater = (LeagueJsonDataTable)(App.Current as App).SecondPageObject;
                //var skater = Json.DeserializeObject<LeagueJsonDataTable>(json);
                _viewModel.League = skater;
                _viewModel.LoadPage(skater.LeagueId);
                _viewModel.PropertyChanged += _viewModel_PropertyChanged;
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        void _viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
         
        }
        private void resultListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // If selected item is null, do nothing
                if (resultListBox.SelectedItem == null)
                    return;

                (App.Current as App).SecondPageObject = (SkaterJson)resultListBox.SelectedItem;// Json.ConvertToString<SkaterJson>((SkaterJson)resultListBox.SelectedItem);

                // Reset selected item to null
                resultListBox.SelectedItem = null;
                NavigationService.Navigate(new Uri("/View/Skater.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        private void eventListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // If selected item is null, do nothing
                if (scheduleListBox.SelectedItem == null)
                    return;

                (App.Current as App).SecondPageObject = (EventJson)scheduleListBox.SelectedItem;// Json.ConvertToString<EventJson>((EventJson)scheduleListBox.SelectedItem);

                // Reset selected item to null
                scheduleListBox.SelectedItem = null;
                NavigationService.Navigate(new Uri("/View/Public/EventPublic.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
    }
}