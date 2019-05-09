using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LGFA.Core.Commands
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;

        private Commands(CommandService service)
        {
            _service = service;
        }
        [Command("help")]
        [Summary("Displays this help message.")]
        [Remarks("Displays this message")]
        [RequireUserPermission(ChannelPermission.SendMessages)]
        public async Task ShowHelp()
        {
            var builder = new EmbedBuilder();

            var user = Context.User as SocketGuildUser;
            if (!user.GuildPermissions.KickMembers)
            {
                    foreach (var command in _service.Commands)
                    {
                        if (!user.GuildPermissions.Administrator && command.Name == "restart") continue;
                        if (!user.GuildPermissions.KickMembers && command.Name=="update") continue;
                        string embedFieldText = command.Summary ?? "No description available\n";
                        builder.AddField(command.Name, embedFieldText, inline: true);
                    }                 
                    await Context.User.SendMessageAsync("List of commands and their usage: ", false, builder.Build());
                    return;
            }else
            {
                foreach (var command in _service.Commands)
                {
                    string embedFieldText = command.Summary ?? "No description available\n";
                    builder.AddField(command.Name, embedFieldText, inline: true);
                }
            }
            await Context.User.SendMessageAsync("List of commands and their usage: ", false, builder.Build());
        }

        [RequireOwner]
        [Command("restart")]
        [Summary(".restart")]
        [Remarks("Restart the bot")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task QuitApp()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            var user = Context.User as SocketGuildUser;
            if (user.Id != 111252573054312448)
            {
                await Context.Channel.SendMessageAsync($"Sorry {user.Mention}, but you don't have permission to run that command.");
            }
            else
            {
                var programName = Assembly.GetExecutingAssembly().Location;
                System.Diagnostics.Process.Start(programName);
                Environment.Exit(0);
            }
        }
    }
}
