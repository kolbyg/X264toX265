using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NLog;
using System.Threading.Tasks;

namespace X264toX265.Controllers
{
    class ConversionController
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        public static void BeginConversion(bool IsForced, bool ExportList)
        {
            logger.Debug("Loading Settings...");
            LoadSettings();
            logger.Debug("Beginning API calls...");
            List<ModelClasses.Radarr.Movie> MoviesToConvert = ListMovies();
            List<ModelClasses.Sonarr.EpisodeFile> EpisodesToConvert = ListSeries();
            logger.Info($"There are {MoviesToConvert.Count.ToString()} movies to convert");
            logger.Info($"There are {EpisodesToConvert.Count.ToString()} episodes to convert");

            if (ExportList)
            {
                logger.Info("List requested, dumping all media marked for conversion to log, conversion will not run");
                logger.Info("***BEGIN EXPORT***");
                logger.Info("***BEGIN SERIES***");
                foreach(ModelClasses.Radarr.Movie movie in MoviesToConvert)
                {
                    if(movie.ConversionRequired)
                    logger.Info($"TITLE: {movie.Title} - CURRENT CODEC: {movie.MovieFiles.MediaInfo.VideoCodec}");
                }
                logger.Info("***END SERIES***");
                logger.Info("***BEGIN MOVIES***");
                foreach(ModelClasses.Sonarr.EpisodeFile ep in EpisodesToConvert)
                {
                    string showName = Utilities.Utilities.Series.Find(x => x.ID == ep.SeriesID).Title;
                    if (ep.ConversionRequired)
                        logger.Info($"SERIES: {showName} - SEASON: {ep.SeasonNumber} - EP ID: {ep.ID} - CURRENT CODEC: {ep.MediaInfo.VideoCodec}");
                }
                logger.Info("***END MOVIES***");
                logger.Info("***END EXPORT***");
                Environment.Exit(0);
            }

            if (MoviesToConvert.Count < Utilities.Utilities.CurrentSettings.MaxUnattendedMovies || Utilities.Utilities.IsForced)
            {
                logger.Info("Beginning movie conversion...");
                ConvertMovies(MoviesToConvert);
            }
            else
                logger.Warn("Too many movies in the queue, increase MaxUnattendedMovies or run with --Force. Movie processing will be skipped.");

            if (EpisodesToConvert.Count < Utilities.Utilities.CurrentSettings.MaxUnattendedEpisodes || Utilities.Utilities.IsForced)
            {
                logger.Info($"Beginning episode conversion...");
                ConvertTV(EpisodesToConvert);
            }
            else
            {
                logger.Warn("Too many episodes in the queue, increase MaxUnattendeEpisodes or run with --Force. Episode processing will be skipped.");
            }

        }
        static List<ModelClasses.Radarr.Movie> ListMovies()
        {
            logger.Info("Processing movies...");
            logger.Debug("Being retrieving movie list from Radarr...");
            LoadMovies();

            logger.Info("Determining which, if any movies require conversion...");
            MediaOperations.CheckFileCodec.GetMovieConversionList(Utilities.Utilities.Movies);
            List<ModelClasses.Radarr.Movie> MoviesToConvert = MediaOperations.ConvertFile.CreateConversionQueue(Utilities.Utilities.Movies);
            return MoviesToConvert;

        }
        static List<ModelClasses.Sonarr.EpisodeFile> ListSeries()
        {
            logger.Info("Processing series...");
            logger.Debug("Being retrieving series list from Sonarr...");
            LoadSeries();

            logger.Info("Determining which, if any episodes require conversion...");
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
                    logger.Warn("The Settings JSON file does not exist, it has been created. Please exit the application and modify the settings file appropriately.");
                    Environment.Exit(0);
                }
                else
                    throw new Exception("There was an error writing the config file.");
            }
            else
            {
                if (File_Operations.Json.LoadSettings())
                    logger.Debug("The settings JSON has been loaded.");
                else
                    throw new Exception("There was an error loading the config file.");
            }
            return false;
        }
        static bool LoadMovies()
        {
            try
            {
                logger.Debug("Retrieving movies json output from the Radarr API");
                string _radarrJsonMovies = Net.APIController.RetrieveMovies(Utilities.Utilities.CurrentSettings.RadarrURL, Utilities.Utilities.CurrentSettings.RadarrAPIKey);
                logger.Debug("Parsing Movie JSON");
                Utilities.Utilities.Movies = File_Operations.Json.ParseMovies(_radarrJsonMovies);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Debug(ex.InnerException);
                return false;
            }
        }
        static bool LoadSeries()
        {
            try
            {
                logger.Debug("Retrieving series json output from the Sonarr API");
                string _sonarrJsonSeries = Net.APIController.RetrieveSeries(Utilities.Utilities.CurrentSettings.SonarrURL, Utilities.Utilities.CurrentSettings.SonarrAPIKey);
                logger.Debug("Parsing Series JSON");
                Utilities.Utilities.Series = File_Operations.Json.ParseSeries(_sonarrJsonSeries);
                logger.Debug("Retrieving episode lists");
                foreach(ModelClasses.Sonarr.Series series in Utilities.Utilities.Series)
                {
                    logger.Debug("Processing " + series.Title);
                    string _sonarrJsonEpisode = Net.APIController.RetrieveEpisodes(Utilities.Utilities.CurrentSettings.SonarrURL, Utilities.Utilities.CurrentSettings.SonarrAPIKey, series.ID);
                    logger.Debug("Parsing Episode JSON");
                    series.Episodes = File_Operations.Json.ParseEpisode(_sonarrJsonEpisode);
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Debug(ex.InnerException);
                return false;
            }
        }
    }
}
