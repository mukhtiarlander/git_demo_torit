using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.League.Classes;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Controls.Message.Enums;
using RDN.Portable.Classes.League.Classes;
using RDN.Portable.Classes.League;

namespace RDN.Library.Classes.Messages.Classes
{
    public class MessageDisplay
    {
        public long MessageId { get; set; }
        public string Title { get; set; }
        public string MessageText { get; set; }
        public string MessageTextHtml { get; set; }
        public DateTime MessageCreated { get; set; }
        public Guid FromId { get; set; }
        public string FromName { get; set; }
        //public DateTime MessageRead { get; set; }
        //public bool IsMessageRead { get; set; }
        public GroupOwnerTypeEnum OwnerType { get; set; }
        public List<MemberDisplayBasic> Recipients { get;set; }
        public List<LeagueGroup> Groups { get; set; }
        public bool SendEmailForMessage { get; set; }
        public string ToMemberNames { get; set; }
        public string ToMemberIds{ get; set; }
        public string ToMemberEnteredIds { get; set; }
        public string ToGroupIds { get; set; }
        

        public MessageDisplay()
        {
            Recipients = new List<MemberDisplayBasic>();
            Groups = new List<LeagueGroup>();
        }
    }
}
