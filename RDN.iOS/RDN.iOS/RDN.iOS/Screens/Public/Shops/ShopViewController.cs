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
using RDN.Portable.Models.Json.Shop;
using RDN.Utilities.Dates;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RDN.iOS.Screens.Public.Leagues
{
    [Register("ShopViewController")]
    public class ShopViewController : UIViewController
    {
        ShopItemJson _shop;
        UIViewController tab1, tab2, tab3;
        UITabBarController tabs;
        UIImageView image;
        UILabel itemName;
        UILabel orderFrom;
        UILabel price;
        UIButton orderNowBtn;

        UILabel descriptionLabel;
        UILabel descriptionTextLabel;

        UILabel detailsLabel;
        UILabel detailsTextLabel;
        UITableView table;
        public ShopViewController(ShopItemJson shop)
        {
            _shop = shop;
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

                this.Title = _shop.Name;


                View.Frame = UIScreen.MainScreen.Bounds;
                View.BackgroundColor = UIColor.White;
                View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

                tabs = new UITabBarController();
                tabs.View.Frame = new RectangleF(0, 0, View.Bounds.Width, View.Bounds.Height);
                tabs.View.Bounds = new RectangleF(0, 0, View.Bounds.Width, View.Bounds.Height);
                tab1 = new UIViewController();

                tab1.Title = "Overview";
                image = new UIImageView();
                if (!String.IsNullOrEmpty(_shop.FirstPhotoUrl))
                {
                    NSUrl nsUrl = new NSUrl(_shop.FirstPhotoUrl);
                    NSData data = NSData.FromUrl(nsUrl);
                    //centers image.
                    image.Frame = new RectangleF(View.Bounds.Width / 2 - 100, 70, 200, 200);
                    image.Image = new UIImage(data);
                }
                tab1.View.AddSubview(image);

                itemName = new UILabel(new RectangleF(0, 280, View.Bounds.Width, 45));
                itemName.Text = _shop.Name;
                itemName.Lines = 0;
                itemName.LineBreakMode = UILineBreakMode.WordWrap;
                itemName.TextAlignment = UITextAlignment.Center;
                itemName.TextColor = UIColor.Purple;
                itemName.Font = UIFont.FromName(itemName.Font.Name, itemName.Font.PointSize + 2);
                tab1.View.AddSubview(itemName);

                orderFrom = new UILabel(new RectangleF(10, 325, View.Bounds.Width, 25));
                orderFrom.Text = _shop.SoldByHuman;
                tab1.View.AddSubview(orderFrom);

                price = new UILabel(new RectangleF(10, 350, View.Bounds.Width, 25));
                price.Text = "$" + _shop.Price.ToString("N2");
                tab1.View.AddSubview(price);

                orderNowBtn = new UIButton(new RectangleF(0, 400, View.Bounds.Width, 45));
                orderNowBtn.SetTitle("Order Now", UIControlState.Normal);
                orderNowBtn.BackgroundColor = UIColor.Purple;
                orderNowBtn.SetTitleColor(UIColor.White, UIControlState.Normal);
                orderNowBtn.Font = UIFont.FromName(price.Font.Name, price.Font.PointSize + 3);
                orderNowBtn.TouchUpInside += orderNowBtn_TouchUpInside;
                tab1.View.AddSubview(orderNowBtn);
                tab1.TabBarItem.SetTitleTextAttributes(new UITextAttributes() { Font = UIFont.FromName(orderFrom.Font.Name, orderFrom.Font.PointSize + 5) }, UIControlState.Normal);


                tab2 = new UIViewController();
                tab2.Title = "Details";
                UIScrollView scrollTab2 = new UIScrollView(new RectangleF(0, 0, View.Bounds.Width, View.Bounds.Height - 70));

                descriptionLabel = new UILabel(new RectangleF(10, 70, View.Bounds.Width, 25));
                descriptionLabel.Text = "Description:";
                descriptionLabel.Font = UIFont.FromName(descriptionLabel.Font.Name, descriptionLabel.Font.PointSize + 2);
                scrollTab2.AddSubview(descriptionLabel);

                descriptionTextLabel = new UILabel(new RectangleF(10, 100, View.Bounds.Width, 25));

                descriptionTextLabel.Text = _shop.Description;
                scrollTab2.AddSubview(descriptionTextLabel);

                detailsLabel = new UILabel(new RectangleF(10, 130, View.Bounds.Width, 25));
                detailsLabel.Font = UIFont.FromName(detailsLabel.Font.Name, detailsLabel.Font.PointSize + 2);
                detailsLabel.Text = "Notes From Seller:";
                scrollTab2.AddSubview(detailsLabel);

                detailsTextLabel = new UILabel(new RectangleF(10, 160, View.Bounds.Width - 10, View.Bounds.Height + 200));
                detailsTextLabel.Text = _shop.NotesNonHtml;
                detailsTextLabel.Lines = 0;
                detailsTextLabel.LineBreakMode = UILineBreakMode.WordWrap;
                scrollTab2.AddSubview(detailsTextLabel);
                tab2.View.AddSubview(scrollTab2);


                tab2.TabBarItem.SetTitleTextAttributes(new UITextAttributes() { Font = UIFont.FromName(orderFrom.Font.Name, orderFrom.Font.PointSize + 5) }, UIControlState.Normal);

                tab3 = new UIViewController();
                table = new UITableView(new RectangleF(10, 70, View.Bounds.Width - 10, View.Bounds.Height - 120));
                table.Source = new ImagesTableView(_shop.PhotoUrlsThumbs, this.NavigationController);
                table.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                table.RowHeight = 215;
                tab3.View.AddSubview(table);
                tab3.Title = "Images";
                tab3.TabBarItem.SetTitleTextAttributes(new UITextAttributes() { Font = UIFont.FromName(orderFrom.Font.Name, orderFrom.Font.PointSize + 5) }, UIControlState.Normal);


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

        void orderNowBtn_TouchUpInside(object sender, EventArgs e)
        {
            UIWebView web = new UIWebView(new RectangleF(0, 50, View.Bounds.Width, View.Bounds.Height));
            web.LoadRequest(new NSUrlRequest(new NSUrl(_shop.RDNUrl)));
            web.ScalesPageToFit = true;
            View.AddSubview(web);
        }




    }
}
