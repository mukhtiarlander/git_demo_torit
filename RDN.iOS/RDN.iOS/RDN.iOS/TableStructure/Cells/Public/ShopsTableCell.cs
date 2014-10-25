using MonoTouch.Foundation;
using MonoTouch.UIKit;
using RDN.iOS.Classes.Error;
using RDN.Portable.Config.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RDN.iOS.TableStructure.Cells.Public
{
    public class ShopsTableCell : UITableViewCell
    {
        UILabel headingLabel, subheadingLabel, priceLabel;
        UIImageView imageView;
        public ShopsTableCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            try
            {
                SelectionStyle = UITableViewCellSelectionStyle.Gray;
                imageView = new UIImageView();
                headingLabel = new UILabel()
                {
                    TextColor = UIColor.Purple,
                    TextAlignment = UITextAlignment.Left,
                    BackgroundColor = UIColor.Clear,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap
                };
                subheadingLabel = new UILabel();
                priceLabel = new UILabel();
                subheadingLabel.Lines = 0;
                subheadingLabel.LineBreakMode = UILineBreakMode.WordWrap;
                subheadingLabel.Font = UIFont.FromName(subheadingLabel.Font.Name, subheadingLabel.Font.PointSize - 2);
                ContentView.Add(headingLabel);
                ContentView.Add(subheadingLabel);
                ContentView.Add(priceLabel);
                ContentView.Add(imageView);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }
        public void UpdateCell(string caption, string subtitle, string price, UIImage image)
        {
            try
            {
                imageView.Image = image;
                headingLabel.Text = caption;
                subheadingLabel.Text = subtitle;
                priceLabel.Text = price;
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
            headingLabel.Frame = new RectangleF(80, 5, ContentView.Bounds.Width - 70, 25);
            subheadingLabel.Frame = new RectangleF(80, 30, ContentView.Bounds.Width - 70, 25);
            priceLabel.Frame = new RectangleF(80, 55, ContentView.Bounds.Width - 70, 25);
        }
    }
}
