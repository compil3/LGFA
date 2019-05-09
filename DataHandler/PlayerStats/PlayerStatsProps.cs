using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LGFA.DataHandler.PlayerStats
{
    public class PlayerStatsProps
    {
        public int Id { get; set; }
        public string playerSystem { get; set; }

        public string position { get; set; }
        public string playerName { get; set; }
        public string gamesPlayed { get; set; }
        public string record { get; set; }
        public string avgMatchRating { get; set; }
        public string goals { get; set; }
        public string assists { get; set; }
        public string cleanSheets { get; set; }
        public string shotsOnGoal { get; set; }
        public string shotsOnTarget { get; set; }
        public string shotPercentage { get; set; }
        public string tackles { get; set; }
        public string tackleAttempts { get; set; }
        public string tacklePercentage { get; set; }
        public string passingPercentage { get; set; }
        public string keyPasses { get; set; }
        public string interceptions { get; set; }
        public string blocks { get; set; }
        public string yellowCard { get; set; }
        public string redCard { get; set; }
        public string manOfTheMatch { get; set; }
        public string playerURL { get; set; }
        public string teamIcon { get; set; }
        public string seasonid { get; set; }
        public string seasontypeid { get; set; }

    }
}

