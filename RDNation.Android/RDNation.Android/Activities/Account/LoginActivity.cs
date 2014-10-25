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
using RDN.Mobile.Account;
using RDNation.Droid.Classes.Account;
using System.Threading;
using RDN.Portable.Account;
using RDN.Portable.Config.Enums;


namespace RDNation.Droid
{
    [Activity(Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class LoginActivity : LegacyBarActivity
    {

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                RequestWindowFeature(WindowFeatures.NoTitle);

                base.OnCreate(bundle);

                SetContentView(Resource.Layout.Login);
                LoggerMobile.Instance.logMessage("Opening LoginAcitvity", LoggerEnum.message);
                LegacyBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
                LegacyBar.SetHomeLogo(Resource.Drawable.icon);
                LegacyBarAction signUpAction = new DefaultLegacyBarAction(this, CreateSignUpIntent(), Resource.Drawable.social_add_person);
                LegacyBar.AddAction(signUpAction);
                LegacyBar.SeparatorColor = Color.Purple;
                AddHomeAction(typeof(Main), Resource.Drawable.icon);
                // Get our button from the layout resource,
                // and attach an event to it

                var loginBtn = FindViewById<Button>(Resource.Id.loginBtn);
                loginBtn.Click += loginBtn_Click;
                var signUpBtn = FindViewById<Button>(Resource.Id.signUpBtn);
                signUpBtn.Click += signUpBtn_Click;

            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);

            }
        }

        void signUpBtn_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(SignUpActivity));
        }

        void loginBtn_Click(object sender, EventArgs e)
        {
            var progressDialog = ProgressDialog.Show(this, "Please wait...", "Checking account info...", true);
            new Thread(new ThreadStart(delegate
            {
                try { 
                UserMobile user = new UserMobile();
                user.UserName = FindViewById<EditText>(Resource.Id.userName).Text;
                user.Password = FindViewById<EditText>(Resource.Id.password).Text;
                bool isAuthenticated = User.Authenticate(user, (Context)this);

                
                if (isAuthenticated)
                {
                    var main = new Intent(this, typeof(Main));
                    StartActivity(main);
                    RunOnUiThread(() => Toast.MakeText(this, "Logged In", ToastLength.Long).Show());
                }
                else
                {
                    RunOnUiThread(() =>
                        {
                            var text = FindViewById<TextView>(Resource.Id.warning);
                            text.Text = "Invalid Email or Password";
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


        private Intent CreateSignUpIntent()
        {
            var intent = new Intent(this, typeof(SignUpActivity));
            return intent;
        }


    }
}

