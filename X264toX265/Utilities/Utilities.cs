using System;
using System.Collections.Generic;
using System.Text;

namespace X264toX265.Utilities
{
    class Utilities
    {
        public static Settings CurrentSettings;
        public static readonly string SettingsPath = Environment.CurrentDirectory + "\\settings.json";
        public static List<ModelClasses.Radarr.Movie> Movies;
        public static List<ModelClasses.Sonarr.Series> Series;
        public static bool IsForced = false; //Was the application run with --force?
    }
}
