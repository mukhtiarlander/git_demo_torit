using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Voting
{
    [ProtoContract]
    [DataContract]
    public class VotingAnswersClass : INotifyPropertyChanged
    {
        [ProtoMember(1)]
        [DataMember]
        public long AnswerId { get; set; }

        private string _Answer;
        [ProtoMember(2)]
        [DataMember]
        public string Answer
        {
            get { return _Answer; }
            set
            {
                _Answer = value;

                OnPropertyChanged("Answer");
            }
        }
        [ProtoMember(3)]
        [DataMember]
        public bool WasRemoved { get; set; }


        public VotingAnswersClass()
        {

        }


        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
