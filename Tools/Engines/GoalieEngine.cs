using System;
using System.Diagnostics;
using System.Net;
using System.Web;
using HtmlAgilityPack;
using LGFABot.Tools.Engines;
using Serilog;

namespace LGFABot.Tools
{
    class GoalieEngine
    {
        #region Goalie Engine

        public static bool GetGoalie(string League, string Trigger, int HistoricalSeasonID, string SeasonTypeId,string Command)
        {
            var st = new StackTrace();
            var sf = st.GetFrame(0);
            var currentMethod = sf.GetMethod();

            var thisFile = new System.Diagnostics.StackTrace(true).GetFrame(0).GetFileName();
            var web = new HtmlWeb();
            var psnDoc = web.Load(Fetch.GetURL(League, Trigger, HistoricalSeasonID, SeasonTypeId));

            #region Player total calculator
            var countPlayers = psnDoc.DocumentNode.SelectNodes(("//*[@id='lgtable_goaliestats51']/tbody/tr"));
            if (countPlayers == null) return false;
            var playerTotal = countPlayers.Count;
            #endregion

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

                        teamIconShort = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[2]/img").Attributes["src"].Value);
                        playerName = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[2]/a").InnerText);
                        gamesPlayed = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[4]").InnerText);
                        record = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[5]").InnerText);
                        goalsAgainst = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[6]").InnerText);
                        shotsAgainst = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[7]").InnerText);
                        saves = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[8]").InnerText);
                        savePercentage = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[9]").InnerText);
                        goalsAgainstAvg = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[10]").InnerText);
                        cleanSheets = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[11]").InnerText);
                        manOfTheMatch = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[12]").InnerText);
                        avgMatchRating = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[13]").InnerText);
                        playerShortURL = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_goaliestats51']/tbody/tr[{i}]/td[2]/a").Attributes["href"].Value);


                        var playerURL = string.Join(string.Empty,
                            "https://www.leaguegaming.com/forums/" + playerShortURL);
                        var temp = HttpUtility.ParseQueryString(new Uri(playerURL).Query);
                        var playerID = int.Parse(temp.Get("userid"));
                        var iconEnlarge = teamIconShort.Replace("p16", "p100");
                        var iconURL = string.Join(string.Empty, "https://www.leaguegaming.com" + iconEnlarge);

                        if (SeasonTypeId == "pre") SeasonTypeId = "pre-season";
                        else if (SeasonTypeId == "reg") SeasonTypeId = "regular";
                        if (savePercentage == String.Empty) savePercentage = "0";

                        Tools.DataSaver.SavePlayerUrl(playerID, playerName, playerURL);
                        //Console.WriteLine($"Saved {playerName} url. GoaliEngine");
                        CareerEngine.GetCareer(playerID, playerName, League);
                        //Console.WriteLine($"{playerName}, career saved. GoalieEngine");


                        DataSaver.SaveGoalie(playerID, League, playerName,
                            gamesPlayed, record, goalsAgainst, shotsAgainst, saves, savePercentage, goalsAgainstAvg,
                            cleanSheets, manOfTheMatch, avgMatchRating, playerURL, iconURL, Command, HistoricalSeasonID, SeasonTypeId);
                        GC.Collect();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, $"Error processing PSN Goalie Stats {currentMethod}");
                return false;
            }
        }
        #endregion
    }
}
