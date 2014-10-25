using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Utilities.Config;
using System.Xml.Serialization;
using System.IO;
using Scoreboard.Library.ViewModel;
using System.Threading.Tasks;

namespace RDNScoreboard.Code
{
    /// <summary>
    /// we use this class to see if the scoreboard crashed or not.
    /// </summary>
    public class ScoreboardCrashed
    {
        /// <summary>
        /// if set to false on opening of scoreboard, we know the scoreboard crashed.
        /// </summary>
        public bool HasClosedProperly { get; set; }

        public ScoreboardCrashed()
        {

        }
        /// <summary>
        /// we save out the scoreboard crashed class if we close properly.
        /// </summary>
        public static void ClosedProperly()
        {
            Task<bool>.Factory.StartNew(
                () =>
                {
                    ScoreboardCrashed scoreboard = new ScoreboardCrashed();
                    scoreboard.HasClosedProperly = true;

                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(scoreboard.GetType());
                    System.IO.StreamWriter file = new System.IO.StreamWriter(ScoreboardConfig.SAVE_CRASHED_FILE);

                    writer.Serialize(file, scoreboard);
                    file.Close();

                    return true;
                });
        }
        /// <summary>
        /// we just opened the scoreboard up so we have to set the flag to false.
        /// </summary>
        public static void Opened()
        {
            Task<bool>.Factory.StartNew(
                  () =>
                  {
                      ScoreboardCrashed scoreboard = new ScoreboardCrashed();
                      scoreboard.HasClosedProperly = false;

                      System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(scoreboard.GetType());
                      System.IO.StreamWriter file = new System.IO.StreamWriter(ScoreboardConfig.SAVE_CRASHED_FILE);

                      writer.Serialize(file, scoreboard);
                      file.Close();

                      return true;
                  });
        }

        public static void LoadTempGameFile()
        {
            GameViewModel.Instance.loadGameFromXml(ScoreboardConfig.SAVE_TEMP_GAMES_FILE);
            GameViewModel.Instance.IsCurrentlySendingOnlineGame = false;
        }
        /// <summary>
        /// checks if the scoreboard just crashed.
        /// </summary>
        /// <returns></returns>
        public static bool DidScoreboardJustCrash()
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(ScoreboardCrashed));

                //loads policy from a saved settings file.
                if (File.Exists(ScoreboardConfig.SAVE_CRASHED_FILE))
                {
                    StreamReader objReader = new StreamReader(ScoreboardConfig.SAVE_CRASHED_FILE);
                    try
                    {
                        ScoreboardCrashed crashed = (ScoreboardCrashed)xs.Deserialize(objReader);
                        objReader.Close();
                        objReader.Dispose();
                        if (crashed.HasClosedProperly == true)
                            return false;
                        else//scoreboard has crashed.
                            return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                else
                {
                    //if no file, we are opening it up for first time, so it has never crashed.

                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, e.GetType());
            }
            return false;
        }
    }
}
