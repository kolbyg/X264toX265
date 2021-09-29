using System;
using System.IO;
using System.Collections.Generic;
using NLog;

namespace X264toX265
{
    class Program
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Media Conversion Utility BETA v0.14");
            logger.Info("Created by Kolby Graham (https://kolbygraham.net)");

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
                Controllers.ConversionController.BeginConversion(false, false);
            }

        }
        static void ParseArguments(string[] args)
        {
            switch (args[0])
            {
                case "--ExportList":
                    Controllers.ConversionController.BeginConversion(false, true);
                    break;
                case "--Force":
                    Controllers.ConversionController.BeginConversion(true, false);
                    break;
            }
        }
    }
}
