using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RDNationLibrary.ViewModel
{
    public class TeamMembersViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Guid SkaterId { get; set; }
        public string SkaterName { get; set; }
        public string SkaterNumber { get; set; }

        /// I dont really care about these IS values.
        /// They are only used for WPF databinding.

        private bool _isBenched;
        public bool IsBenched
        {
            get { return _isBenched; }
            set
            {
                _isBenched = value;
                OnPropertyChanged("IsBenched");
            }
        }
        private bool _isInBox;
        public bool IsInBox
        {
            get { return _isInBox; }
            set
            {
                _isInBox = value;
                OnPropertyChanged("IsInBox");
            }
        }

        private bool _isLeadJammer;
        public bool IsLeadJammer
        {
            get { return _isLeadJammer; }
            set
            {
                _isLeadJammer = value;
                OnPropertyChanged("IsLeadJammer");
            }
        }
        private bool _isPivot;
        public bool IsPivot
        {
            get { return _isPivot; }
            set
            {
                _isPivot = value;
                OnPropertyChanged("IsPivot");
            }
        }
        private bool _isBlocker1;
        public bool IsBlocker1
        {
            get { return _isBlocker1; }
            set
            {
                _isBlocker1 = value;
                OnPropertyChanged("IsBlocker1");
            }
        }
        private bool _isJammer;
        public bool IsJammer
        {
            get { return _isJammer; }
            set
            {
                _isJammer = value;
                OnPropertyChanged("IsJammer");
            }
        }
        private bool _isBlocker2;
        public bool IsBlocker2
        {
            get { return _isBlocker2; }
            set
            {
                _isBlocker2 = value;
                              OnPropertyChanged("IsBlocker2");
            }
        }
        private bool _isBlocker3;
        public bool IsBlocker3
        {
            get { return _isBlocker3; }
            set
            {
                _isBlocker3 = value;
                OnPropertyChanged("IsBlocker3");
            }
        }

        private ObservableCollection<PenaltyViewModel> _penalties = new ObservableCollection<PenaltyViewModel>();
        public ObservableCollection<PenaltyViewModel> Penalties { get { return _penalties; } set { _penalties = value; } }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
