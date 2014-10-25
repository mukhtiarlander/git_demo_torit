
using RDN.Portable.Models.Json;
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

namespace RDN.WP.Library.ViewModels.Public.Shop
{
    public class ShopItemViewModel : INotifyPropertyChanged
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
        public ShopItemViewModel()
        {
            this.ShopItem = new ShopItemJson();
            this.IsLoading = false;
        }
        private ShopItemJson _shopItem;
        public ShopItemJson ShopItem
        {
            get { return _shopItem; }
            set
            {
                _shopItem = value;
                NotifyPropertyChanged("ShopItem");
            }
        }
        public void LoadPage()
        {
            //IsLoading = true;
        }
        private void UpdateAdapter(ShopItemJson obj)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                this.ShopItem = obj;
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
