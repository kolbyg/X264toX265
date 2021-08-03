using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace X264toX265.Controllers
{
    class ConversionController
    {
        public static void BeginConversion(bool IsForced)
        {
            Utilities.Utilities.Logger.Info("Loading Settings");
            LoadSettings();

            //ConvertMovies(IsForced);

            ConvertTV(IsForced);

        }
        static void ConvertMovies(bool IsForced)
        {
            Utilities.Utilities.Logger.Info("Processing Radarr...");
            Utilities.Utilities.Logger.Info("Being retrieving movies");
            LoadMovies();

            Utilities.Utilities.Logger.Info("Determining which, if any movies require conversion");
            MediaOperations.CheckFileCodec.GetMovieConversionList(Utilities.Utilities.Movies);
            List<ModelClasses.Movie> MoviesToConvert = MediaOperations.ConvertFile.CreateConversionQueue(Utilities.Utilities.Movies);
            Utilities.Utilities.Logger.Info($"There are {MoviesToConvert.Count.ToString()} movies to convert");

            if(MoviesToConvert.Count < Utilities.Utilities.CurrentSettings.MaxUnattendedMovies || IsForced)
            {
                Utilities.Utilities.Logger.Info($"Converting...");
                MediaOperations.ConvertFile.ConvertFiles(MoviesToConvert);
            }
            else
            {
                Utilities.Utilities.Logger.Warn("Too many movies in the queue, increase MaxUnattendedMovies or run with --force");
            }
        }
        static void ConvertTV(bool IsForced)
        {
            Utilities.Utilities.Logger.Info("Processing Sonarr...");
            Utilities.Utilities.Logger.Info("Being retrieving series");
            LoadSeries();

            Utilities.Utilities.Logger.Info("Determining which, if any episodes require conversion");
            MediaOperations.CheckFileCodec.GetEpisodeConversionList(Utilities.Utilities.Series);
            List<ModelClasses.Sonarr.EpisodeFile> EpisodesToConvert = MediaOperations.ConvertFile.CreateConversionQueue(Utilities.Utilities.Series);
            Utilities.Utilities.Logger.Info($"There are {EpisodesToConvert.Count.ToString()} episodes to convert");

            if (EpisodesToConvert.Count < Utilities.Utilities.CurrentSettings.MaxUnattendedEpisodes || IsForced)
            {
                Utilities.Utilities.Logger.Info($"Converting...");
                //MediaOperations.ConvertFile.ConvertFiles(EpisodesToConvert);
            }
            else
            {
                Utilities.Utilities.Logger.Warn("Too many movies in the queue, increase MaxUnattendedMovies or run with --force");
            }

        }
        static bool LoadSettings()
        {
            Utilities.Utilities.CurrentSettings = new Utilities.Settings();
            if (!File.Exists(Utilities.Utilities.SettingsPath))
            {
                if (File_Operations.Json.CreateSettings())
                {
                    Utilities.Utilities.Logger.Info("The Settings JSON file does not exist, it has been created. Please exit the application and modify the settings file appropriately.");
                    Environment.Exit(0);
                }
                else
                    throw new Exception("There was an error writing the config file.");
            }
            else
            {
                if (File_Operations.Json.LoadSettings())
                    Utilities.Utilities.Logger.Info("The settings JSON has been loaded.");
                else
                    throw new Exception("There was an error loading the config file.");
            }
            return false;
        }
        static bool LoadMovies()
        {
            try
            {
                Utilities.Utilities.Logger.Info("Retrieving movies json output from the Radarr API");
                string _radarrJsonMovies = Net.APIController.RetrieveMovies(Utilities.Utilities.CurrentSettings.RadarrURL, Utilities.Utilities.CurrentSettings.RadarrAPIKey);
                Utilities.Utilities.Logger.Info("Parsing Radarr JSON");
                Utilities.Utilities.Movies = File_Operations.Json.ParseMovies(_radarrJsonMovies);
                return true;
            }
            catch (Exception ex)
            {
                Utilities.Utilities.Logger.Error(ex.Message);
                Utilities.Utilities.Logger.Debug(ex.InnerException);
                return false;
            }
        }
        static bool LoadSeries()
        {
            try
            {
                Utilities.Utilities.Logger.Info("Retrieving series json output from the Sonarr API");
                string _sonarrJsonSeries = Net.APIController.RetrieveSeries(Utilities.Utilities.CurrentSettings.SonarrURL, Utilities.Utilities.CurrentSettings.SonarrAPIKey);
                Utilities.Utilities.Logger.Info("Parsing Series JSON");
                Utilities.Utilities.Series = File_Operations.Json.ParseSeries(_sonarrJsonSeries);
                Utilities.Utilities.Logger.Info("Retrieving episode lists");
                foreach(ModelClasses.Sonarr.Series series in Utilities.Utilities.Series)
                {
                    Utilities.Utilities.Logger.Info("Processing " + series.Title);
                    string _sonarrJsonEpisode = Net.APIController.RetrieveEpisodes(Utilities.Utilities.CurrentSettings.SonarrURL, Utilities.Utilities.CurrentSettings.SonarrAPIKey, series.ID);
                    Utilities.Utilities.Logger.Info("Parsing Episode JSON");
                    series.Episodes = File_Operations.Json.ParseEpisode(_sonarrJsonEpisode);
                }
                return true;
            }
            catch (Exception ex)
            {
                Utilities.Utilities.Logger.Error(ex.Message);
                Utilities.Utilities.Logger.Debug(ex.InnerException);
                return false;
            }
        }
    }
}
