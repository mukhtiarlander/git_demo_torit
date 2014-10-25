using MonoTouch.Foundation;
using MonoTouch.UIKit;
using RDN.iOS.Classes.Error;
using RDN.iOS.Classes.Public;
using RDN.iOS.TableStructure.Datasources;
using RDN.iOS.TableStructure.Views;
using RDN.Portable.Config.Enums;
using RDN.Portable.Models.Json;
using RDN.Utilities.Dates;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RDN.iOS.Screens.Public.Skaters
{
    [Register("SkaterViewController")]
    public class SkaterViewController : UIViewController
    {
        SkaterJson _skater;
        UILabel wins;
        UILabel loses;
        UILabel leagueLink;
        UILabel Bio;
        UIImageView leagueImage;
        UITableView table;
        public SkaterViewController(SkaterJson skater)
        {
            _skater = skater;
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

                this.Title = _skater.DerbyName;


                View.Frame = UIScreen.MainScreen.Bounds;
                View.BackgroundColor = UIColor.White;
                View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

                UIScrollView scroll = new UIScrollView(new RectangleF(0, 10, View.Bounds.Width, View.Bounds.Height));
                scroll.ContentSize = new SizeF(View.Bounds.Width, 1000);

                View.AddSubview(scroll);

                UIImageView image = new UIImageView();
                NSUrl nsUrl = new NSUrl(_skater.ThumbUrl);
                NSData data = NSData.FromUrl(nsUrl);
                image.Frame = new RectangleF(5, 0, 100, 100);
                image.Image = new UIImage(data);
                scroll.AddSubview(image);

                Action<SkaterJson> skaters = new Action<SkaterJson>(UpdateAdapter);
                Skater.PullSkater(_skater.MemberId, skaters);

                UILabel number = new UILabel(new RectangleF(110, 0, 100, 20));
                number.Text = "# " + _skater.DerbyNumber;
                scroll.AddSubview(number);

                wins = new UILabel(new RectangleF(110, 20, 100, 20));
                wins.Text = "Wins: " + _skater.Wins;
                scroll.AddSubview(wins);

                loses = new UILabel(new RectangleF(110, 40, 100, 20));
                loses.Text = "Loses: " + _skater.Losses;
                scroll.AddSubview(loses);

                if (!String.IsNullOrEmpty(_skater.LeagueLogo))
                {
                    leagueImage = new UIImageView();
                    NSUrl nsUrlLeague = new NSUrl(_skater.LeagueLogo);
                    NSData dataLeague = NSData.FromUrl(nsUrlLeague);
                    leagueImage.Frame = new RectangleF(5, 110, 50, 50);
                    leagueImage.Image = new UIImage(dataLeague);
                    scroll.AddSubview(leagueImage);
                }

                leagueLink = new UILabel(new RectangleF(60, 110, 200, 50));
                leagueLink.Text = _skater.LeagueName;
                scroll.AddSubview(leagueLink);

                UILabel info = new UILabel(new RectangleF(5, 170, 200, 20));
                info.Font = UIFont.FromName(info.Font.Name, info.Font.PointSize + 1);
                info.Text = "Info:";
                scroll.AddSubview(info);

                List<GenericTableItem> items = new List<GenericTableItem>();
                items.Add(new GenericTableItem() { Key = "First Name: ", Value = _skater.FirstName });
                items.Add(new GenericTableItem() { Key = "DOB: ", Value = DateTimeExt.Age(_skater.DOB) + " - " + _skater.DOB.ToShortDateString() });
                items.Add(new GenericTableItem() { Key = "Weight: ", Value = _skater.Weight });
                items.Add(new GenericTableItem() { Key = "Height: ", Value = _skater.HeightFeet + "," + _skater.HeightInches });
                items.Add(new GenericTableItem() { Key = "Gender: ", Value = _skater.Gender });
                table = new UITableView(new RectangleF(0, 190, View.Bounds.Width, 150));
                table.RowHeight = 30;
                //table.row
                table.Source = new GenericTableView(items);
                table.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                scroll.AddSubview(table);


                UILabel BioTitle = new UILabel(new RectangleF(5, 350, 200, 20)) { };
                BioTitle.Font = UIFont.FromName(BioTitle.Font.Name, BioTitle.Font.PointSize + 1);
                BioTitle.Text = "Bio:";
                scroll.AddSubview(BioTitle);

                Bio = new UILabel(new RectangleF(5, 370, View.Bounds.Width - 10, 200));
                Bio.Text = _skater.Bio;
                Bio.Lines = 0;
                Bio.LineBreakMode = UILineBreakMode.WordWrap;
                scroll.AddSubview(Bio);




            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }

        }


        void UpdateAdapter(SkaterJson skater)
        {
            InvokeOnMainThread(() =>
            {
                try
                {
                    _skater = skater;
                    Bio.Text = _skater.Bio;
                    Bio.SizeToFit();
                    loses.Text = "Loses: " + _skater.Losses;
                    wins.Text = "Wins: " + _skater.Wins;
                    leagueLink.Text = _skater.LeagueName;
                    List<GenericTableItem> items = new List<GenericTableItem>();
                    items.Add(new GenericTableItem() { Key = "First Name: ", Value = _skater.FirstName });
                    items.Add(new GenericTableItem() { Key = "DOB: ", Value = DateTimeExt.Age(_skater.DOB) + " - " + _skater.DOB.ToShortDateString() });
                    items.Add(new GenericTableItem() { Key = "Weight: ", Value = _skater.Weight });
                    items.Add(new GenericTableItem() { Key = "Height: ", Value = _skater.HeightFeet + "," + _skater.HeightInches });
                    items.Add(new GenericTableItem() { Key = "Gender: ", Value = _skater.Gender });

                    table.Source = new GenericTableView(items);
                    table.ReloadData();
                    if (!String.IsNullOrEmpty(_skater.LeagueLogo))
                    {
                        leagueImage = new UIImageView();
                        NSUrl nsUrlLeague = new NSUrl(_skater.LeagueLogo);
                        NSData dataLeague = NSData.FromUrl(nsUrlLeague);
                        leagueImage.Frame = new RectangleF(5, 180, 100, 100);
                        leagueImage.Image = new UIImage(dataLeague);
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
