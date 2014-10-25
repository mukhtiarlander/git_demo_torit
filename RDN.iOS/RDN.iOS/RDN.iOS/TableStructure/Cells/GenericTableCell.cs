using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RDN.iOS.TableStructure.Cells
{
    public class GenericTableCell : UITableViewCell
    {
        UILabel headingLabel;
        //UIImageView imageView;
        public GenericTableCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            SelectionStyle = UITableViewCellSelectionStyle.Gray;
            //imageView = new UIImageView();
            headingLabel = new UILabel()
            {
                TextAlignment = UITextAlignment.Left,
                BackgroundColor = UIColor.Clear
            };

            ContentView.Add(headingLabel);
            //ContentView.Add(subheadingLabel);
            //ContentView.Add(imageView);
        }
        public void UpdateCell(string key, string value)
        {
            //imageView.Image = image;
            headingLabel.Text = key + value;
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            //this.Frame = new RectangleF(0, 0, this.Frame.Width, 20);
            //imageView.Frame = new RectangleF(5, 5, 35, 35);
            headingLabel.Frame = new RectangleF(5, 10, ContentView.Bounds.Width, 20);
            //subheadingLabel.Frame = new RectangleF(ContentView.Bounds.Width / 2 + 10, 10, ContentView.Bounds.Width / 2, 25);
        }
    }
}
