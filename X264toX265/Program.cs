using System;
using System.IO;
using System.Collections.Generic;

namespace X264toX265
{
    class Program
    {
        static void Main(string[] args)
        {
            //Check if arguments were passed to the application
            if (CheckArguments(args))
            {
                Utilities.Utilities.Logger.Info("Arguments were passed to the application.");
                //TODO parse arguments
            }
            else
            {
                //TODO: DoDefaultAction
                Utilities.Utilities.Logger.Info("Arguments were NOT passed to the application.");
            }

            Utilities.Utilities.Logger.Info("Loading Settings");
            LoadSettings();

            Utilities.Utilities.Logger.Info("Processing Radarr...");
            Utilities.Utilities.Logger.Info("Being retrieving movies");
            LoadMovies();

            Utilities.Utilities.Logger.Info("Determining which, if any movies require conversion");
            Utilities.Utilities.Movies = MediaOperations.CheckFileCodec.GetMovieConversionList(Utilities.Utilities.Movies);
            List<ModelClasses.Movie> MoviesToConvert = MediaOperations.ConvertFile.CreateConversionQueue(Utilities.Utilities.Movies);
            Utilities.Utilities.Logger.Info($"There are {MoviesToConvert.Count.ToString()} movies to convert");


            //Do sonarr stuff
            Utilities.Utilities.Logger.Info($"Converting...");
            MediaOperations.ConvertFile.ConvertFiles(MoviesToConvert);



        }
        static bool CheckArguments(string[] args)
        {
            return false; //TODO make it actually check arguments
        }
        static bool LoadSettings()
        {
            Utilities.Utilities.CurrentSettings = new Utilities.Settings();
            if(!File.Exists(Utilities.Utilities.SettingsPath))
            {
                if (File_Operations.Json.CreateSettings())
                    Utilities.Utilities.Logger.Info("The Settings JSON file does not exist, it has been created. Please exit the application and modify the settings file appropriately.");
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
                string _radarrJsonMovies = Net.RadarrOperations.RetrieveMovies(Utilities.Utilities.CurrentSettings.RadarrURL, Utilities.Utilities.CurrentSettings.RadarrAPIKey);
                Utilities.Utilities.Logger.Info("Parsing Radarr JSON");
                Utilities.Utilities.Movies = File_Operations.Json.ParseMovies(_radarrJsonMovies);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
