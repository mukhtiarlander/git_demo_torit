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

namespace RDN.iOS.Screens.Public.Skaters
{

    [Register("SkatersViewController")]
    public class SkatersViewController : UIViewController
    {
        int lastPagePulled = 0;
        int PAGE_COUNT = 20;
        string lastLetterPulled = "a";
        SkatersJson initialArray;
        UITableView table;
        LoadingView loading;
        SkatersTableView source;
        UISearchBar searchBar;
        UIButton searchBtn;
        bool IsSearching = false;
        public SkatersViewController()
        {
            initialArray = new SkatersJson();
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
                this.Title = "Derby Skaters";

                View.Frame = UIScreen.MainScreen.Bounds;
                View.BackgroundColor = UIColor.White;
                View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                table = new UITableView(new RectangleF(0, 0, View.Bounds.Width, View.Bounds.Height));

                Action<SkatersJson> skaters = new Action<SkatersJson>(UpdateAdapter);
                Skater.PullSkaters(lastPagePulled, PAGE_COUNT, lastLetterPulled, skaters);
                // Perform any additional setup after loading the view
                loading = new LoadingView();
                loading.ShowActivity("loading skaters");
                source = new SkatersTableView(initialArray.Skaters, this.NavigationController);
                source.GotCell += source_GotCell;
                table.Source = source;


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
                this.NavigationItem.BackBarButtonItem.Title = "Skaters";
                
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
                Action<SkatersJson> skaters = new Action<SkatersJson>(UpdateAdapter);
                Skater.SearchSkaters(lastPagePulled, PAGE_COUNT, searchBar.Text, skaters);
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
                Action<SkatersJson> skaters = new Action<SkatersJson>(UpdateAdapter);
                Skater.SearchSkaters(lastPagePulled, PAGE_COUNT, searchBar.Text, skaters);
            }
            else if (searchBar.Text.Length == 0)
            {
                Action<SkatersJson> skaters = new Action<SkatersJson>(UpdateAdapter);
                Skater.PullSkaters(lastPagePulled, PAGE_COUNT, lastLetterPulled, skaters);
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
                    Action<SkatersJson> skaters = new Action<SkatersJson>(UpdateAdapter);
                    Skater.SearchSkaters(lastPagePulled, PAGE_COUNT, searchBar.Text, skaters);
                }
                else
                {
                    Action<SkatersJson> skaters = new Action<SkatersJson>(UpdateAdapter);
                    Skater.PullSkaters(lastPagePulled, PAGE_COUNT, lastLetterPulled, skaters);
                }
            }
        }

        void table_Scrolled(object sender, EventArgs e)
        {

        }

        void UpdateAdapter(SkatersJson skaters)
        {
            if (IsSearching)
                initialArray.Skaters.Clear();
            initialArray.Skaters.AddRange(skaters.Skaters);
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