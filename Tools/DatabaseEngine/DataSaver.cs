using LiteDB;
using Serilog;
using System;
using System.Diagnostics;
using LGFABot.Tools.Properties;

namespace LGFABot.Tools
{
    public class DataSaver
    {
        #region Method to save player stats

        public static bool Save(int ID, int SeasonId, string SeasonTypeId, string Position, string PlayerName,
            string GamesPlayed, string Record, string AvgMatchRating, string Goals, string Assists, string CleanSheets,
            string ShotsOnGoal,
            string ShotsOnTarget, string ShotPercentage, string Tackles, string TackleAttempts, string TacklePercentage,
            string PassPercentage, string KeyPasses, string Interceptions, string Blocks,
            string YellowCards, string RedCards, string ManOfTheMatch, string PlayerURL, string System,
            string TeamIconURL, string Command)
        {
            var dbName = "";
            var tableName = "";

            if (Command == "schedule")
            {
                dbName = "LGFA_Current.db";
                if (System == "xbox" || System == "psn")
                {
                    var currentSeasonId = int.Parse(Fetch.GetSeason(System));
                    if (currentSeasonId == SeasonId)
                    {
                        if (SeasonTypeId == "regular") tableName = "CRS_Player";
                        else if (SeasonTypeId == "pre-season") tableName = "CPS_Player";
                    }
                }
            }
            else if (Command == "uh")
            {
                if (System == "xbox" || System == "psn")
                {
                    var historicalSeason = SeasonId;
                    if (SeasonTypeId == "regular")
                    {
                        tableName = "HRS_Player" + SeasonId;
                        dbName = "Historical_Reg_Player.db";
                    }
                    else if (SeasonTypeId == "pre-season")
                    {
                        tableName = "HPS_Player" + SeasonId;
                        dbName = "Historical_Pre_Player.db";
                    }
                }
            }


            using (var database = new LiteDatabase(dbName))
            {
                var playerCollection = database.GetCollection<StatProps>(tableName);
                playerCollection.EnsureIndex(x => x.SeasonId);

                var playerStats = new StatProps
                {
                    SeasonId = SeasonId,
                    Id = ID,
                    TeamIcon = TeamIconURL,
                    Position = Position,
                    PlayerName = PlayerName,
                    GamesPlayed = GamesPlayed,
                    Record = Record,
                    AvgMatchRating = AvgMatchRating,
                    Goals = Goals,
                    Assists = Assists,
                    CleanSheets = CleanSheets,
                    ShotsOnGoal = ShotsOnGoal,
                    ShotsOnTarget = ShotsOnTarget,
                    ShotPercentage = ShotPercentage,
                    Tackles = Tackles,
                    TackleAttempts = TackleAttempts,
                    TacklePercentage = TacklePercentage,
                    PassingPercentage = PassPercentage,
                    KeyPasses = KeyPasses,
                    Interceptions = Interceptions,
                    Blocks = Blocks,
                    YellowCards = YellowCards,
                    RedCards = RedCards,
                    ManOfTheMatch = ManOfTheMatch,
                    PlayerUrl = PlayerURL,
                    PlayerSystem = System,
                    SeasonTypeId = SeasonTypeId,
                };
                try
                {
                    var playerFound = playerCollection.FindById(ID);
                    if (playerFound != null)
                    {
                        playerCollection.Update(playerStats);
                        return true;
                    }
                    else
                    {
                        playerCollection.Insert(playerStats);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error saving Historical Stats to database");
                }
            }

            return false;
        }

        #endregion

        #region Method to save goalie stats.

        public static bool SaveGoalie(int ID, string System, string playerName, string gamesPlayed, string record,
            string goalsAgainst, string shotsAgainst, string saves, string savePercentage, string goalsAgainstAvg,
            string cleanSheets, string manOfTheMatch, string avgMatchRating, string playerURL, string iconURL, string Command, int SeasonId, string SeasonTypeId)
        {
            var dbName = "";
            var tableName = "";

            if (Command == "schedule")
            {
                dbName = "LGFA_Current.db";
                if (System == "xbox" || System == "psn")
                {
                    var currentSeasonId = int.Parse(Fetch.GetSeason(System));
                    if (currentSeasonId == SeasonId)
                    {
                        if (SeasonTypeId == "regular") tableName = "CRS_Goalie";
                        else if (SeasonTypeId == "pre-season") tableName = "CPS_Goalie";
                    }
                }
            }
            else if (Command == "uh")
            {

                if (System == "xbox" || System == "psn")
                {
                    var historicalSeason = SeasonId;
                    if (SeasonTypeId == "regular")
                    {
                        tableName = "HRS_Goalie" + SeasonId;
                        dbName = "Historical_Reg_Goalie.db";
                    }
                    else if (SeasonTypeId == "pre-season")
                    {
                        tableName = "HPS_Goalie" + SeasonId;
                        dbName = "Historical_Pre_Goalie.db";
                    }
                }
            }

            using (var database = new LiteDatabase(dbName))
            {

                var goalieCollection = database.GetCollection<GoalieProps>(tableName);

                goalieCollection.EnsureIndex(x => x.SeasonId);
                var goalieStats = new GoalieProps
                {
                    SeasonId = SeasonId,
                    SeasonTypeId = SeasonTypeId,
                    Id = ID,
                    userSystem = System,
                    playerName = playerName,
                    gamesPlayed = gamesPlayed,
                    record = record,
                    goalsAgainst = goalsAgainst,
                    goalsAgainstAvg = goalsAgainstAvg,
                    shotsAgainst = shotsAgainst,
                    saves = saves,
                    savePercentage = savePercentage,
                    cleanSheets = cleanSheets,
                    manOfTheMatch = manOfTheMatch,
                    avgMatchRating = avgMatchRating,
                    playerURL = playerURL,
                    teamIcon = iconURL
                };
                try
                {
                    if (goalieCollection.FindById(ID) != null)
                    {
                        goalieCollection.Update(goalieStats);
                        return true;
                    }
                    else
                    {
                        goalieCollection.Insert(goalieStats);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error saving goalie stats to database");
                }
            }
            return false;
        }

        #endregion

        #region Save Team to database
        public static bool SaveTeam(int Id, int rank, string teamName, string gamesPlayed, string gamesWon,
            string gamesDrawn, string gamesLost, string Points, string Streak, string goalsFor, string goalsAgainst,
            string cleanSheets,
            string lastTenGames, string homeRecord, string awayRecord, string oneGoalGames, string teamIcon,
            string teamUrl, string system, int seasonId, string seasonType, string Command)
        {
            var dbName = "";
            var tableName = "";

            if (Command == "schedule")
            {
                dbName = "LGFA_Current.db";
                if (system == "xbox" || system == "psn")
                {
                    var currentSeasonId = int.Parse(Fetch.GetSeason(system));
                    if (currentSeasonId == seasonId)
                    {
                        if (seasonType == "regular") tableName = "CRS_Team";
                        else if (seasonType == "pre-season") tableName = "CPS_Team";
                    }
                }
            }
            else if (Command == "uh")
            {
                if (system == "xbox" || system == "psn")
                {
                    var historicalSeason = seasonId;
                    if (seasonType == "regular")
                    {
                        tableName = "Regular" + seasonId;
                        dbName = "Historical_Reg_Team.db";
                    }
                    else if (seasonType == "pre-season")
                    {
                        {
                            tableName = "Pre-season" + seasonId;
                            dbName = "Historical_Pre_Team.db";
                        }
                    }
                }
            }

            using (var database = new LiteDatabase(dbName))
            {
                var teamCollection = database.GetCollection<TeamProp>(tableName);
                var teamStats = new TeamProp()
                {
                    Id = Id,
                    Rank = rank,
                    SeasonId = seasonId,
                    SeasonTypeId = seasonType,
                    System = system,
                    TeamName = teamName,
                    GamesPlayed = gamesPlayed,
                    GamesWon = gamesWon,
                    GamesDrawn = gamesDrawn,
                    GamesLost = gamesLost,
                    Points = Points,
                    Streak = Streak,
                    GoalsFor = goalsFor,
                    GoalsAgainst = goalsAgainst,
                    CleanSheets = cleanSheets,
                    LastTenGames = lastTenGames,
                    HomeRecord = homeRecord,
                    AwayRecord = awayRecord,
                    OneGoalGames = oneGoalGames,
                    TeamIconUrl = teamIcon,
                    TeamURL = teamUrl
                };
                try
                {
                    if (teamCollection.FindById(Id) != null)
                    {
                        teamCollection.Update(teamStats);
                        return true;
                    }
                    else
                    {
                        teamCollection.Insert(teamStats);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex,"Error saving team stats.");
                }
            }
            return false;
        }
        #endregion

        #region Save player ID and url to a database
        public static bool SavePlayerUrl(int id, string playerName, string playerUrl)
        {
            var st = new StackTrace();
            var sf = st.GetFrame(0);
            var currentMethod = sf.GetMethod();
            using (var database = new LiteDatabase(@"LGFA.db"))
            {
                var playerCollection = database.GetCollection<PlayerInfo>("Players");
                playerCollection.EnsureIndex(x => x.Id);
                var playerInfo = new PlayerInfo
                {
                    Id = id,
                    playerName = playerName,
                    playerUrl = playerUrl
                };
                try
                {
                    var playerFound = playerCollection.FindById(id);
                    if (playerFound != null)
                    {
                        playerCollection.Update(playerInfo);
                        return true;
                    }
                    else
                    {
                        playerCollection.Insert(playerInfo);
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error processing Player Information {currentMethod} {e}");
                    return false;
                }
            }
        }
        #endregion

        #region Save Career Stats

        public static bool SaveCareer(int id, string playerName, string careerRecord, double gamesPlayed, double amr,
            double goals, string assists, string SOT, double shotAttempts, double shotPercentage, double passesC, double passesA,double passPercentage,
            string keypass, string interceptions, double tackles, double tackleAttempts, double tacklePercentage, string blocks, string red, string yellow)
        {
            var st = new StackTrace();
            var sf = st.GetFrame(0);
            var currentMethod = sf.GetMethod();

            using (var database = new LiteDatabase(@"LGFA.db"))
            {
                var playerCareer = database.GetCollection<Career>("Career");
                playerCareer.EnsureIndex(x => x.Id);
                var playerStats = new Career
                {
                    Id = id,
                    PlayerName = playerName,
                    Record = careerRecord,
                    GamesPlayed = gamesPlayed,
                    AvgMatchRating = amr,
                    Goals = goals,
                    Assists = assists,
                    ShotAttempts = shotAttempts,
                    ShotsOnTarget = SOT,
                    ShotPercentage = shotPercentage,
                    PassesCompleted = passesC,
                    PassesAttempted = passesA,
                    PassingPercentage = passPercentage,
                    KeyPasses = keypass,
                    Interceptions = interceptions,
                    Tackles = tackles,
                    TackleAttempts = tackleAttempts,
                    TacklePercentage = tacklePercentage,
                    Blocks = blocks,
                    RedCards = red,
                    YellowCards = yellow
                };
                try
                {
                    var playerFound = playerCareer.FindById(id);
                    if (playerFound != null)
                    {
                        playerCareer.Update(playerStats);
                        return true;
                    }
                    else
                    {
                        playerCareer.Insert(playerStats);
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error saving Player Career Information {currentMethod} {e}");
                    return false;
                }
            }
        }
        #endregion
    }
}


