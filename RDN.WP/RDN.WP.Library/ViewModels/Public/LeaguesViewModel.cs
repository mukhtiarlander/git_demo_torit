
using RDN.Portable.Models.Json;
using RDN.Portable.Models.Json.Public;
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

namespace RDN.WP.Library.ViewModels.Public
{
    public class LeaguesViewModel : INotifyPropertyChanged
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
        public LeaguesViewModel()
        {
            this.LeaguesCollection = new ObservableCollection<LeagueJsonDataTable>();
            this.IsLoading = false;
        }

        public ObservableCollection<LeagueJsonDataTable> LeaguesCollection
        {
            get;
            set;
        }
        public void LoadPage(string searchTerm, int pageNumber, int pageCount)
        {
            if (pageNumber == 0)
                this.LeaguesCollection.Clear();

            IsLoading = true;
            Action<LeaguesJson> skaters = new Action<LeaguesJson>(UpdateAdapter);
            LeaguesMobile.PullPublicLeagues(pageNumber, pageCount, searchTerm, skaters);
        }
        private void UpdateAdapter(LeaguesJson obj)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                for (int i = 0; i < obj.Leagues.Count; i++)
                    this.LeaguesCollection.Add(obj.Leagues[i]);
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
