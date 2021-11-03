using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace X264toX265.MediaOperations
{
    public class ConversionOptions
    {
        public Transcoder Transcoder { get; set; }
        public ConversionOptions()
        {
            Transcoder = Globals.Settings.Transcoder;
        }
    }
}
