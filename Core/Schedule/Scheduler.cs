using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Quartz;
using Serilog;

namespace LGFA.Core.Schedular
{
    public class Scheduler : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
         Log.Warning($"--> Updating Database, MEM");

            var stopWatch = Stopwatch.StartNew();
            TeamStatsHandler.GetStats("psn", "teamstats");
            Log.Information($"PSN Standings updated");
            TeamStatsHandler.GetStats("xbox", "teamstats");
            Log.Information("Xbox Standings updated");
            var teamMem = GC.GetTotalMemory(false) / 1024 / 1024;
            Log.Information($"TeamStats MEM: {teamMem}, Time: {stopWatch.ElapsedMilliseconds}");
            stopWatch.Stop();

            var pWatch = Stopwatch.StartNew();
            PlayerStatsHandler.GetPlayerStats("psn", "playerstats"); 
            Log.Information($"PSN Field Players updated");
            PlayerStatsHandler.GetPlayerStats("xbox", "playerstats");
            Log.Information($"Xbox Field Players updated");

            var playerMem = GC.GetTotalMemory(false) / 1024 / 1024;
            Log.Information($"PlayerStats MEM: {playerMem}, Time {pWatch.ElapsedMilliseconds}");
            pWatch.Stop();

            var gWatch = Stopwatch.StartNew();
            GoalieStatsHandler.GetGoalieStats("psn", "goaliestats");
            Log.Information($"PSN Goalies updated");

            GoalieStatsHandler.GetGoalieStats("xbox", "goaliestats");
            Log.Information($"Xbox Goalies updated");

            var gMem = GC.GetTotalMemory(false) / 1024 / 1024;
            Log.Information($"GoalieStats MEM: {gMem}, Time {gWatch.ElapsedMilliseconds}");
            gWatch.Stop();
            var updateMem = GC.GetTotalMemory(false) / 1024 / 1024;
            Log.Warning($"--> Databases Updated, MEM: {updateMem}MB");


            Log.CloseAndFlush();
        }

    }
}