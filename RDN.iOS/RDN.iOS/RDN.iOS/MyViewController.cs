using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace RDN.iOS
{
    public class MyViewController : UIViewController
    {
        UIButton skatersBtn;
        UIButton eventsBtn;
        UIButton leaguesBtn;
        UIButton shopsBtn;
        UIButton gamesBtn;

        float buttonWidth = 200;
        float buttonHeight = 50;

        public MyViewController()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.Frame = UIScreen.MainScreen.Bounds;
            View.BackgroundColor = UIColor.White;
            View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

            createButton(skatersBtn, "Skaters");
            createButton(leaguesBtn, "Leagues");
            createButton(eventsBtn, "Events");
            createButton(gamesBtn, "Games");
            createButton(shopsBtn, "Shops");
        }

        private void createButton(UIButton btn, string title)
        {

            btn = UIButton.FromType(UIButtonType.System);

            btn.Frame = new RectangleF(
                View.Frame.Width / 2 - buttonWidth / 2,
                View.Frame.Height / 2 - buttonHeight / 2,
                buttonWidth,
                buttonHeight);

            btn.SetTitle(title, UIControlState.Normal);

            btn.TouchUpInside += (object sender, EventArgs e) =>
            {
                btn.SetTitle("clicked", UIControlState.Normal);
            };

            btn.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin |
                UIViewAutoresizing.FlexibleBottomMargin;

            View.AddSubview(btn);
        }

    }
}

