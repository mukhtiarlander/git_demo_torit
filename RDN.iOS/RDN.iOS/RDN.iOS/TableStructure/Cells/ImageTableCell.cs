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
    public class ImageTableCell : UITableViewCell
    {
        UIImageView imageView;
        float _width;
        public ImageTableCell(NSString cellId, float width)
            : base(UITableViewCellStyle.Default, cellId)
        {
            try
            {
                _width = width;
                SelectionStyle = UITableViewCellSelectionStyle.Gray;
                imageView = new UIImageView();
                ContentView.Add(imageView);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }
        public void UpdateCell(UIImage image)
        {
            try
            {
                imageView.Image = image;
                Accessory = UITableViewCellAccessory.None;
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            imageView.Frame = new RectangleF(_width / 2 - 100, 0, 200, 200);
        }
    }
}
