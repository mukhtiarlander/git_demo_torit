using ProtoBuf;
using RDN.Portable.Classes.Account.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Account.Classes
{
    [ProtoContract]
    [DataContract]
    public class MedicalInformation
    {
        [ProtoMember(1)]
        [DataMember]
        public long MedicalId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public bool Epilepsy { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public bool Diabetes { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public bool HeartProblems { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public bool HeartMurmur { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public bool AsthmaBronchitis { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public bool Hernia { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public bool Concussion { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public bool WearGlasses { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public bool ContactLenses { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public ContactLensesEnum HardSoftLensesEnum { get; set; }
        [ProtoMember(12)]
        [DataMember]
        public bool FractureInThreeYears { get; set; }
        [ProtoMember(13)]
        [DataMember]
        public string FractureText { get; set; }
        [ProtoMember(14)]
        [DataMember]
        public bool Dislocation { get; set; }
        [ProtoMember(15)]
        [DataMember]
        public string DislocationText { get; set; }
        [ProtoMember(16)]
        [DataMember]
        public bool ReoccurringPain { get; set; }
        [ProtoMember(17)]
        [DataMember]
        public string ReoccurringPainText { get; set; }
        [ProtoMember(18)]
        [DataMember]
        public bool BackNeckPain { get; set; }
        [ProtoMember(19)]
        [DataMember]
        public bool TreatedForInjury { get; set; }
        [ProtoMember(20)]
        [DataMember]
        public string DoAnyConditionsAffectPerformanceText { get; set; }
        [ProtoMember(21)]
        [DataMember]
        public string RegularMedsText { get; set; }
        [ProtoMember(22)]
        [DataMember]
        public string SportsInjuriesText { get; set; }
        [ProtoMember(23)]
        [DataMember]
        public string AdditionalDetailsText { get; set; }
        [ProtoMember(24)]
        [DataMember]
        public DateTime LastModified { get; set; }
    }
}
