using System;
using System.IO;
using System.Collections.Generic;

namespace X264toX265
{
    class Program
    {
        static void Main(string[] args)
        {
            Utilities.Utilities.Logger.Info("Media Conversion Utility v0.1");
            Utilities.Utilities.Logger.Info("Created by Kolby Graham");

            Utilities.Utilities.Logger.Debug("Parsing launch arguments...");
            if (CheckArguments(args))
            {
                Utilities.Utilities.Logger.Debug("Arguments were passed to the application.");
                //TODO parse arguments
            }
            else
            {
                Utilities.Utilities.Logger.Debug("Arguments were NOT passed to the application.");
                Utilities.Utilities.Logger.Info("Beginning conversion in unattended mode");
                Controllers.ConversionController.BeginConversion(false);
            }

        }
        static bool CheckArguments(string[] args)
        {
            return false; //TODO make it actually check arguments
        }
    }
}
