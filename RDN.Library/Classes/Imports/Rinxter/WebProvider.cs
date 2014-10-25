using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;
using Newtonsoft.Json;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Federation.Enums;
using RDN.Portable.Classes.Federation.Enums;

namespace RDN.Library.Classes.Imports.Rinxter
{
    internal class WebProvider
    {

        public static T Download_serialized_json_data<T>(string url) where T : new()
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(url);
                    if (json_data[0] == '[' && json_data[json_data.Length - 1] == ']')
                    {
                        json_data = json_data.Remove(0, 1).Remove(json_data.Length - 2, 1);
                    }

                    if (json_data.Contains("Error Occurred While Processing:"))
                    {
                        return new T();
                    }

                }
                catch (Exception)
                {
                }

                // if string with JSON data is not empty, deserialize it to class and return its instance 
                return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
            }
        }


        public static ArrayList GetTournamentsUrl(string url, FederationsEnum federationUrl)
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;

                try
                {
                    json_data = w.DownloadString(url);
                }
                catch (Exception ex)
                {
                    ErrorDatabaseManager.AddException(ex, ex.GetType());

                }

                ArrayList list = new ArrayList();
                if (!string.IsNullOrEmpty(json_data.Replace("[", "").Replace("]", "")))
                {
                    int index = 0;
                    do
                    {
                        index = json_data.IndexOf("\"id\":", index + 1);
                        if (index != -1)
                            list.Add("http://www.rinxter.net/" + federationUrl.ToString().ToLower() + "/ds?type=tournament&tournamentId=" +
                                     json_data.Substring(index + 6, json_data.IndexOf("\",", index + 6) - (index + 6)) +
                                     "&output=obj");
                    } while (index != -1);
                }

                return list;
            }
        }


        public static ArrayList GetLeagueUrl(string url, FederationsEnum federationUrl)
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;

                try
                {
                    json_data = w.DownloadString(url);
                }
                catch (Exception ex)
                {
                    ErrorDatabaseManager.AddException(ex, ex.GetType());
                }

                ArrayList list = new ArrayList();

                if (!string.IsNullOrEmpty(json_data))
                {
                    int index = 0;
                    do
                    {
                        index = json_data.IndexOf("\"id\":", index + 1);
                        if (index != -1)
                            list.Add("http://www.rinxter.net/" + federationUrl.ToString().ToLower() + "/ds?type=league&output=obj&leagueId=" +
                                     json_data.Substring(index + 6, json_data.IndexOf("\",", index + 6) - (index + 6)));
                    } while (index != -1);
                }

                return list;
            }
        }


        public static string GetLeagueUrl(int id, FederationsEnum federationUrl)
        {
            return "http://www.rinxter.net/" + federationUrl.ToString().ToLower() + "/ds?type=league&output=obj&leagueId=" + id;
        }


        public static ArrayList GetTeamUrl(string url, FederationsEnum federationUrl)
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;

                try
                {
                    json_data = w.DownloadString(url);
                }
                catch (Exception ex)
                {
                    ErrorDatabaseManager.AddException(ex, ex.GetType());
                }

                ArrayList list = new ArrayList();

                if (!string.IsNullOrEmpty(json_data))
                {
                    int index = 0;
                    do
                    {
                        index = json_data.IndexOf("\"id\":", index + 1);
                        if (index != -1)
                            list.Add("http://www.rinxter.net/" + federationUrl.ToString().ToLower() + "/ds?type=team&output=obj&teamId=" +
                                     json_data.Substring(index + 6, json_data.IndexOf("\",", index + 6) - (index + 6)));
                    } while (index != -1);
                }

                return list;
            }
        }


        public static string GetTeamUrl(int id, FederationsEnum federationUrl)
        {

            return "http://www.rinxter.net/" + federationUrl.ToString().ToLower() + "/ds?type=team&output=obj&teamId=" + id;

        }


        public static ArrayList GetBoutUrl(string url, FederationsEnum federationUrl)
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;

                try
                {
                    json_data = w.DownloadString(url);
                }
                catch (Exception)
                {
                }

                ArrayList list = new ArrayList();

                if (!string.IsNullOrEmpty(json_data))
                {
                    int index = 0;
                    do
                    {
                        index = json_data.IndexOf("\"id\":", index + 1);
                        if (index != -1)
                            list.Add("http://www.rinxter.net/" + federationUrl.ToString().ToLower() + "/ds?type=bout&boutId=" +
                                     json_data.Substring(index + 6, json_data.IndexOf("\",", index + 6) - (index + 6)) +
                                     "&images=1&output=obj");
                    } while (index != -1);
                }

                return list;
            }
        }


        public static string GetBoutUrl(int id, FederationsEnum federationUrl)
        {
            return "http://www.rinxter.net/" + federationUrl.ToString().ToLower() + "/ds?type=bout&boutId=" + id;

        }


        public static ArrayList GetSkaterUrl(string url, FederationsEnum federationUrl)
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;

                try
                {
                    json_data = w.DownloadString(url);
                }
                catch (Exception)
                {
                }

                ArrayList list = new ArrayList();

                if (!string.IsNullOrEmpty(json_data))
                {
                    int index = 0;
                    do
                    {
                        index = json_data.IndexOf("\"id\":", index + 1);
                        if (index != -1)
                            list.Add("http://www.rinxter.net/" + federationUrl.ToString().ToLower() + "/ds?type=skater&output=obj&skaterId=" +
                                     json_data.Substring(index + 6, json_data.IndexOf("\",", index + 6) - (index + 6)) +
                                     "&images=1&output=obj");
                    } while (index != -1);
                }

                return list;
            }
        }


        public static Dictionary<string, string>[] Download_serialized_json_scores_data(string url)
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(url);
                    if (json_data[0] == '[' && json_data[json_data.Length - 1] == ']')
                    {
                        json_data = json_data.Remove(0, 1).Remove(json_data.Length - 2, 1);
                    }

                    if (json_data.Contains("Error Occurred While Processing:"))
                    {
                        return null;
                    }

                    int count = Regex.Matches(json_data, "\"id\"").Count;

                    Dictionary<string, string>[] dicScores = new Dictionary<string, string>[count];

                    if (!string.IsNullOrEmpty(json_data))
                    {
                        int index1 = 0, index2 = 0, k = 0;
                        do
                        {
                            index1 = json_data.IndexOf("\"id\":", index1 + 1);
                            index2 = json_data.IndexOf("\"id\":", index1 + 10);
                            if (index1 != -1 && index2 != -1)
                            {
                                string tmp = json_data.Substring(index1 - 1, index2 - index1 - 1);
                                ScoreObject ro = JsonConvert.DeserializeObject<ScoreObject>(tmp);

                                Dictionary<string, string> dicScore = new Dictionary<string, string>();
                                dicScore.Add("LD1", ro.data[1].Replace("<b>", "").Replace("</b>", ""));
                                //dicScore.Add("Jammer1", ro.data[2]);
                                dicScore.Add("Points1", ro.data[3]);
                                dicScore.Add("Score1", ro.data[4].Replace("<b>", "").Replace("</b>", ""));
                                dicScore.Add("Score2", ro.data[5].Replace("<b>", "").Replace("</b>", ""));
                                dicScore.Add("LD2", ro.data[6].Replace("<b>", "").Replace("</b>", ""));
                                //dicScore.Add("Jammer2", ro.data[7]);
                                dicScore.Add("Points2", ro.data[8]);

                                dicScores[k] = dicScore;

                            }
                            else if (index1 != -1 && index2 == -1)
                            {
                                string tmp = json_data.Substring(index1 - 1, json_data.Length - index1 - 1);
                                ScoreObject ro = JsonConvert.DeserializeObject<ScoreObject>(tmp);

                                Dictionary<string, string> dicScore = new Dictionary<string, string>();
                                dicScore.Add("LD1", ro.data[1].Replace("<b>", "").Replace("</b>", ""));
                                //dicScore.Add("Jammer1", ro.data[2]);
                                dicScore.Add("Points1", ro.data[3]);
                                dicScore.Add("Score1", ro.data[4].Replace("<b>", "").Replace("</b>", ""));
                                dicScore.Add("Score2", ro.data[5].Replace("<b>", "").Replace("</b>", ""));
                                dicScore.Add("LD2", ro.data[6].Replace("<b>", "").Replace("</b>", ""));
                                //dicScore.Add("Jammer2", ro.data[7]);
                                dicScore.Add("Points2", ro.data[8]);

                                dicScores[k] = dicScore;
                            }

                            k++;

                        } while (index1 != -1);

                    }

                    return dicScores;
                }
                catch (Exception ex)
                {
                    ErrorDatabaseManager.AddException(ex, ex.GetType(), additionalInformation: url);

                }
                return null;
            }
        }

        public static Dictionary<string, string>[] Download_serialized_json_penalties_data(string url)
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;

                try
                {
                    json_data = w.DownloadString(url);
                    if (json_data[0] == '[' && json_data[json_data.Length - 1] == ']')
                    {
                        json_data = json_data.Remove(0, 1).Remove(json_data.Length - 2, 1);
                    }

                    if (json_data.Contains("Error Occurred While Processing:"))
                    {
                        return null;
                    }

                    int count = Regex.Matches(json_data, "\"id\"").Count;

                    Dictionary<string, string>[] dicPenalties = new Dictionary<string, string>[count];

                    if (!string.IsNullOrEmpty(json_data))
                    {
                        int index1 = 0, index2 = 0, k = 0;
                        do
                        {
                            index1 = json_data.IndexOf("\"id\":", index1 + 1);
                            index2 = json_data.IndexOf("\"id\":", index1 + 10);
                            if (index1 != -1 && index2 != -1)
                            {
                                string tmp = json_data.Substring(index1 - 1, index2 - index1 - 1);
                                PenaltyObject ro = JsonConvert.DeserializeObject<PenaltyObject>(tmp);

                                Dictionary<string, string> dicPen = new Dictionary<string, string>();
                                dicPen.Add("SkaterID", ro.id);

                                if (ro.data.Count() == 9)
                                {
                                    dicPen.Add("Major1", ro.data[0]);
                                    dicPen.Add("Major2", ro.data[1]);
                                    dicPen.Add("Major3", ro.data[2]);
                                    dicPen.Add("Major4", ro.data[3]);
                                    dicPen.Add("Major5", ro.data[4]);
                                    dicPen.Add("Major6", ro.data[5]);
                                    dicPen.Add("Major7", ro.data[6]);

                                    dicPen.Add("MinorA1", "");
                                    dicPen.Add("MinorA2", "");
                                    dicPen.Add("MinorA3", "");
                                    dicPen.Add("MinorA4", "");

                                    dicPen.Add("MinorB1", "");
                                    dicPen.Add("MinorB2", "");
                                    dicPen.Add("MinorB3", "");
                                    dicPen.Add("MinorB4", "");

                                    dicPen.Add("MinorC1", "");
                                    dicPen.Add("MinorC2", "");
                                    dicPen.Add("MinorC3", "");
                                    dicPen.Add("MinorC4", "");

                                    dicPen.Add("MinorD1", "");
                                    dicPen.Add("MinorD2", "");
                                    dicPen.Add("MinorD3", "");
                                    dicPen.Add("MinorD4", "");
                                }
                                else
                                {
                                    dicPen.Add("MinorA1", ro.data[2]);
                                    dicPen.Add("MinorA2", ro.data[3]);
                                    dicPen.Add("MinorA3", ro.data[4]);
                                    dicPen.Add("MinorA4", ro.data[5]);

                                    dicPen.Add("MinorB1", ro.data[6]);
                                    dicPen.Add("MinorB2", ro.data[7]);
                                    dicPen.Add("MinorB3", ro.data[8]);
                                    dicPen.Add("MinorB4", ro.data[9]);

                                    dicPen.Add("MinorC1", ro.data[10]);
                                    dicPen.Add("MinorC2", ro.data[11]);
                                    dicPen.Add("MinorC3", ro.data[12]);
                                    dicPen.Add("MinorC4", ro.data[13]);

                                    dicPen.Add("MinorD1", ro.data[14]);
                                    dicPen.Add("MinorD2", ro.data[15]);
                                    dicPen.Add("MinorD3", ro.data[16]);
                                    dicPen.Add("MinorD4", ro.data[17]);

                                    dicPen.Add("Major1", ro.data[18]);
                                    dicPen.Add("Major2", ro.data[19]);
                                    dicPen.Add("Major3", ro.data[20]);
                                    dicPen.Add("Major4", ro.data[21]);
                                    dicPen.Add("Major5", ro.data[22]);
                                    dicPen.Add("Major6", ro.data[23]);
                                    dicPen.Add("Major7", ro.data[24]);
                                }
                                dicPenalties[k] = dicPen;

                            }
                            else if (index1 != -1 && index2 == -1)
                            {
                                string tmp = json_data.Substring(index1 - 1, json_data.Length - index1 - 1);
                                PenaltyObject ro = JsonConvert.DeserializeObject<PenaltyObject>(tmp);

                                Dictionary<string, string> dicPen = new Dictionary<string, string>();
                                dicPen.Add("SkaterID", ro.id);
                                if (ro.data.Count() == 9)
                                {
                                    dicPen.Add("Major1", ro.data[0]);
                                    dicPen.Add("Major2", ro.data[1]);
                                    dicPen.Add("Major3", ro.data[2]);
                                    dicPen.Add("Major4", ro.data[3]);
                                    dicPen.Add("Major5", ro.data[4]);
                                    dicPen.Add("Major6", ro.data[5]);
                                    dicPen.Add("Major7", ro.data[6]);


                                    dicPen.Add("MinorA1", "");
                                    dicPen.Add("MinorA2", "");
                                    dicPen.Add("MinorA3", "");
                                    dicPen.Add("MinorA4", "");

                                    dicPen.Add("MinorB1", "");
                                    dicPen.Add("MinorB2", "");
                                    dicPen.Add("MinorB3", "");
                                    dicPen.Add("MinorB4", "");

                                    dicPen.Add("MinorC1", "");
                                    dicPen.Add("MinorC2", "");
                                    dicPen.Add("MinorC3", "");
                                    dicPen.Add("MinorC4", "");

                                    dicPen.Add("MinorD1", "");
                                    dicPen.Add("MinorD2", "");
                                    dicPen.Add("MinorD3", "");
                                    dicPen.Add("MinorD4", "");
                                }
                                else
                                {
                                    dicPen.Add("MinorA1", ro.data[2]);
                                    dicPen.Add("MinorA2", ro.data[3]);
                                    dicPen.Add("MinorA3", ro.data[4]);
                                    dicPen.Add("MinorA4", ro.data[5]);

                                    dicPen.Add("MinorB1", ro.data[6]);
                                    dicPen.Add("MinorB2", ro.data[7]);
                                    dicPen.Add("MinorB3", ro.data[8]);
                                    dicPen.Add("MinorB4", ro.data[9]);

                                    dicPen.Add("MinorC1", ro.data[10]);
                                    dicPen.Add("MinorC2", ro.data[11]);
                                    dicPen.Add("MinorC3", ro.data[12]);
                                    dicPen.Add("MinorC4", ro.data[13]);

                                    dicPen.Add("MinorD1", ro.data[14]);
                                    dicPen.Add("MinorD2", ro.data[15]);
                                    dicPen.Add("MinorD3", ro.data[16]);
                                    dicPen.Add("MinorD4", ro.data[17]);

                                    dicPen.Add("Major1", ro.data[18]);
                                    dicPen.Add("Major2", ro.data[19]);
                                    dicPen.Add("Major3", ro.data[20]);
                                    dicPen.Add("Major4", ro.data[21]);
                                    dicPen.Add("Major5", ro.data[22]);
                                    dicPen.Add("Major6", ro.data[23]);
                                    dicPen.Add("Major7", ro.data[24]);
                                }
                                dicPenalties[k] = dicPen;
                            }

                            k++;

                        } while (index1 != -1);

                    }

                    return dicPenalties;
                }
                catch (Exception)
                {
                }
                return null;
            }
        }


        public static Dictionary<string, string>[] Download_serialized_json_lineups_data(string url)
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(url);
                    if (json_data[0] == '[' && json_data[json_data.Length - 1] == ']')
                    {
                        json_data = json_data.Remove(0, 1).Remove(json_data.Length - 2, 1);
                    }

                    if (json_data.Contains("Error Occurred While Processing:"))
                    {
                        return null;
                    }

                    int count = Regex.Matches(json_data, "\"id\"").Count;

                    Dictionary<string, string>[] dicLineUps = new Dictionary<string, string>[count];

                    if (!string.IsNullOrEmpty(json_data))
                    {
                        int index1 = 0, index2 = 0, k = 0;
                        do
                        {
                            index1 = json_data.IndexOf("\"id\":", index1 + 1);
                            index2 = json_data.IndexOf("\"id\":", index1 + 10);
                            if (index1 != -1 && index2 != -1)
                            {
                                string tmp = json_data.Substring(index1 - 1, index2 - index1 - 1);
                                LineUpObject ro = JsonConvert.DeserializeObject<LineUpObject>(tmp);

                                Dictionary<string, string> dicLineUp = new Dictionary<string, string>();
                                dicLineUp.Add("Jam", ro.id);
                                dicLineUp.Add("TeamName", ro.data[1]);
                                dicLineUp.Add("Jammer", ro.data[2]);
                                dicLineUp.Add("PivotBlocker", ro.data[3]); //HtmlRemoval.StripTagsRegex();
                                dicLineUp.Add("Blocker", (ro.data[4]));
                                dicLineUp.Add("Blocker1", ro.data[5]);
                                if (ro.data.Count > 6)
                                    dicLineUp.Add("Blocker2", ro.data[6]);
                                else
                                    dicLineUp.Add("Blocker2", "");

                                dicLineUps[k] = dicLineUp;

                            }
                            else if (index1 != -1 && index2 == -1)
                            {
                                string tmp = json_data.Substring(index1 - 1, json_data.Length - index1 - 1);
                                LineUpObject ro = JsonConvert.DeserializeObject<LineUpObject>(tmp);

                                Dictionary<string, string> dicLineUp = new Dictionary<string, string>();
                                dicLineUp.Add("Jam", ro.id);
                                dicLineUp.Add("TeamName", ro.data[1]);
                                dicLineUp.Add("Jammer", ro.data[2]);
                                dicLineUp.Add("PivotBlocker", ro.data[3]); //HtmlRemoval.StripTagsRegex();
                                dicLineUp.Add("Blocker", (ro.data[4]));
                                dicLineUp.Add("Blocker1", ro.data[5]);
                                if (ro.data.Count > 6)
                                    dicLineUp.Add("Blocker2", ro.data[6]);
                                else
                                    dicLineUp.Add("Blocker2", "");

                                dicLineUps[k] = dicLineUp;
                            }

                            k++;

                        } while (index1 != -1);

                    }

                    return dicLineUps;
                }
                catch (Exception)
                {
                }
                return null;
            }
        }


        public static Dictionary<string, string>[] Download_serialized_json_seasion_data(string url)
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(url);
                    if (json_data[0] == '[' && json_data[json_data.Length - 1] == ']')
                    {
                        json_data = json_data.Remove(0, 1).Remove(json_data.Length - 2, 1);
                    }

                    if (json_data.Contains("Error Occurred While Processing:"))
                    {
                        return null;
                    }

                    int count = Regex.Matches(json_data, "season").Count;

                    Dictionary<string, string>[] dicSeasions = new Dictionary<string, string>[count];

                    if (!string.IsNullOrEmpty(json_data))
                    {
                        int index1 = 0, index2 = 0, k = 0;
                        do
                        {
                            index1 = json_data.IndexOf("{", index2);
                            index2 = json_data.IndexOf("{", index1 + 10);
                            if (index1 != -1 && index2 != -1)
                            {
                                string tmp = json_data.Substring(index1, index2 - index1 - 1);
                                Seasion seasionObj = JsonConvert.DeserializeObject<Seasion>(tmp);

                                Dictionary<string, string> dicSeasion = new Dictionary<string, string>();
                                dicSeasion.Add("Value", seasionObj.value);
                                dicSeasion.Add("Text", seasionObj.text);

                                dicSeasions[k] = dicSeasion;

                            }
                            else if (index1 != -1 && index2 == -1)
                            {
                                string tmp = json_data.Substring(index1, json_data.Length - index1);
                                Seasion seasionObj = JsonConvert.DeserializeObject<Seasion>(tmp);

                                Dictionary<string, string> dicSeasion = new Dictionary<string, string>();
                                dicSeasion.Add("Value", seasionObj.value);
                                dicSeasion.Add("Text", seasionObj.text);

                                dicSeasions[k] = dicSeasion;
                            }

                            k++;

                        } while (index2 != -1);

                    }

                    return dicSeasions;
                }
                catch (Exception)
                {
                }
                return null;
            }
        }


        public static Dictionary<string, string>[] Download_serialized_json_regions_data(string url)
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(url);
                    if (json_data[0] == '[' && json_data[json_data.Length - 1] == ']')
                    {
                        json_data = json_data.Remove(0, 1).Remove(json_data.Length - 2, 1);
                    }

                    if (json_data.Contains("Error Occurred While Processing:"))
                    {
                        return null;
                    }

                    int count = Regex.Matches(json_data, "region").Count;

                    Dictionary<string, string>[] dicSeasions = new Dictionary<string, string>[count];

                    if (!string.IsNullOrEmpty(json_data))
                    {
                        int index1 = 0, index2 = 0, k = 0;
                        do
                        {
                            index1 = json_data.IndexOf("{", index2);
                            index2 = json_data.IndexOf("{", index1 + 10);
                            if (index1 != -1 && index2 != -1)
                            {
                                string tmp = json_data.Substring(index1, index2 - index1 - 1);
                                Region seasionObj = JsonConvert.DeserializeObject<Region>(tmp);

                                Dictionary<string, string> dicSeasion = new Dictionary<string, string>();
                                dicSeasion.Add("Value", seasionObj.value);
                                dicSeasion.Add("Text", seasionObj.text);

                                dicSeasions[k] = dicSeasion;

                            }
                            else if (index1 != -1 && index2 == -1)
                            {
                                string tmp = json_data.Substring(index1, json_data.Length - index1);
                                Region seasionObj = JsonConvert.DeserializeObject<Region>(tmp);

                                Dictionary<string, string> dicSeasion = new Dictionary<string, string>();
                                dicSeasion.Add("Value", seasionObj.value);
                                dicSeasion.Add("Text", seasionObj.text);

                                dicSeasions[k] = dicSeasion;
                            }

                            k++;

                        } while (index2 != -1);

                    }

                    return dicSeasions;
                }
                catch (Exception)
                {
                }
                return null;
            }
        }

        //public static void DownloadRemoteImageFile(string uri, string fileName)
        //{
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        //    // Check that the remote file was found. The ContentType
        //    // check is performed since a request for a non-existent
        //    // image file might be redirected to a 404-page, which would
        //    // yield the StatusCode "OK", even though the image was not
        //    // found.
        //    if ((response.StatusCode == HttpStatusCode.OK ||
        //        response.StatusCode == HttpStatusCode.Moved ||
        //        response.StatusCode == HttpStatusCode.Redirect) &&
        //        response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
        //    {

        //        // if the remote file was found, download oit
        //        using (Stream inputStream = response.GetResponseStream())
        //        using (Stream outputStream = File.OpenWrite(fileName))
        //        {
        //            byte[] buffer = new byte[4096];
        //            int bytesRead;
        //            do
        //            {
        //                bytesRead = inputStream.Read(buffer, 0, buffer.Length);
        //                outputStream.Write(buffer, 0, bytesRead);
        //            } while (bytesRead != 0);
        //        }
        //    }
        //}


        public class ScoreObject
        {
            public string id { get; set; }
            public List<string> data { get; set; }
        }

        public class PenaltyObject
        {
            public string id { get; set; }
            public List<string> data { get; set; }
        }

        public class LineUpObject
        {
            public string id { get; set; }
            public List<string> data { get; set; }
        }

        public class Seasion
        {
            public string season { get; set; }
            public string value { get; set; }
            public string text { get; set; }
        }

        public class Region
        {
            public string region { get; set; }
            public string value { get; set; }
            public string text { get; set; }
        }
    }
}

