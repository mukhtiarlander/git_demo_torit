using MonoTouch.Foundation;
using MonoTouch.UIKit;
using RDN.iOS.Classes.Error;
using RDN.iOS.Models;
using RDN.iOS.Screens.Public.Skaters;
using RDN.iOS.TableStructure.Cells;
using RDN.Portable.Config.Enums;
using RDN.Portable.Models.Json;
using RDN.Portable.Models.Json.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.iOS.TableStructure.Views
{

    public class SkatersTableView : UITableViewSource
    {
        public int CellOn = 0;
        List<SkaterJson> tableItems;
        UINavigationController navigate;
        NSString cellIdentifier = new NSString("SkatersTableCell");
        public SkatersTableView(List<SkaterJson> items, UINavigationController navigation)
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

            SkatersTableCell cell = tableView.DequeueReusableCell(cellIdentifier) as SkatersTableCell;
            try
            {
                if (cell == null)
                    cell = new SkatersTableCell(cellIdentifier);
                NSData data = new NSData();
                if (!String.IsNullOrEmpty(tableItems[indexPath.Row].ThumbUrl))
                {
                    NSUrl nsUrl = new NSUrl(tableItems[indexPath.Row].ThumbUrl);
                    data = NSData.FromUrl(nsUrl);
                }
                cell.UpdateCell(tableItems[indexPath.Row].DerbyName, tableItems[indexPath.Row].DerbyNumber, new UIImage(data));
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
            //new UIAlertView("Row Selected", tableItems[indexPath.Row].DerbyName, null, "OK", null).Show();
            //if (tableItems[indexPath.Row].ScreenType == Classes.MainScreenEnum.Skaters)
            //    navigate.PushViewController(new SkatersViewController(),  true);
            navigate.PushViewController(new SkaterViewController(tableItems[indexPath.Row]), true);

            tableView.DeselectRow(indexPath, true); // normal iOS behaviour is to remove the blue highlight
        }
        public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
        {
            //new UIAlertView("DetailDisclosureButton Touched"
            //    , tableItems[indexPath.Row].Title, null, "OK", null).Show();
        }
    }
}
