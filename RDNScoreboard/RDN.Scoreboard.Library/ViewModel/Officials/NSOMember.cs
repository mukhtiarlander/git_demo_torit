using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Scoreboard.Library.ViewModel.Officials.Enums;

namespace Scoreboard.Library.ViewModel.Members.Officials
{
    public enum NSOMemberEnum
    {
        SkaterPictureLocation, SkaterPictureCompressed, SkaterName, RefereeType
    }
    public class NSOMember: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public NSOMember()
        {
            this.SkaterId = Guid.NewGuid();
        }

        //public NSOMember(Guid skaterLinkId)
        //{
        //    this.SkaterLinkId = skaterLinkId;
        //}
    

        public Guid SkaterId { get; set; }
        /// <summary>
        /// id for the skater in the DB.
        /// </summary>
        public Guid SkaterLinkId { get; set; }
        public string SkaterName
        {
            get { return _SkaterName; }
            set
            {
                _SkaterName = value;
                OnPropertyChanged("SkaterName");
            }
        }
        private string _SkaterName;
        public string League { get; set; }
        public CertificationLevelEnum Cert { get; set; }
        public string SkaterNumber { get; set; }

        private string _skaterPictureLocation;
        public string SkaterPictureLocation
        {
            get { return _skaterPictureLocation; }
            set
            {
                _skaterPictureLocation = value;
                OnPropertyChanged("SkaterPictureLocation");
            }
        }

        private byte[] _skaterPictureCompressed;
        /// <summary>
        /// used to transfer the actual picture, encoding it into a byte array.
        /// </summary>
        public byte[] SkaterPictureCompressed
        {
            get { return _skaterPictureCompressed; }
            set
            {
                _skaterPictureCompressed = value;
                OnPropertyChanged("SkaterPictureCompressed");
            }
        }

        private NSOTypeEnum _refereeType;
        public NSOTypeEnum RefereeType
        {
            get { return _refereeType; }
            set
            {
                _refereeType = value;
                OnPropertyChanged("RefereeType");
            }
        }

        

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
