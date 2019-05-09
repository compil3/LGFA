using System.Collections.Generic;

namespace LGFABot.Resources.DataTypes
{
    public class Settings
    {
        public string token { get; set; }
        public ulong owner { get; set; }
        public List<ulong> log { get; set; }
        public string version { get; set; }
    }
    public class UrlSettings
    {
        public string xboxSeasonId { get; set; }
        public string xboxStandingsURL { get; set; }
        public string xboxPlayerStatsURL { get; set; }
        public string xboxDraftListURL { get; set; }
        public string xboxPrevious { get; set; }
        public string psnSeasonId { get; set; }
        public string psnStandingsURL { get; set; }
        public string psnPlayerStatsURL { get; set; }
        public string psnDraftListURL { get; set; }
        public string psnPrevious { get; set; }
        public string currentSeason { get; set; }
        public string previousSeason { get; set; }
        public string newsUrl { get; set; }
}

    public class Season
    {
        public string currentSeason { get; set; }
        public string previousSeason { get; set; }
    }
}
