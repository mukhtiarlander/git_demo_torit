using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.Federation;
using RDN.Library.DataModels.League;
using RDN.Library.DataModels.Messages;
using RDN.Library.DataModels.Team;
using RDN.Library.DataModels.Store;
using RDN.Library.DataModels.League.OrganizationChart;
using RDN.Library.DataModels.Officials;
using RDN.Library.DataModels.League.Task;
using RDN.Library.DataModels.Controls.Forum;
using System.ComponentModel.DataAnnotations.Schema;


namespace RDN.Library.DataModels.Member
{
    [Table("RDN_Members")]
    public class Member : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid MemberId { get; set; }

        public Guid AspNetUserId { get; set; }
        [MaxLength(255)]
        public string Firstname { get; set; }
        [MaxLength(255)]
        public string Lastname { get; set; }
        [Column(TypeName = "text")]
        public string Information { get; set; }
        public bool IsVerified { get; set; }
        [MaxLength(500)]
        public string DerbyName { get; set; }

        public string PlayerNumber { get; set; }
        public int Gender { get; set; }
        public int PositionType { get; set; }
        public bool Retired { get; set; }

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

        //are the considered to be in.
        //careted this flag for those people that just sign up to buy things through RDNation.
        //false == they are connected
        //true == they are just some random person.
        public bool IsNotConnectedToDerby { get; set; }


        public string Website { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }

        public string Bio { get; set; }
        public int WeightInLbs { get; set; }
        public int HeightInches { get; set; }

        public DateTime? DateOfBirth { get; set; }
        /// <summary>
        /// can we show this profile to the public?
        /// defaults to true by using the bool of false
        /// </summary>
        public bool IsProfileRemovedFromPublic { get; set; }

        public DateTime? YearStartedSkating { get; set; }
        public DateTime? YearStoppedSkating { get; set; }

        public long MemberType { get; set; } 

        public Guid CurrentLeagueId { get; set; }

        public string DayJob { get; set; }
        public long TotalForumPosts { get; set; }

        #region Constructor
        public Member()
        {
            IsVerified = false;
            EmailVerifications = new Collection<EmailVerification>();
            Logs = new Collection<MemberLog>();
            Photos = new Collection<MemberPhoto>();
            Federations = new Collection<FederationMember>();
            Leagues = new Collection<LeagueMember>();
            InsuranceNumbers = new Collection<MemberInsurance>();
            MemberContacts = new Collection<MemberContact>();
            RefereeRequest = new Collection<RefereeRequest>();
        }
        #endregion

        #region References

        [Obsolete("this league column is obsolete, please use the list of leagues")]
        public virtual League.League League { get; set; }
        public virtual MemberMedical MedicalInformation { get; set; }
        //public virtual Team.Team Team { get; set; }
        public virtual ContactCard.ContactCard ContactCard { get; set; }
        public virtual ICollection<MemberLog> Logs { get; set; }
        public virtual ICollection<EmailVerification> EmailVerifications { get; set; }
        public virtual ICollection<MemberPhoto> Photos { get; set; }
        public virtual ICollection<Federation.FederationOwnership> FederationOwnership { get; set; }
        public virtual ICollection<FederationMember> Federations { get; set; }
        public virtual ICollection<LeagueMember> Leagues { get; set; }
        public virtual ICollection<TeamMember> Teams { get; set; }
        public virtual ICollection<MemberInsurance> InsuranceNumbers { get; set; }
        public virtual ICollection<MemberContact> MemberContacts { get; set; }
        public virtual MemberSettings Settings { get; set; }
        public virtual MemberNotifications Notifications { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<JobBoard> JobBoards { get; set; }
       
        public virtual ICollection<Organize> Organize { get; set; }
        public virtual ICollection<ItemInfo> ItemInfo { get; set; }
        public virtual ICollection<RefereeRequest> RefereeRequest { get; set; }
        public virtual ICollection<Task> Task { get; set; }
        public virtual ICollection<TaskList> TaskList { get; set; }
        public virtual ICollection<ForumMessageAgree> ForumMessageAgree { get; set; }
        public virtual ICollection<ForumMessageLike> ForumMessageLike { get; set; }



        #endregion
    }
}
