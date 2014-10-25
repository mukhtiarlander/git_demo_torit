using ProtoBuf;
using RDN.Portable.Classes.Voting.Enums;
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
    public class VotingQuestionClass:INotifyPropertyChanged
    {
        [ProtoMember(1)]
        [DataMember]
        public long QuestionId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public int SortableOrderId { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public string Question { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public QuestionTypeEnum QuestionType { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public List<VotesClass> Votes { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public List<VotingAnswersClass> Answers { get; set; }
        
        public bool IsDeleted { get; set; }

        public VotingQuestionClass()
        {
            Answers = new List<VotingAnswersClass>();
            Votes = new List<VotesClass>();
        }

        public int GetPercentage(long answerId)
        {
            if (Votes.Count > 0)
                return Votes.Where(x => x.AnswerIds.Contains(answerId)).Count() * 100 / Votes.Count;
            else
                return 0;
        }

        public int GetBarLength(long answerId, int controlWidth)
        {
            int percentage = GetPercentage(answerId);
            return percentage * controlWidth / 100;
        }

        public event PropertyChangedEventHandler PropertyChanged;
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
