using System;
using System.Net;
using HtmlAgilityPack;
using LGFABot.Tools.Properties;
using LiteDB;
using Serilog;

namespace LGFABot.Tools.Engines
{
    public class CareerEngine
    {
        //Currently only for Offical line stats
        public static bool GetCareer(int id, string playerName, string system)
        {
            var web = new HtmlWeb();
            var doc = web.Load(PlayerUrl(playerName));
            double matchRating;
            var leagueId = "";
            if (system == "xbox") leagueId = "53";
            else if (system == "psn") leagueId = "73";
            var careerDoc = doc.DocumentNode.SelectNodes($"//*[@id='lg_team_user_leagues-{leagueId}']/div[5]/table/tbody/tr[1]/td[1]");
            double shotPercentage;
            double passPercentage;
            var divNum = 5;
            try
            {
                if (careerDoc == null)
                {
                    careerDoc = doc.DocumentNode.SelectNodes($"//*[@id='lg_team_user_leagues-{leagueId}']/div[4]/table/tbody/tr[1]/td[1]");
                    //var countNodes = careerDoc.Count;
                    //Console.WriteLine(countNodes);
                    divNum = 4;
                    foreach (var careerStats in careerDoc)
                    {
                        //type = Offical or Pre-Season etc
                        //var type = WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[1]").InnerText);
                        var careerRecord = WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[2]").InnerText);
                        string[] splitRecord = careerRecord.Split('-');
                        int wins = int.Parse(splitRecord[0]);
                        int draws = int.Parse(splitRecord[1]);
                        int loses = int.Parse(splitRecord[2]);
                        double officalGames = wins + draws + loses;

                        var amr = double.Parse(WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[3]").InnerText));
                        matchRating = amr == null ? 0 : Math.Round((amr / officalGames * 100),2);


                        var goals = double.Parse(WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[4]").InnerText));
                        var assists = WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[5]").InnerText);

                        var shotOnTarget = WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[6]").InnerText);
                        var shotAttempts = double.Parse(WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[7]").InnerText));
                        //shotPercentage = double.Round((goals / shotAttempts) * 100, 1);
                        shotPercentage = Math.Round((goals / shotAttempts * 100),2);

                        var passesCompleted = double.Parse(WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[8]").InnerText));
                        var passAttempts = double.Parse(WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[9]").InnerText));
                        if (passesCompleted == 0 && passAttempts == 0) passPercentage = 0;
                        else passPercentage = Math.Round((passesCompleted / passAttempts * 100),2);
                        //passPercentage = decimal.Round((passesCompleted / passAttempts)*100, 1);
                        var keyPasses = WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[10]").InnerText);

                        var interceptions = WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[11]").InnerText);
                        var tackles = double.Parse(WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[12]").InnerText));
                        var tackleAttempts = double.Parse(WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[13]").InnerText));
                        //var tacklePercentage = decimal.Round((tackles / tackleAttempts) *100 , 1);
                        var tacklePercentage = Math.Round((tackles / tackleAttempts * 100),2);

                        var blocks = WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[14]").InnerText);

                        var redCards = WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[15]").InnerText);
                        var yellowCards = WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[16]").InnerText);

                        Tools.DataSaver.SaveCareer(id, playerName, careerRecord, officalGames, amr,
                            goals,assists, shotOnTarget, shotAttempts, shotPercentage, passesCompleted, passAttempts,
                            passPercentage, keyPasses, interceptions, tackles, tackleAttempts, tacklePercentage, blocks,
                            redCards, yellowCards);
                    }

                    return true;
                }
                else
                {
                    //var countNodes = careerDoc.Count;
                    //Console.WriteLine(countNodes);
                    careerDoc = doc.DocumentNode.SelectNodes($"//*[@id='lg_team_user_leagues-{leagueId}']/div[5]/table/tbody/tr[1]/td[3]");
                    divNum = 5;
                    foreach (var careerStats in careerDoc)
                    {
                        //type = Offical or Pre-Season etc
                        //var type = WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[1]").InnerText);
                        var careerRecord = WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[2]").InnerText);
                        string[] splitRecord = careerRecord.Split('-');
                        int wins = int.Parse(splitRecord[0]);
                        int draws = int.Parse(splitRecord[1]);
                        int loses = int.Parse(splitRecord[2]);
                        double officalGames = wins + draws + loses;

                        var amr = double.Parse(WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[3]").InnerText));
                        matchRating = amr == null ? 0 : Math.Round((amr / officalGames * 100),2);

                        var goals = double.Parse(WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[4]").InnerText));
                        var assists = WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[5]").InnerText);

                        var shotOnTarget = WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[6]").InnerText);
                        var shotAttempts = double.Parse(WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[7]").InnerText));
                        //shotPercentage = double.Round((goals / shotAttempts) * 100, 1);
                        shotPercentage = Math.Round((goals / shotAttempts * 100),2);

                        var passesCompleted = double.Parse(WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[8]").InnerText));
                        var passAttempts = double.Parse(WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[9]").InnerText));
                        if (passesCompleted == 0 && passAttempts == 0) passPercentage = 0;
                        else passPercentage = Math.Round((passesCompleted / passAttempts * 100),2);
                        //passPercentage = decimal.Round((passesCompleted / passAttempts)*100, 1);
                        var keyPasses = WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[10]").InnerText);

                        var interceptions = WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[11]").InnerText);
                        var tackles = double.Parse(WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[12]").InnerText));
                        var tackleAttempts = double.Parse(WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[13]").InnerText));
                        //var tacklePercentage = decimal.Round((tackles / tackleAttempts) *100 , 1);
                        var tacklePercentage = Math.Round((tackles / tackleAttempts * 100),2);

                        var blocks = WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[14]").InnerText);

                        var redCards = WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[15]").InnerText);
                        var yellowCards = WebUtility.HtmlDecode(careerStats.SelectSingleNode($"//*[@id='lg_team_user_leagues-{leagueId}']/div[{divNum}]/table/tbody/tr[1]/td[16]").InnerText);

                        Tools.DataSaver.SaveCareer(id, playerName, careerRecord, officalGames, amr,
                            goals,
                            assists, shotOnTarget, shotAttempts, shotPercentage, passesCompleted, passAttempts,
                            passPercentage, keyPasses, interceptions, tackles, tackleAttempts, tacklePercentage, blocks,
                            redCards, yellowCards);
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{playerName} {e}");
                Log.Logger.Error(e,$"{playerName} failed");
                return false;
            }
        }

        internal static string PlayerUrl(string lookup)
        {
            using (var playerUrl = new LiteDatabase(@"LGFA.db"))
            {
                var player = playerUrl.GetCollection<PlayerInfo>("Players");
                var result = player.Find(x => x.playerName.StartsWith(lookup));
                foreach (var url in result)
                {
                    var playerId = url.Id;
                    var playerNom = url.playerName;
                    var webUrl = url.playerUrl;
                    //Console.WriteLine($"{playerId}\n{playerNom}\n{webUrl}");
                    return webUrl;
                }
            }
            return null;
        }
    }
}
