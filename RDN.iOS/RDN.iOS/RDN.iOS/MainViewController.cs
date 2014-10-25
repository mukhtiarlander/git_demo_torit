using System;
using MonoTouch.UIKit;
using System.Drawing;
using RDN.iOS.TableStructure.Views;
using RDN.iOS.Models;
using System.Collections.Generic;
using RDN.iOS.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.Portable.Settings;
using RDN.iOS.TableStructure.Views.League;

namespace RDN.iOS
{
    public class MainViewController : UIViewController
    {
        UITableView table, leagueTable;
        UITabBarController tabs;
        UIViewController tab1, tab2;
        public MainViewController()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            try
            {
                this.Title = "Welcome to RDNation";

                View.Frame = UIScreen.MainScreen.Bounds;
                View.BackgroundColor = UIColor.White;
                View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                table = new UITableView(new RectangleF(0, 0, View.Bounds.Width, 260)); // defaults to Plain style

                UILabel label = new UILabel();

                List<MainModel> model = new List<MainModel>();
                model.Add(new MainModel() { Title = "Scores/Games", Description = "Roller Derby Games & Scores", ImageName = "games.png", ScreenType = Classes.MainScreenEnum.Games });
                model.Add(new MainModel() { Title = "Events", Description = "Public Calendar Events", ImageName = "calendar.png", ScreenType = Classes.MainScreenEnum.Events });
                model.Add(new MainModel() { Title = "Leagues", Description = "Public Leagues Worldwide", ImageName = "leagues.png", ScreenType = Classes.MainScreenEnum.Leagues });
                model.Add(new MainModel() { Title = "Skaters", Description = "Public Skaters", ImageName = "skater.png", ScreenType = Classes.MainScreenEnum.Skaters });
                model.Add(new MainModel() { Title = "Shops", Description = "Go Shopping for Derby Gear", ImageName = "shop.png", ScreenType = Classes.MainScreenEnum.Shop });
                table.Source = new MainTableView(model, this.NavigationController);

                if (!SettingsMobile.Instance.User.IsLoggedIn)
                {
                    Add(table);
                    this.NavigationItem.SetRightBarButtonItem(new UIBarButtonItem("Login", UIBarButtonItemStyle.Plain, (sender, args) =>
                    {
                        this.NavigationController.PushViewController(new LoginViewController(), true);
                    })
        , true);
                    this.NavigationItem.SetLeftBarButtonItem(new UIBarButtonItem("SignUp", UIBarButtonItemStyle.Plain, (sender, args) =>
                    {
                        this.NavigationController.PushViewController(new SignUpViewController(), true);
                    })
        , true);
                }
                else
                {
                    leagueTable = new UITableView(new RectangleF(0, 0, View.Bounds.Width, 50)); // defaults to Plain style
                    List<MainModel> leagueModel = new List<MainModel>();
                    leagueModel.Add(new MainModel() { Title = "Forum", Description = "The League Forum", ImageName = "forum.png", ScreenType = Classes.MainScreenEnum.LeagueForum });
                    leagueTable.Source = new LeagueTableView(leagueModel, this.NavigationController);

                    tabs = new UITabBarController();
                    tabs.View.Frame = new RectangleF(0, 70, View.Bounds.Width, View.Bounds.Height);
                    tabs.View.Bounds = new RectangleF(0, 70, View.Bounds.Width, View.Bounds.Height);
                    tab1 = new UIViewController();
                    tab1.Title = "League";
                    tab1.TabBarItem.SetTitleTextAttributes(new UITextAttributes() { Font = UIFont.FromName(label.Font.Name, label.Font.PointSize + 5) }, UIControlState.Normal);
                    tab1.View.AddSubview(leagueTable);

                    tab2 = new UIViewController();
                    tab2.Title = "Public";
                    tab2.TabBarItem.SetTitleTextAttributes(new UITextAttributes() { Font = UIFont.FromName(label.Font.Name, label.Font.PointSize + 5) }, UIControlState.Normal);
                    tab2.View.AddSubview(table);

                    var tabsContainer = new UIViewController[] {
                                tab1, tab2
                        };

                    tabs.ViewControllers = tabsContainer;

                    Add(tabs.View);
                }


            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }
    }
}

