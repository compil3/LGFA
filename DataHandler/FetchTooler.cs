using System;
using System.IO;
using System.Reflection;
using LGFABot.Resources.DataTypes;
using LGFABot.Resources.Settings;
using Newtonsoft.Json;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace LGFABot.DataHandler
{
    class FetchTooler
    {
        public static string GetURL(string System, string Trigger)
        {
            string JSON = "";
            var xboxURL = "";
            var psnURL = "";

            var sFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var configFile = @"Configuration\websettings.json";
            var configLocation = Path.Combine(sFile, configFile);
            if (!File.Exists(configLocation)) Log.Error($"GetURL Method: {configLocation} does not exist");
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
                Log.Error(ex, $"Error reading websettings.json ({configLocation}");
                throw;
            }

            UrlSettings settings = JsonConvert.DeserializeObject<UrlSettings>(JSON);
            WebSettings.XboxSeasonId = settings.xboxSeasonId;
            WebSettings.PSNSeasonID = settings.psnSeasonId;
            var url = "";
            if (System == "xbox")
            {
                if (Trigger == "playerstats" || Trigger == "goaliestats")
                {
                    WebSettings.XboxPlayerStatsURL = settings.xboxPlayerStatsURL;
                    url = WebSettings.XboxPlayerStatsURL + WebSettings.XboxSeasonId;
                }
                else if (Trigger == "draftlist")
                {
                    WebSettings.XboxTeamStandingsURL = settings.xboxStandingsURL;
                    url = WebSettings.XboxTeamStandingsURL + WebSettings.XboxSeasonId;
                }
                else if (Trigger == "teamstats")
                {
                    WebSettings.XboxTeamStandingsURL = settings.xboxStandingsURL;
                    url = WebSettings.XboxTeamStandingsURL + WebSettings.XboxSeasonId;
                }
            } else if (System == "psn")
            {
                if (Trigger == "playerstats" || Trigger == "goaliestats")
                {
                    WebSettings.PSNPlayerStatsURL = settings.psnPlayerStatsURL;
                    url = WebSettings.PSNPlayerStatsURL + WebSettings.PSNSeasonID;
                } else if (Trigger == "draftlist")
                {
                    WebSettings.PSNDraftListURL = settings.psnDraftListURL;
                    url = WebSettings.PSNDraftListURL + WebSettings.PSNSeasonID;
                } else if (Trigger == "teamstats")
                {
                    WebSettings.PSNTeamStandingsURL = settings.psnStandingsURL;
                    url = WebSettings.PSNTeamStandingsURL + WebSettings.PSNSeasonID;
                }
            }

            return url;
        }
        #region Switch
        //switch (System)
        //{
        //    case "xbox":
        //        switch (Trigger)
        //        {
        //            case "playerstats":
        //            case "goaliestats":
        //                WebSettings.XboxPlayerStatsURL = settings.xboxPlayerStatsURL;
        //                WebSettings.PSNPlayerStatsURL = settings.psnPlayerStatsURL;
        //                xboxURL = WebSettings.XboxPlayerStatsURL + WebSettings.XboxSeasonId;
        //                psnURL = WebSettings.PSNPlayerStatsURL + WebSettings.PSNSeasonID;
        //                break;
        //            case "draftlist":
        //                WebSettings.XboxDraftListURL = settings.xboxDraftListURL;
        //                WebSettings.PSNDraftListURL = settings.psnDraftListURL;
        //                xboxURL = WebSettings.XboxDraftListURL + WebSettings.XboxSeasonId;
        //                psnURL = WebSettings.PSNDraftListURL + WebSettings.PSNSeasonID;
        //                break;
        //            case "teamstats":
        //                WebSettings.XboxTeamStandingsURL = settings.xboxStandingsURL;
        //                WebSettings.PSNTeamStandingsURL = settings.psnStandingsURL;
        //                xboxURL = WebSettings.XboxTeamStandingsURL + WebSettings.XboxSeasonId;
        //                psnURL = WebSettings.PSNTeamStandingsURL + WebSettings.PSNSeasonID;
        //                break;
        //        }
        //        return xboxURL;
        //}
#endregion

        public static string GetNumberOfPlayersUrl(string system)
        {
            string JSON = "";

            var sFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var configFile = @"Configuration\websettings.json";
            var configLocation = Path.Combine(sFile, configFile);
            if (!File.Exists(configLocation)) Log.Error($"GetNumberOfPlayersURL Method: {configLocation} does not exist");
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
                Log.Error(ex, $"Error reading websettings.json ({configLocation}");
                throw;
            }

            UrlSettings settings = JsonConvert.DeserializeObject<UrlSettings>(JSON);

            var url = "";
            if (system == "xbox")
            {
                WebSettings.XboxSeasonId = settings.xboxSeasonId;
                WebSettings.XboxDraftListURL = settings.xboxDraftListURL;
                url = WebSettings.XboxDraftListURL + WebSettings.XboxSeasonId;
            } else if (system == "psn")
            {
                WebSettings.PSNSeasonID = settings.psnSeasonId;
                WebSettings.PSNDraftListURL = settings.psnDraftListURL;
                url = WebSettings.PSNDraftListURL + WebSettings.PSNSeasonID;
            }
            #region Switch2
            //switch (system)
            //{
            //    case "xbox":
            //        WebSettings.XboxSeasonId = settings.xboxSeasonId;
            //        WebSettings.XboxDraftListURL = settings.xboxDraftListURL;
            //        var xboxURL = WebSettings.XboxDraftListURL + WebSettings.XboxSeasonId;
            //        return xboxURL;
            //    case "psn":                   
            //        WebSettings.XboxSeasonId = settings.xboxSeasonId;
            //        WebSettings.XboxDraftListURL = settings.xboxDraftListURL;
            //        var psnURL = WebSettings.XboxDraftListURL + WebSettings.XboxSeasonId;
            //        return psnURL;
            //}
#endregion
            return url;
        }

        public static string GetSeasonID(string System)
        {
            string JSON = "";
            var sFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var configFile = @"Configuration\websettings.json";
            var configLocation = Path.Combine(sFile, configFile);
            if (!File.Exists(configLocation)) Log.Error($"GetSeasonID Method: {configLocation} does not exist");
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
                Log.Error(ex,$"Error reading websettings.json ({configLocation}" );
                throw;
            }

            UrlSettings seasonID = JsonConvert.DeserializeObject<UrlSettings>(JSON);
            var CurrentSeason = WebSettings.XboxSeasonId;

            if (System == "xbox")
            {
                WebSettings.XboxSeasonId = seasonID.xboxSeasonId;
                CurrentSeason = WebSettings.XboxSeasonId;
            } else if (System == "psn")
            {
                WebSettings.PSNSeasonID = seasonID.psnSeasonId;
                CurrentSeason = WebSettings.PSNSeasonID;
            }
            else
            {
                Log.Warning("Unable to fetch current season from websettings.json.  Please check that the file exists or has a seasonid entry");
                CurrentSeason = "Error";
            }
            return CurrentSeason;
        }
    }
}
