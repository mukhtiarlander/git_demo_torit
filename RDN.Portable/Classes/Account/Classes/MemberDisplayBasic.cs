using ProtoBuf;
using RDN.Portable.Classes.Account.Enums;
using RDN.Portable.Classes.Controls.Calendar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Account.Classes
{
    /// <summary>
    /// basic member, so we can pass JSON back and forth with the most important information.
    /// </summary>
    [ProtoContract]
    [DataContract]
    [DebuggerDisplay("[{this.MemberId} {this.DerbyName}]")]
    [ProtoInclude(1000, typeof(CalendarAttendance))]
    [ProtoInclude(900, typeof(MemberDisplayMessage))]
    [ProtoInclude(800, typeof(MemberDisplayEdit))]
    [ProtoInclude(700, typeof(MemberDisplayDues))]
    [ProtoInclude(600, typeof(MemberSettingsClass))]
    [ProtoInclude(500, typeof(MemberDisplayGame))]
    [ProtoInclude(100, typeof(MemberDisplay))]
    public class MemberDisplayBasic
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid MemberId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string UserName { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public Guid UserId { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string Firstname { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public string LastName { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public string Email { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public string PhoneNumber { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public string DerbyName { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public string DerbyNameUrl { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public long ClassificationId { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public string ClassificationName { get; set; }
        [ProtoMember(12)]
        [DataMember]
        public string PlayerNumber { get; set; }
        [ProtoMember(13)]
        [DataMember]
        public GenderEnum Gender { get; set; }

        [ProtoMember(14)]
        [DataMember]
        public int HeightFeet { get; set; }
        [ProtoMember(15)]
        [DataMember]
        public int HeightInches { get; set; }
        [ProtoMember(16)]
        [DataMember]
        public int WeightLbs { get; set; }
        [ProtoMember(17)]
        [DataMember]
        public DateTime DOB { get; set; }
        [ProtoMember(18)]
        [DataMember]
        public string ThumbUrl { get; set; }

        [ProtoMember(19)]
        [DataMember]
        public MemberType MemberType { get; set; }

        public string Age { get { return ((DateTime.UtcNow - DOB).TotalDays / 365).ToString("N0"); } }

        public bool DidVote { get; set; }

        public string Name
        {
            get
            {
                if (!String.IsNullOrEmpty(DerbyName))
                    return DerbyName;
                else
                    return Firstname;
            }
        }
        /// <summary>
        /// users real name.
        /// First Last
        /// </summary>
        public string RealName
        {
            get
            {
                if (!String.IsNullOrEmpty(Firstname) && !String.IsNullOrEmpty(LastName))
                {
                    return Firstname + " " + LastName;
                }
                else if (!String.IsNullOrEmpty(Firstname))
                    return Firstname;
                else
                    return LastName;
            }
        }
        /// <summary>
        /// users nickname plus real name.
        /// NICKNAME [First Last]
        /// </summary>
        public string SiteName
        {
            get
            {
                string name = string.Empty;
                if (!String.IsNullOrEmpty(DerbyName))
                {
                    name = DerbyName;
                }
                if (!String.IsNullOrEmpty(Firstname) && !String.IsNullOrEmpty(LastName))
                {
                    if (!string.IsNullOrEmpty(name))
                        name += " [" + Firstname + " " + LastName + "]";
                    else
                        name = Firstname + " " + LastName;
                }
                else if (!String.IsNullOrEmpty(Firstname))
                {
                    if (!string.IsNullOrEmpty(name))
                        name += " [" + Firstname + "]";
                    else
                        name = Firstname;
                }
                else if (!String.IsNullOrEmpty(LastName))
                {
                    if (!string.IsNullOrEmpty(name))
                        name += " [" + LastName + "]";
                    else
                        name = LastName;
                }

                return name;
            }
        }

        public MemberDisplayBasic()
        { }


    }
}
