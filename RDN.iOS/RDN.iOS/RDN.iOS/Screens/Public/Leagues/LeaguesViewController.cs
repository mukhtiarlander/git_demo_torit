using System;
using System.Drawing;

using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using RDN.Portable.Models.Json.Public;

using RDN.Portable.Models.Json;
using System.Collections.Generic;
using RDN.iOS.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.iOS.TableStructure.Cells;
using RDN.iOS.TableStructure.Views;
using RDN.iOS.Classes.UI;

namespace RDN.iOS.Screens.Public.Skaters
{

    [Register("LeaguesViewController")]
    public class LeaguesViewController : UIViewController
    {
        int lastPagePulled = 0;
        int PAGE_COUNT = 20;
        string lastLetterPulled = "";
        LeaguesJson initialArray;
        UITableView table;
        LoadingView loading;
        LeaguesTableView source;
        UISearchBar searchBar;
        UIButton searchBtn;
        bool IsSearching = false;
        public LeaguesViewController()
        {
            initialArray = new LeaguesJson();
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
                this.Title = "Derby Leagues";

                View.Frame = UIScreen.MainScreen.Bounds;
                View.BackgroundColor = UIColor.White;
                View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                table = new UITableView(new RectangleF(0, 0, View.Bounds.Width, View.Bounds.Height));

                Action<LeaguesJson> skaters = new Action<LeaguesJson>(UpdateAdapter);
                RDN.iOS.Classes.Public.League.PullLeagues(lastPagePulled, PAGE_COUNT, lastLetterPulled, skaters);
                // Perform any additional setup after loading the view
                loading = new LoadingView();
                loading.ShowActivity("loading leagues");
                source = new LeaguesTableView(initialArray.Leagues, this.NavigationController);
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
                this.NavigationItem.BackBarButtonItem.Title = "Leagues";

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
                Action<LeaguesJson> skaters = new Action<LeaguesJson>(UpdateAdapter);
                RDN.iOS.Classes.Public.League.SearchLeagues(lastPagePulled, PAGE_COUNT, searchBar.Text, skaters);
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
                Action<LeaguesJson> skaters = new Action<LeaguesJson>(UpdateAdapter);
                RDN.iOS.Classes.Public.League.SearchLeagues(lastPagePulled, PAGE_COUNT, searchBar.Text, skaters);
            }
            else if (searchBar.Text.Length == 0)
            {
                Action<LeaguesJson> skaters = new Action<LeaguesJson>(UpdateAdapter);
                RDN.iOS.Classes.Public.League.PullLeagues(lastPagePulled, PAGE_COUNT, lastLetterPulled, skaters);
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
                    Action<LeaguesJson> skaters = new Action<LeaguesJson>(UpdateAdapter);
                    RDN.iOS.Classes.Public.League.SearchLeagues(lastPagePulled, PAGE_COUNT, searchBar.Text, skaters);
                }
                else
                {
                    Action<LeaguesJson> skaters = new Action<LeaguesJson>(UpdateAdapter);
                    RDN.iOS.Classes.Public.League.PullLeagues(lastPagePulled, PAGE_COUNT, lastLetterPulled, skaters);
                }
            }
        }

        void table_Scrolled(object sender, EventArgs e)
        {

        }

        void UpdateAdapter(LeaguesJson skaters)
        {
            if (IsSearching)
                initialArray.Leagues.Clear();
            initialArray.Leagues.AddRange(skaters.Leagues);
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