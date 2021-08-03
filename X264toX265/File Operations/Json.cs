using System;
using System.Collections.Generic;
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
                Utilities.Utilities.Logger.Error(ex.Message);
                Utilities.Utilities.Logger.Debug(ex.InnerException);
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
                Utilities.Utilities.Logger.Error(ex.Message);
                Utilities.Utilities.Logger.Debug(ex.InnerException);
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
                Utilities.Utilities.Logger.Error(ex.Message);
                Utilities.Utilities.Logger.Debug(ex.InnerException);
                return null;
            }
        }
        public static List<ModelClasses.Sonarr.Series> ParseSeries(string SonarrSeries)
        {
            try
            {
                var _settings = new JsonSerializerSettings() { };
                var _series = JsonConvert.DeserializeObject<List<ModelClasses.Sonarr.Series>>(SonarrSeries, _settings);
                return _series;
            }
            catch (Exception ex)
            {
                Utilities.Utilities.Logger.Error(ex.Message);
                Utilities.Utilities.Logger.Debug(ex.InnerException);
                return null;
            }
        }
        public static List<ModelClasses.Sonarr.EpisodeFile> ParseEpisode(string SonarrEpisode)
        {
            try
            {
                var _settings = new JsonSerializerSettings() { };
                var _episode = JsonConvert.DeserializeObject<List<ModelClasses.Sonarr.EpisodeFile>>(SonarrEpisode, _settings);
                return _episode;
            }
            catch (Exception ex)
            {
                Utilities.Utilities.Logger.Error(ex.Message);
                Utilities.Utilities.Logger.Debug(ex.InnerException);
                return null;
            }
        }
    }
}
