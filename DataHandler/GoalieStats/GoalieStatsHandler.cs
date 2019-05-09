using System;
using System.Diagnostics;
using System.Net;
using System.Web;
using HtmlAgilityPack;
using LGFA_Bot.DataHandler.GoalieStats;
using Serilog;

namespace LGFABot.DataHandler.GoalieStats
{
    class GoalieStatsHandler
    {
        public static void GetGoalieStats(string System, string Trigger)
        {
            var st = new StackTrace();
            var sf = st.GetFrame(0);
            var currentMethod = sf.GetMethod();

            var web = new HtmlWeb();
            var psnDoc = web.Load(FetchTooler.GetURL(System, Trigger));

            #region Player total calculator
            var countPlayers = psnDoc.DocumentNode.SelectNodes(("//*[@id='lgtable_goaliestats51']/tbody/tr"));
            var playerTotal = 0;

            foreach (var counted in countPlayers)
            {
                playerTotal++;
            }
            #endregion
            

            #region PSN Goalie Stats
            try
            {
                for (int i = 1; i <= playerTotal; i++)
                {
                    var findPlayerNodes =
                        psnDoc.DocumentNode.SelectNodes($"//*[@id='lgtable_goaliestats51']/tbody/tr[1]");
                    if (findPlayerNodes == null) break;

                    foreach (var player in findPlayerNodes)
                    {
                        #region parse variables
                        var position = "";
                        var lgRank = "";
                        var teamIconShort = "";
                        var playerName = "";
                        var gamesPlayed = "";
                        var record = "";
                        var goalsAgainst = "";
                        var shotsAgainst = "";
                        var saves = "";
                        var savePercentage = "";
                        var goalsAgainstAvg = "";
                        var cleanSheets = "";
                        var manOfTheMatch = "";
                        var avgMatchRating = "";
                        var playerShortURL = "";
#endregion

                        position = WebUtility.HtmlDecode(player
                            .SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[2]/span").InnerText);

                        teamIconShort = WebUtility.HtmlDecode(player
                            .SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[2]/img")
                            .Attributes["src"].Value);
                        playerName = WebUtility.HtmlDecode(player
                            .SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[2]/a").InnerText);
                        gamesPlayed = WebUtility.HtmlDecode(player
                            .SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[4]").InnerText);
                         record = WebUtility.HtmlDecode(player
                            .SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[5]").InnerText);
                         goalsAgainst = WebUtility.HtmlDecode(player
                            .SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[6]").InnerText);
                         shotsAgainst = WebUtility.HtmlDecode(player
                            .SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[7]").InnerText);
                         saves = WebUtility.HtmlDecode(player
                            .SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[8]").InnerText);
                         savePercentage = WebUtility.HtmlDecode(player
                            .SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[9]").InnerText);
                         goalsAgainstAvg = WebUtility.HtmlDecode(player
                            .SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[10]").InnerText);
                         cleanSheets = WebUtility.HtmlDecode(player
                            .SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[11]").InnerText);
                         manOfTheMatch = WebUtility.HtmlDecode(player
                            .SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[12]").InnerText);
                         avgMatchRating = WebUtility.HtmlDecode(player
                            .SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[13]").InnerText);
                         playerShortURL = WebUtility.HtmlDecode(player
                            .SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[2]/a")
                            .Attributes["href"].Value);


                        var playerURL = string.Join(string.Empty,
                            "https://www.leaguegaming.com/forums/" + playerShortURL);
                        var temp = HttpUtility.ParseQueryString(new Uri(playerURL).Query);
                        var playerID = temp.Get("userid");
                        var iconEnlarge = teamIconShort.Replace("p16", "p100");
                        var iconURL = string.Join(string.Empty, "https://www.leaguegaming.com" + iconEnlarge);
                        GoalieStatsSaveHandler.SaveGoalieStats(int.Parse(playerID), "psn", playerName,
                            gamesPlayed, record, goalsAgainst, shotsAgainst, saves, savePercentage, goalsAgainstAvg,
                            cleanSheets, manOfTheMatch, avgMatchRating, playerURL, iconURL);
                        GC.Collect();
                    }
                }
            }
            catch (Exception e)
            {
               Log.Warning(e, $"Error processing PSN Goalie Stats {currentMethod}");
            }
#endregion
        }
    }
}