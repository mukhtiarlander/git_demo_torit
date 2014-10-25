using MonoTouch.Foundation;
using MonoTouch.UIKit;
using RDN.iOS.Classes.Error;
using RDN.iOS.Models;
using RDN.iOS.Screens.Public.Leagues;
using RDN.iOS.Screens.Public.Skaters;
using RDN.iOS.TableStructure.Cells;
using RDN.Portable.Config.Enums;
using RDN.Portable.Models.Json;
using RDN.Portable.Models.Json.Calendar;
using RDN.Portable.Models.Json.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.iOS.TableStructure.Views
{

    public class EventsTableView : UITableViewSource
    {
        public int CellOn = 0;
        List<EventJson> tableItems;
        UINavigationController navigate;
        NSString cellIdentifier = new NSString("EventsTableCell");
        public EventsTableView(List<EventJson> items, UINavigationController navigation)
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
            EventsTableCell cell = new EventsTableCell(cellIdentifier);
            try
            {
                cell = tableView.DequeueReusableCell(cellIdentifier) as EventsTableCell;

                if (cell == null)
                    cell = new EventsTableCell(cellIdentifier);
                string dt = tableItems[indexPath.Row].StartDate.ToString("d/MM") +" "+ tableItems[indexPath.Row].StartDate.ToLocalTime().ToShortTimeString() + " @ " + tableItems[indexPath.Row].Location;
                if (!String.IsNullOrEmpty(tableItems[indexPath.Row].LogoUrl))
                {
                    NSUrl nsUrl = new NSUrl(tableItems[indexPath.Row].LogoUrl);
                    NSData data = NSData.FromUrl(nsUrl);

                    if (data != null)
                        cell.UpdateCell(tableItems[indexPath.Row].Name, dt, new UIImage(data));
                    else
                        cell.UpdateCell(tableItems[indexPath.Row].Name, dt, new UIImage());
                }
                else
                    cell.UpdateCell(tableItems[indexPath.Row].Name, dt, new UIImage());

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
            navigate.PushViewController(new EventViewController(tableItems[indexPath.Row]), true);

            tableView.DeselectRow(indexPath, true); // normal iOS behaviour is to remove the blue highlight
        }
        public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
        {
            //new UIAlertView("DetailDisclosureButton Touched"
            //    , tableItems[indexPath.Row].Title, null, "OK", null).Show();
        }
    }
}
