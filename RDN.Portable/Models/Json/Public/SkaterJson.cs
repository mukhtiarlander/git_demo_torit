using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Models.Json
{
    [DataContract]
    public class SkaterJson
    {


        [DataMember]
        public string MemberId { get; set; }
        [DataMember]
        public string DerbyName { get; set; }
        [DataMember]
        public string DerbyNameUrl { get; set; }
        [DataMember]
        public string DerbyNumber { get; set; }
        [DataMember]
        public string Gender { get; set; }
        [DataMember]
        public string LeagueName { get; set; }
        [DataMember]
        public string LeagueUrl { get; set; }
        [DataMember]
        public string LeagueLogo { get; set; }
        [DataMember]
        public string photoUrl { get; set; }
        [DataMember]
        public string ThumbUrl { get; set; }
        [DataMember]
        public string LeagueId { get; set; }
        [DataMember]
        public DateTime DOB { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string Weight { get; set; }
        private int _age = 0;
        [DataMember]
        public int Age
        {
            get
            {
                if (DOB != new DateTime())
                {
                    DateTime today = DateTime.Today;
                    _age = today.Year - DOB.Year;
                    if (DOB > today.AddYears(-_age)) _age--;
                    return _age;
                }
                return 0;
            }
            set { value = _age; }
        }

        [DataMember]
        public string Bio { get; set; }
        [DataMember]
        public int HeightFeet { get; set; }
        [DataMember]
        public int HeightInches { get; set; }
        /// <summary>
        /// got the games data.
        /// </summary>
        [DataMember]
        public bool GotExtendedContent { get; set; }
        [DataMember]
        public int GamesCount { get; set; }
        [DataMember]
        public int Wins { get; set; }
        [DataMember]
        public int Losses { get; set; }
        [DataMember]
        public string Latitude { get; set; }
        [DataMember]
        public string Longitude { get; set; }
        [DataMember]
        public string Address1 { get; set; }
        [DataMember]
        public string Address2 { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string Zip { get; set; }
        [DataMember]
        public string Country { get; set; }

        public SkaterJson()
        {
        }


    }
}
