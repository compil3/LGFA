using System;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using FluentScheduler;
using LGFA.Core.Aggregator;
using Newtonsoft.Json;

using LGFABot.Resources.DataTypes;
using LGFABot.Resources.Settings;
using LGFABot.Tools;
using LGFABot.Tools.Schedules;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;


namespace LGFABot
{
    class Program
    {


        private DiscordSocketClient Client;
        private CommandService Commands;

        public class Token
        {
            public string tokenID { get; set; }
        }

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {

            Console.WriteLine("Version: 1.1.2");
            

            string JSON = "";
            try
            {
                var sFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var configFile = @"Configuration\settings.json";
                var configLocation = Path.Combine(sFile, configFile);

                using (var stream = new FileStream(configLocation, FileMode.Open, FileAccess.Read))
                using (var readSettings = new StreamReader(stream))
                {
                    JSON = readSettings.ReadToEnd();
                }

            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                throw;
            }
            Settings settings = JsonConvert.DeserializeObject<Settings>(JSON);
            BotSettings.Log = settings.log;
            BotSettings.Owner = settings.owner;
            BotSettings.Token = settings.token;
            BotSettings.Version = settings.version;

            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });

            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug,

            });

            Client.MessageReceived += Client_MessageRecieved;
            Client.GuildMemberUpdated += Client_CheckRole;
            await Commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);

            Client.Ready += Client_Ready;
            Client.Log += Client_Log;

            Log.Warning("Connecting");
            await Client.LoginAsync(TokenType.Bot, BotSettings.Token);

            //StdSchedulerFactory factory = new StdSchedulerFactory();
            //IScheduler scheduler = await factory.GetScheduler();

            //var databaseJob = JobBuilder.Create<Schedule>()
            //    .WithIdentity("DatabaseUpdate", "group1")                
            //    .Build();

            //var historicUpdate = JobBuilder.Create<HistoricUpdate>()
            //    .WithIdentity("HistoryDB", "group1")
            //    .Build();

            //var currentSeasonTrigger = TriggerBuilder.Create()
            //    .WithIdentity("UpdateTrigger", "group1")
            //    .StartNow()
            //    //.WithCronSchedule("0 0 11 ? * WED")
            //    .WithSimpleSchedule(x => x.WithIntervalInHours(4).RepeatForever())
            //    .ForJob(databaseJob)
            //    .Build();

            //var historicalSeasonTrigger = TriggerBuilder.Create()
            //    .WithIdentity("HistoricTrigger", "group1")
            //    .WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever())
            //    .StartNow()
            //    .ForJob(historicUpdate)
            //    .Build();

            //await scheduler.Start();

            //await scheduler.ScheduleJob(databaseJob, currentSeasonTrigger);
            //await scheduler.ScheduleJob(historicUpdate, historicalSeasonTrigger);

            await Client.StartAsync();

            await Task.Delay(-1);

        }

        private async Task Client_CheckRole(SocketGuildUser roleBefore, SocketGuildUser user)
        {
            //get the accepted role id
            var acceptedRole = user.Guild.Roles.FirstOrDefault(s=>s.Name == "Accepted Rules");
            var xboxRole = user.Guild.Roles.FirstOrDefault(xb => xb.Name == "Xbox");
            var psnRole = user.Guild.Roles.FirstOrDefault(p => p.Name == "PSN");
            if (user.Roles.Contains(acceptedRole))
                if (user.Roles.Contains(xboxRole) || user.Roles.Contains(psnRole))
            {
                await roleBefore.RemoveRoleAsync(user.Guild.Roles.FirstOrDefault(x => x.Name == "Accepted Rules"));

            }
            //var acceptRole = user.Guild.Roles.FirstOrDefault(r => r.Name == "Xbox" || r.Name == "PSN");
            //if (channel == null)
            //{
            //    await Console.Out.WriteLineAsync($"{channel} not found");
            //}

            //if (roleBefore.Guild.Roles.Contains(r=>) == null)
            //{
            //    await Console.Out.WriteLineAsync("Role not found");
            //}
            //else
            //{
            //    await roleAfter.RemoveRoleAsync(roleAfter.Guild.Roles.FirstOrDefault(x => x.Name == "Accepted Rules"));
            //}
        }
        private async Task Client_Ready()
        {
            await Client.SetGameAsync("Creating Fake News");
            var chnl = Client.GetGuild(477493363478626319).GetTextChannel(575876659832422401) as ISocketMessageChannel;
            JobManager.Initialize(new News(chnl));
            JobManager.Initialize(new WeeklySchedule());
            JobManager.Initialize(new XboxHistory());
            JobManager.Initialize(new PsnHistory());
        }

        private async Task Client_Log(LogMessage Message)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Async(a => a.Console(theme: AnsiConsoleTheme.Code))
               .WriteTo.Async(a => a.Seq("http://localhost:5341"))
               .CreateLogger();
            try
            {
                Log.Logger.Warning($"{Message.Source}] {Message.Message}");
                Log.CloseAndFlush();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, $"{Message.Source} {Message.Message}");
                Log.CloseAndFlush();
            }
        }


        private async Task Client_MessageRecieved(SocketMessage MessageParam)
        {
            var message = MessageParam as SocketUserMessage;
            var context = new SocketCommandContext(Client, message);

            if (context.Message == null || context.Message.Content == "") return;
            if (context.User.IsBot) return;

            int ArgPos = 0;
            if (!(message.HasStringPrefix(".", ref ArgPos) || message.HasMentionPrefix(Client.CurrentUser, ref ArgPos)) || message.Author.IsBot) return;

            var result = await Commands.ExecuteAsync(context, ArgPos, null);
            if (!result.IsSuccess)
            {
                Log.Logger.Error($"{context.Message.Content} Error: {result.ErrorReason}");
                await context.Channel.SendMessageAsync($"{context.Message.Content} Error: {result.ErrorReason}");
            }
            if (result.IsSuccess) Log.Logger.Warning($"{context.Message.Content} command accepted");
        }


    }
}

