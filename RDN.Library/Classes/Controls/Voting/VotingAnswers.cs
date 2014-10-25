using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Controls.Voting
{
  public   class VotingAnswersClass
    {
          public long AnswerId { get; set; }

        public string Answer { get; set; }
        public bool WasRemoved { get; set; }


        public VotingAnswersClass()
        {

        }
    }
}
