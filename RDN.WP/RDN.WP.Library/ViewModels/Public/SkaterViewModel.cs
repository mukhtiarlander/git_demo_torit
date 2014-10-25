
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

namespace RDN.WP.Library.ViewModels.Public
{
    public class SkaterViewModel : INotifyPropertyChanged
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
        public SkaterViewModel()
        {
            this.Skater = new SkaterJson();
            this.IsLoading = false;
        }
        private SkaterJson _skater;
        public SkaterJson Skater
        {
            get { return _skater; }
            set
            {
                _skater = value;
                NotifyPropertyChanged("Skater");
            }
        }
        public void LoadPage(string memberId)
        {
            IsLoading = true;
            Action<SkaterJson> skaters = new Action<SkaterJson>(UpdateAdapter);
            SkatersMobile.PullPublicSkater(memberId, skaters);
        }
        private void UpdateAdapter(SkaterJson obj)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                this.Skater = obj;
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
