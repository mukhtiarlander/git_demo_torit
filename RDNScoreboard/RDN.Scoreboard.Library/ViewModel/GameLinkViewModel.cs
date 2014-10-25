using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scoreboard.Library.ViewModel
{
    /// <summary>
    /// links that the team can show off.
    /// </summary>
    public enum GameLinkTypeEnum
    {
        YouTube = 0,
        Flickr = 1,
        Twitter = 2,
        Facebook = 3,
        Team1Website = 4,
        Team2Website = 5,
        Other = 6
    }
    /// <summary>
    /// allows the user to add links to their games for youtube, flicker etc.
    /// </summary>
    public class GameLinkViewModel
    {
        public Guid LinkId { get; set; }
        public string GameLink { get; set; }
        public GameLinkTypeEnum LinkType { get; set; }
        /// <summary>
        /// dummy constructor for serialization.
        /// </summary>
        public GameLinkViewModel()
        { }
    }
}
