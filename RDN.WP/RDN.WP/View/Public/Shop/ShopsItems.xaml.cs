using System;
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
using RDN.WP.Library.ViewModels.Public.Shop;
using RDN.Portable.Models.Json.Shop;
using RDN.Portable.Util;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;

namespace RDN.WP.View
{
    public partial class ShopItems : PhoneApplicationPage
    {

        private int PAGE_COUNT = 20;
        private string lastLetterPulled = "a";
        private int lastPagePulled = 0;
        ShopsJson initialArray;
        int _offsetKnob = 7;
        ShopItemsViewModel _viewModel;
        string searchTerm = string.Empty;
        public ShopItems()
        {
            try
            {
                InitializeComponent();
                LoggerMobile.Instance.logMessage("Opening ShopItems", Portable.Util.Log.Enums.LoggerEnum.message);
                _viewModel = (ShopItemsViewModel)Resources["viewModel"];
                resultListBox.ItemRealized += resultListBox_ItemRealized;
                this.Loaded += new RoutedEventHandler(MainPage_Loaded);
                initialArray = new ShopsJson();

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

                progressIndicator.Text = "Loading items...";
                _viewModel.LoadPage(lastLetterPulled, lastPagePulled, PAGE_COUNT);
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
                        if ((e.Container.Content as ShopItemJson).Equals(resultListBox.ItemsSource[resultListBox.ItemsSource.Count - _offsetKnob]))
                        {
                            _viewModel.LoadPage(lastLetterPulled, lastPagePulled++, PAGE_COUNT);
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
                    _viewModel.LoadPage(lastLetterPulled, lastPagePulled++, PAGE_COUNT);
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

                (App.Current as App).SecondPageObject = (ShopItemJson)resultListBox.SelectedItem;// Json.ConvertToString<ShopItemJson>((ShopItemJson)resultListBox.SelectedItem);

                // Reset selected item to null
                resultListBox.SelectedItem = null;
                NavigationService.Navigate(new Uri("/View/Public/Shop/ShopItem.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }


    }
}