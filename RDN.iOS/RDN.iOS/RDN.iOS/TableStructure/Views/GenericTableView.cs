using MonoTouch.Foundation;
using MonoTouch.UIKit;
using RDN.iOS.Models;
using RDN.iOS.Screens.Public.Skaters;
using RDN.iOS.TableStructure.Cells;
using RDN.iOS.TableStructure.Datasources;
using RDN.Portable.Models.Json;
using RDN.Portable.Models.Json.Public;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RDN.iOS.TableStructure.Views
{

    public class GenericTableView : UITableViewSource
    {
        public int CellOn = 0;
        List<GenericTableItem> tableItems;
        //UINavigationController navigate;
        NSString cellIdentifier = new NSString("GenericTableCell");
        public GenericTableView(List<GenericTableItem> items)
        {
            tableItems = items;
            
            //navigate = navigation;
        }
        public override int RowsInSection(UITableView tableview, int section)
        {
            return tableItems.Count;
        }
        public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
        {
            GenericTableCell cell = tableView.DequeueReusableCell(cellIdentifier) as GenericTableCell;
            if (cell == null)
                cell = new GenericTableCell(cellIdentifier);

            cell.UpdateCell(tableItems[indexPath.Row].Key, tableItems[indexPath.Row].Value);
            OnGotCell();
            CellOn = indexPath.Row;
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
            //navigate.PushViewController(new SkaterViewController(tableItems[indexPath.Row]), true);

            tableView.DeselectRow(indexPath, true); // normal iOS behaviour is to remove the blue highlight
        }
        public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
        {
            //new UIAlertView("DetailDisclosureButton Touched"
            //    , tableItems[indexPath.Row].Title, null, "OK", null).Show();
        }
    }
}
