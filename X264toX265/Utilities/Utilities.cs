using System;
using System.Collections.Generic;
using System.Text;

namespace X264toX265.Utilities
{
    class Utilities
    {
        public static Settings CurrentSettings;
        public static readonly string SettingsPath = Environment.CurrentDirectory + "\\settings.json";
        public static List<ModelClasses.Movie> Movies;
        public static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    }
}
