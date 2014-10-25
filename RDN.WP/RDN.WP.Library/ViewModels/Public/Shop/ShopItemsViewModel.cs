
using RDN.Portable.Models.Json;
using RDN.Portable.Models.Json.Calendar;
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

namespace RDN.WP.Library.ViewModels.Public.Shop
{
    public class ShopItemsViewModel : INotifyPropertyChanged
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
        public ShopItemsViewModel()
        {
            this.ShopItems = new ObservableCollection<ShopItemJson>();
            this.IsLoading = false;
        }

        public ObservableCollection<ShopItemJson> ShopItems
        {
            get;
            set;
        }
        public void LoadPage(string searchTerm, int pageNumber, int pageCount)
        {
            if (pageNumber == 0)
                this.ShopItems.Clear();

            IsLoading = true;
            Action<ShopsJson> skaters = new Action<ShopsJson>(UpdateAdapter);
            if (String.IsNullOrEmpty(searchTerm))
            {
                ShopMobile.PullShopItems(pageNumber, pageCount, skaters);
            }
            else
            {
                ShopMobile.SearchShopItems(pageNumber, pageCount, searchTerm, skaters);
            }
        }
        private void UpdateAdapter(ShopsJson obj)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                for (int i = 0; i < obj.Items.Count; i++)
                    this.ShopItems.Add(obj.Items[i]);
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
