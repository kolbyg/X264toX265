using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace X264toX265.File_Operations
{
    class Json
    {
        public static bool CreateSettings()
        {
            try
            {
                string json = JsonConvert.SerializeObject(Utilities.Utilities.CurrentSettings, Formatting.Indented);
                File.WriteAllText(Utilities.Utilities.SettingsPath, json);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        public static bool LoadSettings(){
            try {
                string json = File.ReadAllText(Utilities.Utilities.SettingsPath);

                Utilities.Utilities.CurrentSettings = JsonConvert.DeserializeObject<Utilities.Settings>(json);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static List<ModelClasses.Movie> ParseMovies(string RadarrMovies)
        {
            try
            {
                var _settings = new JsonSerializerSettings() { };
                var _movies = JsonConvert.DeserializeObject<List<ModelClasses.Movie>>(RadarrMovies, _settings);
                return _movies;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static bool LoadMovies(string MoviesJson)
        {
            try
            {
                Utilities.Utilities.CurrentSettings = JsonConvert.DeserializeObject<Utilities.Settings>(MoviesJson);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
