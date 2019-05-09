using System;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using Serilog;
using System.Linq;
using System.Threading.Tasks;
using LGFABot.Tools;
using LGFABot.Tools.Properties;

namespace LGFA_Bot.Core.Commands
{
    public class PlayerStats : ModuleBase<SocketCommandContext>
    {
        #region Field Players

        [Command("ps")]
        [Summary(".ps GamerTag (optional season number).  Eg: .ps Brisan or .ps Brisan 12.  \nIf your gamertag has spaces try .ps \"Gamer tag\"")]
        [Remarks("LGFA stats for the current season or previous seasons")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(ChannelPermission.SendMessages)]
        public async Task GetPlayerStats(string playerLookUp, string seasonId = null)
        {
            var tableName = "";
            var dbName = "";
            var outputSeason = "";

            if (seasonId == null) dbName = "LGFA_Current.db";
            else if (seasonId != null) dbName = "Historical_Reg_Player.db";
            //dbName = seasonId == null ? "LGFA_Current.db" : "Historical_Reg_Player.db";


            var user = Context.User as SocketGuildUser;
            var success = false;
            try
            {
                using (var playerDatabase = new LiteDatabase(dbName))
                {
                    if (seasonId == null)
                    {
                        tableName = "CRS_Player";
                        outputSeason = "Current Season";
                    }
                    else if (seasonId != null)
                    {
                        tableName = "HRS_Player" + seasonId;
                        outputSeason = "Regular Season";
                    }

                    var player = playerDatabase.GetCollection<StatProps>(tableName);

                    var result = player.Find(x =>
                        x.PlayerName.StartsWith(playerLookUp) || x.PlayerName.ToLower().StartsWith(playerLookUp));
                    foreach (var found in result)
                    {
                        if (found.Position == "(G)") break; //change .gs to .ps store all data in the CRS table, mark goalie stats as null for players and vice-versa

                        #region Stats compression

                        string[] goalStats = new string[2];
                        goalStats[0] = found.Goals;
                        goalStats[1] = found.Assists;
                        var scoringRecord = string.Join("-", goalStats);

                        string[] shootingStats = new string[3];
                        shootingStats[0] = found.ShotsOnGoal;
                        shootingStats[1] = found.ShotsOnTarget;
                        shootingStats[2] = found.ShotPercentage + "%";
                        var shootingSeasonal = string.Join("-", shootingStats);

                        string[] tacklingStats = new string[3];
                        tacklingStats[0] = found.Tackles;
                        tacklingStats[1] = found.TackleAttempts;
                        tacklingStats[2] = found.TacklePercentage + "%";
                        var tacklingSeasonal = string.Join("-", tacklingStats);

                        string[] passingStats = new string[2];
                        passingStats[1] = found.PassingPercentage + "%";
                        passingStats[0] = found.KeyPasses;
                        var passingSeasonal = string.Join(" - ", passingStats);

                        string[] defenseStats = new string[2];
                        defenseStats[0] = found.Interceptions;
                        defenseStats[1] = found.Blocks;
                        var defensiveSeasonal = string.Join("-", defenseStats);

                        string[] disciplineStats = new string[3];
                        disciplineStats[0] = found.YellowCards;
                        disciplineStats[1] = found.RedCards;
                        disciplineStats[2] = found.ManOfTheMatch;
                        var disciplineSeasonal = string.Join("-", disciplineStats);

                        #endregion

                        var systemIcon = "";
                        if (found.PlayerSystem == "psn")
                        {
                            systemIcon =
                                "https://media.playstation.com/is/image/SCEA/navigation_home_ps-logo-us?$Icon$";
                        }
                        else if (found.PlayerSystem == "xbox")
                        {
                            systemIcon =
                                "http://www.logospng.com/images/171/black-xbox-icon-171624.png";
                        }

                        var builder = new EmbedBuilder()
                            .WithAuthor(author => author
                                .WithName(
                                    $"{found.PlayerSystem.ToUpper()} {outputSeason} {found.SeasonId} Stats provided by Sky Sports")
                                .WithIconUrl(systemIcon))
                            .WithTitle($"Statistics for ***{found.PlayerName}*** {found.Position}")
                            .WithUrl(found.PlayerUrl)
                            .WithColor(new Color(0x26A20B))
                            .WithCurrentTimestamp()
                            .WithFooter(footer =>
                            {
                                footer
                                    .WithText("leaguegaming.com")
                                    .WithIconUrl("https://www.leaguegaming.com/images/league/icon/l53.png");
                            })
                            .WithThumbnailUrl(found.TeamIcon)
                            .AddField("Record", found.Record, true)
                            .AddField("AMR", found.AvgMatchRating, true)
                            .AddField("Goals-Assists", scoringRecord, true)
                            .AddField("SOG-SOT-SH%", shootingSeasonal, true)
                            .AddField("TK-TKA-TK%", tacklingSeasonal, true)
                            .AddField("Key-P%", passingSeasonal, true)
                            .AddField("Int-BLK", defensiveSeasonal, true)
                            .AddField("YC-RC-MOTM", disciplineSeasonal, true);


                        var embed = builder.Build();
                        success = true;
                        await Context.Channel.SendMessageAsync(null, embed: embed).ConfigureAwait(false);
                        builder = null;
                        GC.Collect();
                        break;
                    }
                }

                if (!success)
                {
                    Log.Logger.Warning($"{playerLookUp} not found.");
                    await Context.Channel.SendMessageAsync(
                        $"Sorry {user.Mention} but ***{playerLookUp}*** was not found.  Try spelling it differently or if you gamertag has spaces try .ps \"Gamertag\".  For more info use .help");
                    return;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            //if (!success)
            //{
            //    Log.Logger.Warning($"{playerLookUp} not found.");
            //    await Context.Channel.SendMessageAsync(
            //        $"Sorry {user.Mention} but ***{playerLookUp}*** was not found.  Try spelling it differently");
            //    return;
            //}

        }
        #endregion

        #region Goalie Stats
        [Command("gs"), Alias("goalie")]
        [Summary(".gs GamerTag (optional season number).  Eg: .gs Bauer or .gs Bauer 12.  \nIf your gamertag has spaces try .ps \"Gamer tag\"")]
        [Remarks("LGFA stats for the current season or previous seasons")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(ChannelPermission.SendMessages)]
        public async Task GetGoalieStats(string playerLookUp, string seasonId = null)
        {
            var tablename = "";
            var dbName = "";
            var outputSeason = "";

            if (seasonId == null) dbName = "LGFA_Current.db";
            else if (seasonId != null) dbName = "Historical_Reg_Goalie.db";

            var user = Context.User as SocketGuildUser;
            bool success = false;

            try
            {
                using (var goalieDatabase = new LiteDatabase(dbName))
                {
                    if (seasonId == null)
                    {
                        tablename = "CRS_Goalie";
                        outputSeason = "Current Season";
                    }
                    else if (seasonId != null)
                    {
                        tablename = "HRS_Goalie" + seasonId;
                        outputSeason = "Regular Season";
                    }

                    var goalie = goalieDatabase.GetCollection<GoalieProps>("CRS_Goalie");
                    var result = goalie.Find(x =>
                        x.playerName.ToLower().StartsWith(playerLookUp) || x.playerName.StartsWith(playerLookUp));
                    foreach (var found in result)
                    {

                        #region Stats Compression

                        string[] shotsFaced = new string[3];
                        shotsFaced[0] = found.shotsAgainst;
                        shotsFaced[1] = found.saves;
                        shotsFaced[2] = found.savePercentage + "%";
                        var shotRecord = string.Join("-", shotsFaced);

                        string[] goalsAllowed = new string[2];
                        goalsAllowed[0] = found.goalsAgainst;
                        goalsAllowed[1] = found.goalsAgainstAvg;
                        var goalRecord = string.Join(" - ", goalsAllowed);

                        string[] goalieCM = new string[2];
                        goalieCM[0] = found.cleanSheets;
                        goalieCM[1] = found.manOfTheMatch;
                        var cleanMan = string.Join("-", goalieCM);

                        #endregion

                        var systemIcon = "";
                        if (found.userSystem == "psn")
                        {
                            systemIcon =
                                "https://media.playstation.com/is/image/SCEA/navigation_home_ps-logo-us?$Icon$";
                        }
                        else if (found.userSystem == "xbox")
                        {
                            systemIcon =
                                "http://www.logospng.com/images/171/black-xbox-icon-171624.png";
                        }

                        var builder = new EmbedBuilder()
                            .WithAuthor(author => author
                                .WithName(
                                    $"{found.userSystem.ToUpper()} Season {found.SeasonId} Stats provided by Sky Sports")
                                .WithIconUrl(systemIcon))
                            .WithTitle($"Statistics for ***{found.playerName}*** (G)")
                            .WithUrl(found.playerURL)
                            .WithColor(new Color(0x26A20B))
                            .WithCurrentTimestamp()
                            .WithFooter(footer =>
                            {
                                footer
                                    .WithText("leaguegaming.com")
                                    .WithIconUrl("https://www.leaguegaming.com/images/league/icon/l53.png");
                            })
                            .WithThumbnailUrl(found.teamIcon)
                            .AddField("Record", found.record, true)
                            .AddField("AMR", found.avgMatchRating, true)
                            .AddField("SH-SAV-S%", shotRecord, true)
                            .AddField("GA-GAA", goalRecord, true)
                            .AddField("CS-MOTM", cleanMan, true);
                        var embed = builder.Build();
                        success = true;
                        await Context.Channel.SendMessageAsync(null, embed: embed).ConfigureAwait(false);
                        builder = null;
                        GC.Collect();
                        break;
                    }
                }

                if (!success)
                {
                    Log.Logger.Warning($"{playerLookUp} not found.");
                    await Context.Channel.SendMessageAsync(
                        $"Sorry {user.Mention} but ***{playerLookUp}*** was not found.  Try spelling it differently");
                    return;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        #endregion

        [Command("career")]
        [Alias("cs")]
        [Summary(".career GamerTag (example .career Brisan)")]
        [Remarks("LGFA career stats for the select player")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(ChannelPermission.SendMessages)]
        public async Task GetCareer(string player)
        {
            var user = Context.User as SocketGuildUser;
            var success = false;
            var url = "";
            try
            {
                using (var career = new LiteDatabase(@"LGFA.db"))
                {
                    var playerUrl = career.GetCollection<PlayerInfo>("Players");

                    var urlResult = playerUrl
                        .Find(x => x.playerName.StartsWith(player) || x.playerName.ToLower().StartsWith(player));
                    foreach (var playerInfo in urlResult)
                    {
                        url = playerInfo.playerUrl;
                    }

                    var playerCareer = career.GetCollection<Career>("Career");
                    var result = playerCareer
                        .Find(x => x.PlayerName.StartsWith(player) || x.PlayerName.ToLower().StartsWith(player));
                    foreach (var found in result)
                    {
                        #region Stats Check & compression

                        var matchRating = Math.Round((found.AvgMatchRating / found.GamesPlayed), 1);

                        string[] scoring = new string[3];
                        scoring[0] = found.Goals.ToString();
                        scoring[1] = found.Assists;
                        scoring[2] = found.KeyPasses;
                        var scoringRecord = string.Join(" - ", scoring);

                        string[] shooting = new string[3];
                        shooting[0] = found.ShotAttempts.ToString();
                        shooting[1] = found.ShotsOnTarget.ToString();
                        shooting[2] = found.ShotPercentage.ToString();
                        var shootingRecord = string.Join(" - ", shooting);

                        string[] tackling = new string[3];
                        tackling[0] = found.Tackles.ToString();
                        tackling[1] = found.TackleAttempts.ToString();
                        tackling[2] = found.TacklePercentage.ToString() + "%";
                        var tacklingCareer = string.Join(" - ", tackling);

                        string[] passing = new string[3];
                        passing[0] = found.PassesCompleted.ToString();
                        passing[1] = found.PassesAttempted.ToString();
                        passing[2] = found.PassingPercentage.ToString();
                        var passingCareer = string.Join(" - ", passing);

                        string[] defensive = new string[2];
                        defensive[0] = found.Interceptions;
                        defensive[1] = found.Blocks;
                        var defensiveCareer = string.Join(" - ", defensive);

                        string[] discipline = new string[2];
                        discipline[0] = found.YellowCards;
                        discipline[1] = found.RedCards;
                        var disciplineCareer = string.Join(" - ", discipline);

                        #endregion

                        var builder = new EmbedBuilder()
                            .WithAuthor(author => author
                                .WithName($"Career Statistics provided by Sky Sports"))
                            .WithTitle($"***{found.PlayerName}***")
                            .WithUrl(url)
                            .WithColor(new Color(0x26A20B))
                            .WithCurrentTimestamp()
                            .WithFooter(footer =>
                            {
                                footer
                                    .WithText("leaguegaming.com")
                                    .WithIconUrl("https://www.leaguegaming.com/images/league/icon/l53.png");
                            })
                            .AddField("GP", found.GamesPlayed, true)
                            .AddField("Record (W-D-L)", found.Record, true)
                            .AddField("AMR", matchRating, true)
                            .AddField("G-A-Key)", scoringRecord, true)
                            .AddField("SOG-SOT-SH%", shootingRecord, true)
                            .AddField("TK-TKA-TK%", tacklingCareer, true)
                            .AddField("Passing (PC-PA-P%)", passingCareer, true)
                            .AddField("Int-BLK", defensiveCareer, true)
                            .AddField("YC-RC-MOTM", disciplineCareer, true);


                        var embed = builder.Build();
                        success = true;
                        await Context.Channel.SendMessageAsync(null, embed: embed).ConfigureAwait(false);
                        builder = null;
                        GC.Collect();
                        break;
                    }
                    if (!success)
                    {
                        Log.Logger.Warning($"{player} not found.");
                        await Context.Channel.SendMessageAsync(
                            $"Sorry {user.Mention} but ***{player}*** was not found. Try spelling it differently or if you gamertag has spaces try .ps \"Gamertag\".  For more info use .help");
                        return;
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}





