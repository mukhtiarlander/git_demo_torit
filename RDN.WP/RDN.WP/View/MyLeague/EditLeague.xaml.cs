using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.Portable.Classes.League.Classes;
using System.Threading.Tasks;
using RDN.WP.Library.Classes.League;
using RDN.Portable.Settings;
using RDN.Portable.Classes.Federation.Enums;
using RDN.Portable.Util;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using Coding4Fun.Toolkit.Controls;

namespace RDN.WP.View.MyLeague
{
    public partial class EditLeague : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        LeagueBase league;
        public EditLeague()
        {
            InitializeComponent();

            progressIndicator = new ProgressIndicator();

            SystemTray.SetProgressIndicator(this, progressIndicator);
            progressIndicator.IsIndeterminate = true;
            progressIndicator.Text = "Pulling League...";
            progressIndicator.IsVisible = true;
            Task.Run(new Action(() =>
            {

                league = LeagueMobile.GetLeagueEdit(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId);
                if (league != null && league.IsSuccessful)
                {
                    Dispatcher.BeginInvoke(delegate
                    {
                        try
                        {
                            if (!String.IsNullOrEmpty(league.Name))
                                LeagueName.Text = league.Name;
                            if (!String.IsNullOrEmpty(league.City)) City.Text = league.City;
                            if (!String.IsNullOrEmpty(league.State)) State.Text = league.State;
                            if (!String.IsNullOrEmpty(league.ZipCode)) Zip.Text = league.ZipCode;
                            if (!String.IsNullOrEmpty(league.Country)) Country.Text = league.Country;
                            Founding.Value = league.Founded;
                            if (!String.IsNullOrEmpty(league.PhoneNumber)) Phone.Text = league.PhoneNumber;
                            if (!String.IsNullOrEmpty(league.Email)) Email.Text = league.Email;
                            if (!String.IsNullOrEmpty(league.Website)) Website.Text = league.Website;
                            if (!String.IsNullOrEmpty(league.Instagram)) Instagram.Text = league.Instagram;
                            if (!String.IsNullOrEmpty(league.Twitter)) Twitter.Text = league.Twitter;
                            if (!String.IsNullOrEmpty(league.Facebook)) Facebook.Text = league.Facebook;

                            int index = league.Cultures.IndexOf(league.Cultures.Where(x => x.LCID == league.CultureSelected).FirstOrDefault());
                            Cultures.ItemsSource = league.Cultures;
                            Cultures.SelectedIndex = index;
                            if (league.RuleSetsPlayedEnum.HasFlag(RuleSetsUsedEnum.MADE))
                                MADE.IsChecked = true;
                            if (league.RuleSetsPlayedEnum.HasFlag(RuleSetsUsedEnum.OSDA))
                                OSDA.IsChecked = true;
                            if (league.RuleSetsPlayedEnum.HasFlag(RuleSetsUsedEnum.RDCL))
                                RDCL.IsChecked = true;
                            if (league.RuleSetsPlayedEnum.HasFlag(RuleSetsUsedEnum.Renegade))
                                Renegade.IsChecked = true;
                            if (league.RuleSetsPlayedEnum.HasFlag(RuleSetsUsedEnum.Texas_Derby))
                                TexasDerby.IsChecked = true;
                            if (league.RuleSetsPlayedEnum.HasFlag(RuleSetsUsedEnum.The_WFTDA))
                                WFTDA.IsChecked = true;
                            if (league.RuleSetsPlayedEnum.HasFlag(RuleSetsUsedEnum.USARS))
                                USARS.IsChecked = true;
                            progressIndicator.IsVisible = false;
                        }
                        catch (Exception exception)
                        {
                            ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                        }

                    });
                }

            }));

        }

        private void Cultures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0)
                {
                    var cat = (Culture)e.AddedItems[0];
                    //reset category for the group
                    league.CultureSelected = cat.LCID;

                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            try
            {
                progressIndicator.IsVisible = true;
                progressIndicator.Text = "Saving League...";

                league.Name = LeagueName.Text;
                league.City = City.Text;
                league.State = State.Text;
                league.ZipCode = Zip.Text;
                league.Country = Country.Text;
                league.Founded = Convert.ToDateTime(Founding.Value);
                league.PhoneNumber = Phone.Text;
                league.Email = Email.Text;
                league.Website = Website.Text;
                league.Instagram = Instagram.Text;
                league.Twitter = Twitter.Text;
                league.Facebook = Facebook.Text;

                league.RuleSetsPlayedEnum = RuleSetsUsedEnum.None;

                if (MADE.IsChecked.GetValueOrDefault())
                    league.RuleSetsPlayedEnum |= RuleSetsUsedEnum.MADE;
                if (OSDA.IsChecked.GetValueOrDefault())
                    league.RuleSetsPlayedEnum |= RuleSetsUsedEnum.OSDA;
                if (RDCL.IsChecked.GetValueOrDefault())
                    league.RuleSetsPlayedEnum |= RuleSetsUsedEnum.RDCL;
                if (Renegade.IsChecked.GetValueOrDefault())
                    league.RuleSetsPlayedEnum |= RuleSetsUsedEnum.Renegade;
                if (TexasDerby.IsChecked.GetValueOrDefault())
                    league.RuleSetsPlayedEnum |= RuleSetsUsedEnum.Texas_Derby;
                if (WFTDA.IsChecked.GetValueOrDefault())
                    league.RuleSetsPlayedEnum |= RuleSetsUsedEnum.The_WFTDA;
                if (USARS.IsChecked.GetValueOrDefault())
                    league.RuleSetsPlayedEnum |= RuleSetsUsedEnum.USARS;

                Task.Run(new Action(() =>
                {

                    LeagueMobile.SaveLeagueEdit(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, league);

                    Dispatcher.BeginInvoke(delegate
                    {
                        try
                        {
                            progressIndicator.IsVisible = false;
                            ToastPrompt t = new ToastPrompt
                            {
                                Title = "League Saved",
                                TextOrientation = System.Windows.Controls.Orientation.Vertical

                            };
                            t.Show();
                        }
                        catch (Exception exception)
                        {
                            ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                        }
                    });
                }));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}