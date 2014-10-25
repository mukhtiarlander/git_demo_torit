
using RDN.Portable.Models.Json;
using RDN.Portable.Models.Json.Calendar;
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

namespace RDN.WP.Library.ViewModels.Public
{
    public class LeagueViewModel : INotifyPropertyChanged
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
        public LeagueViewModel()
        {
            this.EventsCollection = new ObservableCollection<EventJson>();
            this.League = new LeagueJsonDataTable();
            this.IsLoading = false;
        }
        private ObservableCollection<EventJson> _events;
        public ObservableCollection<EventJson> EventsCollection
        {
            get { return _events; }
            set
            {
                _events = value;
                NotifyPropertyChanged("EventsCollection");
            }
        }
        private LeagueJsonDataTable _league;
        public LeagueJsonDataTable League
        {
            get { return _league; }
            set
            {
                _league = value;
                NotifyPropertyChanged("League");
            }
        }
        private Collection<SkaterJson> _skaters;
        public Collection<SkaterJson> Skaters
        {
            get { return _skaters; }
            set
            {
                _skaters = value;
                NotifyPropertyChanged("Skaters");
            }
        }
        public void LoadPage(string leagueId)
        {
            IsLoading = true;
            Action<LeagueJsonDataTable> leagues = new Action<LeagueJsonDataTable>(UpdateAdapter);
            Action<SkatersJson> skaters = new Action<SkatersJson>(UpdateAdapterSkaters);
            Action<EventsJson> evs = new Action<EventsJson>(UpdateAdapterEvents);
            LeaguesMobile.PullPublicLeague(leagueId, leagues);
            SkatersMobile.PullPublicSkatersByLeague(leagueId, skaters);
            LeaguesMobile.PullPublicLeagueEvents(leagueId, evs);
        }
        private void UpdateAdapter(LeagueJsonDataTable obj)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                this.League = obj;
                IsLoading = false;
            });

        }
        private void UpdateAdapterSkaters(SkatersJson obj)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                this.Skaters = new Collection<SkaterJson>();
                for (int i = 0; i < obj.Skaters.Count; i++)
                {
                    this.Skaters.Add(obj.Skaters[i]);
                }
                IsLoading = false;
            });

        }
        private void UpdateAdapterEvents(EventsJson obj)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                this.EventsCollection = new ObservableCollection<EventJson>();
                for (int i = 0; i < obj.Events.Count; i++)
                {
                    this.EventsCollection.Add(obj.Events[i]);
                }
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
