using LiteDB;
using System;

namespace LGFABot.DataHandler.TeamStandings
{
    class TeamDataSaveHandler
    {
        public static void SaveTeamInfo(int ID, int TeamRank, string TeamName, string GamesPlayed, string GamesWon, string GamesDrawn, string GamesLost, string Points,
            string Streak, string GoalsFor, string GoalsAgainst, string CleanSheets, string LastTenGames, string HomeRecord, string AwayRecord, string OneGoalGames, string TeamIconURL, string TeamURL, string System)
        {
            using (var database = new LiteDatabase(@"LGFA.db"))
            {

                try
                {
                    if (System == "xbox")
                    {
                        var xboxCollection = database.GetCollection<TeamStatsProps>(System);
                        xboxCollection.EnsureIndex(x => x.Id);
                        var xboxTeamStats = new TeamStatsProps
                        {
                            Id = ID,
                            league = System,
                            rank = TeamRank,
                            teamName = TeamName,
                            gamesPlayed = GamesPlayed,
                            gamesWon = GamesWon,
                            gamesDrawn = GamesDrawn,
                            gamesLost = GamesLost,
                            points = Points,
                            streak = Streak,
                            goalsFor = GoalsFor,
                            goalsAgainst = GoalsAgainst,
                            cleanSheets = CleanSheets,
                            lastTenGames = LastTenGames,
                            homeRecord = HomeRecord,
                            awayRecord = AwayRecord,
                            oneGoalGames = OneGoalGames,
                            teamIconUrl = TeamIconURL,
                            teamURL = TeamURL
                        };
                        if (xboxCollection.FindById(ID) != null)
                        {
                            xboxCollection.Update(xboxTeamStats);
                        }
                        else xboxCollection.Insert(xboxTeamStats);
                    } else if (System == "psn")
                    {
                        var psnCollection = database.GetCollection<TeamStatsProps>(System);
                        psnCollection.EnsureIndex(x => x.Id);
                        var psnTeamStats = new TeamStatsProps
                        {
                            Id = ID,
                            league = System,
                            rank = TeamRank,
                            teamName = TeamName,
                            gamesPlayed = GamesPlayed,
                            gamesWon = GamesWon,
                            gamesDrawn = GamesDrawn,
                            gamesLost = GamesLost,
                            points = Points,
                            streak = Streak,
                            goalsFor = GoalsFor,
                            goalsAgainst = GoalsAgainst,
                            cleanSheets = CleanSheets,
                            lastTenGames = LastTenGames,
                            homeRecord = HomeRecord,
                            awayRecord = AwayRecord,
                            oneGoalGames = OneGoalGames,
                            teamIconUrl = TeamIconURL,
                            teamURL = TeamURL
                        };
                        if (psnCollection.FindById(ID) != null)
                        {
                            psnCollection.Update(psnTeamStats);
                        }
                        else psnCollection.Insert(psnTeamStats);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            };
        }
    }
}