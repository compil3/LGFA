using System;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using FluentScheduler;
using LGFABot.Tools.Engines;

namespace LGFABot.Tools.Schedules
{
    public class XboxHistory : Registry
    {
        public XboxHistory()
        {
            Action updates = new Action(() =>
            {

                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                    .WriteTo.Seq("http://localhost:5341")
                    .CreateLogger();
                try
                {

                    var previousSeason = "";
                    var currentSeason = "";
                    var savePrevious = "";
                    Console.WriteLine("Updating Historical Statistics");
                    previousSeason = Utilities.GetPrevious("xbox");

                    currentSeason = Utilities.GetSeason("xbox");
                    savePrevious = "";
                    Console.WriteLine("Updating Xbox Historical Statistics");

                    for (int i = int.Parse(previousSeason); i < int.Parse(currentSeason); i++)
                    {
                        //PlayerEngine.GetField("xbox", "playerstats", i, "pre", "uh");
                        //GoalieEngine.GetGoalie("xbox", "goaliestats", i, "pre", "uh");
                        //Log.Logger.Warning("Xbox Goalies pre updated\n");
                        //Log.CloseAndFlush();

                        //TeamEngine.GetTeam("xbox", "teamstats", i, "pre", "uh");

                        PlayerEngine.GetField("xbox", "playerstats", i, "reg", "uh");
                        Log.Logger.Warning("Xbox Players updated\n");
                        Log.CloseAndFlush();

                        GoalieEngine.GetGoalie("xbox", "goaliestats", i, "reg", "uh");
                        Log.Logger.Warning("Xbox Goalies updated.\n");
                        Log.CloseAndFlush();

                        TeamEngine.GetTeam("xbox", "teamstats", i, "reg", "uh");
                        Console.WriteLine($"Xbox Stats saved for: {i}  [Historical]");
                        Log.CloseAndFlush();

                        savePrevious = i.ToString();
                        //Console.WriteLine($"--->{previousSeason}\n---->{i}");
                    }

                    Console.WriteLine(!Utilities.SavePrevious("xbox", savePrevious)
                        ? "Failed to update Xbox Previous Season"
                        : "Successfully updated Xbox previous season.");

                    Console.WriteLine("Historical Stats Updated.");
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Fatal error running historical stats update.\n{ex}");
                    Console.WriteLine($"Fatal error running historical stats update.\n{ex}");
                }
            });
            this.Schedule(updates).ToRunNow().AndEvery(1).Days().At(15, 0);
        }
    }
}
