using MonoTouch.CoreLocation;
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
    [Register("EventViewController")]
    public class EventViewController : UIViewController
    {
        EventJson _event;
        UIButton leagueName;
        UIButton eventLocation;
        UIButton eventAddress;
        UILabel eventDateTime;
        UILabel eventDescription;
        UIButton eventTicketsUrl;
        UIButton eventUrl;
        UIButton eventRDNUrl;
        UIImageView leagueImage;

        UILabel eventName;
        UIViewController tab1, tab2, tab3;
        UITabBarController tabs;
        public EventViewController(EventJson skater)
        {
            _event = skater;
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
                base.ViewDidLoad();

                this.Title = _event.Name;

                View.Frame = UIScreen.MainScreen.Bounds;
                View.BackgroundColor = UIColor.White;
                View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

                UIImageView image = new UIImageView();
                if (!String.IsNullOrEmpty(_event.LogoUrl))
                {
                    NSUrl nsUrl = new NSUrl(_event.LogoUrl);
                    NSData data = NSData.FromUrl(nsUrl);
                    //put the image in center.
                    image.Frame = new RectangleF(View.Bounds.Width / 2 - 50, 70, 100, 100);
                    image.Image = new UIImage(data);
                }
                View.AddSubview(image);

                tabs = new UITabBarController();
                tabs.View.Frame = new RectangleF(0, 190, View.Bounds.Width, 380);
                tabs.View.Bounds = new RectangleF(0, 0, View.Bounds.Width, 380);

                UILabel label = new UILabel();

                tab1 = new UIViewController();
                tab1.Title = "Info";
                tab1.TabBarItem.SetTitleTextAttributes(new UITextAttributes() { Font = UIFont.FromName(label.Font.Name, label.Font.PointSize + 5) }, UIControlState.Normal);

                eventName = new UILabel(new RectangleF(0, 20, View.Bounds.Width, 40));
                eventName.TextAlignment = UITextAlignment.Center;
                eventName.Text = _event.Name;
                eventName.Font = UIFont.FromName(label.Font.Name, label.Font.PointSize + 5);
                eventName.TextColor = UIColor.Purple;
                tab1.View.AddSubview(eventName);

                leagueName = UIButton.FromType(UIButtonType.RoundedRect);
                leagueName.Frame = new RectangleF(0, 80, View.Bounds.Width, 35);
                //leagueName.TextAlignment = UITextAlignment.Center;
                leagueName.SetTitle(_event.OrganizersName, UIControlState.Normal);// = new NSAttributedString(_event.OrganizersName, underlineStyle: NSUnderlineStyle.Single);
                leagueName.TouchUpInside += leagueName_TouchUpInside;

                tab1.View.AddSubview(leagueName);

                eventLocation = UIButton.FromType(UIButtonType.RoundedRect);
                eventLocation.Frame = new RectangleF(0, 110, View.Bounds.Width, 35);
                eventLocation.SetTitle(_event.Location, UIControlState.Normal);
                eventLocation.TouchUpInside += eventLocation_TouchUpInside;
                tab1.View.AddSubview(eventLocation);


                eventAddress = UIButton.FromType(UIButtonType.RoundedRect);
                eventAddress.Frame = new RectangleF(0, 145, View.Bounds.Width, 35);
                eventAddress.SetTitle(_event.AddressHuman, UIControlState.Normal);
                eventAddress.TouchUpInside += eventLocation_TouchUpInside;
                tab1.View.AddSubview(eventAddress);



                eventDateTime = new UILabel(new RectangleF(0, 190, View.Bounds.Width, 35));
                eventDateTime.TextAlignment = UITextAlignment.Center;
                eventDateTime.Text = _event.DateTimeHuman;
                tab1.View.AddSubview(eventDateTime);



                //leagueName = new UILabel()

                tab2 = new UIViewController();
                tab2.Title = "Desc";
                //tab2.View.BackgroundColor = UIColor.Orange;
                //tab2.TabBarItem.Image = UIImage.FromFile("second.png");
                tab2.TabBarItem.SetTitleTextAttributes(new UITextAttributes() { Font = UIFont.FromName(label.Font.Name, label.Font.PointSize + 5) }, UIControlState.Normal);


                eventDescription = new UILabel(new RectangleF(10, 10, View.Bounds.Width - 20, 350));
                eventDescription.LineBreakMode = UILineBreakMode.WordWrap;
                eventDescription.Lines = 0;
                eventDescription.Text = _event.DescriptionNonHtml;
                tab2.View.AddSubview(eventDescription);

                tab3 = new UIViewController();
                tab3.Title = "Links";
                tab3.TabBarItem.SetTitleTextAttributes(new UITextAttributes() { Font = UIFont.FromName(label.Font.Name, label.Font.PointSize + 5) }, UIControlState.Normal);

                UILabel tickets = new UILabel(new RectangleF(10, 10, View.Bounds.Width, 20));
                tickets.Text = "Tickets:";
                tab3.View.AddSubview(tickets);


                eventTicketsUrl = UIButton.FromType(UIButtonType.RoundedRect);
                eventTicketsUrl.Frame = new RectangleF(10, 40, View.Bounds.Width - 20, 20);
                //leagueName.TextAlignment = UITextAlignment.Center;
                eventTicketsUrl.SetTitle(_event.TicketUrl, UIControlState.Normal);
                eventTicketsUrl.TouchUpInside += eventTicketsUrl_TouchUpInside;
                tab3.View.AddSubview(eventTicketsUrl);

                UILabel events = new UILabel(new RectangleF(10, 70, View.Bounds.Width, 20));
                events.Text = "Event:";
                tab3.View.AddSubview(events);


                eventUrl = UIButton.FromType(UIButtonType.RoundedRect);
                eventUrl.Frame = new RectangleF(10, 100, View.Bounds.Width - 20, 20);
                //leagueName.TextAlignment = UITextAlignment.Center;
                eventUrl.SetTitle(_event.EventUrl, UIControlState.Normal);
                eventUrl.TouchUpInside += eventUrl_TouchUpInside;
                tab3.View.AddSubview(eventUrl);

                UILabel rdnLink = new UILabel(new RectangleF(10, 130, View.Bounds.Width, 20));
                rdnLink.Text = "RDNation:";
                tab3.View.AddSubview(rdnLink);

                eventRDNUrl = UIButton.FromType(UIButtonType.RoundedRect);
                eventRDNUrl.Frame = new RectangleF(10, 160, View.Bounds.Width - 20, 20);
                //leagueName.TextAlignment = UITextAlignment.Center;
                eventRDNUrl.SetTitle(_event.RDNUrl, UIControlState.Normal);
                eventRDNUrl.TouchUpInside += eventRDNUrl_TouchUpInside;
                tab3.View.AddSubview(eventRDNUrl);


                var tabsContainer = new UIViewController[] {
                                tab1, tab2, tab3
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

        void eventTicketsUrl_TouchUpInside(object sender, EventArgs e)
        {
            UIWebView web = new UIWebView(View.Bounds);
            web.LoadRequest(new NSUrlRequest(new NSUrl(_event.TicketUrl)));
            web.ScalesPageToFit = true;
            View.AddSubview(web);
        }

        void eventUrl_TouchUpInside(object sender, EventArgs e)
        {
            UIWebView web = new UIWebView(View.Bounds);
            web.LoadRequest(new NSUrlRequest(new NSUrl(_event.EventUrl)));
            web.ScalesPageToFit = true;
            View.AddSubview(web);
        }

        void eventRDNUrl_TouchUpInside(object sender, EventArgs e)
        {
            UIWebView web = new UIWebView(View.Bounds);
            web.LoadRequest(new NSUrlRequest(new NSUrl(_event.RDNUrl)));
            web.ScalesPageToFit = true;
            View.AddSubview(web);
        }

        void eventLocation_TouchUpInside(object sender, EventArgs e)
        {
            short version = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);
            string lmapLocation = _event.Address;
            string mapLocation = _event.Address;


            if (mapLocation.Trim().Length == 0) return;

            if (version >= 5)
            {
                NSUrl url = new NSUrl("http://maps.google.com/maps?q=" + lmapLocation);
                UIApplication.SharedApplication.OpenUrl(url);
            }
            else
            {
                //ActivityThread.Start("Loading Location");
                //string sw = mapLocation;
                //CLGeocoder clg = new CLGeocoder();
                //clg.GeocodeAddress(sw, HandleCLGeocodeCompletionHandler);

            }
        }

        void leagueName_TouchUpInside(object sender, EventArgs e)
        {
            this.NavigationController.PushViewController(new LeagueViewController(new LeagueJsonDataTable() { LeagueName = _event.OrganizersName, LeagueId = _event.OrganizersId }), true);
        }


    }
}
