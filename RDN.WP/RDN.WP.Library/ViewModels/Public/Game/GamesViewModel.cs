
using RDN.Portable.Models.Json;
using RDN.Portable.Models.Json.Calendar;
using RDN.Portable.Models.Json.Games;
using RDN.Portable.Models.Json.Public;
using RDN.Portable.Models.Json.Shop;
using RDN.WP.Library.Classes.Public;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows;

namespace RDN.WP.Library.ViewModels.Public.Game
{
    public class GamesViewModel : INotifyPropertyChanged
    {
        private bool _isLoading = false;

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged("IsLoading");

            }
        }
        public GamesViewModel()
        {
            this.Games = new ObservableCollection<CurrentGameJson>();
            this.IsLoading = false;
        }

        public ObservableCollection<CurrentGameJson> Games
        {
            get;
            set;
        }
        public void LoadPage(int pageNumber, int pageCount)
        {
            if (pageNumber == 0)
                this.Games.Clear();

            IsLoading = true;
            Action<GamesJson> skaters = new Action<GamesJson>(UpdateAdapter);

            GamesMobile.PullCurrentGames(skaters);
            GamesMobile.PullPastGames(pageNumber, pageCount, skaters);
        }
        public void GetNext(int pageNumber, int pageCount)
        {
            IsLoading = true;
            Action<GamesJson> skaters = new Action<GamesJson>(UpdateAdapter);

            GamesMobile.PullPastGames(pageNumber, pageCount, skaters);
        }
        private void UpdateAdapter(GamesJson obj)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                for (int i = 0; i < obj.Games.Count; i++)
                    this.Games.Add(obj.Games[i]);
                IsLoading = false;
            });

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
