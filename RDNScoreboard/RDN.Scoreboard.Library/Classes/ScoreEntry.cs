namespace Scoreboard.Library.Classes
{
    public class ScoreEntry
    {
        public int JamNumber { get; set; }
        public int Period { get; set; }
        public int Team1ScoreInJam { get; set; }
        public int Team2ScoreInJam { get; set; }

        public ScoreEntry()
        {
            Team1ScoreInJam = 0;
            Team2ScoreInJam = 0;
        }
    }
}
