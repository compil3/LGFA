using HtmlAgilityPack;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Diagnostics;
using System.Net;
using System.Web;
using LGFABot.DataHandler;

namespace LGFA.DataHandler.PlayerStats
{
    public class PlayerStatsHandler
    {
        public static void GetPlayerStats(string League, string Trigger)
        {

            var st = new StackTrace();
            var sf = st.GetFrame(0);
            var currentMethod = sf.GetMethod();


            var web = new HtmlWeb();
            var doc = web.Load(FetchTooler.GetURL(League, Trigger));


            #region Player count calculator
            var countPlayers = doc.DocumentNode.SelectNodes("//*[@id='lgtable_memberstats51']/tbody/tr");
            var playerCount = countPlayers.Count;

            #endregion
            //Gets the Draftlist URL to count the number of players
            try
             {
                for (int i = 1; i < playerCount; i++)
                {
                    var findPlayerNodes = doc.DocumentNode.SelectNodes($"//*[@id='lgtable_memberstats51']/tbody/tr[{i}]");
                    if (findPlayerNodes == null) break;
                    foreach (var player in findPlayerNodes)
                    {
                        //var rank = WebUtility.HtmlDecode(player.SelectSingleNode($"//*[@id='lgtable_memberstats51']/tbody/tr[{playerIndex}]/td[1]").InnerText);
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
                        var playerID = temp.Get("userid");

                        StatsSaveHandler.SavePlayerStats(Int32.Parse(playerID), iconURL.Trim(), position.Trim(), playerName.Trim(), gamesPlayed.Trim(), record.Trim(), avgMatchRating.Trim(), goals.Trim(), assists.Trim(), cleanSheets.Trim(),
                            shotsOnGoal.Trim(), shotsOnTarget.Trim(), shotPercentage.Trim(), tackles.Trim(), tackleAttempts.Trim(), tacklePercentage.Trim(), passingPercentage.Trim(), keyPasses.Trim(), interceptions.Trim(),
                            blocks.Trim(), yellowCards.Trim(), redCards.Trim(), manOfTheMatch.Trim(), playerURL, League);
                        GC.Collect();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error Parsing Player Stats: {ex}");
            }
            doc = null;
            GC.Collect();
        }
    }
}