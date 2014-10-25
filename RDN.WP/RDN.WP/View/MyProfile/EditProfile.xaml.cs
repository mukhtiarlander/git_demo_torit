using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.Portable.Classes.Account.Enums;
using RDN.Portable.Util.Enums;
using RDN.Portable.Classes.Account.Classes;
using System.Threading.Tasks;
using RDN.Portable.Account;
using RDN.WP.Library.Account;
using RDN.Portable.Settings;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.Portable.Classes.Geo;
using Coding4Fun.Toolkit.Controls;

namespace RDN.WP.View.MyProfile
{
    public partial class EditProfile : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        MemberDisplayEdit member;
        public EditProfile()
        {
            InitializeComponent();
            try
            {
                member = new MemberDisplayEdit();
                var dict = Enum.GetValues(typeof(GenderEnum))
                           .Cast<GenderEnum>()
                           .ToDictionary(t => (int)t, t => EnumExt.ToFreindlyName(t));

                Gender.ItemsSource = dict;

                progressIndicator = new ProgressIndicator();

                SystemTray.SetProgressIndicator(this, progressIndicator);
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Pulling Member...";
                progressIndicator.IsVisible = true;
                Task.Run(new Action(() =>
                {
                    member = UserMobileWP.GetMemberEdit(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId);
                    if (member != null && member.IsSuccessful)
                    {
                        Dispatcher.BeginInvoke(delegate
                        {
                            try
                            {
                                progressIndicator.IsVisible = false;
                                if (!String.IsNullOrEmpty(member.DerbyName)) DerbyName.Text = member.DerbyName;

                                if (!String.IsNullOrEmpty(member.PlayerNumber)) DerbyNumber.Text = member.PlayerNumber;
                                if (!String.IsNullOrEmpty(member.Firstname)) FirstName.Text = member.Firstname;
                                if (!String.IsNullOrEmpty(member.LastName)) LastName.Text = member.LastName;
                                Gender.SelectedItem = dict.Where(x => x.Key == (int)member.Gender).FirstOrDefault();
                                HeightFeet.Text = member.HeightFeet.ToString();
                                HeightInches.Text = member.HeightInches.ToString();
                                Weight.Text = member.WeightLbs.ToString();
                                DOB.Value = member.DOB;
                                if (!String.IsNullOrEmpty(member.PhoneNumber)) Phone.Text = member.PhoneNumber;
                                if (!String.IsNullOrEmpty(member.Email)) Email.Text = member.Email;
                                if (!String.IsNullOrEmpty(member.DayJob)) Job.Text = member.DayJob;

                                StartSkating.Value = member.StartedSkating;
                                StopSkating.Value = member.StoppedSkating;
                                if (!String.IsNullOrEmpty(member.InsuranceNumWftda)) WFTDANum.Text = member.InsuranceNumWftda;
                                WFTDAExpires.Value = member.InsuranceNumWftdaExpires;
                                if (!String.IsNullOrEmpty(member.InsuranceNumUsars)) USARSNum.Text = member.InsuranceNumUsars;
                                USARSExpires.Value = member.InsuranceNumUsarsExpires;
                                if (!String.IsNullOrEmpty(member.InsuranceNumCRDI)) CRDINum.Text = member.InsuranceNumCRDI;
                                CRDIExpires.Value = member.InsuranceNumCRDIExpires;
                                if (!String.IsNullOrEmpty(member.InsuranceNumOther)) OtherNum.Text = member.InsuranceNumOther;
                                OtherExpires.Value = member.InsuranceNumOtherExpires;
                                HideProfileFromPublic.IsChecked = member.IsProfileRemovedFromPublicView;
                                if (!String.IsNullOrEmpty(member.Address)) Address1.Text = member.Address;
                                if (!String.IsNullOrEmpty(member.Address2)) Address2.Text = member.Address2;
                                if (!String.IsNullOrEmpty(member.City)) City.Text = member.City;
                                if (!String.IsNullOrEmpty(member.State)) State.Text = member.State;
                                if (!String.IsNullOrEmpty(member.ZipCode)) Zip.Text = member.ZipCode;
                                Country.ItemsSource = member.Countries;
                                if (member.Country > 0)
                                    Country.SelectedItem = member.Countries.Where(x => x.CountryId == member.Country).FirstOrDefault();

                            }
                            catch (Exception exception)
                            {
                                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                            }
                        });
                    }

                }));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void Country_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0)
                {
                    var cat = (CountryClass)e.AddedItems[0];

                    member.Country = cat.CountryId;

                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void Gender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0)
                {
                    var cat = (KeyValuePair<int, string>)e.AddedItems[0];
                    //reset category for the group
                    if (member != null)
                        member.Gender = (GenderEnum)cat.Key;

                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            progressIndicator.IsVisible = true;
            progressIndicator.Text = "Saving Member...";

            member.DerbyName = DerbyName.Text;
            member.PlayerNumber = DerbyNumber.Text;
            member.Firstname = FirstName.Text;
            member.LastName = LastName.Text;
            int feet = 0;
            if (Int32.TryParse(HeightFeet.Text, out feet))
                member.HeightFeet = feet;
            int ince = 0;
            if (Int32.TryParse(HeightInches.Text, out ince))
                member.HeightInches = ince;
            int weight = 0;
            if (Int32.TryParse(Weight.Text, out weight))
                member.WeightLbs = weight;

            member.DOB = DOB.Value.GetValueOrDefault();
            member.PhoneNumber = Phone.Text;
            member.Email = Email.Text;
            member.DayJob = Job.Text;

            member.StartedSkating = StartSkating.Value;
            member.StoppedSkating = StopSkating.Value;
            member.InsuranceNumWftda = WFTDANum.Text;
            member.InsuranceNumWftdaExpires = WFTDAExpires.Value;
            member.InsuranceNumUsars = USARSNum.Text;
            member.InsuranceNumUsarsExpires = USARSExpires.Value;
            member.InsuranceNumCRDI = CRDINum.Text;
            member.InsuranceNumCRDIExpires = CRDIExpires.Value;
            member.InsuranceNumOther = OtherNum.Text;
            member.InsuranceNumOtherExpires = OtherExpires.Value;
            member.IsProfileRemovedFromPublicView = HideProfileFromPublic.IsChecked.GetValueOrDefault();
            member.Address = Address1.Text;
            member.Address2 = Address2.Text;
            member.City = City.Text = member.City;
            member.State = State.Text = member.State;
            member.ZipCode = Zip.Text = member.ZipCode;

            Task.Run(new Action(() =>
            {

                UserMobileWP.SaveMemberEdit(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, member);

                Dispatcher.BeginInvoke(delegate
                {
                    progressIndicator.IsVisible = false;
                    ToastPrompt t = new ToastPrompt
                    {
                        Title = "Member Saved",
                        TextOrientation = System.Windows.Controls.Orientation.Vertical

                    };
                    t.Show();
                });
            }));

        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}