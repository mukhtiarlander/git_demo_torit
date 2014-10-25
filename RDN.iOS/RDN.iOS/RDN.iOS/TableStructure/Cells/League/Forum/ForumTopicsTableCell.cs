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
    public class ForumTopicsTableCell : UITableViewCell
    {
        UILabel headingLabel, subheadingLabel, postsViewsLabel;
        public ForumTopicsTableCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            try
            {
                SelectionStyle = UITableViewCellSelectionStyle.Gray;

                headingLabel = new UILabel()
                {
                    TextColor = UIColor.Purple,
                    TextAlignment = UITextAlignment.Left,
                    BackgroundColor = UIColor.Clear,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap
                };
                subheadingLabel = new UILabel();
                subheadingLabel.Lines = 0;
                subheadingLabel.LineBreakMode = UILineBreakMode.WordWrap;
                subheadingLabel.Font = UIFont.FromName(subheadingLabel.Font.Name, subheadingLabel.Font.PointSize - 2);

                postsViewsLabel = new UILabel();
                postsViewsLabel.Lines = 0;
                postsViewsLabel.LineBreakMode = UILineBreakMode.WordWrap;
                postsViewsLabel.Font = UIFont.FromName(postsViewsLabel.Font.Name, postsViewsLabel.Font.PointSize - 2);

                ContentView.Add(headingLabel);
                ContentView.Add(subheadingLabel);
                ContentView.Add(postsViewsLabel);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }
        public void UpdateCell(string title, string lastPostBy, string views, string posts, string lastPostRelative)
        {
            try
            {
                headingLabel.Text = title;
                subheadingLabel.Text = "by " + lastPostBy + ", " + lastPostRelative;
                //postsViewsLabel.Text = "P:" + posts + " / V:" + views;
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
            headingLabel.Frame = new RectangleF(10, 5, ContentView.Bounds.Width - 20, 25);
            subheadingLabel.Frame = new RectangleF(10, 25, ContentView.Bounds.Width - 20, 25);
            //postsViewsLabel.Frame = new RectangleF(ContentView.Bounds.Width - 50, 25, 50, 25);

        }
    }
}
