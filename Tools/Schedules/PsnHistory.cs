using System;
using FluentScheduler;
using LGFABot.Tools.Engines;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace LGFABot.Tools.Schedules
{
    public class PsnHistory : Registry
    {
        public PsnHistory()
        {
            Action updates = new Action(() =>
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                    .WriteTo.Seq("http://localhost:5341")
                    .CreateLogger();
                var previousSeason = "";
                var currentSeason = "";
                var savePrevious = "";
                try
                {
                    previousSeason = Utilities.GetPrevious("psn");
                    currentSeason = Utilities.GetSeason("psn");
                    savePrevious = "";
                    Console.WriteLine("Updating PSN Historical Statistics");

                    for (int i = int.Parse(previousSeason); i < int.Parse(currentSeason); i++)
                    {
                        //Console.WriteLine($"We are inside the fucking loop (iteration {i}).");
                        //PlayerEngine.GetField("psn", "playerstats", i, "pre", "uh");
                        //Log.Logger.Warning("PSN Players pre updated.");
                        //Log.CloseAndFlush();

                        //GoalieEngine.GetGoalie("psn", "goaliestats", i, "pre", "uh");
                        //Log.Logger.Warning("PSN Goalies pre updated.");
                        //Log.CloseAndFlush();
                        ////eamEngine.GetTeam("psn", "teamstats", i, "pre", "uh");

                        PlayerEngine.GetField("psn", "playerstats", i, "reg", "uh");
                        Log.Logger.Warning("PSN Players pre updated.");
                        Log.CloseAndFlush();

                        GoalieEngine.GetGoalie("psn", "goaliestats", i, "reg", "uh");
                        Log.Logger.Warning("PSN Goalies pre updated.");
                        Log.CloseAndFlush();

                        //TeamEngine.GetTeam("psn", "teamstats", i, "reg", "uh");
                        Console.WriteLine($"PSN Stats saved for: {i}  [Historical]\n");
                        Log.CloseAndFlush();

                        savePrevious = i.ToString();

                        //Console.WriteLine($"--->{previousSeason}\n---->{i}");
                    }

                    Console.WriteLine(!Utilities.SavePrevious("psn", savePrevious)
                        ? "Failed to update PSN Previous Season"
                        : "Successfully updated PSN previous season.");
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Fatal error running historical stats update.\n{ex}");
                    Console.WriteLine($"Fatal error running historical stats update.\n{ex}");
                }
            });
            this.Schedule(updates).ToRunNow().AndEvery(1).Months().OnTheSecond(DayOfWeek.Friday);
        }
    }
}
