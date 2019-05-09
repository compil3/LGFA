using LiteDB;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LGFA_Bot.DataHandler.GoalieStats
{
    class GoalieStatsSaveHandler
    {
        public static void SaveGoalieStats(int ID, string System, string PlayerName, string GamesPlayed, string Record, string GoalsAgainst, string ShotsAgainst,
            string Saves, string SavePercentage, string GoalsAgainstAvg, string CleanSheets, string ManOfTheMatch, string AvgMatchRating, string PlayerURL, string TeamIcon )
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Async(a => a.Console(theme: AnsiConsoleTheme.Code))
                .CreateLogger();

            using (var database = new LiteDatabase(@"LGFA.db"))
            {
                try
                {
                    var goalieCollection = database.GetCollection<GoalieStatsProps>("Goalies");
                    goalieCollection.EnsureIndex(x => x.ID);

                    var goalieStats = new GoalieStatsProps
                    {
                        ID = ID,
                        userSystem = System,
                        playerName = PlayerName,
                        gamesPlayed = GamesPlayed,
                        record = Record,
                        goalsAgainst = GoalsAgainst,
                        shotsAgainst = ShotsAgainst,
                        saves = Saves,
                        savePercentage = SavePercentage,
                        goalsAgainstAvg = GoalsAgainstAvg,
                        cleanSheets = CleanSheets,
                        manOfTheMatch = ManOfTheMatch,
                        avgMatchRating = AvgMatchRating,
                        playerURL = PlayerURL,
                        teamIcon = TeamIcon,
                    };
                    if (goalieCollection.FindById(ID) != null)
                    {
                        goalieCollection.Update(goalieStats);
                        Log.Debug($"{PlayerName} saved");
                    }
                    else goalieCollection.Insert(goalieStats);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error saving to goalie database");
                }
            }
        }
    }
}