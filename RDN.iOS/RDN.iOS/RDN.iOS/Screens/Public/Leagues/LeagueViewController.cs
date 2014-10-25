using MonoTouch.Foundation;
using MonoTouch.UIKit;
using RDN.iOS.Classes.Error;
using RDN.iOS.Classes.Public;
using RDN.iOS.TableStructure.Datasources;
using RDN.iOS.TableStructure.Views;
using RDN.Portable.Config.Enums;
using RDN.Portable.Models.Json;
using RDN.Portable.Models.Json.Calendar;
using RDN.Portable.Models.Json.Public;
using RDN.Utilities.Dates;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RDN.iOS.Screens.Public.Leagues
{
    [Register("LeagueViewController")]
    public class LeagueViewController : UIViewController
    {
        LeagueJsonDataTable _league;
        UILabel membersCount;
        //UILabel loses;
        UILabel leagueLink;
        UILabel Bio;
        UIImageView leagueImage;
        UITableView skaterTable;
        UITableView eventTable;
        SkatersJson skatersArray;
        EventsJson eventsArray;

        UILabel cityState;
        UIViewController tab1, tab2;
        UITabBarController tabs;
        UIImageView image;
        public LeagueViewController(LeagueJsonDataTable skater)
        {
            _league = skater;
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            try
            {
                //View = new UniversalView();

                base.ViewDidLoad();

                this.Title = _league.LeagueName;


                View.Frame = UIScreen.MainScreen.Bounds;
                View.BackgroundColor = UIColor.White;
                View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

                //UIScrollView scroll = new UIScrollView(new RectangleF(0, 10, View.Bounds.Width, View.Bounds.Height));
                //scroll.ContentSize = new SizeF(View.Bounds.Width, 1000);

                //View.AddSubview(scroll);

                image = new UIImageView();
                if (!String.IsNullOrEmpty(_league.LogoUrlThumb))
                {
                    NSUrl nsUrl = new NSUrl(_league.LogoUrlThumb);
                    NSData data = NSData.FromUrl(nsUrl);
                    image.Frame = new RectangleF(0, 70, 100, 100);
                    image.Image = new UIImage(data);
                }
                View.AddSubview(image);

                Action<LeagueJsonDataTable> leaguePull = new Action<LeagueJsonDataTable>(UpdateAdapter);
                RDN.iOS.Classes.Public.League.PullLeague(_league.LeagueId, leaguePull);

                Action<SkatersJson> skaters = new Action<SkatersJson>(UpdateAdapterSkaters);
                Skater.PullSkaters(_league.LeagueId, skaters);

                Action<EventsJson> events = new Action<EventsJson>(UpdateAdapterEvents);
                RDN.iOS.Classes.Public.League.PullLeagueEvents(_league.LeagueId, events);

                cityState = new UILabel(new RectangleF(110, 70, View.Bounds.Width, 20));
                cityState.Text = _league.City + ", " + _league.State;
                View.AddSubview(cityState);

                membersCount = new UILabel(new RectangleF(110, 90, 200, 20));
                membersCount.Text = "";
                View.AddSubview(membersCount);


                tabs = new UITabBarController();
                tabs.View.Frame = new RectangleF(0, 190, View.Bounds.Width, 380);
                tabs.View.Bounds = new RectangleF(0, 0, View.Bounds.Width, 380);
                tab1 = new UIViewController();

                tab1.Title = "Roster";
                //tab1.View.Bounds = new RectangleF(0, 100, View.Bounds.Width, tabs.View.Bounds.Height);
                skatersArray = new SkatersJson();
                skaterTable = new UITableView(new RectangleF(0, 0, tabs.View.Bounds.Width, tabs.View.Bounds.Height - tabs.TabBar.Bounds.Height));
                skaterTable.Source = new SkatersTableView(skatersArray.Skaters, this.NavigationController);
                tab1.View.AddSubview(skaterTable);
                tab1.TabBarItem.SetTitleTextAttributes(new UITextAttributes() { Font = UIFont.FromName(membersCount.Font.Name, membersCount.Font.PointSize + 5) }, UIControlState.Normal);


                tab2 = new UIViewController();
                eventsArray = new EventsJson();
                eventTable = new UITableView(new RectangleF(0, 0, tabs.View.Bounds.Width, tabs.View.Bounds.Height - tabs.TabBar.Bounds.Height));
                eventTable.Source = new EventsTableView(eventsArray.Events, this.NavigationController);
                eventTable.RowHeight = 80;
                tab2.View.AddSubview(eventTable);
                tab2.Title = "Schedule";
                tab2.View.BackgroundColor = UIColor.Orange;
                //tab2.TabBarItem.Image = UIImage.FromFile("second.png");
                tab2.TabBarItem.SetTitleTextAttributes(new UITextAttributes() { Font = UIFont.FromName(membersCount.Font.Name, membersCount.Font.PointSize + 5) }, UIControlState.Normal);

                var tabsContainer = new UIViewController[] {
                                tab1, tab2
                        };

                tabs.ViewControllers = tabsContainer;
                //tabs.TabBar.Bounds = new RectangleF(0, 0, View.Bounds.Width, 50);

                View.AddSubview(tabs.View);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }

        }
        void UpdateAdapterEvents(EventsJson skater)
        {
            InvokeOnMainThread(() =>
            {
                try
                {
                    eventsArray.Events.AddRange(skater.Events);
                    eventTable.Source = new EventsTableView(eventsArray.Events, this.NavigationController);
                    eventTable.ReloadData();
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
                }
            });
        }
        void UpdateAdapterSkaters(SkatersJson skater)
        {
            InvokeOnMainThread(() =>
            {
                try
                {
                    membersCount.Text = "Members: " + skater.Skaters.Count;

                    skatersArray.Skaters.AddRange(skater.Skaters);

                    skaterTable.Source = new SkatersTableView(skatersArray.Skaters, this.NavigationController);
                    skaterTable.ReloadData();
                    //loading.Hide();

                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
                }
            });
        }

        void UpdateAdapter(LeagueJsonDataTable skater)
        {
            InvokeOnMainThread(() =>
            {
                try
                {
                    _league = skater;
                    cityState.Text = _league.City + ", " + _league.State;

                    if (!String.IsNullOrEmpty(_league.LogoUrlThumb))
                    {
                        NSUrl nsUrl = new NSUrl(_league.LogoUrlThumb);
                        NSData data = NSData.FromUrl(nsUrl);
                        image.Frame = new RectangleF(0, 70, 100, 100);
                        image.Image = new UIImage(data);
                    }

                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
                }
            });



        }
    }
}
