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
using RDN.Mobile.Database;

namespace RDN.iOS
{
    public class SignUpViewController : UIViewController
    {
        UITableView table;
        UITextField userNameTxt;
        UITextField passwordTxt;
        UITextField derbyNameTxt;
        UITextField firstNameTxt;

        UILabel GenderLbl;
        UILabel PositionLbl;
        UIPickerView Gender;
        UIPickerView Position;
        UILabel derbyNameLbl;
        UILabel firstNameLbl;
        UILabel warningTxt;
        LoadingView loading;
        UISwitch swith;

        UIButton loginBtn;

        public SignUpViewController()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            try
            {
                this.Title = "SignUp to RDNation";
                View.Frame = UIScreen.MainScreen.Bounds;
                View.BackgroundColor = UIColor.White;
                View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

                UIScrollView scroll = new UIScrollView(new RectangleF(0, 0, View.Bounds.Width, View.Bounds.Height));
                scroll.ContentSize = new SizeF(View.Bounds.Width, 1000);

                swith = new UISwitch(new RectangleF(10, 10, 100, 25));
                swith.SetState(true, false);
                swith.TouchUpInside += swith_TouchUpInside;
                scroll.AddSubview(swith);

                UILabel doYouDerbyLbl = new UILabel(new RectangleF(80, 15, 200, 25));
                doYouDerbyLbl.Text = "Do You Derby?";
                scroll.AddSubview(doYouDerbyLbl);

                UILabel userNameLbl = new UILabel(new RectangleF(10, 50, View.Bounds.Width, 25));
                userNameLbl.Text = "Email:";
                scroll.AddSubview(userNameLbl);

                userNameTxt = new UITextField(new RectangleF(10, 75, View.Bounds.Width - 20, 35));
                userNameTxt.Layer.BorderWidth = 1;
                userNameTxt.Layer.MasksToBounds = true;
                userNameTxt.Layer.CornerRadius = 8;
                userNameTxt.Layer.BorderColor = UIColor.Purple.CGColor;
                userNameTxt.Text = SettingsMobile.Instance.User.UserName;
                scroll.AddSubview(userNameTxt);

                UILabel passwordLbl = new UILabel(new RectangleF(10, 110, View.Bounds.Width, 25));
                passwordLbl.Text = "Password:";
                scroll.AddSubview(passwordLbl);

                passwordTxt = new UITextField(new RectangleF(10, 135, View.Bounds.Width - 20, 35));
                passwordTxt.BorderStyle = UITextBorderStyle.RoundedRect;
                passwordTxt.Layer.BorderWidth = 1;
                passwordTxt.Layer.MasksToBounds = true;
                passwordTxt.Layer.CornerRadius = 8;
                passwordTxt.Layer.BorderColor = UIColor.Purple.CGColor;
                //passwordTxt.SecureTextEntry = true;
                scroll.AddSubview(passwordTxt);

                derbyNameLbl = new UILabel(new RectangleF(10, 170, View.Bounds.Width, 25));
                derbyNameLbl.Text = "Derby Name:";
                scroll.AddSubview(derbyNameLbl);

                derbyNameTxt = new UITextField(new RectangleF(10, 195, View.Bounds.Width - 20, 35));
                derbyNameTxt.BorderStyle = UITextBorderStyle.RoundedRect;
                derbyNameTxt.Layer.BorderWidth = 1;
                derbyNameTxt.Layer.MasksToBounds = true;
                derbyNameTxt.Layer.CornerRadius = 8;
                derbyNameTxt.Layer.BorderColor = UIColor.Purple.CGColor;
                //passwordTxt.SecureTextEntry = true;
                scroll.AddSubview(derbyNameTxt);

                firstNameLbl = new UILabel(new RectangleF(10, 230, View.Bounds.Width, 25));
                firstNameLbl.Text = "First Name:";
                scroll.AddSubview(firstNameLbl);

                firstNameTxt = new UITextField(new RectangleF(10, 255, View.Bounds.Width - 20, 35));
                firstNameTxt.BorderStyle = UITextBorderStyle.RoundedRect;
                firstNameTxt.Layer.BorderWidth = 1;
                firstNameTxt.Layer.MasksToBounds = true;
                firstNameTxt.Layer.CornerRadius = 8;
                firstNameTxt.Layer.BorderColor = UIColor.Purple.CGColor;
                scroll.AddSubview(firstNameTxt);

                loginBtn = new UIButton(new RectangleF(View.Bounds.Width / 2 - 50, 320, 100, 35));
                loginBtn.Layer.BorderWidth = 1;
                loginBtn.Layer.MasksToBounds = true;
                loginBtn.Layer.CornerRadius = 8;
                loginBtn.Layer.BorderColor = UIColor.Purple.CGColor;
                loginBtn.SetTitleColor(UIColor.Black, UIControlState.Normal);
                loginBtn.SetTitle("Sign Up", UIControlState.Normal);
                loginBtn.TouchUpInside += loginBtn_TouchUpInside;
                scroll.AddSubview(loginBtn);

                warningTxt = new UILabel(new RectangleF(10, 290, View.Bounds.Width - 20, 35));
                warningTxt.TextColor = UIColor.Red;
                scroll.AddSubview(warningTxt);

                View.AddSubview(scroll);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }

        void swith_TouchUpInside(object sender, EventArgs e)
        {
            derbyNameTxt.Hidden = !swith.On;
            firstNameTxt.Hidden = !swith.On;
            derbyNameLbl.Hidden = !swith.On;
            firstNameLbl.Hidden = !swith.On;
            if (swith.On)
            {
                loginBtn.Frame = new RectangleF(View.Bounds.Width / 2 - 50, 320, 100, 35);
            }
            else
                loginBtn.Frame = new RectangleF(View.Bounds.Width / 2 - 50, 240, 100, 35);
        }

        void loginBtn_TouchUpInside(object sender, EventArgs e)
        {
            try
            {
                if (userNameTxt.Text.Length < 5)
                    userNameTxt.Layer.BorderColor = UIColor.Red.CGColor;
                else
                    userNameTxt.Layer.BorderColor = UIColor.Purple.CGColor;

                if (passwordTxt.Text.Length < 5)
                    passwordTxt.Layer.BorderColor = UIColor.Red.CGColor;
                else
                    passwordTxt.Layer.BorderColor = UIColor.Purple.CGColor;

                if (derbyNameTxt.Text.Length < 3)
                    derbyNameTxt.Layer.BorderColor = UIColor.Red.CGColor;
                else
                    derbyNameTxt.Layer.BorderColor = UIColor.Purple.CGColor;

                if (firstNameTxt.Text.Length < 3)
                    firstNameTxt.Layer.BorderColor = UIColor.Red.CGColor;
                else
                    firstNameTxt.Layer.BorderColor = UIColor.Purple.CGColor;

                if (userNameTxt.Text.Length > 4 && passwordTxt.Text.Length > 4 && derbyNameTxt.Text.Length > 2 && firstNameTxt.Text.Length > 2)
                {
                    loading = new LoadingView();
                    loading.ShowActivity("signing up");
                    SettingsMobile.Instance.User.UserName = userNameTxt.Text;
                    SettingsMobile.Instance.User.Password = passwordTxt.Text;
                    var authenticate = User.SignUp(new UserMobile() { IsConnectedToDerby = swith.On, UserName = userNameTxt.Text, Password = passwordTxt.Text, FirstName = firstNameTxt.Text, DerbyName = derbyNameTxt.Text, Position = 1, Gender = 1 });
                    loading.Hide();
                    if (authenticate.DidSignUp)
                    {
                        SettingsMobile.Instance.User = authenticate;
                        new SqlFactory().DeleteProfile().InsertProfile(new Mobile.Database.Account.SqlAccount(authenticate));
                        this.NavigationController.PushViewController(new MainViewController(), true);
                    }
                    else
                    {
                        warningTxt.Text = authenticate.Error;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }
        //public class StatusPickerViewModel : UIPickerViewModel
        //{
        //    public override int GetComponentCount(UIPickerView picker)
        //    {
        //        return 1;
        //    }

        //    public override int GetRowsInComponent(UIPickerView picker, int component)
        //    {
        //        return 5;
        //    }

        //    public override string GetTitle(UIPickerView picker, int row, int component)
        //    {

        //        return "Component " + row.ToString();
        //    }
        //}
    }
}

