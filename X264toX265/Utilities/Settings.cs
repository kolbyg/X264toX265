using System;
using System.Collections.Generic;
using System.Text;

namespace X264toX265.Utilities
{
    class Settings
    {
        public string RadarrDBPath { get; set; } = "currently_unused";
        public string SonarrDBPath { get; set; } = "currently_unused";
        public string RadarrAPIKey { get; set; } = "INSERT_KEY";
        public string SonarrAPIKey { get; set; } = "INSERT_KEY";
        public string RadarrURL { get; set; } = "http://radarr-server.domain.com:7878";
        public string SonarrURL { get; set; } = "http://sonar-server.domain.com:8989";
        public string FFmpegLocation { get; set; } = "\\lib\\ffmpeg.exe";
        public string ConversionOutputDir { get; set; } = "\\output";

    }
}
