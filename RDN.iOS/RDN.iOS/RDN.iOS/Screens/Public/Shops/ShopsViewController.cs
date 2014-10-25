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

    [Register("ShopsViewController")]
    public class ShopsViewController : UIViewController
    {
        int lastPagePulled = 0;
        int PAGE_COUNT = 20;
        string lastLetterPulled = "";
        ShopsJson initialArray;
        UITableView table;
        LoadingView loading;
        ShopsTableView source;
        UISearchBar searchBar;
        UIButton searchBtn;
        bool IsSearching = false;
        public ShopsViewController()
        {
            initialArray = new ShopsJson();
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
                this.Title = "Derby Supply";

                View.Frame = UIScreen.MainScreen.Bounds;
                View.BackgroundColor = UIColor.White;
                View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                table = new UITableView(new RectangleF(0, 0, View.Bounds.Width, View.Bounds.Height));

                Action<ShopsJson> skaters = new Action<ShopsJson>(UpdateAdapter);
                Shop.PullShopItems(lastPagePulled, PAGE_COUNT, skaters);
                // Perform any additional setup after loading the view
                loading = new LoadingView();
                loading.ShowActivity("loading items");
                source = new ShopsTableView(initialArray.Items, this.NavigationController);
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
                this.NavigationItem.BackBarButtonItem.Title = "Supply";

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
                Action<ShopsJson> skaters = new Action<ShopsJson>(UpdateAdapter);
                Shop.SearchShopItems(lastPagePulled, PAGE_COUNT, searchBar.Text, skaters);
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
                Action<ShopsJson> skaters = new Action<ShopsJson>(UpdateAdapter);
                Shop.SearchShopItems(lastPagePulled, PAGE_COUNT, searchBar.Text, skaters);
            }
            else if (searchBar.Text.Length == 0)
            {
                Action<ShopsJson> skaters = new Action<ShopsJson>(UpdateAdapter);
                Shop.PullShopItems(lastPagePulled, PAGE_COUNT, skaters);
            }
        }

        void searchButton_TouchDown(object sender, EventArgs e)
        {

        }

        void source_GotCell(object sender, EventArgs e)
        {
            if (source.CellOn == (((lastPagePulled + 1) * PAGE_COUNT) - 2))
            {
                Action<ShopsJson> skaters = new Action<ShopsJson>(UpdateAdapter);
                lastPagePulled += 1;
                if (IsSearching)
                {
                    Shop.SearchShopItems(lastPagePulled, PAGE_COUNT, searchBar.Text, skaters);
                }
                else
                {
                    Shop.PullShopItems(lastPagePulled, PAGE_COUNT, skaters);
                }
            }
        }

        void table_Scrolled(object sender, EventArgs e)
        {

        }

        void UpdateAdapter(ShopsJson skaters)
        {
            if (IsSearching)
                initialArray.Items.Clear();
            initialArray.Items.AddRange(skaters.Items);
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