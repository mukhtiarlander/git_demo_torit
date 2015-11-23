using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.DataModels.ContactCard;
using RDN.League.Models.Federation;
using RDN.Portable.Classes.Account.Enums;
using RDN.Portable.Classes.Imaging;
using RDN.Portable.Classes.Insurance;

namespace RDN.League.Models.User
{
    public class EditMember
    {
        public EditMember()
        {
            FederationsApartOf = new List<FederationDisplay>();
            Photos = new List<PhotoItem>();
            Leagues = new List<RDN.Portable.Classes.League.Classes.League>();
            InsuranceNumbers = new List<InsuranceNumber>();
        }

        public ContactCard ContactCard { get; set; }
        public GenderEnum Gender { get; set; }
        public List<FederationDisplay> FederationsApartOf { get; set; }
        public List<PhotoItem> Photos { get; set; }
        public List<RDN.Portable.Classes.League.Classes.League> Leagues { get; set; }
        public List<RDN.Portable.Classes.League.Classes.League> LeaguesToChooseFrom { get; set; }
        public Guid MemberId { get; set; }
        public Guid UserId { get; set; }
        public string DerbyName { get; set; }
        public string DerbyNameLink
        {
            get
            {
                return RDN.Utilities.Strings.StringExt.ToUrlFriendly(DerbyName);

            }
        }
        public string PlayerNumber { get; set; }
        public string Firstname { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// DateTime Started 
        /// </summary>
        public DateTime? StartedSkating { get; set; }
        public DateTime? StoppedSkating { get; set; }

        public string DayJob { get; set; }
        public int HeightFeet { get; set; }
        public int HeightInches { get; set; }
        public int WeightLbs { get; set; }
        public bool IsRetired { get; set; }
        public bool IsProfileRemovedFromPublicView { get; set; }
        public DateTime DOB { get; set; }

        public Guid LeagueId { get; set; }
        public string LeagueName { get; set; }
        public string PhoneNumber { get; set; }
        public MemberType MemberType { get; set; } 
        public int Country { get; set; }
        public SelectList Countries { get; set; }

        //public string Country { get; set; }
        //public int CountryId { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        [AllowHtml]
        public string Bio { get; set; }

        public string Website { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }


        public List<InsuranceNumber> InsuranceNumbers { get; set; }
    }
}