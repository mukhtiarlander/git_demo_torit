
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
    public class EventViewModel : INotifyPropertyChanged
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
        public EventViewModel()
        {
            this.EventPublic = new EventJson();
            this.IsLoading = false;
        }
        private EventJson _EventPublic;
        public EventJson EventPublic
        {
            get { return _EventPublic; }
            set
            {
                _EventPublic = value;
                NotifyPropertyChanged("EventPublic");
            }
        }
     
        public void LoadPage(string leagueId)
        {
            IsLoading = true;
//            Action<EventJson> eventPublic = new Action<EventJson>(UpdateAdapter);
//CalendarMobile.PullCurrentEvents(leagueId, eventPublic);
        }
        private void UpdateAdapter(EventJson obj)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                this.EventPublic = obj;
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
