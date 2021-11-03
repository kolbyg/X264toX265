using System;
using System.IO;
using System.Collections.Generic;
using NLog;

namespace X264toX265
{
    class Program
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        static Controllers.ConversionController ConversionController;
        static void Main(string[] args)
        {
            logger.Info("Media Conversion Utility BETA v0.15");
            logger.Info("Created by Kolby Graham (https://kolbygraham.net)");
            logger.Debug("Loading Settings...");
            //Globals.Settings = new Settings();
            File_Operations.Json.LoadSettings();
            logger.Debug("Setting up the controller");
            ConversionController = new Controllers.ConversionController();
            logger.Debug("Parsing launch arguments...");
            if (args.Length != 0)
            {
                logger.Debug("Arguments were passed to the application.");
                ParseArguments(args);
            }
            else
            {
                logger.Debug("Arguments were NOT passed to the application.");
                logger.Info("Beginning conversion in unattended mode");
                ConversionController.BeginConversion(false, false);
            }

        }
        static void ParseArguments(string[] args)
        {
            switch (args[0])
            {
                case "--ExportList":
                    ConversionController.BeginConversion(false, true);
                    break;
                case "--Force":
                    ConversionController.BeginConversion(true, false);
                    break;
            }
        }
    }
}
