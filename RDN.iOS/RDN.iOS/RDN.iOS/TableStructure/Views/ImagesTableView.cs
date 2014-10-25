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

    public class ImagesTableView : UITableViewSource
    {
        public int CellOn = 0;
        List<string> _urls;
        UINavigationController navigate;
        NSString cellIdentifier = new NSString("ImagesTableCell");
        public ImagesTableView(List<string> urls, UINavigationController navigation)
        {
            _urls = urls;
            navigate = navigation;
        }
        public override int RowsInSection(UITableView tableview, int section)
        {
            return _urls.Count;
        }
        public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
        {

            ImageTableCell cell = tableView.DequeueReusableCell(cellIdentifier) as ImageTableCell;
            try
            {
                if (cell == null)
                    cell = new ImageTableCell(cellIdentifier, navigate.View.Bounds.Width);
                NSData data = new NSData();
                if (!String.IsNullOrEmpty(_urls[indexPath.Row]))
                {
                    NSUrl nsUrl = new NSUrl(_urls[indexPath.Row]);
                    data = NSData.FromUrl(nsUrl);
                }
                cell.UpdateCell(new UIImage(data));
                
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

            tableView.DeselectRow(indexPath, true); // normal iOS behaviour is to remove the blue highlight
        }
        public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
        {

        }
    }
}
