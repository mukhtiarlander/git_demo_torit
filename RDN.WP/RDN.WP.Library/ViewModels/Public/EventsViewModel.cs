
using RDN.Portable.Config.Enums;
using RDN.Portable.Models.Json;
using RDN.Portable.Models.Json.Calendar;
using RDN.Portable.Models.Json.Public;
using RDN.Portable.Util;
using RDN.Portable.Util.Log.Enums;
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
    public class EventsViewModel : INotifyPropertyChanged
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
        public EventsViewModel()
        {
            this.EventsCollection = new ObservableCollection<EventJson>();
            this.IsLoading = false;
        }

        public ObservableCollection<EventJson> EventsCollection
        {
            get;
            set;
        }
        public void LoadPage(string searchTerm, int pageNumber, int pageCount)
        {
            if (pageNumber == 0)
                this.EventsCollection.Clear();

            IsLoading = true;
            Action<EventsJson> skaters = new Action<EventsJson>(UpdateAdapter);
            if (String.IsNullOrEmpty(searchTerm))
            {
                CalendarMobile.PullCurrentEvents(pageNumber, pageCount, skaters);
            }
            else
            {
                CalendarMobile.SearchCurrentEvents(pageNumber, pageCount, searchTerm, skaters);
            }
        }
        public void LocateEvents(int pageNumber, int pageCount, double longitude, double latitude)
        {
            LoggerMobile.Instance.logMessage("Locating Events:" + pageNumber + ":" + pageCount + ":" + longitude + ":" + latitude, LoggerEnum.message);
            if (pageNumber == 0 && this.EventsCollection != null)
                this.EventsCollection.Clear();

            IsLoading = true;
            Action<EventsJson> skaters = new Action<EventsJson>(UpdateAdapter);

            CalendarMobile.PullCurrentEventsByLocation(pageNumber, pageCount, longitude, latitude, skaters);

        }
        private void UpdateAdapter(EventsJson obj)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                for (int i = 0; i < obj.Events.Count; i++)
                    this.EventsCollection.Add(obj.Events[i]);
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
