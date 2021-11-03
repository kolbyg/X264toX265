using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using NLog;

namespace X264toX265.File_Operations
{
    class Json
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        public static void LoadSettings()
        {
            if (!File.Exists(Globals.SettingsPath))
            {
                if (CreateSettings())
                {
                    logger.Warn("The Settings JSON file does not exist, it has been created. Please exit the application and modify the settings file appropriately.");
                    Environment.Exit(0);
                    return;
                }
                else
                {
                    throw new Exception("There was an error writing the config file.");
                }
            }
            else
            {
                Settings settings = ParseSettings();
                if (settings != null)
                {
                    Globals.Settings = settings;
                    logger.Debug("The settings JSON has been loaded.");
                    return;
                }
                else
                {
                    throw new Exception("There was an error loading the config file.");
                }
            }
        }
        public static bool CreateSettings()
        {
            try
            {
                string json = JsonConvert.SerializeObject(new Settings(), Formatting.Indented);
                File.WriteAllText(Globals.SettingsPath, json);
                return true;
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                logger.Debug(ex.InnerException);
                return false;
            }
        }
        public static Settings ParseSettings(){
            try {
                string json = File.ReadAllText(Globals.SettingsPath);

                var obj = JsonConvert.DeserializeObject<Settings>(json);
                return obj;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Debug(ex.InnerException);
                return null;
            }
        }
        public static List<ModelClasses.Radarr.Movie> ParseMovies(string RadarrMovies)
        {
            try
            {
                var _settings = new JsonSerializerSettings() { };
                var _movies = JsonConvert.DeserializeObject<List<ModelClasses.Radarr.Movie>>(RadarrMovies, _settings);
                return _movies;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Debug(ex.InnerException);
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
                logger.Error(ex.Message);
                logger.Debug(ex.InnerException);
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
                logger.Error(ex.Message);
                logger.Debug(ex.InnerException);
                return null;
            }
        }
    }
}
