
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
    public class SkatersViewModel : INotifyPropertyChanged
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
        public SkatersViewModel()
        {
            this.SkatersCollection = new ObservableCollection<SkaterJson>();
            this.IsLoading = false;
        }

        public ObservableCollection<SkaterJson> SkatersCollection
        {
            get;
            set;
        }
        public void LoadPage(string searchTerm, int pageNumber, int pageCount)
        {
            if (pageNumber == 0)
                this.SkatersCollection.Clear();

            IsLoading = true;
            Action<SkatersJson> skaters = new Action<SkatersJson>(UpdateAdapter);
            SkatersMobile.PullPublicSkaters(pageNumber, pageCount, searchTerm, skaters);
        }
        private void UpdateAdapter(SkatersJson obj)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
for (int i = 0; i < obj.Skaters.Count; i++)
                    this.SkatersCollection.Add(obj.Skaters[i]);
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
