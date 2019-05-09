using System;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using Serilog;

namespace LGFABot.Tools
{
    public class TeamEngine
    {
        public static bool GetTeam(string System, string Trigger, int SeasonId, string SeasonTypeId, string Command)
        {
            var web = new HtmlWeb();
            var doc = web.Load(Fetch.GetURL(System, Trigger, SeasonId, SeasonTypeId));

            var findRankNodes = doc.DocumentNode.SelectNodes("//*[@style = 'overflow:hidden;padding:0px;']").ToList();
            var rank = "";
            var teamName = "";
            var teamIndex = 1;
            var gamesPlayed = "";
            var gamesWon = "";
            var gamesDrawn = "";
            var gamesLost = "";
            var points = "";
            var streak = "";
            var goalsFor = "";
            var goalsAgainst = "";
            var cleanSheets = "";
            var lastTenGames = "";
            var homeRecord = "";
            var awayRecord = "";
            var oneGoalGames = "";
            var teamIconUrl = "";
            var teamUrl = "";
            var teamID = "";
            try
            {
                foreach (var team in findRankNodes)
                {
                    #region Splits Team Rank & name into 2 variables

                    var tempRank = team.InnerText;
                    string[] splitRank = tempRank.Split(' ');
                    if (splitRank.Length > 3)
                    {
                        var split = tempRank.Split(new[] {' '}, 3);
                        rank = split[1];
                        teamName = split[2];
                    }
                    else
                    {
                        rank = splitRank[1];
                        teamName = splitRank[2];
                    }

                    #endregion

                    gamesPlayed = team.NextSibling.NextSibling.InnerText;
                    gamesWon = team.NextSibling.NextSibling.NextSibling.NextSibling.InnerText;
                    gamesDrawn = team.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.InnerText;
                    gamesLost = team.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling
                        .NextSibling.InnerText;
                    points = team.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling
                        .NextSibling.NextSibling.NextSibling.InnerText;
                    streak = team.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling
                        .NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.InnerText;
                    goalsFor = team.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling
                        .NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.InnerText;
                    goalsAgainst = team.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling
                        .NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling
                        .NextSibling.NextSibling.InnerText;
                    cleanSheets = team.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling
                        .NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling
                        .NextSibling.NextSibling.NextSibling.NextSibling.InnerText;
                    lastTenGames = team.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling
                        .NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling
                        .NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.InnerText;
                    homeRecord = team.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling
                        .NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling
                        .NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling
                        .InnerText;
                    awayRecord = team.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling
                        .NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling
                        .NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling
                        .NextSibling.NextSibling.InnerText;
                    oneGoalGames = team.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling
                        .NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling
                        .NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling
                        .NextSibling.NextSibling.NextSibling.NextSibling.InnerText;
                    teamIconUrl = team
                        .SelectSingleNode(
                            $"//*[@id='content']/div/div/div/div/div/div/div[1]/table/tbody[{teamIndex}]/tr/td[1]/div/img")
                        .Attributes["src"].Value;
                    teamUrl = team
                        .SelectSingleNode(
                            $"//*[@id='content']/div/div/div/div/div/div/div[1]/table/tbody[{teamIndex}]/tr/td[1]/div/a")
                        .Attributes["href"].Value;

                    // Console.Write($"TeamName: {teamName}\nTeamIcon: {teamIconUrl}\nTeamUrl: {teamUrl}\n\n");
                    var iconEnlarge = teamIconUrl.Replace("p38", "p100");
                    teamIconUrl = string.Join(String.Empty, "https://www.leaguegaming.com" + iconEnlarge);
                    teamUrl = string.Join(string.Empty, "https://www.leaguegaming.com/forums/" + teamUrl);
                    var parameters = HttpUtility.ParseQueryString(new Uri(teamUrl).Query);
                    teamID = parameters.Get("teamid");
                    var tempStrip = rank.Replace(")", "").Trim();
                    rank = tempStrip;

                    if (SeasonTypeId == "pre") SeasonTypeId = "pre-season";
                    else if (SeasonTypeId == "reg") SeasonTypeId = "regular";

                    DataSaver.SaveTeam(int.Parse(teamID), int.Parse(rank), teamName, gamesPlayed, gamesWon,
                        gamesDrawn, gamesLost, points, streak, goalsFor, goalsAgainst, cleanSheets, lastTenGames,
                        homeRecord, awayRecord, oneGoalGames, teamIconUrl, teamUrl, System, SeasonId,
                        SeasonTypeId, Command);
                    teamIndex++;
                    GC.Collect();
                }
                Log.Logger.Warning("Team Databases updated.");
                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Error with TeamStatsHandler: {ex}");
                GC.Collect();
                return false;
            }
        }
    }
}

//#region Data Parser

            //try
            //{
            //    for (int i = 5; i <= countTeams; i++)
            //    {
            //         foreach (var teamInNode in findTeamNodes) {
            //            for (int j = i-1; j < findRankNodes.Count; j++)
            //            {
            //var rankNode = findTeamNodes[j];
            //var rank = rankNode.InnerText;
            //string[] splitTemp = rank.Split(' ');
            //if (splitTemp.Length > 3)
            //{
            //    var split = rank.Split(new[] {' '}, 3);
            //    tempRank = split[1];
            //    tempName = split[2];
            //} else {
            //    tempRank = splitTemp[1];
            //    tempName = splitTemp[2];
            //}
            //break;
       // }

                        //var gamesPlayed = WebUtility.HtmlDecode(teamInNode.NextSibling.NextSibling.InnerText);
                        //if (tempName == "U.S. Sassuolo Calcio")
                        //{
                        //    teamIconUrl = "/images/team/p38/team1031.png";                      
                        //}
                        //else
                        //{
                        //    teamIconUrl = WebUtility.HtmlDecode(teamInNode
                        //        .SelectSingleNode(
                        //            $"//*[@id='content']/div/div/div/div/div/div/div[1]/table/tbody[{i}]/tr/td[1]/div/img")
                        //        .Attributes["src"].Value);
                        //}

                        //var gamesWon = WebUtility.HtmlDecode(teamInNode
                        //    .SelectSingleNode(
                        //        $"//*[@id='lgleague_page_loader']/div/div/div/div/div[2]/table/tbody[{i}]/tr/td[2]")
                        //    .InnerText);
                        //var gamesDrawn = WebUtility.HtmlDecode(teamInNode
                        //    .SelectSingleNode(
                        //        $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{i}]/tr/td[4]")
                        //    .InnerText);
                        //var gamesLost = WebUtility.HtmlDecode(teamInNode
                        //    .SelectSingleNode(
                        //        $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{i}]/tr/td[5]")
                        //    .InnerText);
                        //var points = WebUtility.HtmlDecode(teamInNode
                        //    .SelectSingleNode(
                        //        $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{i}]/tr/td[6]")
                        //    .InnerText);
                        //var streak = WebUtility.HtmlDecode(teamInNode
                        //    .SelectSingleNode(
                        //        $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{i}]/tr/td[7]")
                        //    .InnerText);
                        //var goalsFor = WebUtility.HtmlDecode(teamInNode
                        //    .SelectSingleNode(
                        //        $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{i}]/tr/td[8]")
                        //    .InnerText);
                        //var goalsAgainst = WebUtility.HtmlDecode(teamInNode
                        //    .SelectSingleNode(
                        //        $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{i}]/tr/td[9]")
                        //    .InnerText);
                        //var cleanSheets = WebUtility.HtmlDecode(teamInNode
                        //    .SelectSingleNode(
                        //        $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{i}]/tr/td[10]")
                        //    .InnerText);
                        //var lastTenGames = WebUtility.HtmlDecode(teamInNode
                        //    .SelectSingleNode(
                        //        $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{i}]/tr/td[11]")
                        //    .InnerText);
                        //var homeRecord = WebUtility.HtmlDecode(teamInNode
                        //    .SelectSingleNode(
                        //        $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{i}]/tr/td[12]")
                        //    .InnerText);
                        //var awayRecord = WebUtility.HtmlDecode(teamInNode
                        //    .SelectSingleNode(
                        //        $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{i}]/tr/td[13]")
                        //    .InnerText);
                        //var oneGoalGames = WebUtility.HtmlDecode(teamInNode
                        //    .SelectSingleNode(
                        //        $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{i}]/tr/td[14]")
                        //    .InnerText);
                        ////var teamIconUrl = WebUtility.HtmlDecode(teamInNode.SelectSingleNode($"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{i}]/tr/td[1]/div/img").Attributes["src"].Value);
                        //var teamUrl = WebUtility.HtmlDecode(teamInNode
                        //    .SelectSingleNode(
                        //        $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{i}]/tr/td[1]/div/a")
                        //    .Attributes["href"].Value);


                        ////var rankStripped = tempRank.Replace(")", "").Trim();
                        //var rankConverted = int.Parse(rankStripped);

                        //if (SeasonTypeId == "pre") SeasonTypeId = "pre-season";
                        //else if (SeasonTypeId == "reg") SeasonTypeId = "regular";

                        //Tools.DataSaver.SaveTeam(int.Parse(teamID), int.Parse(rankStripped), tempName, gamesPlayed, gamesWon,
                        //    gamesDrawn, gamesLost, points, streak, goalsFor, goalsAgainst, cleanSheets, lastTenGames,
                        //    homeRecord, awayRecord, oneGoalGames, teamIconURL, finalTeamUrl, System, SeasonId,
                        //    SeasonTypeId, Command);
                        //indexNumber++;
//                        GC.Collect();
//                    break; ;
//                }

//            }

//                return true;
//        }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error with TeamStatsHandler: {ex}");
//            }
//    #endregion

//    doc = null;
//            GC.Collect();

//            return false;
//        }
//    }
//}
