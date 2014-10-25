using MonoTouch.Foundation;
using MonoTouch.UIKit;
using RDN.iOS.Classes.Error;
using RDN.Portable.Config.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RDN.iOS.TableStructure.Cells
{
    public class LeaguesTableCell : UITableViewCell
    {
        UILabel headingLabel, subheadingLabel;
        UIImageView imageView;
        public LeaguesTableCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            try { 
            SelectionStyle = UITableViewCellSelectionStyle.Gray;
            imageView = new UIImageView();
            headingLabel = new UILabel()
            {
                TextColor = UIColor.Purple,
                TextAlignment = UITextAlignment.Left,
                BackgroundColor = UIColor.Clear
            };
            headingLabel.Font = UIFont.FromName(headingLabel.Font.Name, headingLabel.Font.PointSize + 3);

            ContentView.Add(headingLabel);
            //ContentView.Add(subheadingLabel);
            ContentView.Add(imageView);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }
        public void UpdateCell(string caption, string subtitle, UIImage image)
        {
            try { 
            imageView.Image = image;
            headingLabel.Text = caption;
            //subheadingLabel.Text = subtitle;
            Accessory = UITableViewCellAccessory.DisclosureIndicator;
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            imageView.Frame = new RectangleF(0, 5, 70, 70);
            headingLabel.Frame = new RectangleF(85, 30, ContentView.Bounds.Width - 45, 25);
            //subheadingLabel.Frame = new RectangleF(100, 18, 100, 20);
        }
    }
}
