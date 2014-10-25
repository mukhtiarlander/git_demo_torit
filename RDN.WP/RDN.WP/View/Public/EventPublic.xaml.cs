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
using RDN.Portable.Models.Json.Calendar;
using Microsoft.Phone.Tasks;
using RDN.Portable.Models.Json.Public;
using RDN.Portable.Util;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;

namespace RDN.WP.View.Public
{
    public partial class EventPublic : PhoneApplicationPage
    {
        EventViewModel _viewModel;
        public EventPublic()
        {
            try { 
            InitializeComponent();
            LoggerMobile.Instance.logMessage("Opening EventPublic", Portable.Util.Log.Enums.LoggerEnum.message);
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
            _viewModel = (EventViewModel)Resources["viewModel"];
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

            progressIndicator.Text = "Loading event...";
            var skater  = (EventJson) (App.Current as App).SecondPageObject;
            //var skater = Json.DeserializeObject<EventJson>(json);
            _viewModel.EventPublic = skater;
            //_viewModel.LoadPage(skater.MemberId);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try { 
            var task = new BingMapsTask();
            task.ZoomLevel = 2;
            task.SearchTerm = _viewModel.EventPublic.Location + " " + _viewModel.EventPublic.Address;
            task.Show();
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void TextBlock_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try { 
            var league = new LeagueJsonDataTable() { LeagueId = _viewModel.EventPublic.LeagueId, LeagueName = _viewModel.EventPublic.OrganizersName };
            (App.Current as App).SecondPageObject = league;// Json.ConvertToString<LeagueJsonDataTable>((LeagueJsonDataTable)league);

            NavigationService.Navigate(new Uri("/View/Public/League.xaml", UriKind.Relative));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void TicketsTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try { 
                var task = new WebBrowserTask();
                task.Uri = new Uri(_viewModel.EventPublic.TicketUrl);
                task.Show();
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        private void EventTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try { 
            var task = new WebBrowserTask();
            task.Uri = new Uri(_viewModel.EventPublic.EventUrl);
            task.Show();
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        private void RDNTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try { 
            var task = new WebBrowserTask();
            task.Uri = new Uri(_viewModel.EventPublic.RDNUrl);
            task.Show();
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
    }
}