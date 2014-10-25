using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using RDN.Utilities.Error;
using RDNation.Droid.Classes;
using Android.Net;
using System.Threading.Tasks;
using RDN.Utilities.Util;
using LegacyBar.Library.BarBase;
using Android.Graphics;
using LegacyBar.Library.BarActions;
using Android.Content.PM;
using System.Threading;
using RDN.Mobile.Account;
using RDNation.Droid.Classes.Account;
using RDN.Portable.Account;
using RDN.Portable.Config.Enums;


namespace RDNation.Droid
{
    [Activity(Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SignUpActivity : LegacyBarActivity
    {
        //first selected in wheel
        int positionSelected = 1;
        int genderSelected = 1;
        bool doYouDerby;
        Spinner spinnerGender;
        Spinner spinnerPosition;
        EditText DerbyName;
        EditText FirstName;
        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                RequestWindowFeature(WindowFeatures.NoTitle);

                base.OnCreate(bundle);
                LoggerMobile.Instance.logMessage("Opening SignUpAcitivty", LoggerEnum.message);
                SetContentView(Resource.Layout.Signup);

                LegacyBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
                LegacyBar.SetHomeLogo(Resource.Drawable.icon);

                LegacyBar.SeparatorColor = Color.Purple;
                AddHomeAction(typeof(LoginActivity), Resource.Drawable.icon);

                DerbyName = FindViewById<EditText>(Resource.Id.derbyName);
                FirstName = FindViewById<EditText>(Resource.Id.firstName);
                spinnerGender = FindViewById<Spinner>(Resource.Id.spinnerGender);
                spinnerGender.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerGender_ItemSelected);
                var adapterGender = ArrayAdapter.CreateFromResource(
                        this, Resource.Array.gender_array, Resource.Layout.simple_spinner_item);
                adapterGender.SetDropDownViewResource(Resource.Layout.simple_spinner_dropdown_item);
                spinnerGender.Adapter = adapterGender;

                spinnerPosition = FindViewById<Spinner>(Resource.Id.spinnerPosition);
                spinnerPosition.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerPosition_ItemSelected);
                var adapterPosition = ArrayAdapter.CreateFromResource(
                        this, Resource.Array.position_array, Resource.Layout.simple_spinner_item);
                adapterPosition.SetDropDownViewResource(Resource.Layout.simple_spinner_dropdown_item);
                spinnerPosition.Adapter = adapterPosition;

                var signUpBtn = FindViewById<Button>(Resource.Id.signUpBtn);
                signUpBtn.Click += loginBtn_Click;

                var radioYesBtn = FindViewById<RadioButton>(Resource.Id.radio_yes);
                var radioNoBtn = FindViewById<RadioButton>(Resource.Id.radio_no);
                radioNoBtn.Click += RadioButtonClick;
                radioYesBtn.Click += RadioButtonClick;
                //most people that sign up will be in derby.
                doYouDerby = true;


            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);

            }
        }
        private void RadioButtonClick(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Text == "Yes" && rb.Checked)
            {
                doYouDerby = true;
                RunOnUiThread(() =>
                {
                    FirstName.Visibility = ViewStates.Visible;
                    DerbyName.Visibility = ViewStates.Visible;
                    spinnerGender.Visibility = ViewStates.Visible;
                    spinnerPosition.Visibility = ViewStates.Visible;
                });
            }
            else if (rb.Text == "No" && rb.Checked)
            {
                RunOnUiThread(() =>
                {
                    FirstName.Visibility = ViewStates.Gone;
                    DerbyName.Visibility = ViewStates.Gone;
                    spinnerGender.Visibility = ViewStates.Gone;
                    spinnerPosition.Visibility = ViewStates.Gone;
                });
                doYouDerby = false;
            }
        }
        void loginBtn_Click(object sender, EventArgs e)
        {
            var progressDialog = ProgressDialog.Show(this, "Please wait...", "Creating User Account...", true);
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    UserMobile user = new UserMobile();
                    user.UserName = FindViewById<EditText>(Resource.Id.emailAddress).Text;
                    user.Password = FindViewById<EditText>(Resource.Id.password).Text;
                    user.DerbyName = FindViewById<EditText>(Resource.Id.derbyName).Text;
                    user.FirstName = FindViewById<EditText>(Resource.Id.firstName).Text;
                    user.Gender = genderSelected;
                    user.Position = positionSelected;
                    user.IsConnectedToDerby = doYouDerby;
                    var isAuthenticated = User.SignUp(user, (Context)this);

                    if (isAuthenticated.DidSignUp)
                    {
                        var main = new Intent(this, typeof(Main));
                        StartActivity(main);
                        RunOnUiThread(() => Toast.MakeText(this, "Signed Up & Logged In", ToastLength.Long).Show());
                    }
                    else
                    {
                        RunOnUiThread(() =>
                        {
                            var text = FindViewById<TextView>(Resource.Id.warning);
                            text.Text = isAuthenticated.Error;
                        });
                    }
                    RunOnUiThread(() => progressDialog.Hide());
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);

                }
            })).Start();
        }
        private Intent CreateShareIntent()
        {
            var intent = new Intent(this, typeof(SignUpActivity));
            return intent;
        }
        private void spinnerGender_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                Spinner spinner = (Spinner)sender;
                if (e.Position != 0)
                    genderSelected = e.Position;
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);

            }
        }
        private void spinnerPosition_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                Spinner spinner = (Spinner)sender;
                if (e.Position != 0)
                    positionSelected = e.Position;
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);

            }
        }


    }
}

