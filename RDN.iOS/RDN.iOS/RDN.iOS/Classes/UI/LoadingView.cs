using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RDN.iOS.Classes.UI
{
    public class LoadingView : UIAlertView
    {
        private UIActivityIndicatorView _activityView;

        public void ShowActivity(string title)
        {
            Title = title;

            this.Show();
            // Spinner - add after Show() or we have no Bounds.
            _activityView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            _activityView.Frame = new RectangleF((Bounds.Width / 2) - 15, Bounds.Height - 50, 30, 30);
            _activityView.StartAnimating();
            AddSubview(_activityView);

        }

        public void Hide()
        {
            DismissWithClickedButtonIndex(0, true);
        }
    }
}
