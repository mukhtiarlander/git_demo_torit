using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.WP.Classes.UI;
using RDN.WP.Helpers;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Devices;
using RDN.Portable.Models.Json.Public;
using RDN.WP.Library.ViewModels.Public;
using RDN.Portable.Models.Json;
using RDN.Portable.Classes.Utilities;
using RDN.Portable.Models.Json.Calendar;
using RDN.WP.Library.ViewModels.Public.Shop;
using RDN.Portable.Models.Json.Shop;
using RDN.WP.Library.ViewModels.Public.Game;
using RDN.Portable.Models.Json.Games;
using Microsoft.Phone.Tasks;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.Portable.Util;

namespace RDN.WP.View.Public.Game
{
    public partial class Games : PhoneApplicationPage
    {

        private int PAGE_COUNT = 20;
        private string lastLetterPulled = "a";
        private int lastPagePulled = 0;
        GamesJson initialArray;
        int _offsetKnob = 7;
        GamesViewModel _viewModel;
        string searchTerm = string.Empty;
        public Games()
        {
            try
            {
                InitializeComponent();
                LoggerMobile.Instance.logMessage("Opening Games", Portable.Util.Log.Enums.LoggerEnum.message);
                _viewModel = (GamesViewModel)Resources["viewModel"];
                resultListBox.ItemRealized += resultListBox_ItemRealized;
                this.Loaded += new RoutedEventHandler(MainPage_Loaded);
                initialArray = new GamesJson();

           
            }
            catch (Exception exception)
            { ErrorHandler.Save(exception, MobileTypeEnum.WP8); }
        }

     
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var progressIndicator = SystemTray.ProgressIndicator;
                if (progressIndicator != null)
                {
                    return;
                }

                progressIndicator = new ProgressIndicator();

                SystemTray.SetProgressIndicator(this, progressIndicator);

                Binding binding = new Binding("IsLoading") { Source = _viewModel };
                BindingOperations.SetBinding(
                    progressIndicator, ProgressIndicator.IsVisibleProperty, binding);

                binding = new Binding("IsLoading") { Source = _viewModel };
                BindingOperations.SetBinding(
                    progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);

                progressIndicator.Text = "Loading games...";
                _viewModel.LoadPage(lastPagePulled, PAGE_COUNT);
                lastPagePulled += 1;
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        void resultListBox_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            try
            {
                if (!_viewModel.IsLoading && resultListBox.ItemsSource != null && resultListBox.ItemsSource.Count >= _offsetKnob)
                {
                    if (e.ItemKind == LongListSelectorItemKind.Item)
                    {
                        if ((e.Container.Content as CurrentGameJson).Equals(resultListBox.ItemsSource[resultListBox.ItemsSource.Count - _offsetKnob]))
                        {
                            _viewModel.GetNext(lastPagePulled++, PAGE_COUNT);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
     


        private void resultListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // If selected item is null, do nothing
                if (resultListBox.SelectedItem == null)
                    return;

                var game = (CurrentGameJson)resultListBox.SelectedItem;
                var task = new WebBrowserTask();
                task.Uri = new Uri(game.GameUrl);
                task.Show();
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }


    }
}