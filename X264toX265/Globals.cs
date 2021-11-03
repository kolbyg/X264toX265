using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X264toX265.ModelClasses.Radarr;
using X264toX265.ModelClasses.Sonarr;

namespace X264toX265
{
    class Globals
    {
        public static Settings Settings;
        public static readonly string SettingsPath = Environment.CurrentDirectory + "\\settings.json";
        public static List<Series> AllSeries;
        public static List<Movie> AllMovies;
    }
}
