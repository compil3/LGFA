using LGFABot.Resources.DataTypes;
using LGFABot.Resources.Settings;
using Newtonsoft.Json;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LGFA_Bot.DataHandler
{
    public class SeasonFetch
    {
        public static string GetSeasonID()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Async(a => a.Console(theme: AnsiConsoleTheme.Code))
                .CreateLogger();
            string JSON = "";
            string seasonSettings = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).Replace(@"bin\Debug", @"Configuration\Settings\websettings.json");
            if (!File.Exists(seasonSettings))
            {
                Log.Warning($"{seasonSettings} does not exist.");
            }
            using (var stream = new FileStream(seasonSettings, FileMode.Open, FileAccess.Read))
            using (var readSeason = new StreamReader(stream)) { JSON = readSeason.ReadToEnd(); }
            UrlSettings seasonID = JsonConvert.DeserializeObject<UrlSettings>(JSON);
            WebSettings.XboxSeasonId = seasonID.xboxSeasonId;
            WebSettings.PSNSeasonID = seasonID.xboxSeasonId;
            var CurrentSeason = WebSettings.XboxSeasonId;
            return CurrentSeason;
        }
    }
}
