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
using System.Drawing;
using System.Linq;
using System.Text;

namespace RDN.iOS.TableStructure.Views.League
{

    public class TopicPostsTableView : UITableViewSource
    {
        UILabel test = new UILabel();
        public int CellOn = 0;
        List<ForumPostModel> tableItems;
        UINavigationController navigate;
        NSString cellIdentifier = new NSString("TopicPostsTableView");
        public TopicPostsTableView(List<ForumPostModel> items, UINavigationController navigation)
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
            ForumTopicPostsTableCell cell = tableView.DequeueReusableCell(cellIdentifier) as ForumTopicPostsTableCell;
            if (cell == null)
                cell = new ForumTopicPostsTableCell(cellIdentifier);
            cell.UpdateCell(tableItems[indexPath.Row].PostedByName, tableItems[indexPath.Row].Text, tableItems[indexPath.Row].DatePostedByHuman);

            OnGotCell();
            CellOn = indexPath.Row;

            // Find the height...
            SizeF textWidth = new SizeF(tableView.Bounds.Width - 40, float.MaxValue);
            SizeF textSize = tableView.StringSize(tableItems[indexPath.Row].Text, cell.textLabel.Font, textWidth, UILineBreakMode.WordWrap);
            cell.textLabel.Frame = new RectangleF(cell.textLabel.Frame.X, cell.textLabel.Frame.Y, textSize.Width, textSize.Height);
            //cell.textLabel.Bounds = new RectangleF(cell.textLabel.Frame.X, cell.textLabel.Frame.Y, textSize.Width, textSize.Height);
            // Sizing the cell...
            RectangleF rectCell = cell.Frame;
            rectCell.Height = cell.textLabel.Frame.Height;
            cell.Frame = rectCell;


            return cell;
        }
        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = this.tableItems[indexPath.Row];
            SizeF sizeWidth = new SizeF(tableView.Bounds.Width - 40, float.MaxValue);
            float height = tableView.StringSize(item.Text, test.Font, sizeWidth, UILineBreakMode.WordWrap).Height + 40;
            return height;
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
            //new UIAlertView("Row Selected", tableItems[indexPath.Row].Title, null, "OK", null).Show();
            //if (tableItems[indexPath.Row].ScreenType == MainScreenEnum.LeagueForum)
            //    navigate.PushViewController(new ForumPostsViewController(), true);

            tableView.DeselectRow(indexPath, true); // normal iOS behaviour is to remove the blue highlight
        }
        public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
        {
            //new UIAlertView("DetailDisclosureButton Touched"
            //    , tableItems[indexPath.Row].Title, null, "OK", null).Show();
        }
    }
}
