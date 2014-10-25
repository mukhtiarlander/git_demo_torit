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
using FlyoutNavigation;
using MonoTouch.Dialog;


namespace RDN.iOS.Screens.League
{

    [Register("ForumTopicsViewController")]
    public class ForumTopicsViewController : UIViewController
    {
        int lastPagePulled = 0;
        int PAGE_COUNT = 20;
        string lastLetterPulled = "a";
        ForumModel initialModel;
        List<ForumTopicModel> initialArray;
        UITableView table;
        LoadingView loading;
        ForumTableView source;
        UISearchBar searchBar;
        UIButton searchBtn;
        bool IsSearching = false;
        bool hasInitiallyLoadedDirtyBit = false;
        FlyoutNavigationController navigation;
        string currentGroupName;
        public ForumTopicsViewController()
        {
            initialModel = new ForumModel();
            initialArray = new List<ForumTopicModel>();
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
                this.Title = "Forum Posts";

                View.Frame = UIScreen.MainScreen.Bounds;
                View.BackgroundColor = UIColor.White;
                View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                table = new UITableView(new RectangleF(0, 0, View.Bounds.Width, View.Bounds.Height));

                Action<ForumModel> skaters = new Action<ForumModel>(UpdateAdapter);
                Forum.PullForumTopics(SettingsMobile.Instance.User.MemberId.ToString(), SettingsMobile.Instance.User.LoginId.ToString(), "league", 0, 0, 0, 30, skaters);
                // Perform any additional setup after loading the view
                loading = new LoadingView();
                loading.ShowActivity("loading posts");

                var group = initialModel.Groups.Where(x => x.GroupId == 0).FirstOrDefault();
                if (group != null)
                    initialArray.AddRange(group.Topics);
                source = new ForumTableView(initialArray, initialModel.ForumId, this.NavigationController);
                table.Source = source;
                table.RowHeight = 50;

                this.NavigationItem.BackBarButtonItem = new UIBarButtonItem();
                this.NavigationItem.BackBarButtonItem.Title = "Topics";

                //View.Add(table);

                navigation = new FlyoutNavigationController(UITableViewStyle.Plain);
                navigation.View.Frame = new RectangleF(0, 60, View.Bounds.Width, View.Bounds.Height);
                navigation.ViewControllers = new[] {
            new UIViewController { View = table },
        };
                View.AddSubview(navigation.View);


                var button1 = new UIBarButtonItem("Gs", UIBarButtonItemStyle.Plain, (sender, args) =>
                {
                    navigation.ToggleMenu();

                });
                var button2 = new UIBarButtonItem(UIBarButtonSystemItem.Add, (sender, args) =>
                {
                    AddForumTopicViewController add = new AddForumTopicViewController(initialModel.GroupId, initialModel.ForumId, currentGroupName);
                    this.NavigationController.PushViewController(add, true);

                });
                this.NavigationItem.SetRightBarButtonItems(new UIBarButtonItem[] { button1, button2 }, false);

                Action selected = new Action(SelectedNavigationItem);
                navigation.SelectedIndexChanged = selected;

            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }

        void searchBar_CancelButtonClicked(object sender, EventArgs e)
        {
            try
            {
                this.NavigationItem.TitleView = null;
                IsSearching = false;
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }

        void searchButton_TouchDown(object sender, EventArgs e)
        {

        }

        void table_Scrolled(object sender, EventArgs e)
        {

        }

        void SelectedNavigationItem()
        {
            try
            {

                navigation.HideMenu();
                Action<ForumModel> skaters = new Action<ForumModel>(UpdateAdapter);
                Forum.PullForumTopics(SettingsMobile.Instance.User.MemberId.ToString(), SettingsMobile.Instance.User.LoginId.ToString(), "league", initialModel.Groups[navigation.SelectedIndex].GroupId, 0, 0, 30, skaters);
                // Perform any additional setup after loading the view
                loading = new LoadingView();
                loading.ShowActivity("loading group posts");

            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }

        void UpdateAdapter(ForumModel skaters)
        {
            try
            {
                initialModel = skaters;
                var group = initialModel.Groups.Where(x => x.GroupId == initialModel.GroupId).FirstOrDefault();
                currentGroupName = group.GroupName;
                initialArray.Clear();
                if (group != null)
                    initialArray.AddRange(group.Topics);

                RootElement navHead = new RootElement("Navigation");
                Section section = new Section("Groups");
                List<StringElement> groups = new List<StringElement>();
                for (int i = 0; i < skaters.Groups.Count; i++)
                {
                    groups.Add(new StringElement(skaters.Groups[i].GroupName));
                }
                section.AddAll(groups);
                navHead.Add(section);
                InvokeOnMainThread(() =>
                {
                    try
                    {
                        table.ReloadData();

                        loading.Hide();
                        navigation.HideMenu();
                        this.Title = group.GroupName + " Posts";
                        navigation.NavigationRoot = navHead;

                    }
                    catch (Exception exception)
                    {
                        ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
                    }
                });

                SettingsMobile.Instance.CurrentForumId = skaters.ForumId;



            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }
    }
}