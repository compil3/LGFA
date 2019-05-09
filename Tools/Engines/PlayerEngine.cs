using System;
using System.Diagnostics;
using System.Net;
using System.Web;
using HtmlAgilityPack;
using LGFABot.Tools.Engines;
using Serilog;

namespace LGFABot.Tools
{
    public class PlayerEngine
    {
        #region Player Engine
        public static bool GetField(string League, string Trigger, int HistoricalSeasonID, string SeasonTypeId, string Command)
        {

            var st = new StackTrace();
            var sf = st.GetFrame(0);
            var currentMethod = sf.GetMethod();
            var thisFile = new System.Diagnostics.StackTrace(true).GetFrame(0).GetFileName();


            var web = new HtmlWeb();
            var playerWeb = new HtmlWeb();
            var doc = web.Load(Fetch.GetURL(League, Trigger, HistoricalSeasonID, SeasonTypeId));

            #region Player count calculator

            var countPlayers = doc.DocumentNode.SelectNodes("//*[@id='lgtable_memberstats51']/tbody/tr");
            if (countPlayers == null) return false;
            var playerCount = countPlayers.Count;
            #endregion

            #region Player stat parser

            try
            {
                for (int i = 1; i < playerCount; i++)
                {
                    var findPlayerNodes = doc.DocumentNode.SelectNodes($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]");
                    if (findPlayerNodes == null) break;
                    foreach (var player in findPlayerNodes)
                    {
                        var teamIconTemp = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[2]/img").Attributes["src"].Value);
                        var position = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[2]/span").InnerText);
                        var playerName = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[2]/a").InnerText);
                        var gamesPlayed = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[4]").InnerText);
                        var record = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[5]").InnerText);
                        var avgMatchRating = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[6]").InnerText);
                        var goals = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[7]").InnerText);
                        var assists = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[8]").InnerText);
                        var cleanSheets = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[9]").InnerText);
                        var shotsOnGoal = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[10]").InnerText);
                        var shotsOnTarget = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[11]").InnerText);
                        var shotPercentage = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[12]").InnerText);
                        var tackles = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[13]").InnerText);
                        var tackleAttempts = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[14]").InnerText);
                        var tacklePercentage = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[15]").InnerText);
                        var passingPercentage = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[16]").InnerText);
                        var keyPasses = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[17]").InnerText);
                        var interceptions = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[18]").InnerText);
                        var blocks = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[19]").InnerText);
                        var yellowCards = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[20]").InnerText);
                        var redCards = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[21]").InnerText);
                        var manOfTheMatch = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[22]").InnerText);
                        var playerShortURL = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]/td[2]/a").Attributes["href"].Value);

                        var playerURL = string.Join(string.Empty, "https://www.leaguegaming.com/forums/" + playerShortURL);
                        var iconEnlarge = teamIconTemp.Replace("p16", "p100");
                        var iconURL = string.Join(string.Empty, "https://www.leaguegaming.com" + iconEnlarge);
                        var temp = HttpUtility.ParseQueryString(new Uri(playerURL).Query);
                        int playerId = int.Parse(temp.Get("userid"));

                        if (shotPercentage == String.Empty) shotPercentage = "0";
                        if (tacklePercentage == string.Empty) tacklePercentage = "0";
                        if (passingPercentage == String.Empty) passingPercentage = "0";

                        if (SeasonTypeId == "pre") SeasonTypeId = "pre-season";
                        else if (SeasonTypeId == "reg") SeasonTypeId = "regular";

                        Tools.DataSaver.SavePlayerUrl(playerId, playerName, playerURL);
                        //Console.WriteLine($"Saved {playerName} url. PLayerEngine");

                        CareerEngine.GetCareer(playerId, playerName, League);
                        //Console.WriteLine($"{playerName}, career saved PLayerEngine");

                        Tools.DataSaver.Save(playerId, HistoricalSeasonID, SeasonTypeId, position,
                            playerName,gamesPlayed, record, avgMatchRating, goals, assists, cleanSheets, shotsOnGoal,
                            shotsOnTarget,shotPercentage, tackles, tackleAttempts, tacklePercentage, passingPercentage, keyPasses,
                            interceptions, blocks, yellowCards, redCards, manOfTheMatch, playerURL, League, iconURL, Command);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, $"Error processing Player Stats {currentMethod}");
                return false;
            }
            #endregion
        }
        #endregion

    }
}
