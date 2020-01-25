using System;
using System.IO;

namespace X264toX265
{
    class Program
    {
        static void Main(string[] args)
        {
            //Check if arguments were passed to the application
            if (CheckArguments(args))
            {
                Console.WriteLine("Arguments were passed to the application.");
                //TODO parse arguments
            }
            else
            {
                //TODO: DoDefaultAction
                Console.WriteLine("Arguments were NOT passed to the application.");
            }

            Console.WriteLine("Loading Settings");
            LoadSettings();

            Console.WriteLine("Being retrieving movies");
            LoadMovies();
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
                    Console.WriteLine("The Settings JSON file does not exist, it has been created. Please exit the application and modify the settings file appropriately.");
                else
                    throw new Exception("There was an error writing the config file.");
            }
            else
            {
                if (File_Operations.Json.LoadSettings())
                    Console.WriteLine("The settings JSON has been loaded.");
                else
                    throw new Exception("There was an error loading the config file.");
            }
            return false;
        }
        static bool LoadMovies()
        {
            Console.WriteLine("Retrieving movies json output from the Radarr API");
            string _radarrJsonMovies = Net.RadarrOperations.RetrieveMovies(Utilities.Utilities.CurrentSettings.RadarrURL,Utilities.Utilities.CurrentSettings.RadarrAPIKey);
            Console.WriteLine("Parsing Radarr JSON");
            Utilities.Utilities.Movies = File_Operations.Json.ParseMovies(_radarrJsonMovies);
            Console.WriteLine("");
            return false;
        }
    }
}
