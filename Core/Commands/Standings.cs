using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace LGFABot.Core.Commands
{
    public class Standings : ModuleBase<SocketCommandContext>
    {

        //[Command("standings"), Alias("rank"), Summary("LGFA Standings")]
        public async Task GetHtml()
        {
            var user = Context.User as SocketGuildUser;
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Member");

            if (user.Roles.Contains(role))
            {
                string[,] teamStandings = new string[8, 14];
                var seasonid = 15;
                var url = "https://www.leaguegaming.com/forums/index.php?leaguegaming/league&action=league&page=standing&leagueid=53&seasonid=" + seasonid;

                var web = new HtmlWeb();
                var doc = web.Load(url);

                var teamRank = doc.DocumentNode.SelectNodes("//*[@style = 'overflow:hidden;padding:0px;']");

                var teamPositionList = new List<string>();
                var teamGamesPlayed = new List<string>();
                var teamStats = new List<string>();
                var teamNum = 1;
                var teamIndex = 0;
                var teamColIndex = 0;

                foreach (var teamPosition in teamRank)
                {
                    //Standings
                    var positions = WebUtility.HtmlDecode(teamPosition.SelectSingleNode(".//div[@class = 'team_box_icon']").InnerText); //team rank/name (ex 1) Arsenal)
                    teamStandings[teamIndex, teamColIndex] = positions.ToString();
                    teamColIndex++;

                    //Team Record
                    foreach (var teamRecord in teamRank)
                    {
                        var gamesPlay = WebUtility.HtmlDecode(teamRecord.SelectSingleNode($"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamNum}]/tr/td[2]").InnerText);

                        teamStandings[teamIndex, teamColIndex] = gamesPlay;
                        teamColIndex++;

                        var gamesWon = WebUtility.HtmlDecode(teamRecord.SelectSingleNode($"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamNum}]/tr/td[3]").InnerText);
                        teamStandings[teamIndex, teamColIndex] = gamesWon;
                        teamColIndex++;


                        var gamesDrawn = WebUtility.HtmlDecode(teamRecord.SelectSingleNode($"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamNum}]/tr/td[4]").InnerText);
                        teamStandings[teamIndex, teamColIndex] = gamesDrawn;
                        teamColIndex++;

                        var gamesLost = WebUtility.HtmlDecode(teamRecord.SelectSingleNode($"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamNum}]/tr/td[5]").InnerText);
                        teamStandings[teamIndex, teamColIndex] = gamesLost;
                        teamColIndex++;

                        var teamPoints = WebUtility.HtmlDecode(teamRecord.SelectSingleNode($"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamNum}]/tr/td[6]").InnerText);
                        //var teamPoints = (Int32.Parse(gamesWon) * 3) + (Int32.Parse(gamesDrawn) * 1);
                        teamStandings[teamIndex, teamColIndex] = teamPoints;
                        break;
                    }

                    //Team Streak
                    foreach (var streak in teamRank)
                    {
                        var teamStreak = WebUtility.HtmlDecode(streak.SelectSingleNode($"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamNum}]/tr/td[7]").InnerText);
                        break;
                    }


                    //Goal Stats
                    foreach (var goalStats in teamRank)
                    {
                        var goalsScore = WebUtility.HtmlDecode(goalStats.SelectSingleNode($"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamNum}]/tr/td[8]").InnerText);
                        var goalsAllowed = WebUtility.HtmlDecode(goalStats.SelectSingleNode($"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamNum}]/tr/td[9]").InnerText);
                        var cleanGame = WebUtility.HtmlDecode(goalStats.SelectSingleNode($"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamNum}]/tr/td[10]").InnerText);
                        break;
                    }

                    //Records
                    foreach (var teamRecords in teamRank)
                    {
                        var lastTen = WebUtility.HtmlDecode(teamRecords.SelectSingleNode($"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamNum}]/tr/td[11]").InnerText);
                        var homeRecord = WebUtility.HtmlDecode(teamRecords.SelectSingleNode($"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamNum}]/tr/td[12]").InnerText);
                        var awayRecord = WebUtility.HtmlDecode(teamRecords.SelectSingleNode($"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamNum}]/tr/td[13]").InnerText);
                        var oneGoalGames = WebUtility.HtmlDecode(teamRecords.SelectSingleNode($"//*[@id='content']/div/div/div[3]/div/div/div/div/table/tbody[{teamNum}]/tr/td[14]").InnerText);
                        break;
                    }


                    teamNum++;
                    teamColIndex = 0;
                    teamIndex++;


                }

                #region Something??
                List<string> positionStandings = new List<string>();
                for (int i = 0; i < teamStandings.GetLength(0); i++)
                {
                    positionStandings.Add(teamStandings[i, 0]);
                    var rawRank = teamStandings[i, 0];
                    var rank = rawRank.Substring(0, rawRank.IndexOf(' ') + 4);
                   // Console.WriteLine($"Rank: {rank.Replace(")", "")}");
                }
                List<string> pointsInStandings = new List<string>();
                for (int i = 0; i < teamStandings.GetLength(0); i++)
                {
                    pointsInStandings.Add(teamStandings[i, 5]);
                }
                #endregion
                string flattenedStandings = string.Join(Environment.NewLine, positionStandings);
                string flattenedPoints = string.Join(Environment.NewLine, pointsInStandings);
                #region Embed
                var builder = new EmbedBuilder()
                          .WithTitle("LGFA Team Stats Presented By Sky Sports")
                          .WithUrl("https://www.leaguegaming.com/forums/index.php?leaguegaming/league&action=league&page=standing&leagueid=53&seasonid=15")
                          .WithColor(new Color(0x26A20B))
                          .WithCurrentTimestamp()
                          .WithFooter(footer =>
                          {
                              footer
                              .WithText("leaguegaming.com")
                              .WithIconUrl("https://www.leaguegaming.com/images/league/icon/l53.png");
                          })
                          .WithThumbnailUrl("https://www.leaguegaming.com/images/league/icon/l53.png")
                          .AddField("Team", flattenedStandings, true)
                          .AddField("Points", flattenedPoints, true);
                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(
                    null,
                    embed: embed)
                    .ConfigureAwait(false);
                #endregion
            }
            else await Context.Channel.SendMessageAsync($"Sorry {user.Mention} You don't have permission to run that command.");
        }
    }
}