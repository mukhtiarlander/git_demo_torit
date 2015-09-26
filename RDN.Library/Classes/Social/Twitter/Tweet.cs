using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RDN.Library.Classes.Social.Twitter
{
    public class Tweet
    {
        public long TweetId { get; set; }

        public string Text { get; set; }

        public string TextAsHtml { get; set; }

        public string UserName { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
