using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LGFA_Bot.DataHandler.GoalieStats
{
    public class GoalieStatsProps
    {
        public int ID { get; set; }
        public string userSystem { get; set; }
        public string playerName { get; set; }
        public string lgRank { get; set; }
        public string gamesPlayed { get; set; }
        public string record { get; set; }
        public string goalsAgainst { get; set; }
        public string shotsAgainst { get; set; }
        public string saves { get; set; }
        public string savePercentage { get; set; }
        public string goalsAgainstAvg { get; set; }
        public string cleanSheets { get; set; }
        public string manOfTheMatch { get; set; }
        public string avgMatchRating { get; set; }
        public string playerURL { get; set; }
        public string teamIcon { get; set; }
        public string seasonid { get; set; }
        public string seasontypeid { get; set; }

    }
}
