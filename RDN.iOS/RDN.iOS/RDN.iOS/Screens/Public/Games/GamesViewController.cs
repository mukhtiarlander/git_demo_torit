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
using RDN.Portable.Models.Json.Games;
using RDN.iOS.TableStructure.Views.Public;

namespace RDN.iOS.Screens.Public.Games
{

    [Register("GamesViewController")]
    public class GamesViewController : UIViewController
    {
        int lastPagePulled = 0;
        int PAGE_COUNT = 20;
        GamesJson initialArray;
        UITableView table;
        LoadingView loading;
        GamesTableView source;
        public GamesViewController()
        {
            initialArray = new GamesJson();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewDidLoad()
        {
            try
            {
                base.ViewDidLoad();
                this.Title = "Derby Games";

                View.Frame = UIScreen.MainScreen.Bounds;
                View.BackgroundColor = UIColor.White;
                View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                table = new UITableView(new RectangleF(0, 0, View.Bounds.Width, View.Bounds.Height));

                Action<GamesJson> skaters = new Action<GamesJson>(UpdateAdapter);
                Game.PullCurrentGames(skaters);
                Action<GamesJson> skaters1 = new Action<GamesJson>(UpdateAdapter);
                Game.PullPastGames(PAGE_COUNT, lastPagePulled, skaters1);
                // Perform any additional setup after loading the view
                loading = new LoadingView();
                loading.ShowActivity("loading games");
                source = new GamesTableView(initialArray.Games, this.NavigationController, View);
                source.GotCell += source_GotCell;
                table.Source = source;
                table.RowHeight = 160;

                this.NavigationItem.BackBarButtonItem = new UIBarButtonItem();
                this.NavigationItem.BackBarButtonItem.Title = "Games";

                View.Add(table);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
        }
        void source_GotCell(object sender, EventArgs e)
        {
            if (source.CellOn == (((lastPagePulled + 1) * PAGE_COUNT) - 2))
            {
                lastPagePulled += 1;
                Action<GamesJson> skaters = new Action<GamesJson>(UpdateAdapter);
                Game.PullPastGames(lastPagePulled, PAGE_COUNT, skaters);
            }
        }

        void table_Scrolled(object sender, EventArgs e)
        {

        }

        void UpdateAdapter(GamesJson skaters)
        {
            initialArray.Games.AddRange(skaters.Games);
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