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

    [Register("ForumTopicViewController")]
    public class ForumTopicViewController : UIViewController
    {
        ForumTopicModel initialModel;
        List<ForumPostModel> initialArray;
        UITableView table;
        LoadingView loading;
        TopicPostsTableView source;
        bool ReloadData = false;
        public ForumTopicViewController(ForumTopicModel forumTopic)
        {
            initialModel = forumTopic;
            initialArray = new List<ForumPostModel>();
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            ReloadData = true;

        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);


        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (ReloadData)
            {
                Action<ForumTopicModel> skaters = new Action<ForumTopicModel>(UpdateAdapter);
                Forum.PullForumTopic(SettingsMobile.Instance.User.MemberId.ToString(), SettingsMobile.Instance.User.LoginId.ToString(), initialModel.TopicId, skaters);
                loading = new LoadingView();
                loading.ShowActivity("loading topic");

            }
        }



        public override void ViewDidLoad()
        {
            try
            {
                base.ViewDidLoad();
                this.Title = initialModel.TopicName;

                View.Frame = UIScreen.MainScreen.Bounds;
                View.BackgroundColor = UIColor.White;
                View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                table = new UITableView(new RectangleF(0, 0, View.Bounds.Width, View.Bounds.Height));

                Action<ForumTopicModel> skaters = new Action<ForumTopicModel>(UpdateAdapter);
                Forum.PullForumTopic(SettingsMobile.Instance.User.MemberId.ToString(), SettingsMobile.Instance.User.LoginId.ToString(), initialModel.TopicId, skaters);
                // Perform any additional setup after loading the view
                loading = new LoadingView();
                loading.ShowActivity("loading topic");

                var button2 = new UIBarButtonItem(UIBarButtonSystemItem.Reply, (sender, args) =>
                {
                    ReplyForumTopicViewController add = new ReplyForumTopicViewController(initialModel.TopicId, initialModel.TopicName);
                    this.NavigationController.PushViewController(add, true);

                });
                this.NavigationItem.SetRightBarButtonItem(button2, false);


                source = new TopicPostsTableView(initialArray, this.NavigationController);
                table.Source = source;
                //table.RowHeight = 50;

                this.NavigationItem.BackBarButtonItem = new UIBarButtonItem();
                this.NavigationItem.BackBarButtonItem.Title = "Topics";

                View.Add(table);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }



        void UpdateAdapter(ForumTopicModel skaters)
        {
            initialModel = skaters;
            initialArray.Clear();
            initialArray.AddRange(skaters.Posts);
            InvokeOnMainThread(() =>
            {
                try
                {
                    table.ReloadData();
                    loading.Hide();
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
                }
            });



        }
    }
}