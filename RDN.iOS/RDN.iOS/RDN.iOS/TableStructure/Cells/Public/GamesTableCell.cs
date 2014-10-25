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
    public class GamesTableCell : UITableViewCell
    {
        UILabel _headingLabel, _team1Name, _team2Name, _team1Score, _team2Score;
        UIImageView team1ImageView, team2ImageView;
        UIView _headerView;
        public GamesTableCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            try
            {
                SelectionStyle = UITableViewCellSelectionStyle.Gray;
                team1ImageView = new UIImageView();
                team2ImageView = new UIImageView();
                _headingLabel = new UILabel()
                {
                    TextColor = UIColor.White,
                    TextAlignment = UITextAlignment.Left,
                    BackgroundColor = UIColor.Clear
                };
                _team1Name = new UILabel()
                      {
                          TextColor = UIColor.Purple,
                          TextAlignment = UITextAlignment.Left,
                          BackgroundColor = UIColor.Clear
                      };
                _team2Name = new UILabel()
                  {
                      TextColor = UIColor.Purple,
                      TextAlignment = UITextAlignment.Left,
                      BackgroundColor = UIColor.Clear
                  };
                _team1Score = new UILabel()
                     {
                         TextAlignment = UITextAlignment.Left,
                         BackgroundColor = UIColor.Clear
                     };
                _team1Score.Font = UIFont.FromName(_team1Score.Font.Name, _headingLabel.Font.PointSize - 2);
                _team2Score = new UILabel()
                   {
                       TextAlignment = UITextAlignment.Left,
                       BackgroundColor = UIColor.Clear
                   };
                _team2Score.Font = UIFont.FromName(_team2Score.Font.Name, _headingLabel.Font.PointSize - 2);
                _headingLabel.Font = UIFont.FromName(_headingLabel.Font.Name, _headingLabel.Font.PointSize - 3);
                _headerView = new UIView();
                _headerView.BackgroundColor = UIColor.Purple;

                _headerView.AddSubview(_headingLabel);
                ContentView.Add(_headerView);
                ContentView.Add(_team1Name);
                ContentView.Add(_team2Name);
                ContentView.Add(_team1Score);
                ContentView.Add(_team2Score);
                ContentView.Add(team1ImageView);
                ContentView.Add(team2ImageView);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }
        public void UpdateCell(string team1Name, string team2Name, string headerText, int team1Score, int team2Score, UIImage team1Image, UIImage team2Image)
        {
            try
            {
                team1ImageView.Image = team1Image;
                team2ImageView.Image = team2Image;
                _team1Name.Text = team1Name;
                _team2Name.Text = team2Name;
                _team1Score.Text = team1Score.ToString();
                _team2Score.Text = team2Score.ToString();
                if (team1Score > team2Score)
                {
                    _team1Score.TextColor = UIColor.Purple;
                    _team2Score.TextColor = UIColor.Black;
                }
                else
                {
                    _team1Score.TextColor = UIColor.Black;
                    _team2Score.TextColor = UIColor.Purple;
                }
                _headingLabel.Text = headerText;
                Accessory = UITableViewCellAccessory.None;
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }
        public override void LayoutSubviews()
        {
            try
            {
                base.LayoutSubviews();
                _headerView.Frame = new RectangleF(0, 0, ContentView.Bounds.Width, 30);
                _headingLabel.Frame = new RectangleF(10, 5, ContentView.Bounds.Width - 45, 20);
                team1ImageView.Frame = new RectangleF(0, 30, 60, 60);
                team2ImageView.Frame = new RectangleF(0, 100, 60, 60);
                _team1Name.Frame = new RectangleF(75, 50, ContentView.Bounds.Width - 105, 35);
                _team1Score.Frame = new RectangleF(ContentView.Bounds.Width - 35, 50, 45, 35);
                _team2Name.Frame = new RectangleF(75, 110, ContentView.Bounds.Width - 105, 35);
                _team2Score.Frame = new RectangleF(ContentView.Bounds.Width - 35, 110, 45, 35);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }
    }
}
