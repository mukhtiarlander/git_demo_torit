﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.WP.Classes.UI;
using RDN.WP.Helpers;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Devices;
using RDN.Portable.Models.Json.Public;
using RDN.WP.Library.ViewModels.Public;
using RDN.Portable.Models.Json;
using RDN.Portable.Classes.Utilities;
using RDN.Portable.Models.Json.Calendar;
using RDN.Portable.Util;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using Windows.Devices.Geolocation;

namespace RDN.WP.View.Public
{
    public partial class Events : PhoneApplicationPage
    {

        private int PAGE_COUNT = 40;
        //private string lastLetterPulled = "a";
        private int lastPagePulled = 0;
        EventsJson initialArray;
        int _offsetKnob = 7;
        EventsViewModel _viewModel;
        string searchTerm = string.Empty;
        public Events()
        {
            try
            {
                InitializeComponent();
                LoggerMobile.Instance.logMessage("Opening Events", Portable.Util.Log.Enums.LoggerEnum.message);
                _viewModel = (EventsViewModel)Resources["viewModel"];
                resultListBox.ItemRealized += resultListBox_ItemRealized;
                this.Loaded += new RoutedEventHandler(MainPage_Loaded);
                initialArray = new EventsJson();

                ApplicationBar = new ApplicationBar();

                ApplicationBar.Mode = ApplicationBarMode.Default;
                ApplicationBar.Opacity = 1.0;
                ApplicationBar.IsVisible = true;
                ApplicationBar.IsMenuEnabled = true;

                ApplicationBarIconButton searchBtn = new ApplicationBarIconButton();
                searchBtn.IconUri = new Uri("/Assets/Icons/feature.search.png", UriKind.Relative);
                searchBtn.Text = "Search";
                searchBtn.Click += searchBtn_Click;
                ApplicationBar.Buttons.Add(searchBtn);

                ApplicationBarIconButton mapBtn = new ApplicationBarIconButton();
                mapBtn.IconUri = new Uri("/Assets/Icons/appbar.crosshair.png", UriKind.Relative);
                mapBtn.Text = "locate";

                mapBtn.Click += searchByLocation_Click;
                ApplicationBar.Buttons.Add(mapBtn);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        void searchBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchBar.Visibility == System.Windows.Visibility.Collapsed)
                    searchBar.Visibility = System.Windows.Visibility.Visible;
                else
                    searchBar.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        async void searchByLocation_Click(object sender, EventArgs e)
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                _viewModel.IsLoading = true;

                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );
                lastPagePulled = 0;

                _viewModel.LocateEvents(lastPagePulled, PAGE_COUNT, geoposition.Coordinate.Longitude, geoposition.Coordinate.Latitude);

            }
            catch (Exception exception)
            {
                _viewModel.IsLoading = false;
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);

            }
        }
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var progressIndicator = SystemTray.ProgressIndicator;
                if (progressIndicator != null)
                {
                    return;
                }

                progressIndicator = new ProgressIndicator();

                SystemTray.SetProgressIndicator(this, progressIndicator);

                Binding binding = new Binding("IsLoading") { Source = _viewModel };
                BindingOperations.SetBinding(
                    progressIndicator, ProgressIndicator.IsVisibleProperty, binding);

                binding = new Binding("IsLoading") { Source = _viewModel };
                BindingOperations.SetBinding(
                    progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);

                progressIndicator.Text = "Loading Events...";
                _viewModel.LoadPage("", lastPagePulled, PAGE_COUNT);
                lastPagePulled++;
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        void resultListBox_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            try
            {
                if (!_viewModel.IsLoading && resultListBox.ItemsSource != null && resultListBox.ItemsSource.Count >= _offsetKnob)
                {
                    if (e.ItemKind == LongListSelectorItemKind.Item)
                    {
                        if ((e.Container.Content as EventJson).Equals(resultListBox.ItemsSource[resultListBox.ItemsSource.Count - _offsetKnob]))
                        {
                            _viewModel.LoadPage("", lastPagePulled++, PAGE_COUNT);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                searchTerm = SearchTextBox.Text.Trim();

                if (String.IsNullOrEmpty(searchTerm))
                {
                    _viewModel.LoadPage("", lastPagePulled++, PAGE_COUNT);
                    return;
                }

                lastPagePulled = 0;

                _viewModel.LoadPage(searchTerm, lastPagePulled++, PAGE_COUNT);
                if (e.Key == Key.Enter)
                {
                    this.Focus();
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            SearchTextBox.SelectAll();
        }

        private void resultListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // If selected item is null, do nothing
                if (resultListBox.SelectedItem == null)
                    return;

                (App.Current as App).SecondPageObject = (EventJson)resultListBox.SelectedItem;// Json.ConvertToString<EventJson>();

                // Reset selected item to null
                resultListBox.SelectedItem = null;
                NavigationService.Navigate(new Uri("/View/Public/EventPublic.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }


    }
}