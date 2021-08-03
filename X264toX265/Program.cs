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
            bool IsForced = false;
            if (CheckArguments(args))
            {
                Utilities.Utilities.Logger.Info("Arguments were passed to the application.");
                //TODO parse arguments
            }
            else
            {
                Utilities.Utilities.Logger.Info("Arguments were NOT passed to the application.");
                Utilities.Utilities.Logger.Info("Beginning conversion with the defaults");
                Controllers.ConversionController.BeginConversion(IsForced);
            }

        }
        static bool CheckArguments(string[] args)
        {
            return false; //TODO make it actually check arguments
        }
    }
}
