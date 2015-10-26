using RDN.Library.Classes.League.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.Library.Classes.League.Classes
{
    public class CreateLeagueResponse
    {
        public List<CreateLeagueEnum> Errors { get; set; }

        public Guid PendingLeagueId { get; set; }

        public CreateLeagueResponse()
        {
            Errors = new List<CreateLeagueEnum>();
        }
    }
}
