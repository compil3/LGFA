using System;
using System.Diagnostics;
using FluentScheduler;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace LGFABot.Tools
{
    public class WeeklySchedule : Registry
    {
        public WeeklySchedule()
        {
            Action update = new Action(() =>
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                    .WriteTo.Seq("http://localhost:5341")
                    //.WriteTo.Async(a => a.File(@"logs\lgfa.txt", rollingInterval: RollingInterval.Day))
                    .CreateLogger();

                #region Update recent player stats
                Log.Logger.Warning("Updating Database");

                var playerWatach = Stopwatch.StartNew();

                #region Current Season
                PlayerEngine.GetField("xbox", "playerstats", int.Parse(Fetch.GetSeason("xbox")), "reg", "schedule");
                PlayerEngine.GetField("psn", "playerstats", int.Parse(Fetch.GetSeason("psn")), "reg", "schedule");

                GoalieEngine.GetGoalie("xbox", "goaliestats", int.Parse(Fetch.GetSeason("xbox")), "reg", "schedule");
                GoalieEngine.GetGoalie("psn", "goaliestats", int.Parse(Fetch.GetSeason("psn")), "reg", "schedule");
                Console.WriteLine("Player & Goalie Stats updated to most recent.");
                #endregion

                #region Pre-season
                //PlayerEngine.GetField("xbox", "playerstats", int.Parse(Fetch.GetSeason("xbox")), "pre", "schedule");
                //PlayerEngine.GetField("psn", "playerstats", int.Parse(Fetch.GetSeason("psn")), "pre", "schedule");
                Log.Logger.Warning("Players Pre updated");

                //GoalieEngine.GetGoalie("xbox", "goaliestats", int.Parse(Fetch.GetSeason("xbox")), "pre", "schedule");
                //GoalieEngine.GetGoalie("psn", "goaliestats", int.Parse(Fetch.GetSeason("psn")), "pre", "schedule");
                Console.WriteLine("Player & Goalie Pre-Season Stats updated to most recent.");
                #endregion

                #region Team

                TeamEngine.GetTeam("xbox", "teamstats", int.Parse(Fetch.GetSeason("xbox")), "reg", "schedule");
                TeamEngine.GetTeam("psn", "teamstats", int.Parse(Fetch.GetSeason("psn")), "reg", "schedule");
                Console.WriteLine("Current team stats updated");
                //TeamEngine.GetTeam("xbox", "teamstats", int.Parse(Fetch.GetSeason("xbox")), "pre", "schedule");
                //TeamEngine.GetTeam("psn", "teamstats", int.Parse(Fetch.GetSeason("psn")), "pre", "schedule");

                #endregion
                Log.Logger.Warning($"Database updated to most recent.\nUpdated time: {playerWatach.Elapsed.TotalSeconds}");
                playerWatach.Stop();

                Log.CloseAndFlush();
                #endregion
            });
            this.Schedule(update).ToRunNow().AndEvery(4).Hours();

        }

    }
}
