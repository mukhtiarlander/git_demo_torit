using System;
using System.Drawing;

using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using RDN.Portable.Models.Json.Public;
using RDN.iOS.Classes.Public;
using RDN.Portable.Models.Json;
using System.Collections.Generic;
using RDN.iOS.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.iOS.TableStructure.Cells;
using RDN.iOS.TableStructure.Views;
using RDN.iOS.Classes.UI;
using RDN.Portable.Classes.Controls.Forum;
using RDN.iOS.Classes.League;
using RDN.Portable.Settings;
using RDN.iOS.TableStructure.Views.League;
using System.Linq;

namespace RDN.iOS.Screens.League
{

    [Register("ReplyForumTopicViewController")]
    public class ReplyForumTopicViewController : UIViewController
    {
        AddForumTopicModel initialModel;
        UITextField titleTxt;
        UITextView messageTxt;
        UILabel warningTxt;
        LoadingView loading;
        UISwitch swith;

        UIButton postBtn;


        public ReplyForumTopicViewController(long topicId, string title)
        {
            initialModel = new AddForumTopicModel();
            initialModel.ForumId = SettingsMobile.Instance.CurrentForumId;
            initialModel.TopicId = topicId;
            initialModel.TopicTitle = title;
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            try
            {
                base.ViewDidLoad();
                this.Title = "Reply to " + initialModel.TopicTitle;

                View.Frame = UIScreen.MainScreen.Bounds;
                View.BackgroundColor = UIColor.White;
                View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;


                UIScrollView scroll = new UIScrollView(new RectangleF(0, 0, View.Bounds.Width, View.Bounds.Height));
                scroll.ContentSize = new SizeF(View.Bounds.Width, 500);

                UILabel groupLbl = new UILabel(new RectangleF(10, 15, 200, 25));
                groupLbl.Text = "Title: ";
                scroll.AddSubview(groupLbl);
                UILabel groupNameLbl = new UILabel(new RectangleF(60, 15, 200, 25));
                groupNameLbl.Text = initialModel.TopicTitle;
                scroll.AddSubview(groupNameLbl);

                UILabel messageLbl = new UILabel(new RectangleF(10, 50, View.Bounds.Width, 25));
                messageLbl.Text = "Message:";
                scroll.AddSubview(messageLbl);

                messageTxt = new UITextView(new RectangleF(5, 80, View.Bounds.Width - 10, 200));

                messageTxt.Layer.BorderWidth = 1;
                messageTxt.Layer.MasksToBounds = true;
                messageTxt.Layer.CornerRadius = 8;
                messageTxt.Layer.BorderColor = UIColor.Gray.CGColor;
                scroll.AddSubview(messageTxt);

                warningTxt = new UILabel(new RectangleF(10, 290, View.Bounds.Width - 20, 35));
                warningTxt.TextColor = UIColor.Red;
                scroll.AddSubview(warningTxt);

                var button2 = new UIBarButtonItem("Send", UIBarButtonItemStyle.Plain, (sender, args) =>
                {
                    loading = new LoadingView();
                    loading.ShowActivity("sending message");
                    initialModel.DatePosted = DateTime.UtcNow;
                    initialModel.MemberId = SettingsMobile.Instance.User.MemberId.ToString();
                    initialModel.Text = messageTxt.Text;
                    initialModel.ForumType = "league";
                    initialModel.UserId = SettingsMobile.Instance.User.LoginId.ToString();
                    bool isSuccuessful = Forum.ReplyToTopic(initialModel);
                    loading.Hide();
                    
                    this.NavigationController.PopViewControllerAnimated(true);
                    
                });
                this.NavigationItem.SetRightBarButtonItem(button2, false);



                View.AddSubview(scroll);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }
    }
}