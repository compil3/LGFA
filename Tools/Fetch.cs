using System;
using System.IO;
using System.Reflection;
using LGFABot.Resources.DataTypes;
using LGFABot.Resources.Settings;
using Newtonsoft.Json;
using Serilog;

namespace LGFABot.Tools
{
    public class Fetch
    {
        public static string GetURL(string System, string Trigger, int SeasonId, string SeasonType)
        {
            string JSON = "";
            var xboxURL = "";
            var psnURL = "";

            var sFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var configFile = @"Configuration\websettings.json";
            var configLocation = Path.Combine(sFile, configFile);
            if (!File.Exists(configLocation)) Log.Error($"{configLocation} does not exist");
            try
            {
                using (var stream = new FileStream(configLocation, FileMode.Open, FileAccess.Read))
                using (var readSettings = new StreamReader(stream))
                {
                    JSON = readSettings.ReadToEnd();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Logger.Error(ex, $"Error reading websettings.json ({configLocation}");
                throw;
            }
            //convert Season Type from int to string
            UrlSettings settings = JsonConvert.DeserializeObject<UrlSettings>(JSON);
            if (SeasonType == "pre") SeasonType = "0";
            else if (SeasonType == "reg") SeasonType = "1";
            var url = "";
            var sTypeTemp = "&seasontypeid=";
            var seasonTypeId = string.Concat(String.Empty, sTypeTemp, SeasonType);
            switch (System)
            {
                case "xbox":
                    switch (Trigger)
                    {
                        case "playerstats":
                        case "goaliestats":
                            WebSettings.XboxPlayerStatsURL = settings.xboxPlayerStatsURL;
                            //var urlTemp = WebSettings.XboxPlayerStatsURL;
                            url = WebSettings.XboxPlayerStatsURL + SeasonId + seasonTypeId;
                            break;
                        case "draftlist":
                            url = WebSettings.XboxDraftListURL + SeasonId;
                            break;
                        case "teamstats":
                            WebSettings.XboxTeamStandingsURL = settings.xboxStandingsURL;
                            url = WebSettings.XboxTeamStandingsURL + SeasonId + seasonTypeId;
                            break;
                    }
                    return url;

                case "psn":
                    switch (Trigger)
                    {
                        case "playerstats":
                        case "goaliestats":
                            WebSettings.PSNPlayerStatsURL = settings.psnPlayerStatsURL;
                            url = WebSettings.PSNPlayerStatsURL + SeasonId + seasonTypeId;
                            break;
                        case "draftlist":
                            url = WebSettings.PSNDraftListURL + SeasonId;
                            break;
                        case "teamstats":
                            WebSettings.PSNTeamStandingsURL = settings.psnStandingsURL;
                            url = WebSettings.PSNTeamStandingsURL + SeasonId + seasonTypeId;
                            break;
                    }
                    return url;
            }
            return null;
        }

        internal static string GetNewsUrl()
        {
            var JSON = "";
            var sFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var configFile = @"Configuration\websettings.json";
            var configLocation = Path.Combine(sFile, configFile);
            if (!File.Exists(configLocation)) Log.Error($"{configLocation} does not exist");
            try
            {
                using (var stream = new FileStream(configLocation, FileMode.Open, FileAccess.Read))
                using (var readSettings = new StreamReader(stream))
                {
                    JSON = readSettings.ReadToEnd();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Logger.Error(ex, $"Error reading websettings.json ({configLocation}");
                throw;
            }

            UrlSettings settings = JsonConvert.DeserializeObject<UrlSettings>(JSON);
            var url = "";
            WebSettings.NewsUrl = settings.newsUrl;
            url = WebSettings.NewsUrl;
            return url;

        }

        public static string GetSeason(string System)
        {
            var CurrentSeason = "";
            if (System == "xbox")
            {
                string xboxJson = "";

                var sFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var configFile = @"Configuration\xbox.json";
                var configLocation = Path.Combine(sFile, configFile);
                if (!File.Exists(configLocation)) Log.Error($"GetSeasonID Method: {configLocation} does not exist");
                try
                {
                    using (var stream = new FileStream(configLocation, FileMode.Open, FileAccess.Read))
                    using (var readSettings = new StreamReader(stream))
                    {
                        xboxJson = readSettings.ReadToEnd();
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    Log.Logger.Error(ex, $"Error reading websettings.json ({configLocation}");
                    return null;
                }

                Season seasonId = JsonConvert.DeserializeObject<Season>(xboxJson);
                WebSettings.currentSeason = seasonId.currentSeason;
                CurrentSeason = seasonId.currentSeason;
            }
            else if (System == "psn")
            {
                string psnJson = "";
                var sFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var configFile = @"Configuration\psn.json";
                var configLocation = Path.Combine(sFile, configFile);
                if (!File.Exists(configLocation)) Log.Error($"GetSeasonID Method: {configLocation} does not exist");
                try
                {
                    using (var stream = new FileStream(configLocation, FileMode.Open, FileAccess.Read))
                    using (var readSettings = new StreamReader(stream))
                    {
                        psnJson = readSettings.ReadToEnd();
                    }

                }
                catch (UnauthorizedAccessException ex)
                {
                    Log.Logger.Error(ex, $"Error reading websettings.json ({configLocation}");
                    throw;
                }

                Season seasonID = JsonConvert.DeserializeObject<Season>(psnJson);

                WebSettings.PSNSeasonID = seasonID.currentSeason;
                CurrentSeason = WebSettings.PSNSeasonID;
                return CurrentSeason;
            }
            else
            {
                Log.Logger.Error("Unable to fetch current season from websettings.json.  Please check that the file exists or has a seasonid entry");
            }

            return CurrentSeason;
        }

        public static string GetTradeIcon()
        {
            var sFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var iconFile = @"Resources\trade.png";
            var iconLocation = Path.Combine(sFile, iconFile);
            return iconLocation;
        }
    }
}
