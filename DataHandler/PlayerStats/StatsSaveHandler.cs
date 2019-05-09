using LiteDB;
using Serilog;
using System;

namespace LGFA.DataHandler.PlayerStats
{
    class StatsSaveHandler
    {
        public static void SavePlayerStats(int ID, string TeamIcon, string Position, string PlayerName, string GamesPlayed, string Record, string AvgMatchRating, string Goals, string Assists, string CleanSheets, string ShotsOnGoal,
                    string ShotsOnTarget, string ShotPercentage, string Tackles, string TackleAttempts, string TacklePercentage, string PassPercentage, string KeyPasses, string Interceptions, string Blocks,
                    string YellowCards, string RedCards, string ManOfTheMatch, string PlayerURL, string System)
        {
            using (var database = new LiteDatabase(@"LGFA.db"))
            {
                var playerCollection = database.GetCollection<PlayerStatsProps>("Players");
                playerCollection.EnsureIndex(x => x.Id);

                var playerStats = new PlayerStatsProps
                {
                    Id = ID,
                    teamIcon = TeamIcon,
                    position = Position,
                    playerName = PlayerName,
                    gamesPlayed = GamesPlayed,
                    record = Record,
                    avgMatchRating = AvgMatchRating,
                    goals = Goals,
                    assists = Assists,
                    cleanSheets = CleanSheets,
                    shotsOnGoal = ShotsOnGoal,
                    shotsOnTarget = ShotsOnTarget,
                    shotPercentage = ShotPercentage,
                    tackles = Tackles,
                    tackleAttempts = TackleAttempts,
                    tacklePercentage = TacklePercentage,
                    passingPercentage = PassPercentage,
                    keyPasses = KeyPasses,
                    interceptions = Interceptions,
                    blocks = Blocks,
                    yellowCard = YellowCards,
                    redCard = RedCards,
                    manOfTheMatch = ManOfTheMatch,
                    playerURL = PlayerURL,
                    playerSystem = System
                };
                try
                {
                    if (playerCollection.FindById(ID) != null)
                    {
                        playerCollection.Update(playerStats);
                    }
                    else
                    {
                        playerCollection.Insert(playerStats);

                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error saving to database");
                }
            }
        }
    }
}