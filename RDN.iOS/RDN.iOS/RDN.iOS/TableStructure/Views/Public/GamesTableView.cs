using MonoTouch.Foundation;
using MonoTouch.UIKit;
using RDN.iOS.Classes.Error;
using RDN.iOS.Models;
using RDN.iOS.Screens.Public.Leagues;
using RDN.iOS.Screens.Public.Skaters;
using RDN.iOS.TableStructure.Cells;
using RDN.Portable.Config.Enums;
using RDN.Portable.Models.Json;
using RDN.Portable.Models.Json.Games;
using RDN.Portable.Models.Json.Public;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RDN.iOS.TableStructure.Views.Public
{

    public class GamesTableView : UITableViewSource
    {
        public int CellOn = 0;
        List<CurrentGameJson> tableItems;
        UINavigationController navigate;
        UIView _view;
        NSString cellIdentifier = new NSString("GamesTableView");
        public GamesTableView(List<CurrentGameJson> items, UINavigationController navigation, UIView view)
        {
            tableItems = items;
            navigate = navigation;
            _view = view;
        }
        public override int RowsInSection(UITableView tableview, int section)
        {
            return tableItems.Count;
        }
        public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
        {
            GamesTableCell cell = new GamesTableCell(cellIdentifier);
            try
            {
                cell = tableView.DequeueReusableCell(cellIdentifier) as GamesTableCell;

                if (cell == null)
                    cell = new GamesTableCell(cellIdentifier);

                UIImage team1 = null;
                UIImage team2 = null;

                if (!String.IsNullOrEmpty(tableItems[indexPath.Row].Team1LogoUrl))
                {
                    NSUrl nsUrl = new NSUrl(tableItems[indexPath.Row].Team1LogoUrl);
                    NSData data = NSData.FromUrl(nsUrl);
                    if (data != null)
                        team1 = new UIImage(data);
                }
                else
                    team1 = UIImage.FromFile("Images/icon.png");

                if (!String.IsNullOrEmpty(tableItems[indexPath.Row].Team2LogoUrl))
                {
                    NSUrl nsUrl = new NSUrl(tableItems[indexPath.Row].Team2LogoUrl);
                    NSData data = NSData.FromUrl(nsUrl);
                    if (data != null)
                        team2 = new UIImage(data);
                }
                else
                    team2 = UIImage.FromFile("Images/icon.png");

                cell.UpdateCell(tableItems[indexPath.Row].Team1Name, tableItems[indexPath.Row].Team2Name, tableItems[indexPath.Row].GameHeader, tableItems[indexPath.Row].Team1Score, tableItems[indexPath.Row].Team2Score, team1, team2);

                OnGotCell();
                CellOn = indexPath.Row;
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
            return cell;
        }
        public event EventHandler GotCell;
        public void OnGotCell()
        {
            EventHandler handler = GotCell;
            if (null != handler)
                handler(this, EventArgs.Empty);
        }
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            UIWebView web = new UIWebView(new RectangleF(0, 50, _view.Bounds.Width, _view.Bounds.Height));
            web.LoadRequest(new NSUrlRequest(new NSUrl(tableItems[indexPath.Row].GameUrl)));
            web.ScalesPageToFit = true;
            _view.AddSubview(web);
        }
        public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
        {
            //new UIAlertView("DetailDisclosureButton Touched"
            //    , tableItems[indexPath.Row].Title, null, "OK", null).Show();
        }
    }
}
