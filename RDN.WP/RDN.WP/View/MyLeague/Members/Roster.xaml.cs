using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.Portable.Classes.Account.Classes;
using RDN.WP.Library.Database;
using RDN.WP.Classes.Error;
using RDN.Portable.Settings;
using RDN.Portable.Config.Enums;
using RDN.WP.Library.Classes.League;
using RDN.WP.Classes.UI.Roster;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;

namespace RDN.WP.View.MyLeague.Members
{
    public partial class Roster : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        ObservableCollection<MemberDisplayAPI> members;
        public Roster()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                members = new ObservableCollection<MemberDisplayAPI>();

                //(App.Current as App).SecondPageObject = null;
                progressIndicator = SystemTray.ProgressIndicator;
                progressIndicator = new ProgressIndicator();
                SystemTray.SetProgressIndicator(this, progressIndicator);
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Pulling Members...";
                if (SettingsMobile.Instance.User == null)
                {
                    SqlFactory fact = new SqlFactory();
                    SettingsMobile.Instance.User = fact.GetProfile();
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }


            PullMembers();
        }
        void PullMembers()
        {
            Dispatcher.BeginInvoke(delegate
            {
                progressIndicator.IsVisible = true;
            });
            try
            {
                MembersMobile.PullLeagueMembers(SettingsMobile.Instance.User.MemberId.ToString(), SettingsMobile.Instance.User.LoginId.ToString(), new Action<List<MemberDisplayAPI>>(ReturnMembersList));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        void ReturnMembersList(List<MemberDisplayAPI> model)
        {
            try
            {
                members = new ObservableCollection<MemberDisplayAPI>(model);
                RosterList.ItemsSource = members;

                Dispatcher.BeginInvoke(delegate
                {
                    progressIndicator.IsVisible = false;
                });
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void RosterButton_Click(object sender, RoutedEventArgs e)
        {

            LeagueRoster.SelectedIndex = 0;
            RosterList.ItemTemplate = (DataTemplate)Resources["RosterTemplate"];
        }

        private void DatesButton_Click(object sender, RoutedEventArgs e)
        {
            LeagueRoster.SelectedIndex = 0;
            RosterList.ItemTemplate = (DataTemplate)Resources["DatesTemplate"];

        }

        private void JobsButton_Click(object sender, RoutedEventArgs e)
        {

            LeagueRoster.SelectedIndex = 0;
            RosterList.ItemTemplate = (DataTemplate)Resources["JobsTemplate"];
        }

        private void InsuranceButton_Click(object sender, RoutedEventArgs e)
        {

            LeagueRoster.SelectedIndex = 0;
            RosterList.ItemTemplate = (DataTemplate)Resources["InsuranceTemplate"];
        }

        private void SkatingLevelButton_Click(object sender, RoutedEventArgs e)
        {

            LeagueRoster.SelectedIndex = 0;
            RosterList.ItemTemplate = (DataTemplate)Resources["SkatingLevelTemplate"];
        }


        //private void OnLayoutModeChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    // Make sure we don't handle the event during initiation.
        //    if (e.RemovedItems != null && e.RemovedItems.Count == 0) return;

        //    ListPickerItem item = (sender as ListPicker).SelectedItem as ListPickerItem;

        //    LongListSelectorLayoutMode layoutMode = LongListSelectorLayoutMode.Grid;

        //    if (item == GridLayoutModeListItem)
        //    {
        //        layoutMode = LongListSelectorLayoutMode.Grid;
        //    }
        //    else if (item == ListLayoutModeListItem)
        //    {
        //        layoutMode = LongListSelectorLayoutMode.List;
        //    }

        //    IsolatedStorageSettings.ApplicationSettings["LayoutMode"] = layoutMode;
        //    IsolatedStorageSettings.ApplicationSettings.Save();
        //}
    }
}