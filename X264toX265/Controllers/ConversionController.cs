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
            Utilities.Utilities.Logger.Debug("Loading Settings...");
            LoadSettings();

            List<ModelClasses.Radarr.Movie> MoviesToConvert = ListMovies();
            List<ModelClasses.Sonarr.EpisodeFile> EpisodesToConvert = ListSeries();
            Utilities.Utilities.Logger.Info($"There are {MoviesToConvert.Count.ToString()} movies to convert");
            Utilities.Utilities.Logger.Info($"There are {EpisodesToConvert.Count.ToString()} episodes to convert");


            if (MoviesToConvert.Count < Utilities.Utilities.CurrentSettings.MaxUnattendedMovies || Utilities.Utilities.IsForced)
            {
                Utilities.Utilities.Logger.Info("Beginning movie conversion...");
                ConvertMovies(MoviesToConvert);
            }
            else
                Utilities.Utilities.Logger.Warn("Too many movies in the queue, increase MaxUnattendedMovies or run with --force. Movie processing will be skipped.");

            if (EpisodesToConvert.Count < Utilities.Utilities.CurrentSettings.MaxUnattendedEpisodes || Utilities.Utilities.IsForced)
            {
                Utilities.Utilities.Logger.Info($"Beginning episode conversion...");
                ConvertTV(EpisodesToConvert);
            }
            else
            {
                Utilities.Utilities.Logger.Warn("Too many episodes in the queue, increase MaxUnattendeEpisodes or run with --force. Episode processing will be skipped.");
            }

        }
        static List<ModelClasses.Radarr.Movie> ListMovies()
        {
            Utilities.Utilities.Logger.Info("Processing movies...");
            Utilities.Utilities.Logger.Debug("Being retrieving movie list from Radarr...");
            LoadMovies();

            Utilities.Utilities.Logger.Info("Determining which, if any movies require conversion...");
            MediaOperations.CheckFileCodec.GetMovieConversionList(Utilities.Utilities.Movies);
            List<ModelClasses.Radarr.Movie> MoviesToConvert = MediaOperations.ConvertFile.CreateConversionQueue(Utilities.Utilities.Movies);
            return MoviesToConvert;

        }
        static List<ModelClasses.Sonarr.EpisodeFile> ListSeries()
        {
            Utilities.Utilities.Logger.Info("Processing series...");
            Utilities.Utilities.Logger.Debug("Being retrieving series list from Sonarr...");
            LoadSeries();

            Utilities.Utilities.Logger.Info("Determining which, if any episodes require conversion...");
            MediaOperations.CheckFileCodec.GetEpisodeConversionList(Utilities.Utilities.Series);
            List<ModelClasses.Sonarr.EpisodeFile> EpisodesToConvert = MediaOperations.ConvertFile.CreateConversionQueue(Utilities.Utilities.Series);
            return EpisodesToConvert;
        }
        static void ConvertMovies(List<ModelClasses.Radarr.Movie> MoviesToConvert)
        {
            MediaOperations.ConvertFile.ConvertFiles(MoviesToConvert);
        }
        static void ConvertTV(List<ModelClasses.Sonarr.EpisodeFile> EpisodesToConvert)
        {
            MediaOperations.ConvertFile.ConvertFiles(EpisodesToConvert);
        }
        static bool LoadSettings()
        {
            Utilities.Utilities.CurrentSettings = new Utilities.Settings();
            if (!File.Exists(Utilities.Utilities.SettingsPath))
            {
                if (File_Operations.Json.CreateSettings())
                {
                    Utilities.Utilities.Logger.Warn("The Settings JSON file does not exist, it has been created. Please exit the application and modify the settings file appropriately.");
                    Environment.Exit(0);
                }
                else
                    throw new Exception("There was an error writing the config file.");
            }
            else
            {
                if (File_Operations.Json.LoadSettings())
                    Utilities.Utilities.Logger.Debug("The settings JSON has been loaded.");
                else
                    throw new Exception("There was an error loading the config file.");
            }
            return false;
        }
        static bool LoadMovies()
        {
            try
            {
                Utilities.Utilities.Logger.Debug("Retrieving movies json output from the Radarr API");
                string _radarrJsonMovies = Net.APIController.RetrieveMovies(Utilities.Utilities.CurrentSettings.RadarrURL, Utilities.Utilities.CurrentSettings.RadarrAPIKey);
                Utilities.Utilities.Logger.Debug("Parsing Movie JSON");
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
                Utilities.Utilities.Logger.Debug("Retrieving series json output from the Sonarr API");
                string _sonarrJsonSeries = Net.APIController.RetrieveSeries(Utilities.Utilities.CurrentSettings.SonarrURL, Utilities.Utilities.CurrentSettings.SonarrAPIKey);
                Utilities.Utilities.Logger.Debug("Parsing Series JSON");
                Utilities.Utilities.Series = File_Operations.Json.ParseSeries(_sonarrJsonSeries);
                Utilities.Utilities.Logger.Debug("Retrieving episode lists");
                foreach(ModelClasses.Sonarr.Series series in Utilities.Utilities.Series)
                {
                    Utilities.Utilities.Logger.Debug("Processing " + series.Title);
                    string _sonarrJsonEpisode = Net.APIController.RetrieveEpisodes(Utilities.Utilities.CurrentSettings.SonarrURL, Utilities.Utilities.CurrentSettings.SonarrAPIKey, series.ID);
                    Utilities.Utilities.Logger.Debug("Parsing Episode JSON");
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
