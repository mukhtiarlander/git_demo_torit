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
using RDN.Portable.Classes.League;
using RDN.Portable.Settings;
using RDN.WP.Library.Database;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.WP.Library.Classes.League;
using RDN.Portable.Classes.League.Classes;
using RDN.WP.Classes.UI;
using System.Collections.ObjectModel;

namespace RDN.WP.View.MyLeague.Generic
{
    public partial class MemberGroupChooser : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        List<MemberDisplayModel> Members;
        List<LeagueGroupModel> Groups;
        bool areGroupsOff = false;
        /// <summary>
        /// contains both the selected group and member Ids
        /// </summary>
        ObservableCollection<NameIdModel> SelectedIds { get; set; }

        public MemberGroupChooser()
        {
            InitializeComponent();
            SelectedIds = new ObservableCollection<NameIdModel>();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                SelectedIds = new ObservableCollection<NameIdModel>();
                Members = new List<MemberDisplayModel>();
                Groups = new List<LeagueGroupModel>();

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
                var check = this.NavigationContext.QueryString.Where(x => x.Key == "g").FirstOrDefault();
                if (check.Value == "off")
                {
                    areGroupsOff = true;
                    Pivot.Items.Remove(Pivot.Items.Single(p => ((PivotItem)p).Name == "groupPivotItem"));
                }
                if ((App.Current as App).SecondPageObject != null)
                    SelectedIds = (ObservableCollection<NameIdModel>)(App.Current as App).SecondPageObject;

                PullGroups();
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }

        }
        void PullGroups()
        {
            Dispatcher.BeginInvoke(delegate
            {
                progressIndicator.IsVisible = true;
            });
            try
            {
                LeagueMobile.PullLeagueMembers(SettingsMobile.Instance.User.MemberId.ToString(), SettingsMobile.Instance.User.LoginId.ToString(), new Action<List<MemberDisplayBasic>>(ReturnLeagueMembers));
                if (!areGroupsOff)
                    LeagueMobile.PullLeagueGroups(SettingsMobile.Instance.User.MemberId.ToString(), SettingsMobile.Instance.User.LoginId.ToString(), new Action<List<LeagueGroupBasic>>(ReturnLeagueGroups));

            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        void ReturnLeagueMembers(List<MemberDisplayBasic> model)
        {
            try
            {
                for (int i = 0; i < model.Count; i++)
                {
                    var mem = new MemberDisplayModel()
                      {
                          DerbyName = model[i].DerbyName,
                          LastName = model[i].LastName,
                          Firstname = model[i].Firstname,
                          MemberId = model[i].MemberId,
                          ThumbUrl = model[i].ThumbUrl
                      };
                    if (SelectedIds.Select(x => x.Id).Contains(mem.MemberId.ToString()))
                        mem.IsChecked = true;
                    Members.Add(mem);

                }
                Dispatcher.BeginInvoke(delegate
                {
                    MembersList.ItemsSource = Members;
                    progressIndicator.IsVisible = false;
                });
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        void ReturnLeagueGroups(List<LeagueGroupBasic> model)
        {
            try
            {
                for (int i = 0; i < model.Count; i++)
                {
                    var group = new LeagueGroupModel()
                        {
                            Id = model[i].Id,
                            GroupName = model[i].GroupName
                        };
                    if (SelectedIds.Select(x => x.Id).Contains(group.Id.ToString()))
                        group.IsChecked = true;
                    Groups.Add(group);
                }
                Dispatcher.BeginInvoke(delegate
                {
                    GroupsList.ItemsSource = Groups;
                    progressIndicator.IsVisible = false;
                });
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void GroupChecked_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void MemberCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Done_Click(object sender, EventArgs e)
        {
            SelectedIds.Clear();
            for (int i = 0; i < Members.Count; i++)
                if (Members[i].IsChecked)
                    SelectedIds.Add(new NameIdModel() { Id = Members[i].MemberId.ToString(), Name = Members[i].DerbyName.ToString() });

            if (!areGroupsOff)
            {
                for (int i = 0; i < Groups.Count; i++)
                    if (Groups[i].IsChecked)
                        SelectedIds.Add(new NameIdModel() { Id = Groups[i].Id.ToString(), Name = Groups[i].GroupName.ToString() });
            }
            (App.Current as App).SecondPageObject = SelectedIds;
            NavigationService.GoBack();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

    }
}