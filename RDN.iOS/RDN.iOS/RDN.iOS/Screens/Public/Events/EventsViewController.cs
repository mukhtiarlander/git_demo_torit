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
using RDN.Portable.Models.Json.Calendar;

namespace RDN.iOS.Screens.Public.Events
{

    [Register("EventsViewController")]
    public class EventsViewController : UIViewController
    {
        int lastPagePulled = 0;
        int PAGE_COUNT = 20;
        string lastLetterPulled = "";
        EventsJson initialArray;
        UITableView table;
        LoadingView loading;
        EventsTableView source;
        UISearchBar searchBar;
        UIButton searchBtn;
        bool IsSearching = false;
        public EventsViewController()
        {
            initialArray = new EventsJson();
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
                //View = new UniversalView();

                base.ViewDidLoad();
                this.Title = "Derby Events";

                View.Frame = UIScreen.MainScreen.Bounds;
                View.BackgroundColor = UIColor.White;
                View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                table = new UITableView(new RectangleF(0, 0, View.Bounds.Width, View.Bounds.Height));

                Action<EventsJson> skaters = new Action<EventsJson>(UpdateAdapter);
                Calendar.PullEvents(lastPagePulled, PAGE_COUNT, skaters);
                // Perform any additional setup after loading the view
                loading = new LoadingView();
                loading.ShowActivity("loading events");
                source = new EventsTableView(initialArray.Events, this.NavigationController);
                source.GotCell += source_GotCell;
                table.Source = source;
                table.RowHeight = 80;


                searchBar = new UISearchBar(new RectangleF(0, 0, 200, 44));
                searchBar.SetShowsCancelButton(true, false);
                searchBar.CancelButtonClicked += searchBar_CancelButtonClicked;
                searchBar.SearchButtonClicked += searchBar_SearchButtonClicked;


                this.NavigationItem.SetRightBarButtonItem(new UIBarButtonItem(UIBarButtonSystemItem.Search, (sender, args) =>
    {
        searchBar.BecomeFirstResponder();
        UIView searchBarView = new UIView(new RectangleF(0, 0, 250, 44));
        searchBarView.AddSubview(searchBar);
        this.NavigationItem.TitleView = searchBarView;
    })
, true);

                this.NavigationItem.BackBarButtonItem = new UIBarButtonItem();
                this.NavigationItem.BackBarButtonItem.Title = "Events";

                View.Add(table);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }

        }

        void searchBar_SearchButtonClicked(object sender, EventArgs e)
        {
            try
            {
                IsSearching = true;
                lastPagePulled = 0;
                Action<EventsJson> skaters = new Action<EventsJson>(UpdateAdapter);
                Calendar.SearchEvents(lastPagePulled, PAGE_COUNT, searchBar.Text, skaters);
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

        void searchBar_TextChanged(object sender, UISearchBarTextChangedEventArgs e)
        {
            if (searchBar.Text.Length >= 2)
            {
                lastPagePulled = 0;
                Action<EventsJson> skaters = new Action<EventsJson>(UpdateAdapter);
                Calendar.SearchEvents(lastPagePulled, PAGE_COUNT, searchBar.Text, skaters);
            }
            else if (searchBar.Text.Length == 0)
            {
                Action<EventsJson> skaters = new Action<EventsJson>(UpdateAdapter);
                Calendar.PullEvents(lastPagePulled, PAGE_COUNT, skaters);
            }
        }

        void searchButton_TouchDown(object sender, EventArgs e)
        {

        }

        void source_GotCell(object sender, EventArgs e)
        {
            if (source.CellOn == (((lastPagePulled + 1) * PAGE_COUNT) - 2))
            {
                lastPagePulled += 1;
                if (IsSearching)
                {
                    Action<EventsJson> skaters = new Action<EventsJson>(UpdateAdapter);
                    Calendar.SearchEvents(lastPagePulled, PAGE_COUNT, searchBar.Text, skaters);
                }
                else
                {
                    Action<EventsJson> skaters = new Action<EventsJson>(UpdateAdapter);
                    Calendar.PullEvents(lastPagePulled, PAGE_COUNT, skaters);
                }
            }
        }

        void table_Scrolled(object sender, EventArgs e)
        {

        }

        void UpdateAdapter(EventsJson skaters)
        {
            if (IsSearching)
                initialArray.Events.Clear();
            initialArray.Events.AddRange(skaters.Events);
            InvokeOnMainThread(() =>
            {
                try
                {
                    //table.InsertRows(indexPaths.ToArray(), UITableViewRowAnimation.Fade);

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