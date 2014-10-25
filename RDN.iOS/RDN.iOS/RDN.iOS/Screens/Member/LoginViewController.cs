using System;
using MonoTouch.UIKit;
using System.Drawing;
using RDN.iOS.TableStructure.Views;
using RDN.iOS.Models;
using System.Collections.Generic;
using MonoTouch.CoreGraphics;
using RDN.iOS.Classes.Account;
using RDN.iOS.Classes.UI;
using RDN.iOS.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.Portable.Settings;
using RDN.Portable.Account;

namespace RDN.iOS
{
    public class LoginViewController : UIViewController
    {
        UITableView table;
        UITextField userNameTxt;
        UITextField passwordTxt;
        UILabel warningTxt;
        LoadingView loading;

        public LoginViewController()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            try
            {
                this.Title = "Login to RDNation";
                View.Frame = UIScreen.MainScreen.Bounds;
                View.BackgroundColor = UIColor.White;
                View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                UILabel userNameLbl = new UILabel(new RectangleF(10, 70, View.Bounds.Width, 25));
                userNameLbl.Text = "Email:";
                View.AddSubview(userNameLbl);


                userNameTxt = new UITextField(new RectangleF(10, 100, View.Bounds.Width - 20, 35));
                userNameTxt.Layer.BorderWidth = 1;
                userNameTxt.Layer.MasksToBounds = true;
                userNameTxt.Layer.CornerRadius = 8;

                userNameTxt.Layer.BorderColor = UIColor.Purple.CGColor;
                userNameTxt.Text = SettingsMobile.Instance.User.UserName;
                View.AddSubview(userNameTxt);

                UILabel passwordLbl = new UILabel(new RectangleF(10, 145, View.Bounds.Width, 25));
                passwordLbl.Text = "Password:";
                View.AddSubview(passwordLbl);

                passwordTxt = new UITextField(new RectangleF(10, 175, View.Bounds.Width - 20, 35));
                passwordTxt.BorderStyle = UITextBorderStyle.RoundedRect;
                passwordTxt.Layer.BorderWidth = 1;
                passwordTxt.Layer.MasksToBounds = true;
                passwordTxt.Layer.CornerRadius = 8;
                passwordTxt.Layer.BorderColor = UIColor.Purple.CGColor;
                passwordTxt.SecureTextEntry = true;
                View.AddSubview(passwordTxt);

                UIButton loginBtn = new UIButton(new RectangleF(View.Bounds.Width / 2 - 50, 240, 100, 35));
                loginBtn.Layer.BorderWidth = 1;
                loginBtn.Layer.MasksToBounds = true;
                loginBtn.Layer.CornerRadius = 8;
                loginBtn.Layer.BorderColor = UIColor.Purple.CGColor;
                loginBtn.SetTitleColor(UIColor.Black, UIControlState.Normal);
                loginBtn.SetTitle("Login", UIControlState.Normal);
                loginBtn.TouchUpInside += loginBtn_TouchUpInside;
                View.AddSubview(loginBtn);

                warningTxt = new UILabel(new RectangleF(10, 210, View.Bounds.Width - 20, 35));
                warningTxt.TextColor = UIColor.Red;
                View.AddSubview(warningTxt);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }

        void loginBtn_TouchUpInside(object sender, EventArgs e)
        {
            try
            {
                loading = new LoadingView();
                loading.ShowActivity("logging in");
                SettingsMobile.Instance.User.UserName = userNameTxt.Text;
                SettingsMobile.Instance.User.Password = passwordTxt.Text;
                bool authenticate = User.Authenticate(new UserMobile() { UserName = userNameTxt.Text, Password = passwordTxt.Text });
                loading.Hide();
                if (authenticate)
                {
                    this.NavigationController.PushViewController(new MainViewController(), true);
                }
                else
                {
                    passwordTxt.Text = "";
                    warningTxt.Text = "Wrong login details. Try Again.";
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }
    }
}

