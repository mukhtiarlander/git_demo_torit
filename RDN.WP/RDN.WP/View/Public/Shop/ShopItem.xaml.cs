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
using RDN.WP.Library.ViewModels.Public.Shop;
using RDN.Portable.Models.Json.Shop;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using RDN.Portable.Util;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;

namespace RDN.WP.View.Public.Shop
{
    public partial class ShopItem : PhoneApplicationPage
    {
        ShopItemViewModel _viewModel;
        public ShopItem()
        {
            try
            {
                InitializeComponent();
                this.Loaded += new RoutedEventHandler(MainPage_Loaded);
                LoggerMobile.Instance.logMessage("Opening ShopItem", Portable.Util.Log.Enums.LoggerEnum.message);
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
                _viewModel = (ShopItemViewModel)Resources["viewModel"];
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

                //progressIndicator.Text = "Loading skater...";
                var skater = (ShopItemJson) (App.Current as App).SecondPageObject;
                //var skater = Json.DeserializeObject<ShopItemJson>(json);
                _viewModel.ShopItem = skater;
                _viewModel.LoadPage();
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void BuyItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var task = new WebBrowserTask();
                task.Uri = new Uri(_viewModel.ShopItem.RDNUrl);
                task.Show();
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

    }
}