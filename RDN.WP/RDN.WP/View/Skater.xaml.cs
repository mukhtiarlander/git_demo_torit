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
using RDN.Portable.Util;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;

namespace RDN.WP.View
{
    public partial class Skater : PhoneApplicationPage
    {
        SkaterViewModel _viewModel;
        public Skater()
        {
            try { 
            InitializeComponent();
            LoggerMobile.Instance.logMessage("Opening Skater", Portable.Util.Log.Enums.LoggerEnum.message);
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            try { 
            _viewModel = (SkaterViewModel)Resources["viewModel"];
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

            progressIndicator.Text = "Loading skater...";
            var skater = (SkaterJson)(App.Current as App).SecondPageObject;
            //var skater = Json.DeserializeObject<SkaterJson>(json);
            _viewModel.Skater = skater;
            _viewModel.LoadPage(skater.MemberId);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
    }
}