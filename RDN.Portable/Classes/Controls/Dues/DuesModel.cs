using ProtoBuf;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Controls.Dues.Classify;
using RDN.Portable.Classes.Controls.Dues.Enums;
using RDN.Portable.Classes.Payment.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Dues
{
    [ProtoContract]
    [DataContract]
  public   class DuesPortableModel
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid DuesId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public double DuesCost { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public string DuesCostDisplay { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string OwnerEntity { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public int DayOfMonthToCollectDefault { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public int DaysBeforeDeadlineToNotifyDefault { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public Guid LeagueOwnerId { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public string LeagueOwnerName { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public Guid CurrentMemberId { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public string CurrentMemberDerbyName { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public string DuesEmailText { get; set; }
        [ProtoMember(12)]
        [DataMember]
        public List<DuesItem> DuesFees { get; set; }
        [ProtoMember(13)]
        [DataMember]
        public List<MemberDisplayDues> Members { get; set; }
        [ProtoMember(14)]
        [DataMember]
        public List<FeeClassified> Classifications { get; set; }
        [ProtoMember(15)]
        [DataMember]
        public string PayPalEmailAddress { get; set; }
        [ProtoMember(16)]
        [DataMember]
        public string EmailResponse { get; set; }
        [ProtoMember(17)]
        [DataMember]
        public string LeagueEmailAddress { get; set; }
        [ProtoMember(18)]
        [DataMember]
        public bool AcceptPaymentsOnline { get; set; }
        [ProtoMember(19)]
        [DataMember]
        public WhoPaysProcessorFeesEnum WhoPaysProcessorFeesEnum { get; set; }
        [ProtoMember(20)]
        [DataMember]
        public string ProcessorFeesTotal { get; set; }
        [ProtoMember(21)]
        [DataMember]
        public bool LockDownManagementToManagersOnly { get; set; }
        /// <summary>
        /// currency type USB, EUR etc...
        /// </summary>
        [ProtoMember(22)]
        [DataMember]
        public string Currency { get; set; }
        [ProtoMember(23)]
        [DataMember]
        public Guid CurrentUserId { get; set; }
        [ProtoMember(24)]
        [DataMember]
        public bool IsSuccessful{ get; set; }
        [ProtoMember(25)]
        [DataMember]
        public List<CurrencyExchange> Currencies { get; set; }


        public DuesPortableModel()
        {
            DuesFees = new List<DuesItem>();
            Members = new List<MemberDisplayDues>();
            Classifications = new List<FeeClassified>();
            Currencies = new List<CurrencyExchange>();
      
        }
    }
}
