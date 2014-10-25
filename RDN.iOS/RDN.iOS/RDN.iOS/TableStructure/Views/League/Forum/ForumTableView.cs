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
using RDN.iOS.TableStructure.Cells.Public;
using RDN.Portable.Classes.Controls.Forum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.iOS.TableStructure.Views.League
{

    public class ForumTableView : UITableViewSource
    {
        public int CellOn = 0;
        List<ForumTopicModel> tableItems;
        UINavigationController navigate;
        Guid ForumId;
        NSString cellIdentifier = new NSString("ForumPostsTableCell");
        public ForumTableView(List<ForumTopicModel> items, Guid forumId, UINavigationController navigation)
        {
            tableItems = items;
            navigate = navigation;
            ForumId = forumId;

        }
        public override int RowsInSection(UITableView tableview, int section)
        {
            return tableItems.Count;
        }
        public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
        {
            ForumTopicsTableCell cell = tableView.DequeueReusableCell(cellIdentifier) as ForumTopicsTableCell;
            if (cell == null)
                cell = new ForumTopicsTableCell(cellIdentifier);
            cell.UpdateCell(tableItems[indexPath.Row].TopicName, tableItems[indexPath.Row].LastPostByName, tableItems[indexPath.Row].ViewCount.ToString(), tableItems[indexPath.Row].PostCount.ToString(), tableItems[indexPath.Row].LastPostRelativeTime);

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
                        navigate.PushViewController(new ForumTopicViewController(tableItems[indexPath.Row]), true);

            tableView.DeselectRow(indexPath, true); // normal iOS behaviour is to remove the blue highlight
        }
        public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
        {
            //new UIAlertView("DetailDisclosureButton Touched"
            //    , tableItems[indexPath.Row].Title, null, "OK", null).Show();
        }
    }
}
