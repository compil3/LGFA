using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using LGFABot.Tools;
using LGFABot.Tools.Properties;

namespace LGFABot.Core.Commands
{
    public class TeamStats : ModuleBase<SocketCommandContext>
    {
        #region Team Stat Method
        [Command("ts")]
        [Alias("teamstats")]
        [Summary(".ts TeamName xbox/psn [eg: .ts Liverpool Xbox]")]
        [Remarks("Returns statistics for the team entered if found.")]
        [RequireUserPermission(ChannelPermission.SendMessages)]
        public async Task GetTeamStats(string teamName, string league) //string seasonType = null, int seasonNumber = null
        {
            var tableName = "";
            var dbName = "";
            var outPutSeason = "";

            var user = Context.User as SocketGuildUser;
            var success = false;
            try
            {
                using (var teamDatabase = new LiteDatabase(@"LGFA_Current.db"))
                {
                    //if (seasonType == null && seasonNumber == 0)
                    //{
                    //    tableName = "CRS_Team";
                    //    outPutSeason = "Current Season";
                    //} else if (seasonType == null && seasonNumber < int.Parse(FetchTooler.GetSeasonID(league)))
                    //{
                    //    tableName = 
                    //}
                    //if (seasonNumber == null)
                    //{
                    //    switch (seasonType)
                    //    {
                    //        case "reg":
                    //            tableName = "CRS_Team";
                    //            outPutSeason = "Regular Season";
                    //            break;
                    //        case "pre":
                    //            tableName = "CPS_Team";
                    //            outPutSeason = "Pre-Season";
                    //            break;
                    //    }
                    //} else
                    //{
                    //    switch (seasonType)
                    //    {
                    //        case "reg":
                    //            tableName = "Regular" + seasonNumber;
                    //            outPutSeason = "Regular Season";
                    //            break;
                    //        case "pre":
                    //            tableName = "Pre-season" + seasonNumber;
                    //            outPutSeason = "Pre-Season";
                    //            break;
                    //    }
                    //}

                    var teams = teamDatabase.GetCollection<TeamProp>(@"CRS_Team");

                    var results = teams.Find(x =>
                        x.TeamName.ToLower().StartsWith(teamName) || x.TeamName.StartsWith(teamName));

                    foreach (var found in results)
                    {
                        var temp = found.TeamName;
                        var systemIcon = "";
                        if (league == "psn" || league == "Psn" || league == "PSN")
                        {
                            systemIcon =
                                "https://media.playstation.com/is/image/SCEA/navigation_home_ps-logo-us?$Icon$";
                        }
                        else if (league == "xbox" || league == "Xbox")
                        {
                            systemIcon =
                                "http://www.logospng.com/images/171/black-xbox-icon-171624.png";
                        }

                        //Win-Draw-Lose compressed
                        string[] WDLArray = new string[3];
                        WDLArray[0] = found.GamesWon;
                        WDLArray[1] = found.GamesDrawn;
                        WDLArray[2] = found.GamesLost;
                        var winDrawLose = string.Join("-", WDLArray);

                        //GF-GA-CS compressed
                        string[] ScoringArray = new string[3];
                        ScoringArray[0] = found.GoalsFor;
                        ScoringArray[1] = found.GoalsAgainst;
                        ScoringArray[2] = found.CleanSheets;
                        var scoringRecord = string.Join("-", ScoringArray);

                        //add link to team page ex https://www.leaguegaming.com/forums/index.php?leaguegaming/league&action=league_page&page=team_page&teamid=837&leagueid=53&xboxSeasonId=15
                        var builder = new EmbedBuilder()
                            .WithAuthor(author => author
                                .WithName($"Club Statistics provided by Sky Sports")
                                .WithIconUrl(systemIcon))
                            .WithTitle(
                                $"Season {Fetch.GetSeason(league)} Statistic for ***{found.TeamName}***")
                            .WithUrl(found.TeamURL)
                            .WithColor(new Color(0x26A20B))
                            .WithCurrentTimestamp()
                            .WithFooter(footer =>
                            {
                                footer
                                    .WithText("leaguegaming.com")
                                    .WithIconUrl("https://www.leaguegaming.com/images/league/icon/l53.png");
                            })
                            .WithThumbnailUrl(found.TeamIconUrl)
                            //                        .WithImageUrl("https://www.leaguegaming.com/images/league/icon/l53.png")
                            .AddField("Table Position", found.Rank)
                            .AddField("W-D-L", winDrawLose, true)
                            .AddField("Pts", found.Points, true)
                            .AddField("Stk", found.Streak, true)
                            .AddField("GF-GA-CS", scoringRecord, true)
                            .AddField("L10", found.LastTenGames, true)
                            .AddField("Home", found.HomeRecord, true)
                            .AddField("Away", found.AwayRecord, true)
                            .AddField("1GG", found.OneGoalGames, true);

                        var embed = builder.Build();
                        await Context.Channel.SendMessageAsync(null, embed: embed).ConfigureAwait(false);
                        success = true;
                        builder = null;
                        GC.Collect();
                        break;
                    }

                    #endregion

                    #region PSN team stat search and display

                }

                if (!success)
                {
                    Log.Information($"{teamName} not found.");
                    await Context.Channel.SendMessageAsync(
                        $"Sorry {user.Mention} but ***{teamName}*** was not found.  Try spelling it differently");
                    return;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
    #endregion
}
