using System;
using System.IO;
using System.Reflection;
using HtmlAgilityPack;

using LGFABot.Resources.DataTypes;
using LGFABot.Resources.Settings;
using LGFABot.DataHandler.TeamStandings;

using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Serilog;

namespace LGFABot.DataHandler
{
    public class TeamStatsHandler
    {
        public static void GetStats(string System, string Trigger)
        {
            var web = new HtmlWeb();
            var doc = web.Load(FetchTooler.GetURL(System,Trigger));

            var findTeamNodes = doc.DocumentNode.SelectNodes("//*[@style = 'overflow:hidden;padding:0px;']");
            var teamIndexNumber = 1;

            #region Data Fetch and Parse

            try
            {
                foreach (var teamInNode in findTeamNodes)
                {
                    var rank = WebUtility.HtmlDecode(teamInNode
                        .SelectSingleNode(
                            $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamIndexNumber}]/tr/td[1]/div/text()")
                        .InnerText);
                    var teamName = WebUtility.HtmlDecode(teamInNode
                        .SelectSingleNode(
                            $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamIndexNumber}]/tr/td[1]/div/a")
                        .InnerText);
                    var gamesPlayed = WebUtility.HtmlDecode(teamInNode
                        .SelectSingleNode(
                            $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamIndexNumber}]/tr/td[2]")
                        .InnerText);
                    var gamesWon = WebUtility.HtmlDecode(teamInNode
                        .SelectSingleNode(
                            $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamIndexNumber}]/tr/td[3]")
                        .InnerText);
                    var gamesDrawn = WebUtility.HtmlDecode(teamInNode
                        .SelectSingleNode(
                            $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamIndexNumber}]/tr/td[4]")
                        .InnerText);
                    var gamesLost = WebUtility.HtmlDecode(teamInNode
                        .SelectSingleNode(
                            $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamIndexNumber}]/tr/td[5]")
                        .InnerText);
                    var points = WebUtility.HtmlDecode(teamInNode
                        .SelectSingleNode(
                            $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamIndexNumber}]/tr/td[6]")
                        .InnerText);
                    var streak = WebUtility.HtmlDecode(teamInNode
                        .SelectSingleNode(
                            $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamIndexNumber}]/tr/td[7]")
                        .InnerText);
                    var goalsFor = WebUtility.HtmlDecode(teamInNode
                        .SelectSingleNode(
                            $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamIndexNumber}]/tr/td[8]")
                        .InnerText);
                    var goalsAgainst = WebUtility.HtmlDecode(teamInNode
                        .SelectSingleNode(
                            $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamIndexNumber}]/tr/td[9]")
                        .InnerText);
                    var cleanSheets = WebUtility.HtmlDecode(teamInNode
                        .SelectSingleNode(
                            $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamIndexNumber}]/tr/td[10]")
                        .InnerText);
                    var lastTenGames = WebUtility.HtmlDecode(teamInNode
                        .SelectSingleNode(
                            $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamIndexNumber}]/tr/td[11]")
                        .InnerText);
                    var homeRecord = WebUtility.HtmlDecode(teamInNode
                        .SelectSingleNode(
                            $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamIndexNumber}]/tr/td[12]")
                        .InnerText);
                    var awayRecord = WebUtility.HtmlDecode(teamInNode
                        .SelectSingleNode(
                            $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamIndexNumber}]/tr/td[13]")
                        .InnerText);
                    var oneGoalGames = WebUtility.HtmlDecode(teamInNode
                        .SelectSingleNode(
                            $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamIndexNumber}]/tr/td[14]")
                        .InnerText);
                    var teamIconUrl = WebUtility.HtmlDecode(teamInNode
                        .SelectSingleNode(
                            $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamIndexNumber}]/tr/td[1]/div/img")
                        .Attributes["src"].Value);
                    var teamUrl = WebUtility.HtmlDecode(teamInNode
                        .SelectSingleNode(
                            $"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamIndexNumber}]/tr/td[1]/div/a")
                        .Attributes["href"].Value);


                    // Console.WriteLine($"Team Icon Url:{teamUrl}");
                    var iconEnlarge = teamIconUrl.Replace("p38", "p100");
                    var teamIconURL = string.Join(String.Empty, "https://www.leaguegaming.com" + iconEnlarge);
                    var finalTeamUrl = string.Join(string.Empty, "https://www.leaguegaming.com/forums/" + teamUrl);

                    var rankStripped = rank.Replace(")", "").Trim();
                    var rankConverted = int.Parse(rankStripped);
                    var parameters = HttpUtility.ParseQueryString(new Uri(finalTeamUrl).Query);
                    var teamID = parameters.Get("teamid");



                    TeamDataSaveHandler.SaveTeamInfo(int.Parse(teamID), rankConverted, teamName.Trim(),
                        gamesPlayed.Trim(), gamesWon.Trim(), gamesDrawn.Trim(), gamesLost.Trim(), points.Trim(),
                        streak.Trim(),
                        goalsFor.Trim(), goalsAgainst.Trim(), cleanSheets.Trim(), lastTenGames.Trim(),
                        homeRecord.Trim(), awayRecord.Trim(), oneGoalGames.Trim(), teamIconURL.Trim(),
                        finalTeamUrl.Trim(), System);
                    teamIndexNumber++;
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, $"Error with TeamStatsHandler: {ex}");
            }

            doc = null;
            GC.Collect();


        }

        #endregion
    }
}

