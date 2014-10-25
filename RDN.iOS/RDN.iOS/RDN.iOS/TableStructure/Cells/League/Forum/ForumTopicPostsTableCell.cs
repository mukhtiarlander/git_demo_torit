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
    public class ForumTopicPostsTableCell : UITableViewCell
    {
      public   UILabel headingLabel, textLabel;
        public ForumTopicPostsTableCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            try
            {
                SelectionStyle = UITableViewCellSelectionStyle.Gray;

                headingLabel = new UILabel()
                {
                    TextAlignment = UITextAlignment.Left,
                    BackgroundColor = UIColor.Gray,
                    TextColor = UIColor.White,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap
                };
                headingLabel.Font = UIFont.FromName(headingLabel.Font.Name, headingLabel.Font.PointSize - 2);

                textLabel = new UILabel();
                
                textLabel.Lines = 0;
                textLabel.LineBreakMode = UILineBreakMode.WordWrap;
                textLabel.SizeToFit();
                textLabel.Font = UIFont.FromName(textLabel.Font.Name, textLabel.Font.PointSize);

                ContentView.Add(headingLabel);
                ContentView.Add(textLabel);
                //ContentView.SizeToFit();
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }
        public void UpdateCell(string lastPostBy, string text, string lastPostRelative)
        {
            try
            {
                headingLabel.Text = " " + lastPostBy + ", " + lastPostRelative;
                textLabel.Text = text;
                //textLabel.SizeToFit();
                //ContentView.SizeToFit();
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
            headingLabel.Frame = new RectangleF(5, 0, ContentView.Bounds.Width - 10, 25);

            textLabel.Frame = new RectangleF(10, 25, ContentView.Bounds.Width - 20, 300);
            
            textLabel.SizeToFit();
            //postsViewsLabel.Frame = new RectangleF(ContentView.Bounds.Width - 50, 25, 50, 25);

        }
    }
}
