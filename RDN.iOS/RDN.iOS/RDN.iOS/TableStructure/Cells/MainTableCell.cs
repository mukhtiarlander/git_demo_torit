using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RDN.iOS.TableStructure.Cells
{
    public class MainTableCell : UITableViewCell
    {
        UILabel headingLabel, subheadingLabel;
        UIImageView imageView;
        public MainTableCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            SelectionStyle = UITableViewCellSelectionStyle.Gray;
            imageView = new UIImageView();
            headingLabel = new UILabel()
            {
                TextColor = UIColor.Purple,
                TextAlignment = UITextAlignment.Left,
                BackgroundColor = UIColor.Clear
            };
            headingLabel.Font = UIFont.FromName(headingLabel.Font.Name, headingLabel.Font.PointSize + 3);
            //subheadingLabel = new UILabel()
            //{
            //    TextColor = UIColor.Black,
            //    BackgroundColor = UIColor.Clear
            //};
            ContentView.Add(headingLabel);
            //ContentView.Add(subheadingLabel);
            ContentView.Add(imageView);
        }
        public void UpdateCell(string caption, string subtitle, UIImage image)
        {
            imageView.Image = image;
            headingLabel.Text = caption;
            //subheadingLabel.Text = subtitle;
            Accessory = UITableViewCellAccessory.DisclosureIndicator;
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            imageView.Frame = new RectangleF(5, 5, 33, 33);
            headingLabel.Frame = new RectangleF(40, 13, ContentView.Bounds.Width - 40, 25);
            //subheadingLabel.Frame = new RectangleF(100, 18, 100, 20);
        }
    }
}
