using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Utilities;
using RDN.Portable.Classes.Voting;
using RDN.Portable.Config;
using RDN.Portable.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.WP.Library.Classes.League
{
    public class PollsMobile
    {
        public static PollBase GetPolls(Guid memId, Guid uid)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new MemberPassParams() { Mid = memId, Uid = uid }), MobileConfig.LEAGUE_POLLS_GET_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<PollBase>(json);
        }
        public static VotingClass GetPoll(Guid memId, Guid uid, long pollId)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new MemberPassParams() { Mid = memId, Uid = uid, IdOfAnySort = pollId }), MobileConfig.LEAGUE_POLLS_GET_POLL_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<VotingClass>(json);
        }

        public static VotingClass SendNewPoll(VotingClass voting)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(voting), MobileConfig.LEAGUE_POLLS_ADD_NEW_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<VotingClass>(json);
        }
        public static VotingClass SendUpdatedPoll(VotingClass voting)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(voting), MobileConfig.LEAGUE_POLLS_UPDATE_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<VotingClass>(json);
        }
        public static VotingClass VoteOnPoll(VotingClass voting)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(voting), MobileConfig.LEAGUE_POLLS_VOTE_POLL_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<VotingClass>(json);
        }
        public static VotingClass ClosePoll(VotingClass voting)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(voting), MobileConfig.LEAGUE_POLLS_CLOSE_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<VotingClass>(json);
        }
        public static VotingClass DeletePoll(VotingClass voting)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(voting), MobileConfig.LEAGUE_POLLS_DELETE_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<VotingClass>(json);
        }
    }
}
