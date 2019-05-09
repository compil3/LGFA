using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LGFABot.Resources.DataTypes;
using LGFABot.Tools;
using Newtonsoft.Json;
using Serilog;

namespace LGFABot.Core.Commands
{
    public class UpdateSeason : ModuleBase<SocketCommandContext>
    {
        #region
        [Command("update")]
        [Alias("xu", "pu", "xbox", "psn")]
        [Summary(".update #")]
        [Remarks("Updates the season id")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task UpdateSeasonNumber(string system, string newSeason)
        {
            var user = Context.User as SocketGuildUser;
            //Me, Dunks, Brisan
            //remove this as it's not needed
            if (user.Id == 111252573054312448 || user.Id == 336185156106846209 || user.Id == 321805929517678612)
            {
                string JSON = "";
                var xboxCurrentSeason = "";
                var xboxPreviousSeason = xboxCurrentSeason;
                var psnCurrentSeason = "";
                var psnPreviousSeason = psnCurrentSeason;

                var sFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var settingsFile = @"Configuration\websettings.json";
                var configLocation = Path.Combine(sFile, settingsFile);


                if (!File.Exists(configLocation))
                    Log.Error($"UpdateSeason method: {configLocation} does not exist (Ln 27");
                try
                {
                    using (var stream = new FileStream(configLocation, FileMode.Open, FileAccess.ReadWrite))
                    using (var readSettings = new StreamReader(stream))
                    {
                        JSON = readSettings.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Error reading websettings.json ({configLocation}");
                    throw;
                }

                UrlSettings jsonObj = JsonConvert.DeserializeObject<UrlSettings>(JSON);
                xboxCurrentSeason = jsonObj.xboxSeasonId;
                psnCurrentSeason = jsonObj.psnSeasonId;
                if (system == "xbox")
                {
                    if (int.Parse(newSeason) > int.Parse(xboxCurrentSeason) + 1 || int.Parse(newSeason) < int.Parse(xboxCurrentSeason) - 1 )
                    {
                        await Context.Channel.SendMessageAsync(
                            $"Season id entered was caught offside (***enter the real season id jackass***).");
                        return;
                    }
                    else
                    {
                        jsonObj.xboxPrevious = jsonObj.xboxSeasonId;
                        jsonObj.xboxSeasonId = newSeason;
                        string content = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                        File.WriteAllText(configLocation, content);
                        await ReplyAsync($"***{system}*** Season ID Change Request.\nPrevious Season: {xboxCurrentSeason}\nSeason Updated to: ***{newSeason}***");
                        PlayerEngine.GetField(system, "playerstats", int.Parse(jsonObj.xboxPrevious), "pre", "uh");
                        GoalieEngine.GetGoalie(system, "goaliestats", int.Parse(jsonObj.xboxPrevious), "pre", "uh");
                        TeamEngine.GetTeam(system, "teamstats", int.Parse(jsonObj.xboxPrevious), "pre", "uh");


                    }
                }
                else if (system == "psn")
                {
                    if (int.Parse(newSeason) > int.Parse(psnCurrentSeason) + 1 || int.Parse(newSeason) < int.Parse(psnCurrentSeason) - 1)
                    {
                        await Context.Channel.SendMessageAsync(
                            $"Season id entered was caught offside (***enter the real season id jackass***).");
                        return;
                    }
                    else
                    {

                        jsonObj.psnSeasonId = newSeason;
                        string content = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                        File.WriteAllText(configLocation, content);
                        await ReplyAsync($"***{system}*** Season ID Change Request.\nPrevious Season: {psnCurrentSeason}\nSeason Updated to: ***{newSeason}***");
                    }
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync(
                    $"Sorry {user.Mention}, but you don't have permission to use that command.");
            }
        }
        #endregion     
    }
}
