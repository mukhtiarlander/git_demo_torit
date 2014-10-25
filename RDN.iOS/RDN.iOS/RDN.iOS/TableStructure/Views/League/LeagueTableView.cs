using MonoTouch.Foundation;
using MonoTouch.UIKit;
using RDN.iOS.Classes;
using RDN.iOS.Classes.League;
using RDN.iOS.Models;
using RDN.iOS.Screens.League;
using RDN.iOS.Screens.Public.Events;
using RDN.iOS.Screens.Public.Games;
using RDN.iOS.Screens.Public.Skaters;
using RDN.iOS.TableStructure.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.iOS.TableStructure.Views.League
{

    public class LeagueTableView : UITableViewSource
    {

        List<MainModel> tableItems;
        UINavigationController navigate;
        NSString cellIdentifier = new NSString("MainTableCell");
        public LeagueTableView(List<MainModel> items, UINavigationController navigation)
        {
            tableItems = items;
            navigate = navigation;
        }
        public override int RowsInSection(UITableView tableview, int section)
        {
            return tableItems.Count;
        }
        public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
        {


            MainTableCell cell = tableView.DequeueReusableCell(cellIdentifier) as MainTableCell;
            if (cell == null)
                cell = new MainTableCell(cellIdentifier);
            cell.UpdateCell(tableItems[indexPath.Row].Title, tableItems[indexPath.Row].Description, UIImage.FromFile("Images/" + tableItems[indexPath.Row].ImageName));
            return cell;
        }
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //new UIAlertView("Row Selected", tableItems[indexPath.Row].Title, null, "OK", null).Show();
            if (tableItems[indexPath.Row].ScreenType == MainScreenEnum.LeagueForum)
                navigate.PushViewController(new ForumTopicsViewController(), true);
            
            tableView.DeselectRow(indexPath, true); // normal iOS behaviour is to remove the blue highlight
        }
        public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
        {
            new UIAlertView("DetailDisclosureButton Touched"
                , tableItems[indexPath.Row].Title, null, "OK", null).Show();
        }
    }
}
