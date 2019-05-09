using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGFABot.Tools;
using LGFABot.Tools.Properties;
using Serilog;

namespace LGFABot.Core.Commands
{
    public class TeamStandings : ModuleBase<SocketCommandContext>
    {
        [Command("standings")]
        [Alias("table")]
        [Summary(".standings system  Eg .standings xbox reg ( or .standings xbox reg 14")]
        [Remarks("Pulls up the standings table for PSN or Xbox")]
        [RequireUserPermission(GuildPermission.SendMessages)]
        public async Task GetTeamRanks(string league)
        {
            var tableName = "";
            var dbName = "";
            var outPutSeason = "";


            var success = false;
            var user = Context.User as SocketGuildUser;
            var userRole = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Member");

            try
            {


                using (var teamDatabase = new LiteDatabase(@"LGFA_Current.db"))
                {
                    var collection = teamDatabase.GetCollection<TeamProp>(@"CRS_Team");
                    var rankResults = collection
                        .Find(x => x.TeamName != null)
                        .Where(x => x.Rank > 0 && x.System == league)
                        .OrderBy(x => x.Rank);
                    if (rankResults == null)
                    {
                        await ReplyAsync("No statistics found.");
                        return;
                    }

                    List<string> teamStandings = new List<string>();
                    List<string> teamPointsInStandings = new List<string>();
                    List<int> teamRank = new List<int>();

                    foreach (var found in rankResults)
                    {
                        teamStandings.Add(found.Rank + ") " + found.TeamName);
                        teamPointsInStandings.Add(found.Points);
                        teamRank.Add(found.Rank);
                        var psnFound = found.System;
                        var xboxFound = found.System;
                    }

                    string flattenedRank = string.Join(Environment.NewLine, teamRank);
                    string flattenStandings = string.Join(Environment.NewLine, teamStandings);
                    string flattendPoints = string.Join(Environment.NewLine, teamPointsInStandings);

                    var leagueid = 0;
                    if (league == "xbox") leagueid = 53;
                    else leagueid = 73;

                    var systemIcon = "";
                    if (league == "psn")
                        systemIcon = "https://media.playstation.com/is/image/SCEA/navigation_home_ps-logo-us?$Icon$";
                    else if (league == "xbox")
                        systemIcon = "http://www.logospng.com/images/171/black-xbox-icon-171624.png";

                    //var seasonTypeId = "";
                    //if (seasonType == "reg") seasonTypeId = "1";
                    //else if (seasonType == "pre") seasonTypeId = "0";
                    var seasonId = Fetch.GetSeason(league);
                    var builder = new EmbedBuilder()
                        .WithAuthor(author => author
                            .WithName($"LGFA {league.ToUpper()} Table provided by Sky Sport")
                            .WithIconUrl(systemIcon))
                        .WithTitle($"*** Regular Season ({Fetch.GetSeason(league)}) Standings***")
                        .WithUrl(
                            $"https://www.leaguegaming.com/forums/index.php?leaguegaming/league&action=league&page=standing&leagueid=" +
                            leagueid + "&seasonid=" + seasonId + "$seasontypeid=" + 1)
                        .WithColor(new Color(0x26A20B))
                        .WithCurrentTimestamp()
                        .WithFooter(footer =>
                        {
                            footer
                                .WithText("leaguegaming.com")
                                .WithIconUrl("https://www.leaguegaming.com/images/league/icon/l53.png");
                        })
                        .WithThumbnailUrl("https://www.leaguegaming.com/images/league/icon/l53.png")
                        .AddField("Ranking", flattenStandings, true)
                        .AddField("Points", flattendPoints, true);

                    var embed = builder.Build();
                    await ReplyAsync(
                            null,
                            embed: embed)
                        .ConfigureAwait(false);
                    success = true;
                    builder = null;
                    GC.Collect();
                }

                if (!success)
                {
                    Log.Logger.Information($".standings Error: The input text has too few parameters.");
                    await Context.Channel.SendMessageAsync(
                        "Command is missing parameters. Try adding ***xbox*** or ***psn*** (.standings psn)");
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
