using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LGFABot.DataHandler.TeamStandings
{
    public class TeamStatsProps
    {
        [Key]
        public int Id { get; set; }
        public string league { get; set; }

        public int rank { get; set; }
        public string teamName { get; set; }
        public string gamesPlayed { get; set; }
        public string gamesWon { get; set; }
        public string gamesDrawn { get; set; }
        public string gamesLost { get; set; }
        public string points { get; set; }
        public string streak { get; set; }
        public string goalsFor { get; set; }
        public string goalsAgainst { get; set; }
        public string cleanSheets { get; set; }
        public string lastTenGames { get; set; }
        public string homeRecord { get; set; }
        public string awayRecord { get; set; }
        public string oneGoalGames { get; set; }
        public string teamIconUrl { get; set; }
        public string teamURL { get; set; }
        public string seasonID { get; set; }
        public string seasonTypeID { get; set; }
    }
}
