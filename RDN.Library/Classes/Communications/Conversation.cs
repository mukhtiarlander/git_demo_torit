using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Communications
{
    public class Conversation
    {
        public Guid OwnerId { get; set; }
        public long Id { get; set; }
        public string MemberName { get; set; }
        public string Chat { get; set; }
        public DateTime Created { get; set; }
        public string Time { get; set; }
        public List<Conversation> Conversations { get; set; }

        static Conversation instance = new Conversation();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Conversation()
        {

        }

        public static Conversation Instance
        {
            get
            {
                return instance;
            }
        }

        public bool AddConversation(Guid id, long convoId, string memberName, string chat, DateTime created)
        {
            Conversation con = new Conversation();
            con.Chat = chat;
            con.OwnerId = id;
            con.Id = convoId;
            con.Created = created;
            con.MemberName = memberName;
            con.Time = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(con.Created);
            if (instance.Conversations == null)
                instance.Conversations = new List<Conversation>();
            instance.Conversations.Add(con);

            return true;
        }
        public bool AddConversation(Conversation sation)
        {
            sation.Time = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(sation.Created);
            if (instance.Conversations == null)
                instance.Conversations = new List<Conversation>();
            instance.Conversations.Add(sation);

            return true;
        }
    }
}
